using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InfoBook", menuName = "Breaking or Faking/Info Book")]
public class InfoBookData : DocumentData
{
    [SerializeField] private string title = "Daily Facts";
    [SerializeField] private List<InfoPage> pages = new List<InfoPage>();

    public string Title => title;
    public IReadOnlyList<InfoPage> Pages => pages;
    public override string Label => "Info book";

    // The info book is read-only reference, so it has no selectable sections.
    public override IEnumerable<Section> GetSections()
    {
        yield break;
    }
}

[Serializable]
public class InfoPage
{
    [SerializeField] private string heading;
    [TextArea(1, 4)]
    [SerializeField] private List<string> facts = new List<string>();

    public string Heading => heading;
    public IReadOnlyList<string> Facts => facts;
}
