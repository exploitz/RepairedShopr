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
            APIClient.RootObject tickets = await _APIClient.DownloadTickets();
            LoadDataView(tickets);
            textBox_Ticket_Issue.Text = tickets.tickets[0].GetInitIssue;
        }

        private async void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string query = "";
            query = ((ComboBox)sender).SelectedValue.ToString();
            APIClient.RootObject tickets = await _APIClient.DownloadTickets(query);
            //MessageBox.Show(tickets.tickets[0].comments[tickets.tickets[0].comments.Count - 1].body);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            string query = "";
            query = comboBox1.Text;
            APIClient.RootObject tickets = await _APIClient.DownloadTickets(query);
            //MessageBox.Show(tickets.tickets[0].comments[tickets.tickets[0].comments.Count - 1].body);
            LoadDataView(tickets);
        }

        private async void button_ResetDataView_Click(object sender, EventArgs e)
        {
            APIClient.RootObject tickets = await _APIClient.DownloadTickets();
            LoadDataView(tickets);
        }

        /// <summary>
        /// Event fired when the selection is changed on dataGridView1
        /// </summary>
        /// <param name="sender">The DataGridView sending the event</param>
        /// <param name="e">Paramenters passed with the event</param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            try
            {   //Attempts to pull the computer brand name from the selected customer and autofill the parts searchbox
                string[] tmparray = dataGridView_Tickets.SelectedRows[0].Cells[2].Value.ToString().Split(' ');
                string brand;
                brand = (tmparray[0] == "Rework:") ? tmparray[1] + " " : tmparray[0] + " ";
                textBox_CustBrandParts.Text = brand;

                string ticketID = this.dataGridView_Tickets.SelectedRows[0].Cells["id"].Value.ToString();
                string initIssue = this.dataGridView_Comments.Rows[dataGridView_Comments.Rows.Count - 2].Cells[3].Value.ToString();

                textBox_Ticket_Issue.Text = initIssue;
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
    }
}
