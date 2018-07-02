using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Models
{
    [Serializable]
    public class Loan
    {
        public float LoanAmount { get; set; }

        public float Tenure { get; set; }

        public float Installment { get; set; }
    }
}