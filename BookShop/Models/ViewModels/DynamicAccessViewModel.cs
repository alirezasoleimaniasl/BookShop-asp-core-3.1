using BookShop.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Models.ViewModels
{
    public class DynamicAccessViewModel
    {
        public string[] ActionIds { set; get; }
        public string RoleId { set; get; }
        public ApplicationRole RoleIncludeRoleClaims { set; get; }
        public ICollection<ControllerViewModel> SecuredControllerActions { set; get; }
    }
}
