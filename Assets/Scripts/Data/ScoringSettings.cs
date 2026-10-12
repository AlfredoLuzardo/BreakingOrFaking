using UnityEngine;

[CreateAssetMenu(fileName = "ScoringSettings", menuName = "Breaking or Faking/Scoring Settings")]
public class ScoringSettings : ScriptableObject
{
    [SerializeField] private int correctDecisionPoints = 1;
    [SerializeField] private int wrongDecisionPoints = -1;

    public int CorrectDecisionPoints => correctDecisionPoints;
    public int WrongDecisionPoints => wrongDecisionPoints;
}
