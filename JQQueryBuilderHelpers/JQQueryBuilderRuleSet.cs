using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JQQueryBuilderHelpers
{
    public class JQQueryBuilderRuleSet : IJQQueryBuilderRule
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("condition")]
        public JQQueryBuilderCondition Condition { get; set; }

        [JsonProperty("rules")]
        public IEnumerable<IJQQueryBuilderRule> Rules { get; set; }
    }
}