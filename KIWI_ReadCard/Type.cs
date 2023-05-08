using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIWI_ReadCard
{
    internal class Type
    {
        public class PatientForm {
            public string name;
            public string id;
            public string gender;
            public string phone="NULL";
            public string department;
            public string birth;
        }

        public class getApiDepartment
        {
            public string stateCode;
            public string message;
            public string count;
            public List<getApiDepartmentResults> results;
        }

        public class getApiDepartmentResults {
            public string _id;
            public string name;
            public string address;
            public bool active;
        }

        public class postApiBloodForm
        {
            public string patientID;
            public string number;
        }
    }
}
