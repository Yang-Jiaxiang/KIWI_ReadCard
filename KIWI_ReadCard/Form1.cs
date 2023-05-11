using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace KIWI_ReadCard
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public Form2 form2 = new Form2();
        public string apiToken = "";
        public string patientDepartment = "";
        public string patientID = "";

        public Form1()
        {
            InitializeComponent();
            instance = this;
            openForm2();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button_ReadCard_Click(object sender, EventArgs e)
        {
            Type.PatientForm patientJson = JsonConvert.DeserializeObject<Type.PatientForm>(Reader.getData());//反序列化
            //判斷是否有資料
            if (patientJson.id != null)
            {
                patientID=patientJson.id;
                patientJson.department = patientDepartment;
                string resStateCode = await apiPost.apiPostPatient(patientJson,apiToken);
            }
            else
            {
                addLog("#未插入健保卡");
            }
        }


        public void addLog(string text)
        {
            DateTime currentDateTime = DateTime.Now;
            textBox_log.Text = textBox_log.Text + currentDateTime +":"+ text + "\r\n";
        }

        public void openForm2()
        {
            form2.ShowDialog();
        }

        public void closeForm2()
        {

            form2.Close();
            addProcedureCodeToComboBox1();
        }


        public void setToken(string token)
        {
            apiToken = token;
        }

        public async void addProcedureCodeToComboBox1()
        {
            try
            {
                Dictionary<string, string> items = new Dictionary<string, string>
                {
                    {"19014C", "19014C(健保)"},
                    {"19014CNE1", "19014CNE1(自費)"},
                    {"19014CNE2", "19014CNE2(自費)"}
                };

                comboBox1.DataSource = new BindingSource(items, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving departments: " + ex.Message);
            }
                           
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            patientDepartment = comboBox1.Text;
        }

    }
}
