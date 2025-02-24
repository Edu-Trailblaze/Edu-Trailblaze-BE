using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts.Domain.Interfaces;
using System.Threading.Tasks;

namespace Contracts.Domain
{
    public abstract class EntityAuditBase<T> : EntityBase<T>, IAuditable
    {
       public  DateTimeOffset CreatedAt { get; set; }
       public DateTimeOffset? UpdatedAt { get; set; }
       
    }
}
