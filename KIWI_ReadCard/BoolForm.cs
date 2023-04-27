using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KIWI_ReadCard
{
    public partial class BoolForm : Form
    {
        public static BoolForm instance;

        public BoolForm()
        {
            InitializeComponent();
            instance = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(Form1.instance.patientID);
            Console.WriteLine(textBox1.Text);
        }
    }
}
