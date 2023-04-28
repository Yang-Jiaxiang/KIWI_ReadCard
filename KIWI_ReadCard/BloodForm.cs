using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KIWI_ReadCard.Type;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KIWI_ReadCard
{
    public partial class BloodForm : Form
    {
        public static BloodForm instance;

        public BloodForm()
        {
            InitializeComponent();
            instance = this;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            postApiBloodForm postApiBloodForm = new postApiBloodForm()
            {
                patientID = Form1.instance.patientID,
                number = textBox1.Text
            };
            CreateBlood(postApiBloodForm);
            string StudyInstanceUID = CreateWorkList(Form1.instance.patientID);
            dynamic Report = CreateReport(postApiBloodForm, StudyInstanceUID);
            string Report_id = Report["_id"].ToString();
            dynamic Schedule = CreateSchedule(postApiBloodForm.patientID, Report_id, StudyInstanceUID);
            Form1.instance.closeBloodForm();
        }

        private string CreateWorkList(string patientID)
        {
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/worklist/" + patientID, Form1.instance.apiToken);
            dynamic response = request.Get();
            if (response == null)
            {
                return "No Data";
            }
            else
            {
                Console.WriteLine(response[0]);
                return response[0];
            }
        }

        private void CreateBlood(postApiBloodForm postApiBloodForm)
        {
            string BloodJson = JsonConvert.SerializeObject(postApiBloodForm);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/blood", Form1.instance.apiToken);
            dynamic BloodResponse = request.Post(BloodJson);
        }

        private dynamic CreateReport(postApiBloodForm postApiBloodForm , string StudyInstanceUID)
        {
            var Report = new
            {
                patientID = postApiBloodForm.patientID,
                procedureCode = "19009C",
                status = "pending",
                blood = postApiBloodForm.number,
                StudyInstanceUID = StudyInstanceUID
            };

            string ReportJson = JsonConvert.SerializeObject(Report);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/report", Form1.instance.apiToken);
            dynamic response = request.Post(ReportJson);
            return response;
        }

        private dynamic CreateSchedule(string patientID, string Report_id, string StudyInstanceUID)
        {
            var Schedule = new
            {
                patientID = patientID,
                reportID = Report_id,
                procedureCode = "19009C",
                StudyInstanceUID = StudyInstanceUID,
                status = "wait-blood",
            };
            string json = JsonConvert.SerializeObject(Schedule);
            fetchApi.APIRequest request = new fetchApi.APIRequest("api/schedule", Form1.instance.apiToken);
            dynamic response = request.Post(json);
            return response;
        }
    }
}
