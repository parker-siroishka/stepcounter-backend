using System.ComponentModel.DataAnnotations;
using StepCounter.Enums;

namespace StepCounter.Entities.User;

public class UserPreferences
{
    [Key]
    public Guid UserId { get; set; }
    public int DailyStepGoal { get; set; }
    public double DailyDistanceGoal { get; set; }
    public Theme  Theme { get; set; }
    public Unit Unit { get; set; }

    public UserPreferences()
    {
        UserId = Guid.NewGuid();
        DailyStepGoal = 10000;
        DailyDistanceGoal = 8.0;
        Theme = Theme.Light;
        Unit = Unit.Metric;
    }
}