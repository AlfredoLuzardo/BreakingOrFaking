using UnityEngine.EventSystems;
using UnityEngine.UI;

// A ScrollRect that only scrolls with the mouse wheel and the scrollbar.
// Dragging is left to DraggableDocument, so dragging a document moves it instead of scrolling it.
public class WheelScrollRect : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
    }

    public override void OnDrag(PointerEventData eventData)
    {
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
    }
}
