using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairedShopr
{
    public partial class InputDialog : Form
    {
        
        private APIClient _APIClient;
        private string _ticketID;

        public InputDialog(APIClient apiclient, string ticketID)
        {
            InitializeComponent();
            _APIClient = apiclient;
            _ticketID = ticketID;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Insert logic to push comment
            string comment = textBox1.Text;
            string subject = comboBox_subject.Text;
            _APIClient.SendTicketComment(_ticketID, comment, subject);
            this.Close();

            //Enter code to send push event here
        }
    }
}
