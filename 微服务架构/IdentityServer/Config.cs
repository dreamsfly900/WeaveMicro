// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

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
                new ApiResource(OAuthConfig.UserApi.ApiName, "api service")
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
                    ClientId = OAuthConfig.UserApi.ClientId, //访问客户端Id,必须唯一
                    //使用客户端授权模式，客户端只需要clientid和secrets就可以访问对应的api资源。
                    //AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,  //密码模式
                    ClientSecrets = { new Secret(OAuthConfig.UserApi.Secret.Sha256())  },
                    AllowOfflineAccess = true,//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true
                    AllowedScopes ={
                        OAuthConfig.UserApi.ApiName,
                        StandardScopes.OfflineAccess,//如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AccessTokenLifetime = 3600 * 10 //失效时间
                }
            };
        }
    }
    /// <summary>
    /// 新增的自定义微信客户端的授权模式
    /// </summary>
    public static class GrantTypeConstants
    {
        /// <summary>
        /// GrantType - 微信端授权
        /// </summary>
        public const string ResourceWeixinOpen = "weixinopen";
    }

    public class OAuthConfig
    {
        /// <summary>
        /// 过期秒数
        /// </summary>
        public const int ExpireIn = 36000;

        /// <summary>
        /// 用户Api相关
        /// </summary>
        public static class UserApi
        {
            public static string ApiName = "api1";

            public static string ClientId = "client";

            public static string Secret = "secret";
        }
    }
}