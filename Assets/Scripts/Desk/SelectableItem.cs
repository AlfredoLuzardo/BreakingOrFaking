using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SelectableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color hoverColor = new Color(1f, 0.93f, 0.55f, 0.5f);
    [SerializeField] private Color selectedColor = new Color(1f, 0.82f, 0.2f, 0.9f);

    private bool isHovered;
    private bool isSelected;

    public event Action Clicked;

    public bool IsSelected => isSelected;

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
    }

    private void Awake()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (isSelected)
        {
            background.color = selectedColor;
        }
        else if (isHovered)
        {
            background.color = hoverColor;
        }
        else
        {
            background.color = normalColor;
        }
    }
}
