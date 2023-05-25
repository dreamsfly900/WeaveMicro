using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;
using wRPC;
using wRPCService;

namespace WeaveDoc
{
    /// <summary>
    /// API文档生成器，格式为OpenApi3.0形式。
    /// </summary>
    public class ApiDocGen
    {
        /// <summary>
        /// 控制器列表缓存
        /// </summary>
        private static Dictionary<string, object> Controllers = ToolLoad.Load(new Dictionary<string, object>());
        /// <summary>
        /// 文档引用的类型
        /// </summary>
        private static ConcurrentDictionary<string, int> Types = new ConcurrentDictionary<string, int>();
        /// <summary>
        /// 文档名称缓存，在注册路由时获取，在生成文档时使用
        /// </summary>
        internal static string docName = string.Empty;
        /// <summary>
        /// API文档缓存，每次启动只生成一次
        /// </summary>
        private static JObject json = null;
        /// <summary>
        /// API文档传入参数，发生变化时需要重新生成
        /// </summary>
        private static string infoData = null;
        /// <summary>
        /// 生成API文档OpenApi3格式，需要先使用AddRoute中先行注册
        /// </summary>
        /// <param name="data">网关及认证服务器列表数据，格式为json字符串的Base64编码</param>
        /// <returns>文档OpenApi3格式，以JObject类型体现</returns>
        [InstallFun(FunAttribute.Get, "Api文档生成")]
        public JObject info(string data)
        {
            //当传入参数变化时，清空缓存，记录参数
            if (data != infoData) { infoData = data; json = null; }
            //如果有缓存，则直接返回
            if (json != null && json.HasValues) return json;

            //获取传送过来的网关列表和认证服务列表
            var gatewaysAndOAuths = JsonSerializer.Deserialize<DocInfoData>(
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(data)));

