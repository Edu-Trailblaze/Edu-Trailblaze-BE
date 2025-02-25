namespace Cart.Domain.Exceptions
{
    public class InvalidEntityType : ApplicationException
    {
        public InvalidEntityType(string entity, string type)
            : base($"Entity \"{entity}\" not support type: \"{type}\".")
        {
        }
    }
}
