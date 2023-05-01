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
using KIWI_ReadCard.fetchApi;



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

        private async void button1_Click(object sender, EventArgs e)
        {
            Auth.AuthLoginApiType.LoginForm UserData = new Auth.AuthLoginApiType.LoginForm
            {
                username = textBox1_username.Text,
                password = textBox2_password.Text
            };
            Auth.AuthLoginApiType.AuthLoginApiResponse AuthApiResponseData = await Auth.AuthLogin(UserData);
            if (AuthApiResponseData.status == 200)
            {
                Form1.instance.setToken(AuthApiResponseData.token);
                Form1.instance.addLog(AuthApiResponseData.message + ",登入成功");
                Form1.instance.closeForm2();
            }
            else
            {
                MessageBox.Show(AuthApiResponseData.message, "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
