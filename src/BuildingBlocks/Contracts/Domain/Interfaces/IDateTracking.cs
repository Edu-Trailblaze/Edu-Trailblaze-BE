using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Domain.Interfaces
{
    public interface IDateTracking
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }

    }
}
