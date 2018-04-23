using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Comments;
using WebApi.Data.Items;
using WebApi.Data.User;

namespace WebApi.Data
{
    public class APIDbContext: IdentityDbContext<ApplicationUser>
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {

        } 
        
        //在 Entity Framework 中，实体集通常与数据表相对应，具体实体与表中的行相对应。
        #region 属性
        public DbSet<Item> Items { get; set; }
        public DbSet<Comment> Comments { get; set; }
        //  public DbSet<ApplicationUser> Users { get; set; }

        #endregion

        //配置对象实体间与数据库之间的关系，以及如何映射到数据库
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationUser>().HasMany(x => x.Items).WithOne(x => x.Author);
            builder.Entity<ApplicationUser>().HasMany(u => u.Comments).WithOne(x => x.Author).HasPrincipalKey(y => y.Id);
            builder.Entity<Item>().ToTable("Items");
            builder.Entity<Item>().Property(i => i.Id).ValueGeneratedOnAdd();
            builder.Entity<Item>().HasOne(x => x.Author).WithMany(u => u.Items);
            builder.Entity<Item>().HasMany(x => x.Comments).WithOne(c => c.Item);

            builder.Entity<Comment>().ToTable("Comment");
            //DeleteBehavior.Restrict设置不被级联删除，删除实体时，导航属性中相对应的值可级联删除，可不进行任何操作，可设置为空
            builder.Entity<Comment>().HasOne(c => c.Author).WithMany(u => u.Comments).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Comment>().HasOne(c => c.Item).WithMany(i => i.Comments);
            builder.Entity<Comment>().HasOne(c => c.Parent).WithMany(c => c.Children);
            builder.Entity<Comment>().HasMany(c => c.Children).WithOne(c => c.Parent);
            base.OnModelCreating(builder);
        }


    }
}
