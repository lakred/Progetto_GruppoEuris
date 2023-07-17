namespace ConcordiaDBLibrary.Models.Extensions;

using Models.Abstract;
using Models.Classes;

public static class ExperimentExtension
{
    public static int OrderingByPriority(this Experiment experiment)
    {
        if (experiment.Priority is not null)
        {
            if (experiment.Priority.Name.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }
            else if (experiment.DueDate < DateTimeOffset.Now.AddDays(5))
            {
                return 1;
            }
            else if (experiment.Priority.Name.Equals("MEDIUM", StringComparison.OrdinalIgnoreCase))
            {
                return 2;
            }
            else if (experiment.Priority.Name.Equals("LOW", StringComparison.OrdinalIgnoreCase))
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
        else
        {
            return 5;
        }
    }

    public static int Ordering(this Experiment experiment)
    {
        if (experiment.State is not null)
        {
            if (!experiment.State.Name.Equals("FINISH", StringComparison.OrdinalIgnoreCase))
            {
                return experiment.OrderingByPriority();
            }
            else
            {
                return 4;
            }
        }
        else
        {
            return 5;
        }
    }
}