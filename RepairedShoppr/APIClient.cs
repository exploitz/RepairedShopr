using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace RepairedShopr
{

    public class APIClient
    {
        private string APIkey = "80ca8e01-86eb-4b97-866d-04289e3f366f";
        private string APIurl = "https://wwwarlingtoncomputercare.repairshopr.com/api/v1/";
        //private static string api_key;
        private static HttpClient client;

        public delegate void ProgressHandler(object sender, ProgressArgs progressArgs);
        public event ProgressHandler ProgressUpdated;

        public delegate void AddCommentHandler(object sender, ProgressArgs progressArgs);
        public event AddCommentHandler AddComment;

        /// <summary>
        /// Class that holds the arguments passed by ProgressHandler (message and progress)
        /// </summary>
        public class ProgressArgs : EventArgs
        {
            private int progress;
            private string message;

            public ProgressArgs(int progress, string message)
            {
                this.progress = progress;
                this.message = message;
            }

            // This is a straightforward implementation for 
            // declaring a public field
            public int Progress
            { get { return progress; } }
            public string Message
            { get { return message; } }
        }

        public bool LoadAPI()
        {
            try
            {
                client = new HttpClient();

                client.BaseAddress = new Uri(APIurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ProgressUpdated(this, new ProgressArgs(100, "API Library Loaded"));
                return true;
            }
            catch (Exception e)
            {
                ProgressUpdated(this, new ProgressArgs(0, "Error Loading API Library."));
                return false;
            }
        }

        public async void SendTicketComment(string ticketID, string comment, string subject)
        {
            //https://wwwarlingtoncomputercare.repairshopr.com/api/v1/tickets/4222419/comment?api_key=51e372a7-23ad-450f-af0f-6b8cef11777c&hidden=1&subject=Update&body=RepairdShopr API Test Comment 1
            ProgressUpdated(this, new ProgressArgs(0, "<Comment> Sending ticket comment ..."));
            if (ticketID != "")
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("api_key", APIkey));
                parameters.Add(new KeyValuePair<string, string>("hidden", "1"));
                parameters.Add(new KeyValuePair<string, string>("subject", subject));
                parameters.Add(new KeyValuePair<string, string>("body", comment));
               
                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync("https://wwwarlingtoncomputercare.repairshopr.com/api/v1/tickets/" + ticketID + "/comment?", content);
                if (response.IsSuccessStatusCode)
                {
                    ProgressUpdated(this, new ProgressArgs(100, "<Comment> Ticket comment submitted successfully!"));
                }
            }
        }

        public async void APIPost(string apiFunction, List<KeyValuePair<string, string>> args = null)
        {
            //https://wwwarlingtoncomputercare.repairshopr.com/api/v1/tickets/4222419/comment?api_key=51e372a7-23ad-450f-af0f-6b8cef11777c&hidden=1&subject=Update&body=RepairdShopr API Test Comment 1
            ProgressUpdated(this, new ProgressArgs(0, "<API> Sending POST query ..."));
            if (apiFunction != "")
            {
                List<KeyValuePair<string, string>> parameters = args;

                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(apiFunction, content);
                //var response = await client.PostAsync("https://wwwarlingtoncomputercare.repairshopr.com/api/v1/tickets/" + ticketID + "/comment?", content);
                if (response.IsSuccessStatusCode)
                {
                    ProgressUpdated(this, new ProgressArgs(100, "<API> POST query Complete."));
                }
            }
        }


        public async Task<RootObject> APIGet(string apiFunction, List<KeyValuePair<string, string>> args = null)
        {
            ProgressUpdated(this, new ProgressArgs(0, "<API> Querying API ... "));
            RootObject root = null;

            //Construct query string here....
            string baseFunctionCall = apiFunction;
            List<KeyValuePair<string, string>> parameters;

            if (args != null)
                parameters = args;
            else 
                parameters = new List<KeyValuePair<string, string>>();
            
            parameters.Add(new KeyValuePair<string, string>("api_key", APIkey));
            var content = APIFunctions.EncodeVariables(parameters);
            
            ProgressUpdated(this, new ProgressArgs(33, "<API> Downloading api data ... "));

            HttpResponseMessage response = null;
            if (apiFunction.Contains("?"))
                response = await client.GetAsync(baseFunctionCall + "&" + content);
            else
                response = await client.GetAsync(baseFunctionCall + "?" + content);
            if (response.IsSuccessStatusCode)
            {
                root = await response.Content.ReadAsAsync<RootObject>();
                ProgressUpdated(this, new ProgressArgs(100, "<API> Complete. "));
            }
            return root;
        }

        
        /// <summary>
        /// The root object that contains the child elements returned from the RepairShopr API.
        /// </summary>
        public class RootObject
        {
            public List<Ticket> tickets { get; set; }
            public List<Customer> customers { get; set; }
            public Comment comment { get; set; }
        }
        
        /// <summary>
        /// Class that holds the customer data returned from the RepairShopr API.
        /// </summary>
        public class Customer
        {
            public int id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string fullname { get; set; }
            public string business_name { get; set; }
            public object email { get; set; }
            public string phone { get; set; }
            public string mobile { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string pdf_url { get; set; }
            public string address { get; set; }
            public string address_2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public object latitude { get; set; }
            public object longitude { get; set; }
            public object notes { get; set; }
            public bool get_sms { get; set; }
            public bool opt_out { get; set; }
            public bool disabled { get; set; }
            public object location_name { get; set; }
            public object location_id { get; set; }
            public Properties properties { get; set; }
            public object online_profile_url { get; set; }
            public List<object> contacts { get; set; }
        }

        /// <summary>
        /// Class that holds the ticket data returned from the RepairShopr API.
        /// </summary>
        public class Ticket
        {
            public string id { get; set; }
            public string number { get; set; }
            public string subject { get; set; }
            public string created_at { get; set; }
            public string customer_id { get; set; }
            public string customer_business_then_name { get; set; }
            public string due_date { get; set; }
            public object resolved_at { get; set; }
            public object start_at { get; set; }
            public object end_at { get; set; }
            public object location_id { get; set; }
            public string problem_type { get; set; }
            public string status { get; set; }
            public Properties properties { get; set; }
            public object user_id { get; set; }
            public string updated_at { get; set; }
            public object pdf_url { get; set; }
            public object priority { get; set; }
            public List<Comment> comments { get; set; }
            public object user { get; set; }
            public string _TicketData { get { return this.number + " - " + this.customer_business_then_name; } }

            private string comment;
            public string GetInitIssue
            {
                get
                {
                    if (this.comments != null)
                        comment = this.comments[this.comments.Count - 1].body;
                    return comment;
                }
            }
        }

        /// <summary>
        /// Class that holds the comment data returned from the RepairShopr API.
        /// </summary>
        public class Comment
        {
            public string id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string ticket_id { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public string tech { get; set; }
            public bool hidden { get; set; }
            public string user_id { get; set; }
        }

        /// <summary>
        /// Class that holds the ticket properties (when entering new ticket) from the RepairShopr API.
        /// </summary>
        public class Properties
        {
            public string Laptop { get; set; }
            public string Desktop { get; set; }
            public string Password { get; set; }
            [JsonProperty("AC Adapter")]
            public string AC_Adapter { get; set; }
            [JsonProperty("Important Data ")]
            public string ImportantData { get; set; }
            
        }
    
    }
}
