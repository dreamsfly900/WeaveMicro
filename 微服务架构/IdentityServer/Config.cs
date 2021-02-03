// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //new Client
                //{
                //    ClientId = "client",

                //    // no interactive user, use the clientid/secret for authentication
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,

                //    // secret for authentication
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },

                //    // scopes that client has access to
                //    AllowedScopes = { "api1" }
                //},
                new Client
                {
                    ClientId = "client", //访问客户端Id,必须唯一
                    //使用客户端授权模式，客户端只需要clientid和secrets就可以访问对应的api资源。
                    //AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,  //密码模式
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes ={ "api1", IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.Profile }
                }
            };
        }
    }
}