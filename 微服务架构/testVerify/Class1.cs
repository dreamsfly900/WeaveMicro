using System;
using System.Security.Claims;
using WeaveVerify;

namespace testVerify
{
    public class Verifyabc : IdentityBase
    {
        public override string PrjName { get; set; } = "abc";

        public override Verifymode attestation(string Loginname, string Password)
        {
            Verifymode vm = new Verifymode();
            if (true)
            {
                vm.Verify = true;
                vm.Claims = new Claim[] {
                    new Claim("UserId", "123"),
                    new Claim("Name", "admin"),
                    new Claim("GivenName", "sdfq") } ;
               
            }
            else
            {
                vm.Verify = false;
                vm.ERRMessage = "XXX错误~！";
            }
            return vm;
        }
    }
}
