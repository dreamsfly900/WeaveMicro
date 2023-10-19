using System;
using System.Security.Claims;

namespace WeaveVerify
{
    public abstract class IdentityBase
    {
        public abstract string PrjName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Loginname"></param>
        /// <param name="Password"></param>
        /// <returns>
        ///     new Claim[]  {   new Claim("UserId", "123"),   new Claim("Name","admin"),   new Claim("GivenName", "sdfq")   };
        /// </returns>
public abstract Verifymode attestation(String Loginname, String Password);
    }
    public class Verifymode
    {
        public Claim[]  Claims { get; set; }
        public bool Verify { get; set; }
        public string ERRMessage { get; set; }
    }
}
