using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HubApp.Utilities;

namespace HubApp.TriviaHelper
{
    //[JsonConverter(typeof(HtmlConverter<Result>))]
    public class Result
    {
        [JsonProperty(PropertyName = "category")]
        //[Html]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "type")]
        //[Html]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        //[Html]
        public string Difficulty { get; set; }

        [JsonProperty(PropertyName = "question")]
        //[Html]
        public string Question { get; set; }

        [JsonProperty(PropertyName = "correct_answer")]
        //[Html]
        public string CorrectAnswer { get; set; }

        [JsonProperty(PropertyName = "incorrect_answers")]
        //[Html]
        public List<string> IncorrectAnswers { get; set; }
    }
}
