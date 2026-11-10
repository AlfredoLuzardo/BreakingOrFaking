using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Case", menuName = "Breaking or Faking/Story Case")]
public class StoryCase : ScriptableObject
{
    [Tooltip("The article the player approves or denies.")]
    [SerializeField] private ArticleData article;
    [Tooltip("Other documents on the desk for this story: photos, statements, records...")]
    [SerializeField] private List<DocumentData> evidence = new List<DocumentData>();
    [Tooltip("The answer: on if the article should be approved.")]
    [SerializeField] private bool isTrue = true;

    public ArticleData Article => article;
    public IReadOnlyList<DocumentData> Evidence => evidence;
    public bool IsTrue => isTrue;
    public string Title => article != null ? article.Title : name;

    public IEnumerable<DocumentData> AllDocuments()
    {
        if (article != null)
        {
            yield return article;
        }

        foreach (DocumentData document in evidence)
        {
            if (document != null)
            {
                yield return document;
            }
        }
    }

    private void OnValidate()
    {
        if (article == null)
        {
            Debug.LogWarning($"{name}: a case needs an article to approve or deny.", this);
            return;
        }

        bool hasContradictions = AllDocuments().Any(document => document.HasContradictions());
        if (isTrue && hasContradictions)
        {
            Debug.LogWarning($"{name}: a true case contains rule violations. Check Is True and the rule links.", this);
        }
        else if (!isTrue && !hasContradictions)
        {
            Debug.LogWarning($"{name}: a false case has no rule links to explain its answer.", this);
        }
    }
}
