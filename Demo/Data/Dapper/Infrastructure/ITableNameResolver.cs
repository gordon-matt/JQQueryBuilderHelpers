using System;
using System.Collections.Generic;
using Demo.Data.Domain;

namespace Demo.Data.Dapper.Infrastructure
{
    public interface ITableNameResolver
    {
        string GetTableName(Type entityType);
    }

    public class TableNameResolver : ITableNameResolver
    {
        private static IDictionary<Type, string> tableNames = new Dictionary<Type, string>
        {
            { typeof(Person), "People" },
        };

        public string GetTableName(Type entityType)
        {
            if (tableNames.ContainsKey(entityType))
            {
                return tableNames[entityType];
            }

            return null;
        }
    }
}