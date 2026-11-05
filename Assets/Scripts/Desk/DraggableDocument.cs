using UnityEngine;
using UnityEngine.EventSystems;

// Lets the player pick a document up, move it around its parent (the desk) and drop it.
// Clicking a document brings it to the front. The document can't be dragged off the desk.
[RequireComponent(typeof(RectTransform))]
public class DraggableDocument : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("How much bigger the document gets while held, so it feels picked up.")]
    [SerializeField] private float liftScale = 1.03f;

    private readonly Vector3[] corners = new Vector3[4];
    private RectTransform rect;
    private RectTransform desk;
    private Vector2 grabOffset;
    private Vector3 restingScale;
    private bool isDragging;

    private void Awake()
    {
        rect = (RectTransform)transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rect.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        desk = rect.parent as RectTransform;
        if (desk == null || !TryGetDeskPoint(eventData, out Vector2 point))
        {
            return;
        }

        isDragging = true;
        grabOffset = (Vector2)rect.localPosition - point;
        restingScale = rect.localScale;
        rect.localScale = restingScale * liftScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && TryGetDeskPoint(eventData, out Vector2 point))
        {
            rect.localPosition = point + grabOffset;
            KeepOnDesk();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rect.localScale = restingScale;
            isDragging = false;
        }
    }

    private bool TryGetDeskPoint(PointerEventData eventData, out Vector2 point)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            desk, eventData.position, eventData.pressEventCamera, out point);
    }

    private void KeepOnDesk()
    {
        rect.GetWorldCorners(corners);
        Vector2 min = desk.InverseTransformPoint(corners[0]);
        Vector2 max = min;
        for (int i = 1; i < corners.Length; i++)
        {
            Vector2 corner = desk.InverseTransformPoint(corners[i]);
            min = Vector2.Min(min, corner);
            max = Vector2.Max(max, corner);
        }

        Rect bounds = desk.rect;
        float shiftX = Overshoot(min.x, max.x, bounds.xMin, bounds.xMax);
        float shiftY = Overshoot(min.y, max.y, bounds.yMin, bounds.yMax);
        rect.localPosition += new Vector3(shiftX, shiftY, 0f);
    }

    // How far to move back so [min, max] fits inside [lower, upper]. Too big to fit: centre it.
    private static float Overshoot(float min, float max, float lower, float upper)
    {
        if (max - min > upper - lower)
        {
            return (lower + upper - min - max) / 2f;
        }

        if (min < lower)
        {
            return lower - min;
        }

        return max > upper ? upper - max : 0f;
    }
}
