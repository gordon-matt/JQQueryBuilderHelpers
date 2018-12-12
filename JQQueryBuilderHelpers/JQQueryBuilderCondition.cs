using System.Runtime.Serialization;

namespace JQQueryBuilderHelpers
{
    public enum JQQueryBuilderCondition : byte
    {
        [EnumMember(Value = "AND")]
        And,

        [EnumMember(Value = "OR")]
        Or
    }
}