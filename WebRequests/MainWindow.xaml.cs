using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json.Linq;

namespace WebRequests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // classe maken met date en al !!!
        string date = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        //public async Task Login(string cookieName, string CookieValue)
        //{
        //    var baseAddress = new Uri("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/Logon.action");
        //    var cookieContainer = new CookieContainer();
        //    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        var content = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("username", "volkani"),
        //            new KeyValuePair<string, string>("password", "Maas1234"),
        //        });
        //        cookieContainer.Add(baseAddress, new Cookie(cookieName, CookieValue));
        //        var result = await client.PostAsync("/VLDailyRetail_Hs81Lz82/Logon.action", content);
        //        result.EnsureSuccessStatusCode();
        //        var responseString = await result.Content.ReadAsStringAsync();
        //    }
        //}

        //public async Task GetEmployeeId(string cookieName, string CookieValue)
        //{
        //    var baseAddress = new Uri("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/Calendar_select.action?_dc=");
        //    var cookieContainer = new CookieContainer();
        //    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        cookieContainer.Add(baseAddress, new Cookie(cookieName, CookieValue));
        //        var result = await client.GetStringAsync(baseAddress);
        //    }
        //}

        //public async Task GetCalendar(string cookieName, string cookieValue)
        //{
        //    var baseAddress = new Uri("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/SynPeople_getData.action?_dc=1651694124159");
        //    var cookieContainer = new CookieContainer();
        //    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        var content = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("employeeId", "904")
        //        });
        //        cookieContainer.Add(baseAddress, new Cookie(cookieName, cookieValue));
        //        var result = await client.PostAsync("/VLDailyRetail_Hs81Lz82/SynPeople_getData.action?_dc=1651694124159", content);
        //        result.EnsureSuccessStatusCode();
        //        var responseString = await result.Content.ReadAsStringAsync();
        //    }
        //}

        private async Task<string> GetCookieValue(string url, string cookieName)
        {
            var cookieContainer = new CookieContainer();
            var uri = new Uri(url);
            using (var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            })
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    await httpClient.GetAsync(uri);
                    var cookie = cookieContainer.GetCookies(uri).Cast<Cookie>().FirstOrDefault(x => x.Name == cookieName);
                    return cookie?.Value;
                }
            }
        }

        public async Task AllInOne(string cookieName, string CookieValue)
        {
            var baseAddress = new Uri("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/Logon.action");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", "volkani"),
                    new KeyValuePair<string, string>("password", "Maas1234"),
                });
                cookieContainer.Add(baseAddress, new Cookie(cookieName, CookieValue));
                var result = await client.PostAsync("/VLDailyRetail_Hs81Lz82/Logon.action", content);
                result.EnsureSuccessStatusCode();
                var responseString = await result.Content.ReadAsStringAsync();

                // get employeeId
                var employeeIdAdress = new Uri("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/Calendar_select.action?_dc=");
                var employeeGetResult = await client.GetStringAsync(employeeIdAdress);
                string empId = employeeId(employeeGetResult);

                // get calendar
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("employeeId", empId),
                    new KeyValuePair<string, string>("startDate", "2022-05-02"),
                    new KeyValuePair<string, string>("stopDate", "2022-05-08")
                });
                result = await client.PostAsync("/VLDailyRetail_Hs81Lz82/SynPeople_getData.action?_dc=1651703824749", content);
                result.EnsureSuccessStatusCode();
                responseString = await result.Content.ReadAsStringAsync();

                //Read response as JSON
                JObject s = JObject.Parse(responseString);
                JToken days = s["employees"][0]["days"];
                foreach (var d in days)
                {
                    if (d["allDayResults"].HasValues)
                    {
                        date += $"{d["date"]} : {d["allDayResults"]["dayResultRequests"][0]["formattedValue"]}\n";
                    }
                }
            }
        }

        public string employeeId(string s)
        {
            string findstr = "employeeId";
            int index = s.IndexOf(findstr);
            string empId = "";
            if (index < 0)
            {
                MessageBox.Show("Not found");
                return "";
            }
            else
            {
                index += 14;
                for (int i = 0; i < 3; i++)
                {
                    empId += s[index + i];
                }
                return empId;
            }
        }

        private async void BtnPost_Click(object sender, RoutedEventArgs e)
        {
            string cookieValue = await GetCookieValue("https://asp002.syntegro.be:7366/VLDailyRetail_Hs81Lz82/Logon.action", "JSESSIONID");
            //await Login("JSESSIONNID", cookieValue);
            //await GetCalendar("JSESSIONNID", cookieValue);
            //await GetEmployeeId("JSESSIONNID", cookieValue);
            await AllInOne("JSESSIONNID", cookieValue);
            TxtBlock.Text = date;
        }
    }
}
