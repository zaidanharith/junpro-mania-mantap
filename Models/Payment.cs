using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }

    public class Payment
    {
        [Key]
        public int ID { get; set; }

        public string Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public PaymentStatus Status { get; set; }
        public Payment(int id, string method, decimal amount)
        {
            ID = id;
            Method = method;
            Amount = amount;
            Date = DateTime.Now;
            Status = PaymentStatus.Pending;
        }

        public bool ProcessPayment()
        {
            try
            {
                Status = PaymentStatus.Success;
                return true;
            }
            catch (Exception)
            {
                Status = PaymentStatus.Failed;
                return false;
            }
        }

        public void UpdateStatus(PaymentStatus newStatus)
        {
            if (Enum.IsDefined(typeof(PaymentStatus), newStatus))
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