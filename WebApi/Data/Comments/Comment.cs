using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Items;
using WebApi.Data.User;

namespace WebApi.Data.Comments
{
    public class Comment
    {
        public Comment()
        {

        }
        #region  属性
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Flags { get; set; }
        [Required]
        public string UserId { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        #endregion
        #region  导航属性
        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser Author { get; set; }
        [ForeignKey("ParentId")]
        public virtual Comment Parent { get; set; }
        public List<Comment> Children { get; set; }
        #endregion
    }
}
