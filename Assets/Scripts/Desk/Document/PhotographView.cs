using UnityEngine;
using UnityEngine.UI;

public class PhotographView : DocumentView
{
    private const string DateFormat = "MM.dd.yy";

    [SerializeField] private Image photo;
    [SerializeField] private RectTransform textArea;

    public override bool CanShow(DocumentData data)
    {
        return data is PhotographData;
    }

    public override void Show(DocumentData data)
    {
        ClearContent();
        PhotographData photograph = (PhotographData)data;

        photo.sprite = photograph.Photo;
        photo.enabled = photograph.Photo != null;

        AddSectionIfWritten(photograph.Caption);
        AddSectionIfWritten(photograph.Credit);

        if (!string.IsNullOrWhiteSpace(photograph.Date))
        {
            AddText(photograph.FormatDate(DateFormat), SectionStyle.Body, textArea);
        }
    }

    private void AddSectionIfWritten(Section section)
    {
        if (section != null && !string.IsNullOrWhiteSpace(section.Text))
        {
            AddSection(section, textArea);
        }
    }
}
