using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Slams an APPROVED / SPIKED stamp onto the document, plays a thud and jolts the page.
public class DocumentStamp : MonoBehaviour
{
    [Header("Where")]
    [Tooltip("The document panel the stamp lands on. It also jolts when stamped.")]
    [SerializeField] private RectTransform document;
    [Tooltip("Leave empty to use TextMeshPro's default font.")]
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
        ApplyLook(approved);
        PlaceRandomly();
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

    private void ApplyLook(bool approved)
    {
        string text = approved ? approvedText : deniedText;
        Color color = approved ? approvedColor : deniedColor;

        stampLabel.text = text;
        stampLabel.color = color;
        foreach (Image border in borders)
        {
            border.color = color;
        }

        stamp.sizeDelta = stampLabel.GetPreferredValues(text) + padding;
    }

    private void PlaceRandomly()
    {
        stamp.anchoredPosition = new Vector2(
            Random.Range(-maxOffset.x, maxOffset.x),
            Random.Range(-maxOffset.y, maxOffset.y));
        stamp.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-maxRotation, maxRotation));
        stamp.SetAsLastSibling();
    }

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

    private IEnumerator Thump()
    {
        if (document == null || Mathf.Approximately(thumpScale, 1f))
        {
            yield break;
        }

        Vector3 original = document.localScale;
        for (float time = 0f; time < thumpDuration; time += Time.deltaTime)
        {
            float squash = Mathf.Sin(time / thumpDuration * Mathf.PI);
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

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip);
    }

    private void EnsureBuilt()
    {
        if (stamp != null)
        {
            return;
        }

        stamp = CreateStampRoot(document != null ? document : (RectTransform)transform);
        stampLabel = CreateLabel(stamp);
        borders = CreateBorders(stamp);
        stamp.gameObject.SetActive(false);
    }

    private RectTransform CreateStampRoot(RectTransform parent)
    {
        RectTransform root = CreateChild("Stamp", parent, typeof(CanvasGroup), typeof(LayoutElement));
        root.anchorMin = new Vector2(0.5f, 0.5f);
        root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0.5f);

        // Never blocks clicks, and never gets pushed around by a layout group.
        stampGroup = root.GetComponent<CanvasGroup>();
        stampGroup.blocksRaycasts = false;
        stampGroup.interactable = false;
        root.GetComponent<LayoutElement>().ignoreLayout = true;
        return root;
    }

    private TMP_Text CreateLabel(RectTransform parent)
    {
        RectTransform rect = CreateChild("Label", parent, typeof(TextMeshProUGUI));
        SetAnchors(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        TMP_Text label = rect.GetComponent<TMP_Text>();
        if (font != null)
        {
            label.font = font;
        }

        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.characterSpacing = 8f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }

    private Image[] CreateBorders(RectTransform parent)
    {
        float t = borderThickness;
        return new[]
        {
            CreateBorder("Top", parent, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -t), Vector2.zero),
            CreateBorder("Bottom", parent, Vector2.zero, new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, t)),
            CreateBorder("Left", parent, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(t, 0f)),
            CreateBorder("Right", parent, new Vector2(1f, 0f), Vector2.one, new Vector2(-t, 0f), Vector2.zero),
        };
    }

    private static Image CreateBorder(string name, RectTransform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rect = CreateChild(name, parent, typeof(Image));
        SetAnchors(rect, anchorMin, anchorMax, offsetMin, offsetMax);

        Image image = rect.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private static RectTransform CreateChild(string name, Transform parent, params System.Type[] components)
    {
        GameObject child = new GameObject(name, typeof(RectTransform));
        child.layer = parent.gameObject.layer;
        foreach (System.Type component in components)
        {
            child.AddComponent(component);
        }

        RectTransform rect = (RectTransform)child.transform;
        rect.SetParent(parent, false);
        return rect;
    }

    private static void SetAnchors(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
