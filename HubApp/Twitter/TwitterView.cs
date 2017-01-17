using System.IO;
using System.Web.Helpers;
using Tweetinvi;

namespace HubApp.Twitter
{
    class TwitterView
    {
        private const string CREDENTIALS_FILE = "Resources/keys.json";

        /// <summary>
        ///     Instantiates the Twitter class. Potentially will have parameters.
        /// </summary>
        public TwitterView()
        {
            string userCredentials = CollectUserCredentials();
            AuthenticateUser(userCredentials);
        }

        /// <summary>
        ///     Retreives user credentials
        /// </summary>
        /// <returns>
        ///     string containing the contents of the key json file.
        /// </returns>
        private string CollectUserCredentials()
        {
            string jsonContents = File.ReadAllText(CREDENTIALS_FILE);
            return jsonContents;

        }

        /// <summary>
        ///     Authenticates the user based on the json string.
        /// </summary>
        /// <param name="json"></param>
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

        /// <summary>
        ///     Publishes a post to the user's account to verify the user authentication was valid.
        /// </summary>
        /// <param name="message"></param>
        private void TestAuthentication(string message)
        {
            Tweet.PublishTweet(message);
        }
    }
}
