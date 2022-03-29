namespace Skeleton.Domain.Core.Models
{
    public abstract class Entity : NetDevPack.Domain.Entity
    {
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}