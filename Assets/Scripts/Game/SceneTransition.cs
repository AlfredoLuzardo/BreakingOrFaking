using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Fades the screen out, loads a scene, and fades back in. Optionally shows a title card while the screen is dark.
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float titleFadeDuration = 0.3f;
    [SerializeField] private float titleHoldDuration = 1f;
    [SerializeField] private float titleFontSize = 120f;
    [SerializeField] private Color overlayColor = new Color(0.08f, 0.06f, 0.05f);
    [SerializeField] private Color titleColor = new Color(0.95f, 0.92f, 0.85f);

    private static SceneTransition instance;

    private CanvasGroup overlayGroup;
    private TMP_Text titleText;
    private bool isTransitioning;

    public static void LoadScene(string sceneName, string titleCard = null)
    {
        if (instance == null)
        {
            instance = Create();
        }

        if (!instance.isTransitioning)
        {
            instance.StartCoroutine(instance.Transition(sceneName, titleCard));
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        instance = null;
    }

    private static SceneTransition Create()
    {
        GameObject root = new GameObject("SceneTransition", typeof(RectTransform));
        DontDestroyOnLoad(root);
        SceneTransition transition = root.AddComponent<SceneTransition>();
        transition.BuildOverlay();
        return transition;
    }

    private void BuildOverlay()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        overlayGroup = gameObject.AddComponent<CanvasGroup>();
        overlayGroup.alpha = 0f;
        overlayGroup.blocksRaycasts = false;

        Image overlay = CreateFullScreenChild("Overlay").AddComponent<Image>();
        overlay.color = overlayColor;

        titleText = CreateFullScreenChild("TitleCard").AddComponent<TextMeshProUGUI>();
        titleText.font = TMP_Settings.defaultFontAsset;
        titleText.fontSize = titleFontSize;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = titleColor;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.raycastTarget = false;
        titleText.alpha = 0f;
    }

    private GameObject CreateFullScreenChild(string objectName)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform));
        RectTransform rect = (RectTransform)child.transform;
        rect.SetParent(transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return child;
    }

    private IEnumerator Transition(string sceneName, string titleCard)
    {
        isTransitioning = true;
        overlayGroup.blocksRaycasts = true;
        bool hasTitle = !string.IsNullOrEmpty(titleCard);
        titleText.text = hasTitle ? titleCard : string.Empty;
        titleText.alpha = 0f;

        yield return Fade(alpha => overlayGroup.alpha = alpha, overlayGroup.alpha, 1f, fadeDuration);

        if (hasTitle)
        {
            yield return Fade(alpha => titleText.alpha = alpha, 0f, 1f, titleFadeDuration);
        }

        float holdUntil = Time.unscaledTime + (hasTitle ? titleHoldDuration : 0f);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        if (loading == null)
        {
            Debug.LogError($"Scene \"{sceneName}\" could not be loaded. Check that it is in Build Settings.");
        }

        while ((loading != null && !loading.isDone) || Time.unscaledTime < holdUntil)
        {
            yield return null;
        }

        if (hasTitle)
        {
            yield return Fade(alpha => titleText.alpha = alpha, 1f, 0f, titleFadeDuration);
        }

        yield return Fade(alpha => overlayGroup.alpha = alpha, 1f, 0f, fadeDuration);

        overlayGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    private static IEnumerator Fade(Action<float> apply, float from, float to, float duration)
    {
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            apply(Mathf.SmoothStep(from, to, elapsed / duration));
            yield return null;
        }

        apply(to);
    }
}
