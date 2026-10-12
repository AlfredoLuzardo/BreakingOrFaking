using System.Collections.Generic;

// Holds the current day's decisions and score between scenes.
public static class GameSession
{
    private static readonly List<DecisionRecord> decisions = new List<DecisionRecord>();

    public static int DayNumber { get; private set; }
    public static int Score { get; private set; }
    public static IReadOnlyList<DecisionRecord> Decisions => decisions;

    public static void StartDay(int dayNumber)
    {
        DayNumber = dayNumber;
        Score = 0;
        decisions.Clear();
    }

    public static void AddDecision(DecisionRecord decision, int points)
    {
        decisions.Add(decision);
        Score += points;
    }
}
