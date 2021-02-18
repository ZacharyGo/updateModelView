using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrystalSystemAPI.Models
{
    public class ImageUploadData {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string TransactionName { get; set; }
        public string TransactionDetail { get; set; }
    }
}