using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T :  new()
        {
           using  var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection(sectionName);
            var option  = new T();
            section.Bind(option);
            return option;
        }
    }
}
