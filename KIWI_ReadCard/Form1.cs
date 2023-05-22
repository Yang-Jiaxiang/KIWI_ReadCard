using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
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
        public string eventID = "";

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
            patientID = patientJson.id;
            //判斷是否有資料
            if (patientJson.id != null)
            {
                CheckPatient(patientJson);
                eventID = ((MyItem)comboBox1.SelectedItem).RealValue;
                opneBloodForm();
            }
            else
            {
                addLog("#未插入健保卡");
            }
        }

        private async void CheckPatient(Type.PatientForm patientJson)
        {
            fetchApi.APIRequest requestPatient = new fetchApi.APIRequest("api/patient/" + patientJson.id, apiToken);
            dynamic responsePatient = requestPatient.Get();
            if (responsePatient == null)
            {
                dynamic PatientData = await CreatePatient(patientJson);
                addLog("新增病患成功" + patientJson.name);
            }
            else
            {
                addLog("病患存在資料庫" + patientJson.name);
            }
        }

        private dynamic CreatePatient(Type.PatientForm patientJson)
        {
            patientJson.department = GetDepartmentIDByEvent();
            string json = JsonConvert.SerializeObject(patientJson);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/patient", apiToken);
            dynamic response = request.Post(json);
            return response;
        }

        public string GetDepartmentIDByEvent()
        {
            fetchApi.APIRequest requestEvent = new fetchApi.APIRequest("api/event/" + ((MyItem)comboBox1.SelectedItem).RealValue, apiToken);
            dynamic responseEvent = requestEvent.Get();
            Console.WriteLine(responseEvent["departmentID"]);
            return responseEvent["departmentID"];
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
            {
                fetchApi.APIRequest request = new fetchApi.APIRequest("api/event?limit=100&offset=0", apiToken);
                dynamic response = request.Get();

                foreach(dynamic result in response["results"])
                {
                    comboBox1.Items.Add(new MyItem(result["name"], result["_id"]) );
                }
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

        public struct MyItem
        {
            public MyItem(string displayName, string realValue)
            {
                DisplayName = displayName;
                RealValue = realValue;
            }
            public string DisplayName { get; set; }
            public string RealValue { get; set; }
            // must have this override method to display the right string.
            public override string ToString()
            {
                return DisplayName;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           eventID= ((MyItem)comboBox1.SelectedItem).RealValue;
        }
    }
}
