using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NEE.Database
{
    public class AADE_Log
    {

        // --- Basic AADE audit columns ---

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [MaxLength(60)]
        public string AuditTransactionId { get; set; }

        [Required]
        [MaxLength(60)]
        public string EntityCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntitySubUnit { get; set; }


        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [MaxLength(60)]
        public string TransactionReason { get; set; }


        [Required]
        [MaxLength(100)]
        public string ServerHostName { get; set; }

        [Required]
        [MaxLength(20)]
        public string ServerHostIP { get; set; }


        [Required]
        [MaxLength(100)]
        public string EndUserDeviceInfo { get; set; }

        [Required]
        [MaxLength(20)]
        public string EndUserDeviceIP { get; set; }


        [Required]
        [MaxLength(100)]
        public string EndUserId { get; set; }


        // --- Our specific columns ---

        [Required]
        [MaxLength(30)]
        public string Reason { get; set; }

        [Required]
        [MaxLength(9)]
        public string AFM { get; set; }

        [Required]
        public int Year { get; set; }

        [MaxLength(30)]
        public string ApplicationId { get; set; }


        // --- Extra AADE audit columns ---

        public int? ElapsedMS { get; set; }

        public long? CallSeqId { get; set; }

        [MaxLength(1000)]
        public string ErrorMessage { get; set; }


        // --- Extra AADE logging columns (Request / Response as Json)  --- 

        public int? ReqLen { get; set; }

        public int? ResLen { get; set; }


        [MaxLength(1000)]
        public string ReqJson { get; set; }

        [MaxLength(2000)]
        public string ResJson { get; set; }

    }
}
