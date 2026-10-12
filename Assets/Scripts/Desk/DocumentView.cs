using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DocumentView : MonoBehaviour
{
    [SerializeField] private Transform sectionContainer;
    [SerializeField] private NewsSectionView sectionPrefab;
    [SerializeField] private TMP_Text messageText;

    private readonly List<NewsSectionView> sectionViews = new List<NewsSectionView>();

    public event Action<NewsSectionView> SectionClicked;

    public void Show(NewsItemData item)
    {
        ClearSections();
        messageText.gameObject.SetActive(false);

        foreach (NewsSection section in item.Sections)
        {
            NewsSectionView view = Instantiate(sectionPrefab, sectionContainer);
            view.Setup(section);
            view.Clicked += () => SectionClicked?.Invoke(view);
            sectionViews.Add(view);
        }
    }

    public void ShowMessage(string message)
    {
        ClearSections();
        messageText.text = message;
        messageText.gameObject.SetActive(true);
    }

    private void ClearSections()
    {
        foreach (NewsSectionView view in sectionViews)
        {
            view.gameObject.SetActive(false);
            Destroy(view.gameObject);
        }

        sectionViews.Clear();
    }
}
