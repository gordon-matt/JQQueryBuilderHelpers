using System.Runtime.Serialization;

namespace JQQueryBuilderHelpers
{
    public enum JQQueryBuilderInputType : byte
    {
        [EnumMember(Value = "text")]
        Text,

        [EnumMember(Value = "textarea")]
        TextArea,

        [EnumMember(Value = "radio")]
        Radio,

        [EnumMember(Value = "checkbox")]
        Checkbox,

        [EnumMember(Value = "select")]
        Select
    }
}