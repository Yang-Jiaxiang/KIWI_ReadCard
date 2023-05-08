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
        public BloodForm BloodForm = new BloodForm();
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
                if (resStateCode == "200" || resStateCode == "300") opneBloodForm();
                Console.WriteLine(resStateCode);
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

        public void opneBloodForm()
        {
            BloodForm.ShowDialog();
        }

        public void openForm2()
        {
            form2.ShowDialog();
        }

        public void closeForm2()
        {

            form2.Close();
            addDepartmentComboBox1();
        }

        public void closeBloodForm()
        {
            BloodForm.Close();
        }


        public void setToken(string token)
        {
            apiToken = token;
        }

        public async void addDepartmentComboBox1()
        {
            try
            {/*
                fetchApi.APIRequest request = new fetchApi.APIRequest("api/department?limit=10&offset=0", apiToken);
                dynamic response = request.Get();
                //List<Type.getApiDepartmentResults> DepartmentList= response["results"];
                Console.WriteLine(response["count"]);
                */
                Type.getApiDepartment Department = await apiGet.apiGetDepartment();
                List<Type.getApiDepartmentResults> DepartmentList = Department.results;
                
                System.Object[] ItemObject = new System.Object[DepartmentList.Count];
                for (int i = 0; i < DepartmentList.Count; i++)
                {
                    ItemObject[i] = DepartmentList[i].name;
                }
                comboBox1.Items.AddRange(ItemObject);
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
