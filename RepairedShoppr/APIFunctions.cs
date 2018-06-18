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

                //List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
                string content = "";
                if (status != "")
                   content = EncodeVariables(BuildAPICall(nameof(status), status));
                return baseAPIurl + "?" + content;
            }

            public string AddComment(List<KeyValuePair<string,string>> args)
            {
                return "";
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
