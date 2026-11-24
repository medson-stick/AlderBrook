using System.Collections;
using UnityEngine;

// Fades a full-screen black CanvasGroup in and out. The overlay canvas is authored in the scene.
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    // Makes sure the overlay starts fully transparent and non-interactive.
    private void Awake()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    // Fades the screen to black.
    public IEnumerator FadeOut(float duration)
    {
        return FadeTo(1f, duration);
    }

    // Fades the screen back in.
    public IEnumerator FadeIn(float duration)
    {
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
}
