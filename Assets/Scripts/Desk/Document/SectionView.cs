using TMPro;
using UnityEngine;

public class SectionView : SelectableItem
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float titleFontSize = 44f;
    [SerializeField] private float headerFontSize = 32f;
    [SerializeField] private float bodyFontSize = 26f;

    public Section Section { get; private set; }

    public void Setup(Section section)
    {
        Section = section;
        enabled = true;
        SetText(section.Text, section.Style);
    }

    // Item metadata and page headings are display-only, not selectable evidence.
    public void SetupText(string text, SectionStyle style)
    {
        Section = null;
        SetSelected(false);
        enabled = false;
        SetText(text, style);
    }

    private void SetText(string text, SectionStyle style)
    {
        label.text = text;

        switch (style)
        {
            case SectionStyle.Title:
                label.fontSize = titleFontSize;
                label.fontStyle = FontStyles.Bold;
                break;
            case SectionStyle.Header:
                label.fontSize = headerFontSize;
                label.fontStyle = FontStyles.Bold;
                break;
            default:
                label.fontSize = bodyFontSize;
                label.fontStyle = FontStyles.Normal;
                break;
        }
    }
}
