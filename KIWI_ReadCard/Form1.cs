using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static KIWI_ReadCard.Type;

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
                if (resStateCode == "200"){
                    addLog(patientJson.name + "，新增成功");
                };
                if(resStateCode == "300") {
                    addLog(patientJson.name + "，報到成功");
                };
                if (resStateCode == "500") { 
                    addLog("請從新操作上一步驟");
                    return;
                };

                string procedureCode = comboBox1.SelectedValue.ToString();

                dynamic WorkListData = CreateWorkList2Dcm4chee(patientJson.id);

                dynamic Report = CreateReport(patientJson, WorkListData);
                string Report_id = Report["_id"].ToString();
                CreateSchedule(patientJson, Report_id, WorkListData, procedureCode);
                addLog("已經病患 "+ patientJson.name+" 加入排程");

            }
            else
            {
                addLog("#未插入健保卡");
            }
        }

        private dynamic CreateWorkList2Dcm4chee(string pID)
        {
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/worklist/" + pID, apiToken);
            dynamic response = request.Get();
            if (response == null)
            {
                return "No Data";
            }
            else
            {
                return response;
            }
        }

        private dynamic CreateReport(dynamic Patient, dynamic WodkListData)
        {
            var Report = new
            {
                patientID = Patient.id,
                accessionNumber = WodkListData["accessionNumber"],
                StudyInstanceUID = WodkListData["StudyInstanceUID"]
            };
            // 將 JSON 物件序列化為 JSON 字串
            string json = JsonConvert.SerializeObject(Report);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/report", apiToken);
            dynamic response = request.Post(json);

            return response;
        }

        private dynamic CreateSchedule(dynamic Patient, string Report_id, dynamic WodkListData,string procedureCode)
        {
            var Schedule = new
            {
                patientID = Patient.id,
                reportID = Report_id,
                procedureCode = procedureCode,
                accessionNumber = WodkListData["accessionNumber"],
                StudyInstanceUID = WodkListData["StudyInstanceUID"],
                status = "wait-examination",
            };// 將 JSON 物件序列化為 JSON 字串
            string json = JsonConvert.SerializeObject(Schedule);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/schedule", apiToken);
            dynamic response = request.Post(json);
            return response;
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
