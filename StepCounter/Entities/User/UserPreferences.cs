using StepCounter.Enums;

namespace StepCounter.Entities;

public class UserPreferences
{
    public int DailyStepGoal { get; set; }
    public float DailyDistanceGoal { get; set; }
    public Theme  Theme { get; set; }
}