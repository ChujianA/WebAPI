using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Common;
using WebApi.Data;
using WebApi.Data.User;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Connect")]
    public class ConnectController : BaseController
    {
        private APIDbContext dbContext;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManage;
        private TokenHelper tokenHelper;
        public ConnectController(APIDbContext dbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> user, TokenHelper TokenHelper)
        :base(dbContext,signInManager,user){
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            userManage = user;
            tokenHelper = TokenHelper;
        }
        [HttpPost("token")]
        [EnableCors("any")]
        //用户注册
        public async Task<IActionResult> POSTAsync([FromBody] UserViewModel usrvm)
        {
            if (usrvm == null)
                return new StatusCodeResult(500);
            else
            {
                ResultViewModel resultModel = new ResultViewModel();
                try
                {
                    ApplicationUser user = await userManage.FindByNameAsync(usrvm.UserName);
                    bool isExist = user != null ? await userManage.FindByEmailAsync(usrvm.UserName) != null : false;
                    if (isExist)
                    {
                        resultModel.IsSuccess = false; resultModel.Message = "已存在此账号";
                    }
                    else
                    {
                        var CreateDate = DateTime.Now;
                        user = new ApplicationUser()
                        {
                            UserName = usrvm.UserName,
                            PasswordHash = usrvm.UserPwd,
                            CreateDate = CreateDate,
                            LastModifiedDate = CreateDate,
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                        };
                        
                        var result = await userManage.CreateAsync(user, usrvm.UserPwd);
                      
                        var result2=await userManage.AddToRoleAsync(user, "Registered");
                        if (result.Succeeded && result2.Succeeded)
                        {
                            var jwt = tokenHelper.CreateJWTToken(user.Id);
                            string tokenJson = JsonConvert.SerializeObject(jwt);
                            resultModel.IsSuccess = true;
                            resultModel.Token = tokenJson;
                        } else
                        { resultModel.IsSuccess = false;resultModel.Message = result.Errors.FirstOrDefault().Description; }
                      
                    }
                }
                catch (Exception e)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Message = e.Message;
                }
                return new JsonResult(resultModel, DefaultJsonSettings);
            }
        }

        [HttpPost]
        [EnableCors("any")]
        public async Task<IActionResult> POST(string userName, string userPwd, string grant_type, string client_id, string scope,int count=1)
        {
            ResultViewModel resultModel =new ResultViewModel { IsSuccess = false, Message = "登陆失败"}; ;
            //var isPersistent = rememnerMe == 1 ? true : false;
            var isPersistent = true;
            try
            {
                var signResult = await signInManager.PasswordSignInAsync(userName, userPwd, isPersistent, count > 3);
                if (signResult.Succeeded)
                { 
                    var jwt = tokenHelper.CreateJWTToken(await GetCurentUserId());
                    string tokenJson=JsonConvert.SerializeObject(jwt);
                    resultModel = new ResultViewModel { IsSuccess = true,Token= tokenJson };
                }
                if (signResult.IsLockedOut)
                    resultModel = new ResultViewModel { IsSuccess = false, Message = "你登陆已超过3次，该账户已被锁" };
                
            }
            catch (Exception e)
            {
                resultModel = new ResultViewModel { IsSuccess = false, Message =e.Message };
            }
            return new JsonResult(resultModel, DefaultJsonSettings);
        }
    }
}