using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Slams an APPROVED / SPIKED stamp onto the document, plays a thud and jolts the page.
// The stamp is built in code, so no prefab is needed.
public class DocumentStamp : MonoBehaviour
{
    [Header("Where")]
    [Tooltip("The document panel the stamp lands on (the Document object). It also jolts when stamped.")]
    [SerializeField] private RectTransform document;
    [Tooltip("Font for the stamp text. Leave empty to use TextMeshPro's default font.")]
    [SerializeField] private TMP_FontAsset font;

    [Header("Look")]
    [SerializeField] private string approvedText = "APPROVED";
    [SerializeField] private string deniedText = "SPIKED";
    [SerializeField] private Color approvedColor = new Color(0.15f, 0.5f, 0.2f, 0.9f);
    [SerializeField] private Color deniedColor = new Color(0.75f, 0.12f, 0.1f, 0.9f);
    [SerializeField] private float fontSize = 64f;
    [SerializeField] private float borderThickness = 6f;
    [SerializeField] private Vector2 padding = new Vector2(48f, 20f);
    [Tooltip("Random tilt, in degrees, either way.")]
    [SerializeField] private float maxRotation = 12f;
    [Tooltip("How far from the centre of the document the stamp can land.")]
    [SerializeField] private Vector2 maxOffset = new Vector2(60f, 80f);

    [Header("Timing")]
    [SerializeField] private float slamDuration = 0.08f;
    [Tooltip("How big the stamp starts before it slams down to normal size.")]
    [SerializeField] private float startScale = 1.8f;
    [SerializeField] private float thumpDuration = 0.12f;
    [Tooltip("How much the document shrinks for a moment when hit. 1 = no jolt.")]
    [SerializeField] private float thumpScale = 0.98f;
    [Tooltip("How long the stamp stays on screen before the next item.")]
    [SerializeField] private float holdDuration = 0.6f;

    [Header("Sound")]
    [Tooltip("Set this AudioSource's Output to your mixer's Master group so the volume setting applies.")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip approvedSound;
    [SerializeField] private AudioClip deniedSound;

    private RectTransform stamp;
    private CanvasGroup stampGroup;
    private TMP_Text stampLabel;
    private Image[] borders;

    public IEnumerator Play(bool approved)
    {
        EnsureBuilt();

        string text = approved ? approvedText : deniedText;
        Color color = approved ? approvedColor : deniedColor;
        stampLabel.text = text;
        stampLabel.color = color;
        foreach (Image border in borders)
        {
            border.color = color;
        }

        stamp.sizeDelta = stampLabel.GetPreferredValues(text) + padding;
        stamp.anchoredPosition = new Vector2(
            Random.Range(-maxOffset.x, maxOffset.x),
            Random.Range(-maxOffset.y, maxOffset.y));
        stamp.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-maxRotation, maxRotation));
        stamp.SetAsLastSibling(); // draw on top of the page
        stamp.gameObject.SetActive(true);

        yield return Slam();
        PlaySound(approved);
        yield return Thump();
        yield return new WaitForSeconds(holdDuration);
    }

    public void Hide()
    {
        if (stamp != null)
        {
            stamp.gameObject.SetActive(false);
        }
    }

    // Starts big and see-through, then drops to full size and full colour.
    private IEnumerator Slam()
    {
        for (float time = 0f; time < slamDuration; time += Time.deltaTime)
        {
            float progress = time / slamDuration;
            stamp.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, progress * progress);
            stampGroup.alpha = progress;
            yield return null;
        }

        stamp.localScale = Vector3.one;
        stampGroup.alpha = 1f;
    }

    // The page squashes slightly and springs back, like it was hit.
    private IEnumerator Thump()
    {
        if (document == null || Mathf.Approximately(thumpScale, 1f))
        {
            yield break;
        }

        Vector3 original = document.localScale;
        for (float time = 0f; time < thumpDuration; time += Time.deltaTime)
        {
            float progress = time / thumpDuration;
            float squash = Mathf.Sin(progress * Mathf.PI); // 0 -> 1 -> 0
            document.localScale = original * Mathf.Lerp(1f, thumpScale, squash);
            yield return null;
        }

        document.localScale = original;
    }

    private void PlaySound(bool approved)
    {
        AudioClip clip = approved ? approvedSound : deniedSound;
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(0.95f, 1.05f); // stops repeated stamps sounding identical
        audioSource.PlayOneShot(clip);
    }

    private void EnsureBuilt()
    {
        if (stamp != null)
        {
            return;
        }

        RectTransform parent = document != null ? document : (RectTransform)transform;

        GameObject stampObject = new GameObject("Stamp",
            typeof(RectTransform), typeof(CanvasGroup), typeof(LayoutElement));
        stampObject.layer = parent.gameObject.layer;
        stamp = (RectTransform)stampObject.transform;
        stamp.SetParent(parent, false);
        stamp.anchorMin = new Vector2(0.5f, 0.5f);
        stamp.anchorMax = new Vector2(0.5f, 0.5f);
        stamp.pivot = new Vector2(0.5f, 0.5f);

        // Never blocks clicks, and never gets pushed around by a layout group.
        stampGroup = stampObject.GetComponent<CanvasGroup>();
        stampGroup.blocksRaycasts = false;
        stampGroup.interactable = false;
        stampObject.GetComponent<LayoutElement>().ignoreLayout = true;

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.layer = stampObject.layer;
        RectTransform labelRect = (RectTransform)labelObject.transform;
        labelRect.SetParent(stamp, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        stampLabel = labelObject.GetComponent<TMP_Text>();
        if (font != null)
        {
            stampLabel.font = font;
        }

        stampLabel.fontSize = fontSize;
        stampLabel.fontStyle = FontStyles.Bold;
        stampLabel.characterSpacing = 8f;
        stampLabel.alignment = TextAlignmentOptions.Center;
        stampLabel.raycastTarget = false;

        float t = borderThickness;
        borders = new[]
        {
            CreateBorder("Top", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -t), Vector2.zero),
            CreateBorder("Bottom", Vector2.zero, new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, t)),
            CreateBorder("Left", Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(t, 0f)),
            CreateBorder("Right", new Vector2(1f, 0f), Vector2.one, new Vector2(-t, 0f), Vector2.zero),
        };

        stampObject.SetActive(false);
    }

    private Image CreateBorder(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject borderObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        borderObject.layer = stamp.gameObject.layer;
        RectTransform rect = (RectTransform)borderObject.transform;
        rect.SetParent(stamp, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        Image image = borderObject.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }
}
