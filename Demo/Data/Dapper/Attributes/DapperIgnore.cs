using System;

namespace Demo.Data.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }
}