using System.ComponentModel;

namespace Skeleton.Domain.Enums
{
    public enum Status
    {
        [Description("Em andamento")] InProgress = 0,
        [Description("Finalizado")] Finished = 1
    }
}