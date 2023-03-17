using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.XPath;
using wRPC;
/*
本文件定义的方法进行的操作是符合标准的，框架自定义的部分适配方法，在ApiDocGen中（主要在ZOperationFilter中操作）
*/
namespace WeaveDoc
{
    /// <summary>
    /// 默认Swagger文档生成器
    /// </summary>
    internal static class SwaggerGen
    {
        /// <summary>
        /// 创建生成器
        /// </summary>
        /// <param name="apiDescriptions">Api描述列表</param>
        /// <param name="options">生成配置参数</param>
        /// <param name="schemaOptions">模式生成配置参数</param>
        /// <returns>Swagger生成器</returns>
        public static SwaggerGenerator CreateGen(
            IEnumerable<ApiDescription> apiDescriptions,
            SwaggerGeneratorOptions options = null,
            SchemaGeneratorOptions schemaOptions = null)
        {
            schemaOptions.UseAllOfForInheritance = true;
            return new SwaggerGenerator(options ?? new SwaggerGeneratorOptions()
                , new ApiGroupProvider(apiDescriptions)
                , new SchemaGenerator(schemaOptions ?? new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new JsonSerializerOptions()))
            );
        }
        /// <summary>
        /// 创建Api描述信息
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="httpMethod">HTTP谓词</param>
        /// <param name="relativePath">路由地址</param>
        /// <returns>Api描述信息</returns>
        public static ApiDescription CreateApi(
            MethodInfo methodInfo,
            string groupName = "v1",
            string httpMethod = "POST",
            string relativePath = "resoure")
        {
            var action = CreateAction(methodInfo);
            var api = new ApiDescription
            {
                ActionDescriptor = action,
                GroupName = groupName,
                HttpMethod = httpMethod,
                RelativePath = relativePath,
            };
            //复制参数信息
            foreach (var p in action.Parameters)
            {
                api.ParameterDescriptions.Add(new ApiParameterDescription()
                {
                    Name = p.Name,
                    ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(p.ParameterType),
                    ParameterDescriptor = p
                });
            }
            //设置返回类型
            var returnType = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null
                ? methodInfo.ReturnType.GetGenericArguments().FirstOrDefault()
                : methodInfo.ReturnType;
            //如果是框架定义的返回类型
            if (returnType == typeof(IApiResult))
            {
                //通过IL获取到定义的临时变量,从中获取最终可能返回的类型
                var localType = methodInfo.GetMethodBody().LocalVariables
                    ?.Select(v => v.LocalType
                        .FindMembers(MemberTypes.All, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, null)
                        .Select(m => (m as FieldInfo)?.FieldType)
                        .Where(d => typeof(IApiResult).IsAssignableFrom(d)))
                    ?.FirstOrDefault(d => d.Count() > 0)
                    ?.FirstOrDefault();
                if (localType != null) returnType = localType;
            }
            api.SupportedResponseTypes.Add(new ApiResponseType()
            {
                StatusCode = 200,
                ApiResponseFormats = new List<ApiResponseFormat>() { new ApiResponseFormat { MediaType = "application/json" } },
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(returnType),
                Type = returnType
            });
            return api;
        }
        /// <summary>
        /// 创建动作描述
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <returns>动作描述</returns>
        private static ActionDescriptor CreateAction(MethodInfo method)
        {
            var httpMethodAttribute = method.GetCustomAttribute<HttpMethodAttribute>();
            return new ControllerActionDescriptor
            {
                AttributeRouteInfo = new AttributeRouteInfo { Template = httpMethodAttribute?.Template, Name = httpMethodAttribute?.Name ?? method.Name },
                ControllerTypeInfo = method.DeclaringType.GetTypeInfo(),
                ControllerName = method.DeclaringType.Name,
                MethodInfo = method,
                Parameters = method.GetParameters().Select(p => new ControllerParameterDescriptor
                {
                    Name = p.Name,
                    ParameterInfo = p,
                    ParameterType = p.ParameterType,
                } as ParameterDescriptor).ToList(),
                RouteValues = new Dictionary<string, string> { ["controller"] = method.DeclaringType.Name.Replace("Controller", string.Empty) }
            };
        }
    }
    /// <summary>
    /// API分组信息（Tags）描述应用，使用控制器的XML描述
    /// </summary>
    internal class TagsCommentsFilter : IDocumentFilter
    {
        /// <summary>
        /// XML文档查询器
        /// </summary>
        private readonly XPathNavigator xml;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xmlDoc">XML文档对象</param>
        public TagsCommentsFilter(XPathDocument xmlDoc) => xml = xmlDoc.CreateNavigator();
        /// <summary>
        /// IDocumentFilter接口应用Tags描述
        /// </summary>
        /// <param name="swaggerDoc">API文档对象</param>
        /// <param name="context">文档过滤器上下文</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //获取所有Tags对应的控制器Type
            var types = context.ApiDescriptions
                .Select(apiDesc => new { apiDesc.GroupName, Controller = apiDesc.ActionDescriptor as ControllerActionDescriptor })
                .Where(actionDesc => actionDesc.Controller != null)
                .GroupBy(actionDesc => actionDesc.GroupName)
                .Select(group => new KeyValuePair<string, Type>(group.Key, group.First().Controller.ControllerTypeInfo.AsType()));
            foreach (var type in types)
            {
                //Tags的描述使用其控制器的描述
                var path = $"/doc/members/member[@name='{XmlCommentsNodeNameHelper.GetMemberNameForType(type.Value)}']";
                var summaryNode = xml.SelectSingleNode(path)?.SelectSingleNode("summary");
                if (summaryNode == null) continue;
                swaggerDoc.Tags.Add(new OpenApiTag
                {
                    Name = type.Key,
                    Description = XmlCommentsTextHelper.Humanize(summaryNode.InnerXml)
                });
            }
        }
    }
    /// <summary>
    /// API分组信息提供器
    /// </summary>
    internal class ApiGroupProvider : IApiDescriptionGroupCollectionProvider
    {
        /// <summary>
        /// API描述列表缓存
        /// </summary>
        private readonly IEnumerable<ApiDescription> apiList;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="apiList">API描述列表</param>
        public ApiGroupProvider(IEnumerable<ApiDescription> apiList) => this.apiList = apiList;
        /// <summary>
        /// API分组描述集合
        /// </summary>
        public ApiDescriptionGroupCollection ApiDescriptionGroups
        {
            get
            {
                //根据GroupName进行分组，GroupName在创建API描述时传入
                var groups = apiList
                    .GroupBy(o => o.GroupName)
                    .Select(g => new ApiDescriptionGroup(g.Key, g.ToList()))
                    .ToList();
                return new ApiDescriptionGroupCollection(groups, 1);
            }
        }
    }
}