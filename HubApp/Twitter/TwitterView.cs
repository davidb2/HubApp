using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Helpers;
using System.Reflection;
using Tweetinvi;

namespace HubApp.Twitter
{
    class TwitterView
    {
        private const string CREDENTIALS_FILE = "Resources/keys.json";
        public TwitterView()
        {
            string userCredentials = CollectUserCredentials();
            AuthenticateUser(userCredentials);
            TestAuthentication("Hello, World! This message was sent by a C# program.");
        }

        private string CollectUserCredentials()
        {
            string jsonContents = File.ReadAllText(CREDENTIALS_FILE);
            return jsonContents;

        }
        private void AuthenticateUser(string json)
        {
            dynamic keys = Json.Decode(json);
            Auth.SetUserCredentials(
                keys.ConsumerKey,
                keys.ConsumerSecret,
                keys.AccessToken,
                keys.AccessTokenSecret
            );
        }
        private void TestAuthentication(string message)
        {
            Tweet.PublishTweet(message);
        }
    }
}
