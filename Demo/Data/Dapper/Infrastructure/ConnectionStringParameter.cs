using System;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Configuration;

namespace Demo.Data.Dapper.Infrastructure
{
    public class ConnectionStringParameter : Parameter
    {
        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            valueProvider = null;

            if (pi.ParameterType != typeof(string) || pi.Name != "connectionString")
            {
                return false;
            }

            valueProvider = () =>
            {
                var configuration = context.Resolve<IConfiguration>();
                return configuration.GetConnectionString("DefaultConnection");
            };

            return true;
        }
    }
}