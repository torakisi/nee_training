using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using NEE.Core.BO;

namespace NEE.Web.Models.ApplicationViewModels
{
    public class ApplicationManagementViewModel
    {
        public ApplicationOwner ApplicationOwner { get; set; }
    }
}