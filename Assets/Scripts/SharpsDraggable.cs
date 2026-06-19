using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Put this on the needle UI Image (inside sharpsPanel). Drag it into the
/// sharps bin to dispose safely. Set 'bin' to the bin's RectTransform.
/// Reports success/failure to MiniGameController on release.
/// </summary>
[RequireComponent(typeof(Image))]
public class SharpsDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform bin;
    public MiniGameController miniGames;
    public float dropRadius = 90f; // px; how close to the bin counts as "in"

    private RectTransform _rt;
    private Vector2 _start;
    private Canvas _canvas;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _start = _rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData e) { }

    public void OnDrag(PointerEventData e)
    {
        float scale = (_canvas != null) ? _canvas.scaleFactor : 1f;
        _rt.anchoredPosition += e.delta / scale;
    }

    public void OnEndDrag(PointerEventData e)
    {
        bool success = bin != null &&
            Vector2.Distance(_rt.position, bin.position) <= dropRadius;

        if (!success) _rt.anchoredPosition = _start; // snap back on miss
        if (miniGames != null) miniGames.CompleteSharps(success);
    }
}
