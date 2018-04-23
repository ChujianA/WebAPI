using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Common
{
    public class TokenHelper
    {

        public static readonly string Issuer = "http://localhost:9460";
        //token的过期时间
        private TimeSpan TokenExpiration;
        //定义SecurityKey，类中的属性用于签名认证
        private SigningCredentials SigningCredentials;
        //定义私钥
        private static readonly string Private_Key = "private_key_1234567890";
        //使用对称算法生成的对称安全密钥
        public static readonly SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Private_Key));
        public TokenHelper()
        {
            TokenExpiration = TimeSpan.FromMinutes(10);
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
        public object CreateJWTToken(string uId)
        {
             DateTime now = DateTime.UtcNow;
            var claims = new[] {
               //发布人
              new Claim(JwtRegisteredClaimNames.Iss,Issuer),
              //给当前用户授权
              new Claim(JwtRegisteredClaimNames.Sub,uId),
              new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
              //产生时间
              new Claim(JwtRegisteredClaimNames.Iat,new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
          };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.Add(TokenExpiration), 
                signingCredentials: SigningCredentials);
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            var jwt = new
            {
                access_token = encodedToken,
                expiration = (int)TokenExpiration.TotalSeconds
            };
            return jwt;
        }
        
    }
}
