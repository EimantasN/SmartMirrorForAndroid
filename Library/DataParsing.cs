using Android.App;
using Android.Widget;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

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
                && x.Attributes["class"].Value.Contains("nowvalue__text_l")).InnerText.Trim() +" °C";
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
                dataRecieved = mdataRecieved
            };
            WriteToConsole("Temperature data recieved", console);

            return model;
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

    }
}
