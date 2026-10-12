using TMPro;
using UnityEngine;

public class NewsSectionView : SelectableItem
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float titleFontSize = 44f;
    [SerializeField] private float headerFontSize = 32f;
    [SerializeField] private float bodyFontSize = 26f;

    public NewsSection Section { get; private set; }

    public void Setup(NewsSection section)
    {
        Section = section;
        label.text = section.Text;

        switch (section.Style)
        {
            case NewsSectionStyle.Title:
                label.fontSize = titleFontSize;
                label.fontStyle = FontStyles.Bold;
                break;
            case NewsSectionStyle.Header:
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
