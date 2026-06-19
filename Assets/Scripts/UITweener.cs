using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lightweight animation system — no external libraries needed.
/// Call UITweener.FadeIn(panel), .ScaleIn(button), .SlideUp(panel), etc.
/// Attach to any GameObject; it manages its own coroutines.
/// </summary>
public class UITweener : MonoBehaviour
{
    public static UITweener Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ===================================================================
    // FADE
    // ===================================================================
    public void FadeIn(GameObject obj, float duration = 0.3f, float delay = 0f)
    {
        StartCoroutine(FadeCoroutine(obj, 0f, 1f, duration, delay));
    }

    public void FadeOut(GameObject obj, float duration = 0.3f, float delay = 0f)
    {
        StartCoroutine(FadeCoroutine(obj, 1f, 0f, duration, delay));
    }

    IEnumerator FadeCoroutine(GameObject obj, float from, float to, float duration, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = obj.AddComponent<CanvasGroup>();

        obj.SetActive(true);
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;

        if (to <= 0.01f) obj.SetActive(false);
    }

    // ===================================================================
    // SCALE (pop-in effect)
    // ===================================================================
    public void ScaleIn(GameObject obj, float duration = 0.3f, float delay = 0f)
    {
        StartCoroutine(ScaleCoroutine(obj, Vector3.zero, Vector3.one, duration, delay, true));
    }

    public void ScaleOut(GameObject obj, float duration = 0.25f, float delay = 0f)
    {
        StartCoroutine(ScaleCoroutine(obj, Vector3.one, Vector3.zero, duration, delay, false));
    }

    public void Punch(GameObject obj, float intensity = 0.15f, float duration = 0.2f)
    {
        StartCoroutine(PunchCoroutine(obj, intensity, duration));
    }

    IEnumerator ScaleCoroutine(GameObject obj, Vector3 from, Vector3 to, float duration, float delay, bool setActive)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        var rect = obj.GetComponent<RectTransform>();
        if (rect == null) yield break;

        if (setActive) obj.SetActive(true);
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseOutBack(elapsed / duration);
            rect.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }
        rect.localScale = to;

        if (!setActive) obj.SetActive(false);
    }

    IEnumerator PunchCoroutine(GameObject obj, float intensity, float duration)
    {
        var rect = obj.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector3 original = rect.localScale;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Pop up then back
            float punch = Mathf.Sin(t * Mathf.PI) * intensity;
            rect.localScale = original * (1f + punch);
            yield return null;
        }
        rect.localScale = original;
    }

    // ===================================================================
    // SLIDE
    // ===================================================================
    public void SlideUp(GameObject obj, float distance = 100f, float duration = 0.35f, float delay = 0f)
    {
        StartCoroutine(SlideCoroutine(obj, new Vector2(0, -distance), Vector2.zero, duration, delay));
    }

    public void SlideDown(GameObject obj, float distance = 100f, float duration = 0.35f, float delay = 0f)
    {
        StartCoroutine(SlideCoroutine(obj, new Vector2(0, distance), Vector2.zero, duration, delay));
    }

    IEnumerator SlideCoroutine(GameObject obj, Vector2 offset, Vector2 target, float duration, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        var rect = obj.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector2 startPos = target + offset;
        obj.SetActive(true);

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseOutCubic(elapsed / duration);
            rect.anchoredPosition = Vector2.LerpUnclamped(startPos, target, t);
            yield return null;
        }
        rect.anchoredPosition = target;
    }

    // ===================================================================
    // COLOR FLASH (for correct/wrong feedback)
    // ===================================================================
    public void FlashColor(Image img, Color flashColor, float duration = 0.3f)
    {
        StartCoroutine(FlashColorCoroutine(img, flashColor, duration));
    }

    IEnumerator FlashColorCoroutine(Image img, Color flashColor, float duration)
    {
        if (img == null) yield break;
        Color original = img.color;
        img.color = flashColor;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            img.color = Color.Lerp(flashColor, original, elapsed / duration);
            yield return null;
        }
        img.color = original;
    }

    // ===================================================================
    // EASING FUNCTIONS
    // ===================================================================
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
}
