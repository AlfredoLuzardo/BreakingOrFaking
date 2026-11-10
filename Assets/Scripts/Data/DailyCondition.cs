using UnityEngine;

[CreateAssetMenu(fileName = "DailyCondition", menuName = "Breaking or Faking/Daily Condition")]
public class DailyCondition : ScriptableObject
{
    [SerializeField] private string weather = "Sunny";
    [SerializeField] private string degree = "25C";
    [SerializeField] private Sprite icon;

    public string Weather => weather;
    public string Degree => degree;
    public Sprite Icon => icon;
}
