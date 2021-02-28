using System;

namespace BackendService.Domain.Entity
{
    public abstract class AuditableBaseEntity
    {
        public virtual int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateTime { get; set; }
        public int UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
