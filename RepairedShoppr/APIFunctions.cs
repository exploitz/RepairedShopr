using System.Collections.Generic;
using System.Net.Http;

namespace RepairedShopr
{
    public class APIFunctions
    {
        public static string EncodeVariables(List<KeyValuePair<string, string>> args)
        {
            var content = new FormUrlEncodedContent(args).ReadAsStringAsync().Result;
            return content;
        }

        public static List<KeyValuePair<string, string>> BuildAPICall(string argument, string value)
        {
            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
            content.Add(new KeyValuePair<string, string>(argument, value));
            return content;
        }

        public class Tickets : APIFunctions
        {
            private string baseAPIurl = "tickets/";

            public static class TicketCommentOptions
            {
                public static string InitialIssue = "Initial Issue";
                public static string Diagnosis = "Diagnosis";
                public static string Update = "Update";
                public static string Contacted = "Contacted";
                public static string Approval = "Approval";
                public static string Complete = "Complete";
            }

            public static class TicketStatusOptions
            {
                public static string NotClosed = "Not Closed";
                public static string Resolved = "Resolved";
            }

            public string GetTickets(
                string status = "",
                string customer_id = "", 
                string number = "",
                string resolved_after = "",
                string term = "",
                string ticket_search_id = "")
            {

                string content = "";
                if (status != "")
                   content = EncodeVariables(BuildAPICall(nameof(status), status));
                return baseAPIurl + "?" + content;
            }

            public string AddComment(string ticketID, string comment, string subject)
            {
                //var response = await client.PostAsync("https://wwwarlingtoncomputercare.repairshopr.com/api/v1/tickets/" + ticketID + "/comment?", content);
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("hidden", "1"));
                parameters.Add(new KeyValuePair<string, string>("subject", subject));
                parameters.Add(new KeyValuePair<string, string>("body", comment));
                
                return baseAPIurl + ticketID + "/comment?";
            }
        }

        public class Customers : APIFunctions
        {
            private string baseAPIurl = "customers/";

            public string GetCustomers()
            {
                return baseAPIurl;
            }
        }

        public class Invoices : APIFunctions
        {
            private string baseAPIurl = "Invoices/";
            public Invoices()
            {

            }

            public string GetInvoices()
            {
                return baseAPIurl;
            }
        }
        public class Products : APIFunctions
        {
            private string baseAPIurl = "Products/";
            public Products()
            {

            }

            public string GetProducts()
            {
                return baseAPIurl;
            }
        }
    }
}
