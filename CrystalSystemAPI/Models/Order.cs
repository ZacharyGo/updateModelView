using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrystalSystemAPI.Models
{
    public class Order
    {
        public string FProductDescription { get; set; }
        public string DocumentDate { get; set; }
        public string Document { get; set; }
        public string LotNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string OrderNumber { get; set; }
    }
}