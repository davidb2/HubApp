using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Net;
using Newtonsoft.Json;
using HubApp.Utilities;

namespace HubApp.TriviaHelper
{
    //[JsonConverter(typeof(HtmlConverter<Question>))]
    public class Question
    {
        [JsonProperty(PropertyName = "response_code")]
        public int ResponseCode { get; set; }
        [JsonProperty(PropertyName = "results")]
        public List<Result> Results { get; set; }

        public void Normalize()
        {
            foreach (Result result in Results)
            {
                result.Category = WebUtility.HtmlDecode(result.Category);
                result.CorrectAnswer = WebUtility.HtmlDecode(result.CorrectAnswer);
                result.Difficulty = WebUtility.HtmlDecode(result.Difficulty);
                result.Question = WebUtility.HtmlDecode(result.Question);
                result.Type = WebUtility.HtmlDecode(result.Type);
                for (int i = 0; i < result.IncorrectAnswers.Count; i++)
                {
                    result.IncorrectAnswers[i] = WebUtility.HtmlDecode(result.IncorrectAnswers[i]);
                }
            }
        }
    }
}
