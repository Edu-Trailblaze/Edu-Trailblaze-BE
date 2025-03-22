using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages
{
    public interface IIntegrationEvent
    {
        DateTime CreationDate { get; }
        Guid Id { get; set; }
    }
}
