using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ResourceOwnerPasswordValidator()
        {

        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法

            string Loginname = context.UserName.Trim();//用户名
            string Password = context.Password.Trim();//密码
            string type = context.Request.Raw["prj"].Trim();//项目名称

            if (string.IsNullOrEmpty(type))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "The requested resource does not exist");
                return;
            }
            try
            {
                if (type == "Signal")//信号宝
                {
                    SignalDAL.AdminService aDal = new SignalDAL.AdminService();

                    SignalModel.T_Admin admin = aDal.GetAdmin(Loginname);//获取账户信息

                    if (admin != null && admin.Password == WebStructure.DBUtility.MD5Helper.GetMD5(Password))
                    {
                        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);

                        string sign = "appid=" + Loginname + "&timestamp=" + Math.Round(ts.TotalMilliseconds, 0).ToString() + "&proj="+ type;

                        string authTicket = getSha256(sign);
                        aDal.UpdateTicket(admin.Id, authTicket);
            
                        //验证成功
                        context.Result = new GrantValidationResult(
                             subject: context.UserName,
                             authenticationMethod: "custom",
                             claims: new Claim[] {
                                new Claim("UserId", admin.Id.ToString()),
                                new Claim("Name",admin.LoginName),
                                new Claim("Phone", admin.Phone),
                                new Claim("Role","")
                             });
                    }
                    else
                    {
                        //验证失败
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名或密码不正确");
                    }
                }
            }
            catch (Exception e)
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }

            //if (context.UserName == "admin" && context.Password == "123")
            //{
            //    context.Result = new GrantValidationResult(
            //     subject: context.UserName,
            //     authenticationMethod: "custom",
            //     claims: GetUserClaims());
            //}
            //else
            //{

            //    //验证失败
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            //}
        }
        public static String getSha256(String strData)
        {
            try
            {
                byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(strData);
                
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (System.Exception ex)
            {
                throw new Exception("get sha256 error:" + ex.Message, ex);
            }
        }
        //可以根据需要设置相应的Claim
        private Claim[] GetUserClaims()
        {
            return new Claim[]
            {
            new Claim("UserId", "123"),
            new Claim("Name","admin"),
            new Claim("GivenName", "sdfq"),
            new Claim("FamilyName", "zxc"),
            new Claim("Email", "123211@qq.com"),
            new Claim("Role","admin")
            };
        }
    }
}