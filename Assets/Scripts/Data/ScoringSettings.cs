using UnityEngine;

[CreateAssetMenu(fileName = "ScoringSettings", menuName = "Breaking or Faking/Scoring Settings")]
public class ScoringSettings : ScriptableObject
{
    [Header("Decisions")]
    [SerializeField] private int correctDecisionPoints = 100;
    [SerializeField] private int wrongDecisionPoints = -50;

    [Header("Rulebook checks")]
    [Tooltip("Points for each contradiction found. Each section and rule pair only scores once per day.")]
    [SerializeField] private int contradictionFoundPoints = 25;
    [Tooltip("Points for each check that shows NO CONTRADICTION, once the free wrong checks are used up.")]
    [SerializeField] private int wrongCheckPoints = -10;
    [SerializeField] private int freeWrongChecksPerDay = 3;

    public int CorrectDecisionPoints => correctDecisionPoints;
    public int WrongDecisionPoints => wrongDecisionPoints;
    public int ContradictionFoundPoints => contradictionFoundPoints;
    public int WrongCheckPoints => wrongCheckPoints;
    public int FreeWrongChecksPerDay => freeWrongChecksPerDay;
}
