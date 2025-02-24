using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Domain.Exceptions
{
    public class InvalidEntityType : ApplicationException
    {
        public InvalidEntityType(string entity,string type)
            : base($"Entity \"{entity}\" not support type: \"{type}\".")
        {
        }
    }
}
