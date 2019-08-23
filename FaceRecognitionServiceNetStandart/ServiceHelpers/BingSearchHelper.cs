using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FaceRecognitionService.ServiceHelpers
{
    public class BingSearchHelper
    {
        private static string ImageSearchEndPoint = "https://api.cognitive.microsoft.com/bing/v5.0/images/search";
        private static string AutoSuggestionEndPoint = "https://api.cognitive.microsoft.com/bing/v5.0/suggestions";
        private static string NewsSearchEndPoint = "https://api.cognitive.microsoft.com/bing/v5.0/news/search";

        private static HttpClient autoSuggestionClient { get; set; }
        private static HttpClient searchClient { get; set; }

        private static string autoSuggestionApiKey;
        public static string AutoSuggestionApiKey
        {
            get { return autoSuggestionApiKey; }
            set
            {
                var changed = autoSuggestionApiKey != value;
                autoSuggestionApiKey = value;
                if (changed)
                {
                    InitializeBingClients();
                }
            }
        }

        private static string searchApiKey;
        public static string SearchApiKey
        {
            get { return searchApiKey; }
            set
            {
                var changed = searchApiKey != value;
                searchApiKey = value;
                if (changed)
                {
                    InitializeBingClients();
                }
            }
        }

        private static void InitializeBingClients()
        {
            autoSuggestionClient = new HttpClient();
            autoSuggestionClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AutoSuggestionApiKey);

            searchClient = new HttpClient();
            searchClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SearchApiKey);
        }

        public static async Task<IEnumerable<string>> GetImageSearchResults(string query, string imageContent = "Face", int count = 20, int offset = 0)
        {
            List<string> urls = new List<string>();

            var result = await searchClient.GetAsync(string.Format("{0}?q={1}&safeSearch=Strict&imageType=Photo&color=ColorOnly&count={2}&offset={3}{4}", ImageSearchEndPoint, WebUtility.UrlEncode(query), count, offset, string.IsNullOrEmpty(imageContent) ? "" : "&imageContent=" + imageContent));
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            if (data.value != null && data.value.Count > 0)
            {
                for (int i = 0; i < data.value.Count; i++)
                {
                    urls.Add(data.value[i].contentUrl.Value);
                }
            }

            return urls;
        }

        public static async Task<IEnumerable<string>> GetAutoSuggestResults(string query, string market = "en-US")
        {
            List<string> suggestions = new List<string>();

            var result = await autoSuggestionClient.GetAsync(string.Format("{0}/?q={1}&mkt={2}", AutoSuggestionEndPoint, WebUtility.UrlEncode(query), market));
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            if (data.suggestionGroups != null && data.suggestionGroups.Count > 0 &&
                data.suggestionGroups[0].searchSuggestions != null)
            {
                for (int i = 0; i < data.suggestionGroups[0].searchSuggestions.Count; i++)
                {
                    suggestions.Add(data.suggestionGroups[0].searchSuggestions[i].displayText.Value);
                }
            }

            return suggestions;
        }


        public static async Task<IEnumerable<NewsArticle>> GetNewsSearchResults(string query, int count = 20, int offset = 0, string market = "en-US")
        {
            List<NewsArticle> articles = new List<NewsArticle>();

            var result = await searchClient.GetAsync(string.Format("{0}/?q={1}&count={2}&offset={3}&mkt={4}", NewsSearchEndPoint, WebUtility.UrlEncode(query), count, offset, market));
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);

            if (data.value != null && data.value.Count > 0)
            {
                for (int i = 0; i < data.value.Count; i++)
                {
                    articles.Add(new NewsArticle
                    {
                        Title = data.value[i].name,
                        Url = data.value[i].url,
                        Description = data.value[i].description,
                        ThumbnailUrl = data.value[i].image?.thumbnail?.contentUrl,
                        Provider = data.value[i].provider?[0].name
                    });
                }
            }
            return null;
        }

    }

    public class NewsArticle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Provider { get; set; }
    }
}