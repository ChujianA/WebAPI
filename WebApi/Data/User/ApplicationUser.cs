
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Comments;
using WebApi.Data.Items;

namespace WebApi.Data.User
{
    public class ApplicationUser: IdentityUser
    {

        #region 属性
        public string DisplayName { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        [Required]
        public int Type { get; set; }

        public virtual List<Item> Items { get; set; }

        public virtual List<Comment> Comments { get; set; }
        #endregion
    }
}
