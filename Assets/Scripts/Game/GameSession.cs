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

    public static int CorrectCalls { get; private set; }
    public static int CorrectCallPoints { get; private set; }
    public static int WrongCalls { get; private set; }
    public static int WrongCallPoints { get; private set; }
    public static int ContradictionsFound { get; private set; }
    public static int ContradictionPoints { get; private set; }
    public static int WrongChecks { get; private set; }
    public static int PenalizedWrongChecks { get; private set; }
    public static int WrongCheckPoints { get; private set; }

    // Resets the session each time Play mode starts.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        StartNewGame();
    }

    public static void StartNewGame()
    {
        DayIndex = 0;
        TotalScore = 0;
        BeginDay(0);
    }

    public static void BeginDay(int dayNumber)
    {
        DayNumber = dayNumber;
        DayScore = 0;
        decisions.Clear();
        CorrectCalls = 0;
        CorrectCallPoints = 0;
        WrongCalls = 0;
        WrongCallPoints = 0;
        ContradictionsFound = 0;
        ContradictionPoints = 0;
        WrongChecks = 0;
        PenalizedWrongChecks = 0;
        WrongCheckPoints = 0;
    }

    public static void AddDecision(DecisionRecord decision, int points)
    {
        decisions.Add(decision);
        DayScore += points;

        if (decision.IsCorrect)
        {
            CorrectCalls++;
            CorrectCallPoints += points;
        }
        else
        {
            WrongCalls++;
            WrongCallPoints += points;
        }
    }

    public static void AddContradictionFound(int points)
    {
        ContradictionsFound++;
        ContradictionPoints += points;
        DayScore += points;
    }

    public static void AddWrongCheck(int points)
    {
        WrongChecks++;
        WrongCheckPoints += points;
        DayScore += points;

        if (points != 0)
        {
            PenalizedWrongChecks++;
        }
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
