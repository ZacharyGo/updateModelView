using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrystalSystemAPI.Models
{
    public class BatchOutputData
    {
        public string ProductId { get; set; }
        public string RegNumber { get; set; }
        public string TransacNumber { get; set; }
        public string FinalOutPut { get; set; }
        public bool isProductionQuantity { get; set; }
        public double AssemblyQuantity { get; set; }
        public string GUIDINVTransactionDetail { get; set; }
    }
}