//Requires Tweetinvi (0.9.10.2) https://github.com/linvi/tweetinvi
//Parses a tweet entities to basic HTML

using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models;

namespace HubApp.Twitter
{
    class TweetHelper
    {
        public static List<EntityHelper> ParseTweet(ITweet tweet)
        {
            // string outputText = tweet.FullText;

            if (tweet.Entities != null)
            {
                var Entities = new List<EntityHelper>();

                if (tweet.Entities.UserMentions != null)
                {
                    foreach (var item in tweet.Entities.UserMentions)
                    {
                        Entities.Add(new EntityHelper
                        {
                            Text = item.ScreenName,
                            UserId = item.Id ?? 0,
                            UserFullName = item.Name,
                            UserScreenName = item.ScreenName,
                            Indice = item.Indices.ToArray(),
                            EntityType = EntityHelper.entityType.UserMention,
                        });
                    }
                }

                if (tweet.Entities.Urls != null)
                {
                    foreach (var item in tweet.Entities.Urls)
                    {
                        Entities.Add(new EntityHelper
                        {
                            Text = item.URL,
                            DisplayedUrl = item.DisplayedURL,
                            ExpandedUrl = item.ExpandedURL,
                            Indice = item.Indices,
                            Url = item.URL,
                            EntityType = EntityHelper.entityType.Url,
                        });
                    }
                }

                if (tweet.Entities.Hashtags != null)
                {
                    foreach (var item in tweet.Entities.Hashtags)
                    {
                        Entities.Add(new EntityHelper
                        {
                            Text = item.Text,
                            Indice = item.Indices,
                            EntityType = EntityHelper.entityType.Hashtag,
                        });
                    }
                }

                if (tweet.Entities.Medias != null)
                {
                    foreach (var item in tweet.Entities.Medias)
                    {
                        Entities.Add(new EntityHelper
                        {
                            Text = item.URL,
                            Indice = item.Indices,
                            DisplayedUrl = item.DisplayURL,
                            ExpandedUrl = item.ExpandedURL,
                            MediaUrl = item.MediaURLHttps,
                            MediaType = item.MediaType,
                            EntityType = EntityHelper.entityType.Media,
                        });
                    }
                }

                //if (tweet.Entities.Symbols != null)
                //{
                //    foreach (var item in tweet.Entities.Symbols)
                //    {
                //        Entities.Add(new EntityHelper
                //        {
                //            Text = item.Text,
                //            Indice = item.Indices,
                //            EntityType = EntityHelper.entityType.Symbol,
                //        });
                //    }
                //}

                Entities.Sort((x, y) => x.Indice[0].CompareTo(y.Indice[0]));
                return Entities;
            }
            return new List<EntityHelper>();
        }
    }

    class EntityHelper
    {
        public enum entityType { Hashtag, Media, Symbol, Url, UserMention }
        public entityType EntityType;
        public int[] Indice = new int[2];
        public string Text = string.Empty;
        public string DisplayedUrl;
        public string ExpandedUrl;
        public string Url;
        public long UserId;
        public string UserFullName;
        public string UserScreenName;
        public string MediaType;
        public string MediaUrl;

    }
}