using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Domain.Interfaces;

namespace Contracts.Domain
{
    public class EntityBase<TKey> : IEntityBase<TKey>  
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; set; }
    }
}
