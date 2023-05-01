using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KIWI_ReadCard.fetchApi
{
    internal class Auth
    {
        public static async Task<AuthLoginApiType.AuthLoginApiResponse> AuthLogin(AuthLoginApiType.LoginForm UserData)
        {
            string BACKEND_URL = ConfigurationManager.AppSettings["BACKEND_URL"];
            // 建立 HttpClient 實例
            var client = new HttpClient();

            // 設定請求的標頭，指定傳遞的資料格式為 JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 將 JSON 資料轉換為 HttpContent 物件
            var content = new StringContent(JsonConvert.SerializeObject(UserData), Encoding.UTF8, "application/json");

            // 發送 HTTP POST 請求
            var response = await client.PostAsync(BACKEND_URL + "auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                AuthLoginApiType.SuccessfulMessage SuccessfulMessage = JsonConvert.DeserializeObject<AuthLoginApiType.SuccessfulMessage>(responseContent);//反序列化
                AuthLoginApiType.AuthLoginApiResponse ResponseData = new AuthLoginApiType.AuthLoginApiResponse
                {
                    status = 200,
                    token = SuccessfulMessage.token,
                    message = SuccessfulMessage.message
                };

                return ResponseData;
            }
            else
            {
                AuthLoginApiType.ErrorMessage message = JsonConvert.DeserializeObject<AuthLoginApiType.ErrorMessage>(responseContent);//反序列化
                AuthLoginApiType.AuthLoginApiResponse ResponseData = new AuthLoginApiType.AuthLoginApiResponse
                {
                    status = 400,
                    message = message.message
                };
                return ResponseData;

            }
        }

        public class AuthLoginApiType
        {
            public class LoginForm
            {
                public string username;
                public string password;
            }
            public class SuccessfulMessage
            {
                public string message;
                public string token;
            }
            public class ErrorMessage
            {
                public string message;
            }

            public class AuthLoginApiResponse
            {
                public int status;
                public string message;
                public string token;
            }
        }
    }
}
