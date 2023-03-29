using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Database
{
    public class KED_Log
    {


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
        public DateTime AuditTransactionDate { get; set; }


        //[Required]
        [MaxLength(20)]
        public string AuditUnit { get; set; }

        [Required]
        [MaxLength(30)]
        public string AuditProtocol { get; set; }

        [Required]
        [MaxLength(100)]
        public string AuditUserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string AuditUserIp { get; set; }



        [Required]
        [MaxLength(60)]
        public string TransactionReason { get; set; }


        [Required]
        [MaxLength(100)]
        public string ServerHostName { get; set; }

        [Required]
        [MaxLength(20)]
        public string ServerHostIP { get; set; }


        // --- Our specific columns ---

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; }

        [MaxLength(9)]
        public string AFM { get; set; }

        [MaxLength(11)]
        public string AMKA { get; set; }


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

