using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages
{
    public record IntegrationBaseEvent(): IIntegrationEvent
    {
        public DateTime CreationDate { get;  } = DateTime.UtcNow;
        public Guid Id { get; set; } 
    }
}
