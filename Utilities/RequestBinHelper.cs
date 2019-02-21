using System;
using System.Net.Http;
using System.Text;
using Cocodrinks.Models;
using Microsoft.AspNetCore.Http;

namespace Cocodrinks.Utilities
{
    public class RequestBinHelper
    {
        private static string APIKEY = "TBGqhqTSglEitQ0xmXa3";
        public static void sendOrder(Order order){
            var client = new HttpClient();
            Uri siteUri = new Uri("https://edsard.nl/requestbin/?key="+RequestBinHelper.APIKEY);
            //send order object to requestbin for processing.
            String jsonOrder = "{";
            jsonOrder += "\"order\":{";
            jsonOrder += "  \"id\":\""+order.Id+"\",";
            jsonOrder += "  \"deliverydate\":\""+order.DeliveryDate.ToString()+"\"";
            jsonOrder += "  }";
            jsonOrder += "}";
            client.PostAsync(siteUri.ToString(), new StringContent(jsonOrder, Encoding.UTF8, "application/json"));
        }
    }
}
