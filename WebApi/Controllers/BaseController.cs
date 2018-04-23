using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Data.User;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
   
    public class BaseController : Controller
    {
        private APIDbContext dbContext;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> user;
        public BaseController(APIDbContext dbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> user)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            this.user = user;
        }

        protected JsonSerializerSettings DefaultJsonSettings {
            get {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                };
            }
        }
        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetCurentUserId()
        {
            if (!User.Identity.IsAuthenticated)
                return "";
           
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            else {
                var userInfo = await user.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (userInfo == null)
                    return "";
                return userInfo.Id;
            }
            
              
                
        }
      
    }
}
