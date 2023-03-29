using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{
    public class NEE_AppDistrict
    {
        [Key, Column(Order = 0)]
        [Required]
        [MaxLength(29)]
        public string AppId { get; set; }
       
        public string DistrictId { get; set; }

    }
}
