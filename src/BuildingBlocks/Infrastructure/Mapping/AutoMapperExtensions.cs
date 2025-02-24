using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var flag  = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flag);
            foreach (var prop in destinationProperties)
            {
                if (sourceType.GetProperty(prop.Name, flag) == null)
                {
                    expression.ForMember(prop.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }
    }
}
