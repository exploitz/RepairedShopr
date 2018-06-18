using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairedShopr
{

    public partial class MainWindow : Form
    {
        APIClient _APIClient;
        APIClient.RootObject lastTickets;

        public MainWindow()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;

            _APIClient = new APIClient();
            _APIClient.ProgressUpdated += new APIClient.ProgressHandler(ApiClient_ProgressUpdated);

        }

        private bool LoadDataView(APIClient.RootObject tickets)
        {
            try
            {
                ticketsBindingSource.DataSource = tickets;
                ticketsBindingSource.ResetBindings(false);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private void ApiClient_ProgressUpdated(object sender, APIClient.ProgressArgs progressArgs)
        {
            toolStripStatusLabel_Status.Text = progressArgs.Message;
            toolStripProgressBar_Status.Value = progressArgs.Progress;
        }

        private async void MainWindow_Load(object sender, EventArgs e)
        {
            //Setup the window positioning and size
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;

            //Load the API and then pull the tickets using it
            _APIClient.LoadAPI();
            APIClient.RootObject root = await _APIClient.APIGet(
                new APIFunctions.Tickets().GetTickets(
                    APIFunctions.Tickets.TicketStatusOptions.NotClosed));
            if(LoadDataView(root))
            {
                lastTickets = root;
                UpdateTicketInfoAsync();
            }
            else
            {
                //Error handling
            }

        }

        private async Task UpdateTicketInfoAsync(int index = 0)
        {
            textBox_Ticket_Issue.Text = lastTickets.tickets[index].GetInitIssue;
            label_Password.Text = "Password: " + lastTickets.tickets[index].properties.Password;
            groupBox_Customer.Text = lastTickets.tickets[index].customer_business_then_name;
            APIClient.RootObject customers = await _APIClient.APIGet(new APIFunctions.Customers().GetCustomers(),
                new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("query", lastTickets.tickets[index].customer_business_then_name) 
                    });

            label_Phone.Text = "Phone: " + customers.customers[0].phone;
            label_Email.Text = "Email: " + customers.customers[0].email;
        }

        private async void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string query = "";
            query = ((ComboBox)sender).SelectedValue.ToString();
            APIClient.RootObject tickets = await _APIClient.APIGet(new APIFunctions.Customers().GetCustomers(),
                new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("query", query)
                    });
            //MessageBox.Show(tickets.tickets[0].comments[tickets.tickets[0].comments.Count - 1].body);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            string query = "";
            query = comboBox1.Text;
            APIClient.RootObject tickets = await _APIClient.APIGet(new APIFunctions.Tickets().GetTickets(),
                new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("query", query)
                    });
            LoadDataView(tickets);
        }

        private async void button_ResetDataView_Click(object sender, EventArgs e)
        {
            APIClient.RootObject tickets = await _APIClient.APIGet(new APIFunctions.Tickets().GetTickets());
            LoadDataView(tickets);
        }

        /// <summary>
        /// Event fired when the selection is changed on dataGridView1
        /// </summary>
        /// <param name="sender">The DataGridView sending the event</param>
        /// <param name="e">Paramenters passed with the event</param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            label_Phone.Text = "LOADING DATA";
            try
            {   //Attempts to pull the computer brand name from the selected customer and autofill the parts searchbox
                string[] tmparray = dataGridView_Tickets.SelectedRows[0].Cells[2].Value.ToString().Split(' ');
                string brand;
                brand = (tmparray[0] == "Rework:") ? tmparray[1] + " " : tmparray[0] + " ";
                textBox_CustBrandParts.Text = brand;

                
                int ind = dataGridView_Tickets.SelectedRows[0].Index;
                UpdateTicketInfoAsync(ind);
            }
            catch { }

        }

        private void dataGridView_Comments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //    if (dataGridView_Comments.NewRowIndex == e.RowIndex)
            //    {
            //        dataGridView_Comments.Rows[e.RowIndex].ReadOnly = false;
            //    }
            //    else
            //    {
            //        dataGridView_Comments.Rows[e.RowIndex].ReadOnly = true;
            //    }
        }

        private void dataGridView_Comments_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowCommentDialogue();
        }

        private void dataGridView_Comments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;

                    // Get mouse position relative to the vehicles grid
                    var relativeMousePosition = dataGridView_Comments.PointToClient(Cursor.Position);

                    // Show the context menu
                    this.contextMenu1.Show(dataGridView_Comments, relativeMousePosition);
                }
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            ShowCommentDialogue();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            string ticketID = this.dataGridView_Tickets.SelectedRows[0].Cells["id"].Value.ToString();
            Process.Start("https://wwwarlingtoncomputercare.repairshopr.com/tickets/" + ticketID);
        }

        private void ShowCommentDialogue()
        {
            string ticketID = this.dataGridView_Tickets.SelectedRows[0].Cells["id"].Value.ToString();
            InputDialog id = new InputDialog(_APIClient, ticketID);
            id.StartPosition = FormStartPosition.CenterParent;
            id.ShowDialog();

        }

        private async void menuItem8_Click(object sender, EventArgs e)
        {
           //APIClient.RootObject tickets = await _APIClient.CallAPI(APIClient.APIFunctions.Customers.FindByID, "12998142");
           //MessageBox.Show(tickets.customers[0].fullname);
        }

        private async void button_refresh_ClickAsync(object sender, EventArgs e)
        {
            APIClient.RootObject tickets = await _APIClient.APIGet(new APIFunctions.Tickets().GetTickets());
            LoadDataView(tickets);
        }
    }
}