            var apis = new List<ApiDescription>();
            //遍历所有控制器及方法
            foreach (var controller in Controllers)
            {
                var methods = controller.Value.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    //非API方法跳过
                    var installFunAttr = method.GetCustomAttribute<wRPC.InstallFunAttribute>();
                    if (installFunAttr == null) continue;
                    //控制器定义的路由被当作API分类
                    var route = controller.Key.Split('/').ToList();
                    if (route.FirstOrDefault() == "api") route.Remove("api");
                    string groupName = string.Join("/", route);
                    //创建API
                    string httpMethod = installFunAttr.Type == FunAttribute.Get ? "GET" : "POST";
                    string relativePath = string.Join("/", controller.Key, method.Name);
                    var api = SwaggerGen.CreateApi(method, groupName, httpMethod, relativePath);
                    //FILE方法需要额外添加参数
                    if (installFunAttr.Type == FunAttribute.file) api.AddFileParameter("fileName", true);

                    //其他与标准不同的地方，在ZOperationFilter中处理
                    apis.Add(api);
                }
            }
            //创建需要的配置，以及应用XML文档描述
            var (options, schemaOptions) = this.CreateOptions().ApplyXml();
            //添加网关信息
            options.AddGateways(gatewaysAndOAuths);
            //添加全局认证信息
            options.AddOAuths(gatewaysAndOAuths);
            //应用自定义过滤器,以适应自有框架特性
            options.OperationFilters.Add(new ZOperationFilter());
            //防止类型重名
            schemaOptions.SchemaIdSelector = type =>
            {
                //无保留字符，包括大写字母和小写字母、十进制数字、连字符、句点、下划线和波浪号。
                //另外，如果模型都是字段，则也不显示内容
                string name = type.ToString().Replace("+", "~");

                if (Types.ContainsKey(name)) { Types[name] += 1; return name + "_" + Types[name]; }
                else { Types.TryAdd(name, 0); return name; }
            };
            //DataTable等不支持的类型直接返回Object
            schemaOptions.CustomTypeMappings.Add(typeof(DataTable), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = nameof(DataTable) } });
            schemaOptions.CustomTypeMappings.Add(typeof(DataSet), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = nameof(DataSet) } });
            schemaOptions.CustomTypeMappings.Add(typeof(ExpandoObject), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = nameof(ExpandoObject) } });
            schemaOptions.CustomTypeMappings.Add(typeof(System.Collections.IDictionary), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "IDictionary" } });
            schemaOptions.CustomTypeMappings.Add(typeof(IDictionary<,>), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "IDictionary<,>" } });
            schemaOptions.CustomTypeMappings.Add(typeof(Dictionary<,>), () => new OpenApiSchema() { Type = "object", Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Dictionary<,>" } });
            //应用自定义过滤器,以适应自有框架特性
            options.DocumentFilters.Add(new ZCustomTypeMappingsFilter(schemaOptions.CustomTypeMappings));
            /*
            bool noSupportType = returnType == typeof(DataTable)
                || returnType == typeof(DataSet)
                || returnType == typeof(ExpandoObject)
                || returnType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                ;*/

            //组合文档内容
            var doc = new StringWriter();

            try
            {
                SwaggerGen.CreateGen(apis, options, schemaOptions)
                    .GetSwagger("v1")
                    .SerializeAsV3(new OpenApiJsonWriter(doc));
            }
            catch (Exception err)
            {
                //不加不影响使用，加了生活会更美好
                Console.ForegroundColor = ConsoleColor.Red;
                var errResult = new JObject { { "code", 503 }, { "msg", "服务器错误" } };
                //判断异步Api方法中忘记在TRY的return前加await
                string msg = "Failed to generate Operation for action - ";
                int offset = err.Message.IndexOf(msg);
                if (offset > -1 && err.GetBaseException()?.Message
                    ?.Contains("The same schemaId is already used for type \"$TResult\"") == true)
                {
                    msg = err.Message.Substring(offset + msg.Length, err.Message.IndexOf(" (", offset) - msg.Length);
                    msg = $"无法创建API（{msg}）,是否异步Api方法中忘记在TRY的return前加await了？";
                    Console.WriteLine($"ERR: {msg}");
                    errResult["msg"] += "：" + msg;
                }
                else
                {
                    Console.WriteLine(err);
                    errResult["msg"] += "：" + err;
                }
                Console.ResetColor();
                return errResult;
            }
            //缓存API文档并返回
            json = JObject.Parse(doc.ToString());
            return json;
        }

        /// <summary>
        /// 添加Swagger文档生成的路由地址
        /// <code>
        /// //插入到 RemoteService 类的构造函数中，必须在ToolLoad.GetService方法调用后紧跟执行
        /// sric = ToolLoad.GetService();
        /// ApiDocGen.AddRoute(Name, ref sric, service);
        /// </code>
        /// </summary>
        /// <param name="name">文档名称</param>
        /// <param name="ser">服务方法列表</param>
        /// <param name="service">服务访问通道</param>
        public static void AddRoute(string name, ref service[] ser, ServiceChannel service)
        {
            //缓存文档标题，不同的入口，所以要缓存一下
            docName = name;
            var temp = ser.ToList();
            temp.Add(new service()
            {
                Authorize = false,
                Method = "Get",
                Route = $"{docName}/swagger/info",
                annotation = "Api文档生成",
                parameter = new[] { "data" },
                parameterexplain = new[] { "String,@网关及认证信息|" }
            });
            //更新服务方法列表，用于注册到服务中心，否则网关找不到不会传输方法到服务中
            ser = temp.ToArray();
            //同上，原架构中查找API的过滤方法不同
            //ServiceChannel是根据控制器的RouteAttribute先过滤一遍，不管方法存在否，因此这里也要Hacker一下
            var o = (typeof(ServiceChannel).GetField("keyValuePairs", BindingFlags.NonPublic | BindingFlags.Instance)).GetValue(service) as Dictionary<string, object>;
            o.Add($"{docName}/swagger", new ApiDocGen());
        }
    }

    /// <summary>
    /// API方法操作过滤器，框架自定义的标准主要在这里实现
    /// </summary>
    public class ZOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="operation">API方法</param>
        /// <param name="context">过滤器上下文</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //获取方法中的自定义特性
            var installFunAttr = context.MethodInfo.GetCustomAttribute<wRPC.InstallFunAttribute>();
            var authorizeAttr = context.MethodInfo.GetCustomAttribute<wRPC.AuthorizeAttribute>();
            //应用方法的描述（当XML注释不存在时）
            operation.Description = string.IsNullOrWhiteSpace(operation.Summary) ? installFunAttr.Annotation : operation.Summary;
            operation.Summary = operation.Description;// operation.OperationId + " (" + operation.Description + ")";
            operation.OperationId = context.ApiDescription.RelativePath;
            //初始化请求的ContentType
            operation.InitRequestContentType(installFunAttr);

            var paramsList = context.MethodInfo.GetParameters();
            //根据参数列表检查请求的ContentType，因为框架不支持部分HTTP特征
            operation.CheckRequestContentType(paramsList);
            foreach (var p in paramsList)
            {
                var pp = operation.Parameters.SingleOrDefault(d => d.Name == p.Name);
                if (pp == null) continue;
                //设置参数是否可选，目前根据框架现状进行设置，后期可对其扩展
                //GET方法所有引用类型参数均可选，NONE POST 方法全必填
                pp.Required = !(installFunAttr.Type == FunAttribute.Get && (!p.ParameterType.IsPrimitive() || p.ParameterType.Equals(typeof(string))));
                //pp.Required = true;// !p.IsOptional && !p.HasDefaultValue; //框架中所有的参数都是必填项，无论是否可选
                //应用参数的描述（当XML注释不存在时）
                if (string.IsNullOrWhiteSpace(pp.Description)) pp.Description = p.GetCustomAttribute<wRPC.ParamAttribute>()?.explain;
                //应用POST/NONE/FILE的请求体的 描述、示例、和其他特性，因为其参数列表最终要从Parameters中删除
                if (installFunAttr.Type != FunAttribute.Get) operation.ApplyToRequestBody(paramsList, p, pp);
            }
            //POST/NONE/FILE的参数都放到请求体中去了，因此参数列表清空。（未考虑到全局参数的影响）
            if (installFunAttr.Type != FunAttribute.Get) operation.Parameters.Clear();
            //是否需要授权
            if (authorizeAttr != null) operation.AddOAuths();
            operation.Responses["200"].Description = "执行成功";
        }

    }

    /// <summary>
    /// 添加自定义类型的文档过滤器，如DataTable等。
    /// </summary>
    public class ZCustomTypeMappingsFilter : IDocumentFilter
    {
        /// <summary>
        /// 自定义类型
        /// </summary>
        private IDictionary<Type, Func<OpenApiSchema>> CustomTypeMappings = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="customTypeMappings">自定义类型</param>
        public ZCustomTypeMappingsFilter(IDictionary<Type, Func<OpenApiSchema>> customTypeMappings) {
            CustomTypeMappings = customTypeMappings;
        }
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="swaggerDoc">Swagger文档</param>
        /// <param name="context">过滤器上下文</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var o in CustomTypeMappings) {
                var type = o.Value.Invoke();
                swaggerDoc.Components.Schemas.Add(type.Reference.Id, type);
            }
        }
    }
    /// <summary>
    /// API文档扩展类
    /// </summary>
    internal static class ApiDocExtension
    {
        /// <summary>
        /// 指定对象类型是否可以省略在OpenApi文档中的定义
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>是否可以省略在OpenApi文档中的定义</returns>
        internal static bool IsPrimitive(this Type type) => type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        /// <summary>
        /// 创建默认配置
        /// </summary>
        /// <param name="gen">API文档生成器</param>
        /// <returns>(SwaggerGeneratorOptions, SchemaGeneratorOptions)</returns>
        internal static (SwaggerGeneratorOptions, SchemaGeneratorOptions) CreateOptions(this ApiDocGen gen)
        {
            var options = new SwaggerGeneratorOptions
            {
                //设置标题
                SwaggerDocs = new Dictionary<string, OpenApiInfo> { ["v1"] = new OpenApiInfo { Version = "v1.0", Title = ApiDocGen.docName } },
                //包含所有API，否则默认的生成器返回为空
                DocInclusionPredicate = (s, apiDesc) => true,
                //将传入的GroupName作为Tag名称
                TagsSelector = (apiDesc) => new[] { apiDesc.GroupName },
            };
            //需要认证的API可以使用所有认证方式
            options.SecuritySchemesSelector = (list) => options.SecuritySchemes;
            return (options, new SchemaGeneratorOptions());
        }
        /// <summary>
        /// 应用XML文档
        /// </summary>
        /// <param name="options">配置信息：(SwaggerGeneratorOptions, SchemaGeneratorOptions)</param>
        /// <returns>(SwaggerGeneratorOptions, SchemaGeneratorOptions)</returns>
        internal static (SwaggerGeneratorOptions, SchemaGeneratorOptions) ApplyXml(this (SwaggerGeneratorOptions, SchemaGeneratorOptions) options)
        {
            //遍历根目录下所有XML文档，将其应用
            var xmlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
            foreach (var file in xmlFiles)
            {
                var xmlDoc = new XPathDocument(file);
                options.Item1.ParameterFilters.Add(new XmlCommentsParameterFilter(xmlDoc));
                options.Item1.RequestBodyFilters.Add(new XmlCommentsRequestBodyFilter(xmlDoc));
                options.Item1.OperationFilters.Add(new XmlCommentsOperationFilter(xmlDoc));
                options.Item1.DocumentFilters.Add(new TagsCommentsFilter(xmlDoc));

                options.Item2.SchemaFilters.Add(new XmlCommentsSchemaFilter(xmlDoc));
            }
            return options;
        }
        /// <summary>
        /// 添加网关信息
        /// </summary>
        /// <param name="options">Swagger配置信息</param>
        /// <param name="gateways">网关列表</param>
        /// <returns>Swagger配置信息</returns>
        internal static SwaggerGeneratorOptions AddGateways(this SwaggerGeneratorOptions options, DocInfoData gateways)
        {
            //支持HTTP和HTTPS切换
            var variables = new Dictionary<string, OpenApiServerVariable>
            {
                { "http", new OpenApiServerVariable() { Default = "http", Enum = new List<string> { "http", "https" } } }
            };
            foreach (var o in gateways.Gateways)
                options.Servers.Add(new OpenApiServer() { Url = $"{{http}}://{o.IP}:{o.port}", Description = o.Sid, Variables = variables });
            return options;
        }
        /// <summary>
        /// 添加全局认证信息
        /// </summary>
        /// <param name="options">Swagger配置信息</param>
        /// <param name="oauths">认证服务器列表</param>
        /// <returns>Swagger配置信息</returns>
        internal static SwaggerGeneratorOptions AddOAuths(this SwaggerGeneratorOptions options, DocInfoData oauths)
        {
            //生成认证服务列表，如果没有，默认1221端口
            var x_urls = new OpenApiArray();
            oauths.AuthServers.ForEach(d => x_urls.Add(new OpenApiString(d)));
            if (x_urls.Count == 0) x_urls.Add(new OpenApiString("http://127.0.0.1:1221"));
            //添加OAuth认证，仅适用IdServer
            options.SecuritySchemes.Add("api1", new OpenApiSecurityScheme()
            {
                Description = "JWT认证授权",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Password = new OpenApiOAuthFlow()
                    {
                        TokenUrl = new Uri((x_urls.FirstOrDefault() as OpenApiString).Value + "/connect/token"),///connect/token用来在Js中判断授权拦截
                        Scopes = new Dictionary<string, string>() { ["api1"] = "默认" },
                        Extensions = new Dictionary<string, IOpenApiExtension> { ["x-urls"] = x_urls }
                    }
                }
            });
            //添加JWT认证，方便其他情况
            options.SecuritySchemes.Add("jwt", new OpenApiSecurityScheme()
            {
                Description = "请输入授权Token(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });
            return options;
        }
        /// <summary>
        /// 添加文件参数
        /// </summary>
        /// <param name="api">API描述</param>
        /// <param name="paramName">参数名称，默认fileName</param>
        /// <param name="isRequired">是否必填，默认必填</param>
        /// <returns>API描述</returns>
        internal static ApiDescription AddFileParameter(this ApiDescription api, string paramName = "fileName", bool isRequired = true)
        {
            api.ParameterDescriptions.Add(new ApiParameterDescription()
            {
                Name = paramName,
                Source = BindingSource.FormFile,
                IsRequired = isRequired,
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(IFormFile)),
            });
            return api;
        }
        /// <summary>
        /// 初始化请求的ContentType
        /// </summary>
        /// <param name="operation">Api动作</param>
        /// <param name="attr">自定义的HTTP谓词特性</param>
        /// <returns>Api动作</returns>
        internal static OpenApiOperation InitRequestContentType(this OpenApiOperation operation, InstallFunAttribute attr)
        {
            //GET方法不进行操作
            if (attr.Type == FunAttribute.Get) return operation;
            //FILE方法一定是 multipart/form-data ，则上个注释就好
            if (attr.Type == FunAttribute.file) operation.RequestBody.Content["multipart/form-data"].Schema.Description = "请选择文件";
            //POST/NONE方法，如果到此时没有设置过 ContentType ，初始化
            else if (operation.RequestBody == null || operation.RequestBody.Content.Count == 0)
            {
                if (operation.RequestBody == null) operation.RequestBody = new OpenApiRequestBody();
                List<string> requestContentType = new List<string>();
                //NONE方法主要使用JSON传输，特殊情况下可以使用FORM
                if (attr.Type == FunAttribute.NONE) { requestContentType.Add("application/json"); requestContentType.Add("application/x-www-form-urlencoded"); }
                //POST方法只能使用FORM传输
                if (attr.Type == FunAttribute.POST) { requestContentType.Add("application/x-www-form-urlencoded"); }
                if (attr.Type == FunAttribute.file) { requestContentType.Add("multipart/form-data"); }
                requestContentType.ForEach(d => operation.RequestBody.Content.Add(d, new OpenApiMediaType()));
            }
            return operation;
        }
        /// <summary>
        /// 根据方法的参数列表检查请求的ContentType
        /// </summary>
        /// <param name="operation">Api动作</param>
        /// <param name="paramsList">参数列表</param>
        /// <returns>Api动作</returns>
        internal static OpenApiOperation CheckRequestContentType(this OpenApiOperation operation, ParameterInfo[] paramsList)
        {
            //目前不支持的模式，看以后是否支持。理论上可以想办法支持
            if (operation.RequestBody != null)
            {
                //多个参数不能使用json
                if (paramsList.Length > 1 && operation.RequestBody.Content.ContainsKey("application/json"))
                    operation.RequestBody.Content.Remove("application/json");
                //有复杂类型不能使用Form
                if (paramsList.Where(d => !IsPrimitive(d.ParameterType)).Count() > 0
                    && operation.RequestBody.Content.ContainsKey("application/x-www-form-urlencoded"))
                    operation.RequestBody.Content.Remove("application/x-www-form-urlencoded");

                //如果所有请求类型均不正确，修改API描述以便提醒开发人员
                if (operation.RequestBody.Content.Count == 0)
                {
                    operation.Description += "【接口定义错误，请联系开发人员】";
                    operation.Summary = operation.Description;
                }
            }
            return operation;
        }
        /// <summary>
        /// 给方法添加认证信息（固定的api和jwt）
        /// </summary>
        /// <param name="operation">方法描述对象</param>
        /// <returns>方法描述对象</returns>
        internal static OpenApiOperation AddOAuths(this OpenApiOperation operation)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "暂无访问权限" });
            //给api添加锁的标注
            new List<string>() { "api1", "jwt" }.ForEach(s =>
            {
                operation.Security.Add(
                    new OpenApiSecurityRequirement() { {
                            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = s } }
                            , new[] { s }
                    } }
                );
            });
            return operation;
        }
        /// <summary>
        /// 将参数应用到请求体中（当HTTP谓词为POST时）
        /// </summary>
        /// <param name="operation">API方法描述对象</param>
        /// <param name="paramsList">参数列表</param>
        /// <param name="p">当前参数信息</param>
        /// <param name="pp">当前参数对应的文档描述对象信息</param>
        /// <returns>API方法描述对象</returns>
        internal static OpenApiOperation ApplyToRequestBody(this OpenApiOperation operation, ParameterInfo[] paramsList, ParameterInfo p, OpenApiParameter pp)
        {
            //复制参数属性
            pp.Schema.Description = pp.Description;
            pp.Schema.Example = pp.Example;
            foreach (var c in operation.RequestBody.Content)
            {
                //对于JSON格式，仅单一参数有效
                if (paramsList.Length == 1 && c.Key == "application/json") c.Value.Schema = pp.Schema;
                else
                {
                    //对于表单格式，需要逐个添加参数信息
                    if (c.Value.Schema == null) c.Value.Schema = new OpenApiSchema() { Type = "object" };
                    c.Value.Schema.Properties.Add(p.Name, pp.Schema);
                }
                //确定参数必填性
                if (pp.Required) c.Value.Schema.Required.Add(p.Name);
            }
            return operation;
        }
    }
}