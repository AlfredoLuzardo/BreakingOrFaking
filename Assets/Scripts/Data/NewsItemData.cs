using UnityEngine;

[CreateAssetMenu(fileName = "NewsItem", menuName = "Breaking or Faking/News Item")]
public class NewsItemData : ScriptableObject
{
    [SerializeField] private string headline;
    [TextArea(3, 10)]
    [SerializeField] private string body;
    [SerializeField] private bool isReal;

    public string Headline => headline;
    public string Body => body;
    public bool IsReal => isReal;
}
