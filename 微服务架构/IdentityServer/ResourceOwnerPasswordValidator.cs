using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
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
            if (context.UserName == "admin" && context.Password == "123")
            {
                context.Result = new GrantValidationResult(
                 subject: context.UserName,
                 authenticationMethod: "custom",
                 claims: GetUserClaims());
            }
            else
            {

                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
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