using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Payment
    {
        [Key]
        public int ID { get; private set; }

        public string Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; private set; }
        public string Status { get; set; }

        public virtual Order Order { get; set; }

        public Payment(int paymentId, string method, decimal amount)
        {
            ID = paymentId;
            Method = method;
            Amount = amount;
            Date = DateTime.Now;
            Status = "Pending";
        }

        public bool ProcessPayment()
        {
            try
            {
                Status = "Success";
                return true;
            }
            catch (Exception)
            {
                Status = "Failed";
                return false;
            }
        }

        public void UpdateStatus(string newStatus)
        {
            if (newStatus is "Pending" or "Success" or "Failed")
            {
                Status = newStatus;
            }
            else
            {
                throw new ArgumentException("Invalid payment status");
            }
        }

        public string GetPaymentSummary()
        {
            return $"Payment ID: {ID}\n" +
                   $"Method: {Method}\n" +
                   $"Amount: ${Amount:F2}\n" +
                   $"Date: {Date}\n" +
                   $"Status: {Status}";
        }
    }
}