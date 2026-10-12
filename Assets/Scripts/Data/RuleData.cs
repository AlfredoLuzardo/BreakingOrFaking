using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "Breaking or Faking/Rule")]
public class RuleData : ScriptableObject
{
    [TextArea(2, 4)]
    [SerializeField] private string text;

    public string Text => text;
}
