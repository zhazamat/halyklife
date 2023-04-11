﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hbk.Models
{
    [Table("thanksboard")]
    public class ThanksBoard
    {
        [Key, Required]
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;
        public DateTime DateReceived { get; set; }
       
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        public int? ReceiverId { get; set; }

        [JsonIgnore]
        public virtual Employee Receiver { get; set; }
        public int? SenderId { get; set; }
        [JsonIgnore]
        public virtual Employee Sender { get; set; }

        internal object DateDiffDays(DateTime dateReceived, DateTime now)
        {
            throw new NotImplementedException();
        }
    }
   /* 
    public enum Category
    {
        Confidence,
        Efficiency,
        Progressorism,
        Openness,
        Innovation,
        Responsibility,
        Manifold
    }
    */
}
