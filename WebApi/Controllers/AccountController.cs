using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Data.User;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : BaseController
    {
        //提供在持久化存储中管理角色的API
        private RoleManager<IdentityRole> RoleManager;
        //提供在持久化存储中管理用户的API
        private UserManager<ApplicationUser> UserManager;
        private SignInManager<ApplicationUser> signInManager;
        private APIDbContext dbContext;
        public AccountController(APIDbContext dbContext,RoleManager<IdentityRole> RoleManager, UserManager<ApplicationUser> UserManager, SignInManager<ApplicationUser> signInManager)
            :base(dbContext, signInManager,UserManager)
        {
            this.dbContext = dbContext;
            this.RoleManager = RoleManager;
            this.UserManager = UserManager;
        }
        // GET: api/Account
        /// <summary>
        /// 获取登陆用户的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var id = await GetCurentUserId();
            var user = dbContext.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
                return NotFound(new { error = string.Format("用户没用登陆") });
            else
                return new JsonResult(new UserViewModel {
                    UserName = user.UserName,
                    UserPwd = user.PasswordHash
                },DefaultJsonSettings);
        }

        [HttpPost("Logount")]
        public IActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                 signInManager.SignOutAsync().Wait();
            }
            return Ok();
        }
    }
}
