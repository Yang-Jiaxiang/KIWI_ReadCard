using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using static KIWI_ReadCard.Form2;
using System.Windows.Forms;
using System.Net;
using static KIWI_ReadCard.Type;

namespace KIWI_ReadCard
{
    internal class apiGet
    {
        public static string BACKEND_URL = ConfigurationManager.AppSettings["BACKEND_URL"];
        public static async Task<Type.getApiDepartment> apiGetDepartment()
        {
            // 建立 HttpClient 實例
            var client = new HttpClient();
            // 設定請求的標頭，指定傳遞的資料格式為 JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 發送 HTTP POST 請求
            var response = await client.GetAsync(BACKEND_URL + "api/department?limit=10&offset=0");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // 讀取 HTTP 回應的正文內容
                Type.getApiDepartment resData = JsonConvert.DeserializeObject<Type.getApiDepartment>(responseContent);//反序列化
                resData.stateCode = "200";
                return resData;
            }
            else
            {
                Type.getApiDepartment resData = JsonConvert.DeserializeObject<Type.getApiDepartment>(responseContent);//反序列化
                resData.stateCode = "500";
                return resData;
            }
        }

    }

    internal class apiPost {
        public static async Task<string> apiPostPatient(Type.PatientForm patientForm, string apiToken)
        {
            string BACKEND_URL = ConfigurationManager.AppSettings["BACKEND_URL"];

            // 創建一個 HttpClientHandler，並指定 Cookie 容器
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };

            // 將 Cookie 加入 Cookie 容器
            cookieContainer.Add(new Uri(BACKEND_URL), new Cookie("accessToken", apiToken));

            // 創建 HttpClient 實例，並設置 HttpClientHandler
            var client = new HttpClient(handler);

            // 設定請求的標頭，指定傳遞的資料格式為 JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 將 JSON 資料轉換為 HttpContent 物件
            var content = new StringContent(JsonConvert.SerializeObject(patientForm), Encoding.UTF8, "application/json");

            // 發送 HTTP POST 請求
            var response = await client.PostAsync(BACKEND_URL + "api/patient", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "200";
            }
            else
            {
                if (responseContent.Contains("E11000"))
                {
                    return "300";
                }
                else
                {
                    ErrorMessage message = JsonConvert.DeserializeObject<ErrorMessage>(responseContent);//反序列化
                    if (message.message== "Need a token")
                    {
                        Form1.instance.openForm2();
                    }
                    else
                    {
                        Form1.instance.addLog(message.message);
                    }                                  
                }
                return "500";
            }
        }

        public static async Task<string> postApiBlood(postApiBloodForm BloodForm , string apiToken)
        {
            string BACKEND_URL = ConfigurationManager.AppSettings["BACKEND_URL"];

            // 創建一個 HttpClientHandler，並指定 Cookie 容器
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };

            // 將 Cookie 加入 Cookie 容器
            cookieContainer.Add(new Uri(BACKEND_URL), new Cookie("accessToken", apiToken));

            // 創建 HttpClient 實例，並設置 HttpClientHandler
            var client = new HttpClient(handler);

            // 設定請求的標頭，指定傳遞的資料格式為 JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 將 JSON 資料轉換為 HttpContent 物件
            var content = new StringContent(JsonConvert.SerializeObject(BloodForm), Encoding.UTF8, "application/json");

            // 發送 HTTP POST 請求
            var response = await client.PostAsync(BACKEND_URL + "api/patient", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Form1.instance.addLog(BloodForm.patientID + "，新增成功");
                return "200";
            }
            else
            {
                ErrorMessage message = JsonConvert.DeserializeObject<ErrorMessage>(responseContent);//反序列化
                if (message.message == "Need a token")
                {
                    Form1.instance.openForm2();
                }
                else
                {
                    Form1.instance.addLog(message.message);
                }
                return "500";
            }
        }

        private class ErrorMessage
        {
            public string message;
        }
    }         
}
