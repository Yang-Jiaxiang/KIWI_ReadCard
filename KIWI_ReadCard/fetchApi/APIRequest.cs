using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace KIWI_ReadCard.fetchApi
{
    internal class APIRequest
    {
        private string apiUrl;
        private string accessToken;
        private string eventID;

        public APIRequest(string apiUrl, string accessToken, string eventID)
        {
            this.apiUrl = ConfigurationManager.AppSettings["BACKEND_URL"] + apiUrl;
            this.accessToken = accessToken;
            this.eventID = eventID;
        }

        public dynamic Get()
        {
            dynamic response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add(HttpRequestHeader.Cookie, $"accessToken={accessToken}");

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                            string responseString = reader.ReadToEnd();

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            response = serializer.Deserialize<dynamic>(responseString);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        public dynamic Post(string postData)
        {
            dynamic response = null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.Headers.Add(HttpRequestHeader.Cookie, $"accessToken={accessToken}; event={eventID}");

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                            string responseString = reader.ReadToEnd();

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            response = serializer.Deserialize<dynamic>(responseString);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        public dynamic Patch(string postData)
        {
            dynamic response = null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "PATCH";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.Headers.Add(HttpRequestHeader.Cookie, $"accessToken={accessToken}");

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                            string responseString = reader.ReadToEnd();

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            response = serializer.Deserialize<dynamic>(responseString);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }
    }
}
