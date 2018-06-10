using Android.App;
using Android.Widget;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class DataParsing
    {
        public TemperatureNow ParseTemperatureData(string city, TextView console)
        {
            HtmlDocument resultat = new HtmlDocument();
            if (city.Contains("Kaunas"))
            {
                WriteToConsole("Getting temperature data form city Kaunas", console);
                resultat.LoadHtml(GetPage("https://www.gismeteo.com/weather-kaunas-4202/now/"));

            }
            else if (city.Contains("Šiauliai"))
            {
                WriteToConsole("Getting temperature data form city Šiauliai", console);
                resultat.LoadHtml(GetPage("https://www.gismeteo.com/weather-šiauliai-4170/now/"));

            }
            else if (city.Contains("Tytuvėnai"))
            {
                WriteToConsole("Getting temperature data form city Tytuvėnai", console);
                resultat.LoadHtml(GetPage("https://www.gismeteo.com/weather-tytuvėnai-16201/now/"));
            }
            else
            {
                WriteToConsole("Getting temperature data form city Kaunas", console);
                resultat.LoadHtml(GetPage("https://www.gismeteo.com/weather-kaunas-4202/now/"));
            }
            string mImage = "";
            string mWind = "";
            string mPressure = "";
            string mHumidity = "";
            string mWater = "";
            string mSunrise = "";
            string mSunset = "";
            string mrealFeel = "";
            string mTemperature = "";
            string mdataRecieved = "";

            HtmlNode tbody = resultat.DocumentNode.Descendants().
                First(x =>
                (x.Name == "div") && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("forecast_wrap horizontal"));

            try
            {
                mTemperature = tbody.Descendants()
                .First(x =>
                (x.Name == "span")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("nowvalue__text_l")).InnerText.Trim();
            }
            catch { }

            mdataRecieved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                mSunrise = tbody.Descendants()
                .First(x =>
                (x.Name == "div")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("nowastro__time")).InnerText.Trim();
            }
            catch { }

            try
            {
                mSunset = tbody.Descendants()
                .Where(x =>
                (x.Name == "div")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("nowastro__time")).ElementAt(1).InnerText.Trim();
            }
            catch { }

            try
            {
                var realFellData = tbody.Descendants()
                .First(x =>
                (x.Name == "div")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("now__feel")).InnerText.Trim().Where(x => Char.IsDigit(x)).ToArray();
            mrealFeel = string.Join("", realFellData) +" °C";
            }
            catch { }

            try
            {
                mWind = tbody.Descendants()
                .First(x =>
                (x.Name == "div")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("nowinfo__item nowinfo__item_wind"))
                .Descendants()
                .First(x =>
                (x.Name == "div")
                && x.Attributes["class"] != null
                && x.Attributes["class"].Value.Contains("nowinfo__value")).InnerText.Trim() + " m/s";
            }
            catch { }

            try
            {
                mPressure = tbody.Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__item nowinfo__item_pressure"))
               .Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__value")).InnerText.Trim() + " mm/Hg";
            }
            catch { }

            try
            {
                mHumidity = tbody.Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__item nowinfo__item_humidity"))
               .Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__value")).InnerText.Trim() + " %";
            }
            catch { }

            try
            {
            mWater = tbody.Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__item nowinfo__item_water"))
               .Descendants()
               .First(x =>
               (x.Name == "div")
               && x.Attributes["class"] != null
               && x.Attributes["class"].Value.Contains("nowinfo__value")).InnerText.Trim() + " °C";
            }
            catch { }

            try
            {
                mImage = resultat.DocumentNode.Descendants()
                   .First(x =>
                   (x.Name == "div")
                   && x.Attributes["class"] != null
                   && x.Attributes["class"].Value.Contains("tab tooltip")).Attributes["data-text"].Value.Trim();
            }
            catch { }

            var model = new TemperatureNow
            {
                Wind = mWind,
                Pressure = mPressure,
                Humidity = mHumidity,
                Water = mWater,
                Sunrise = mSunrise,
                Sunset = mSunset,
                realFeel = mrealFeel,
                Temperature = mTemperature,
                dataRecieved = mdataRecieved,
                image =mImage
            };
            WriteToConsole("Temperature data recieved", console);

            return model;
        }
        public List<LinkomanijosData> GetLinkomanijosHdMovies()
        {
            List<LinkomanijosData> data = new List<LinkomanijosData>();
            string name = "";
            string subtext = "";
            string date = "";
            string size = "";
            string downloaded = "";
            string seeder = "";

            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(GetPageLinkomanija());

            HtmlNode tbody = resultat.DocumentNode.Descendants().
                Where(x =>
                (x.Name == "table")).ElementAt(3);

            List<HtmlNode> items = tbody.Descendants().
                Where(x =>
                (x.Name == "tr")).ToList();
            items.RemoveAt(0);

            items.Take(3).ToList().ForEach(x =>
            {
                var rows = x.Descendants().Where(y => y.Name == "td").ToList();

                name = rows.ElementAt(1).Descendants().First(y => y.Name == "b").InnerText.Trim();
                subtext = rows.ElementAt(1).Descendants().First(y => y.Name == "span").InnerText.Trim();
                date = rows.ElementAt(4).Descendants().First(y => y.Name == "nobr").InnerHtml.Replace("<br>", " ").Trim();
                size = rows.ElementAt(5).InnerHtml.Replace("<br>", " ").Trim();
                downloaded = rows.ElementAt(6).InnerText.Trim();
                seeder = rows.ElementAt(7).Descendants().First(y => y.Name == "span").InnerText.Trim();

                var model = new LinkomanijosData
                {
                    name = name,
                    subtext = subtext,
                    date = date,
                    size = size,
                    downloaded = downloaded,
                    seeder = seeder
                };

                data.Add(model);
            });
            return data;
        }

        public TransportTrafi TrafiApi()
        {
            //Nuo manes iki griunvaldo gatves
            string HomeToAkro = "http://api-ext.trafi.com/routes?start_lat=54.901694&start_lng=23.961288&end_lat=54.893767&end_lng=23.925123&is_arrival=false&api_key=cb99332fe299a1e5a5b8d434c03b24f9";

            string HomeToGym = "http://api-ext.trafi.com/routes?start_lat=54.901694&start_lng=23.961288&end_lat=54.915975&end_lng=23.960933&is_arrival=false&api_key=cb99332fe299a1e5a5b8d434c03b24f9";

            var model = new TransportTrafi
            {
                HomeToAkro = getTrafiData(HomeToAkro),
                HomeToGym = getTrafiData(HomeToGym)
            };

            return model;
        }

        public RootObject getTrafiData(string url)
        {
            using (var client = new HttpClient())
            {

                HttpResponseMessage response = client.GetAsync(url).Result;

                response.EnsureSuccessStatusCode();

                using (HttpContent content = response.Content)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<RootObject>(responseBody);
                }
            }
        }

        public string GetPage(string url)
        {
            try
            {
                HttpClient http = new HttpClient();
                var response = http.GetByteArrayAsync(url).Result;
                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                return source;
            }
            catch
            {
                Console.WriteLine("Klaida gaunant puslapi");
                return "";
            };
        }

        public void WriteToConsole(string newtext, TextView console)
        {
            Application.SynchronizationContext.Post(_ => { console.Text = newtext + "  time - " + DateTime.Now.ToLocalTime().ToString() + "\n" + console.Text; }, null);
        }

        public static string GetPageLinkomanija()
        {
            try
            {
                var httpClient = new HttpClient();
                var httpContent = new HttpRequestMessage(HttpMethod.Post, "https://www.linkomanija.net/takelogin.php");


                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", "eimis12371"));
                values.Add(new KeyValuePair<string, string>("password", "napaleon"));
                values.Add(new KeyValuePair<string, string>("commit", "Prisijungti"));
                values.Add(new KeyValuePair<string, string>("returnto", "/browse.php?cat=52&sort=seeders&h=10&d=DESC"));

                httpContent.Content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = httpClient.SendAsync(httpContent).Result;

                var result = response.Content.ReadAsStringAsync().Result;

                //String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                String source = WebUtility.HtmlDecode(result);
                return source;
            }
            catch
            {
                Console.WriteLine("Klaida gaunant puslapi");
                return "";
            };
        }

    }
}
