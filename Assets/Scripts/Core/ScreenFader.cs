using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Fades a full-screen black overlay in and out.
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;

    // Keeps only one fader and creates the overlay if needed.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureFadeCanvas();
        canvasGroup.alpha = 0f;
    }

    // Fades the screen to black.
    public IEnumerator FadeOut(float duration)
    {
        EnsureFadeCanvas();
        return FadeTo(1f, duration);
    }

    // Fades the screen back in.
    public IEnumerator FadeIn(float duration)
    {
        EnsureFadeCanvas();
        return FadeTo(0f, duration);
    }

    // Lerps the overlay alpha over time.
    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    // Creates a full-screen black overlay when one is not assigned.
    private void EnsureFadeCanvas()
    {
        if (canvasGroup != null) return;

        GameObject canvasObject = new GameObject("ScreenFaderCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        GameObject imageObject = new GameObject("FadeImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image image = imageObject.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
