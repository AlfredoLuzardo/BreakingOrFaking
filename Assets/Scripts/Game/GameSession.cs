using System.Collections.Generic;
using UnityEngine;

// Holds the current day, its decisions and the scores between scenes.
public static class GameSession
{
    private static readonly List<DecisionRecord> decisions = new List<DecisionRecord>();

    public static int DayIndex { get; private set; }
    public static int DayNumber { get; private set; }
    public static int DayScore { get; private set; }
    public static int TotalScore { get; private set; }
    public static IReadOnlyList<DecisionRecord> Decisions => decisions;

    // Resets the session each time Play mode starts.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        StartNewGame();
    }

    public static void StartNewGame()
    {
        DayIndex = 0;
        DayNumber = 0;
        DayScore = 0;
        TotalScore = 0;
        decisions.Clear();
    }

    public static void BeginDay(int dayNumber)
    {
        DayNumber = dayNumber;
        DayScore = 0;
        decisions.Clear();
    }

    public static void AddDecision(DecisionRecord decision, int points)
    {
        decisions.Add(decision);
        DayScore += points;
    }

    public static void AddPoints(int points)
    {
        DayScore += points;
    }

    public static void FinishDay()
    {
        TotalScore += DayScore;
    }

    public static void AdvanceToNextDay()
    {
        DayIndex++;
    }
}
