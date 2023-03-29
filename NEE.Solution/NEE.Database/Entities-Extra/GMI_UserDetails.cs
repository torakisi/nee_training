using System.ComponentModel.DataAnnotations;

namespace NEE.Database.Entities_Extra
{
    public class GMI_UserDetails
    {
        /// <summary>
        /// The UserName/Email from AspNetUsers Table
        /// </summary>
        [Key]
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        //  --- Payload:  ---

        [MaxLength(30)]
        public string Municipality { get; set; }

    }
}
