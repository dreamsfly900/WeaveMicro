using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Reflection;
using WeaveVerify;
using System.Collections.Concurrent;

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

                if (Program.keyValuePairs.ContainsKey(type))
                {
                    var Verifymode = Program.keyValuePairs[type].attestation(Loginname, Password);
                    if (Verifymode.Verify)
                    {
                        //验证成功
                        context.Result = new GrantValidationResult(
                             subject: context.UserName,
                             authenticationMethod: "custom",
                             claims: Verifymode.Claims
                             );
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, Verifymode.ERRMessage);
                    }
                }
                else
                { context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "不能认证相关项目"); }
                
            }
            catch (Exception e)
            {
                //验证失败                
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential:" + e.Message);
            }

            
        }
       
  
    
        //可以根据需要设置相应的Claim
      
    }
}