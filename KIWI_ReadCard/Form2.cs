using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;



namespace KIWI_ReadCard
{
    public partial class Form2 : Form
    {
        public static Form2 instance;
        public Form2()
        {
            InitializeComponent();
            instance = this;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        static async Task Login(string username, string password)
        {
            LoginForm LoginForm = new LoginForm { username = username ,password=password};
            string BACKEND_URL = ConfigurationManager.AppSettings["BACKEND_URL"];
            // 建立 HttpClient 實例
            var client = new HttpClient();

            // 設定請求的標頭，指定傳遞的資料格式為 JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 將 JSON 資料轉換為 HttpContent 物件
            var content = new StringContent(JsonConvert.SerializeObject(LoginForm), Encoding.UTF8, "application/json");

            // 發送 HTTP POST 請求
            var response = await client.PostAsync(BACKEND_URL + "auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                
                SuccessfulMessage SuccessfulMessage = JsonConvert.DeserializeObject<SuccessfulMessage>(responseContent);//反序列化
                Form1.instance.setToken(SuccessfulMessage.token);
                Form1.instance.addLog(SuccessfulMessage.message+",登入成功");
                Form1.instance.addDepartmentComboBox1();
                Form1.instance.closeForm2();
            }
            else
            {
                Console.WriteLine($"HTTP POST 請求失敗，狀態碼為 {responseContent}");
                ErrorMessage message = JsonConvert.DeserializeObject<ErrorMessage>(responseContent);//反序列化
                MessageBox.Show(message.message, "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1_username.Text;
            string password = textBox2_password.Text;
            Login(username, password);
        }

        private class LoginForm{
            public string username;
            public string password;
        }

        private class ErrorMessage {
            public string message;
        }

        private class SuccessfulMessage {
            public string message;
            public string token;
        }

    }
}
