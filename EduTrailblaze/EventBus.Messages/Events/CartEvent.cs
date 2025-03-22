using EventBus.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public record CartEvent() : IntegrationBaseEvent,ICartEvent
    {
        public int CourseId { get ; set ; }

    }
}
