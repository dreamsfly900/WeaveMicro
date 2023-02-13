using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WeaveDoc
{
    /// <summary>
    /// Swagger扩展类，用于在Startup的Configure方法中注入
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Swagger配置缓存，用于在首页动态写入文档路径以支持多文档动态更新
        /// </summary>
        private static SwaggerUIOptions options = null;
        /// <summary>
        /// 开启Swagger接口文档。在Startup的Configure方法中使用
        /// </summary>
        /// <param name="builder">builder</param>
        /// <returns>builder</returns>
        public static IApplicationBuilder UseDoc(this IApplicationBuilder builder)
        {
            //读取config.json文件中SwaggerPath的值，来判断是否启用，以及确定文档路径
            //SwaggerPath:文档根目录，设置为空为根目录，设置为null为禁用
            string path = AppDomain.CurrentDomain.BaseDirectory + "config.json";
            string swaggerPath = string.Empty;
            if (File.Exists(path))//理论上肯定存在且有值，简单判断下
            {
                var json = File.ReadAllText(path);
                var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });
                //如果设置为数字或其他，为其字符串形式，如 1.23，True，[1,2,3]都支持，但太复杂的不支持（尤其带回车的）
                if (config.ContainsKey("SwaggerPath")) swaggerPath = config["SwaggerPath"]?.ToString();
                //如果设置为Null，则不启用文档，生产环境下可以指定
                if (swaggerPath == null) return builder;
            }
            //提前截断Swagger首页，在Swagger处理生成首页内容前，重新设定文档路径的值，支持动态多文档
            builder.Use(async (context, next) =>
            {
                if (context.Request.Method == "GET"
                    && Regex.IsMatch(context.Request.Path.Value, $"^/{Regex.Escape(options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
                {
                    options.ConfigObject.Urls = ConfigManage.Resources.Select(d => new UrlDescriptor { Url = d.Value, Name = d.Key });
                }
                await next.Invoke();
            });
            //注入到首页的js代码，用来支持多个自有认证服务，且可以手动指定。js代码比较粗糙，哈哈
            string js = @"
<script>
//文档加载2秒后（防止网络缓慢），改变授权界面（当点击首页授权按钮时触发）
setTimeout(() => document.querySelector('button.authorize').addEventListener('click', ModifOAuthPage, false), 2000);
function ModifOAuthPage(){
    setTimeout(() => {
        //隐藏部分字段的显示
        if(document.querySelector('div.scope-def')!=null)document.querySelector('div.scope-def').hidden = true;
        if(document.querySelector('div.scopes')!=null)document.querySelector('div.scopes').hidden = true;
        if(document.querySelector('p.flow')!=null)document.querySelector('p.flow').hidden = true;
        //隐藏请求地址，因为要改成可变的
        document.querySelector('button.btn.modal-btn.auth.authorize.button').parentElement.parentElement.querySelector('code').parentElement.hidden=true;
        //当已经添加了认证服务下拉框的控件，则退出不再处理
        if(document.querySelector('#oauth_server')!=null)return;
        //认证弹框界面中的第一个授权按钮添加同样的事件处理
        document.querySelector('button.btn.modal-btn.auth.authorize.button').addEventListener('click', ModifOAuthPage, false);
        
        //如果不包含用户名控件，说明不是password模式，退出
        if(document.querySelector('#oauth_username')==null)return;

        var username = document.querySelector('#oauth_username').parentNode.parentElement;

        //添加认证服务下拉框组件，添加各认证服务，添加手动设置选项
        var server = document.querySelector('#password_type').parentElement.parentElement.cloneNode(true);
        server.querySelector('select').setAttribute('id', 'oauth_server');
        server.querySelector('select').setAttribute('data-name', 'oauth_server');
        server.querySelector('select').value='';
        var optionsLength = server.querySelector('select').childNodes.length;
        for(var i=0;i<4;i++){server.querySelector('select').options.remove(0);}
        server.querySelector('select').childNodes.forEach(o=>server.querySelector('select').removeChild(o));
        var urls = window.ui.getStore().getState()
            .get('spec').get('resolvedSubtrees').get('components').get('securitySchemes')
            .get('api1').get('flows').get('password').get('x-urls').toArray();
        urls.forEach((s,i)=>server.querySelector('select').options.add(new Option(s,s,i==0)));
        server.querySelector('select').options.add(new Option('手动设置',''));
        server.querySelector('label').setAttribute('for', 'oauth_server');
        server.querySelector('label').textContent = '认证服务器：';
        username.insertAdjacentElement('beforebegin', server);

        //添加手动设置认证服务地址的文本框，并处理与下拉框的联动
        var server_val = username.querySelector('input').cloneNode(true);
        server_val.setAttribute('id', 'server_val');
        server_val.setAttribute('data-name', 'server_val');
        username.insertAdjacentElement('beforebegin', server_val);
        server_val.hidden=server.querySelector('select').value.length>0;
        server.querySelector('select').onchange=(e)=>server_val.hidden=(e.target.value.length>0);

        //添加项目名称控件
        var prj = username.cloneNode(true);
        prj.querySelector('input').setAttribute('id', 'oauth_prj');
        prj.querySelector('input').setAttribute('data-name', 'oauth_prj');
        prj.querySelector('input').value='KFQX';
        prj.querySelector('label').setAttribute('for', 'oauth_prj');
        prj.querySelector('label').textContent = '项目：';
        username.insertAdjacentElement('beforebegin', prj);

        //隐藏其他不需要的控件
        document.querySelector('#client_id').parentElement.parentElement.hidden = true;
        document.querySelector('#client_secret').parentElement.parentElement.hidden = true;
        document.querySelector('#password_type').parentElement.parentElement.hidden = true;
        document.querySelector('#password_type').value = 'request-body';

        document.querySelector('div.renderedMarkdown').nextElementSibling.hidden=true;
    },100);
}
function ModifOAuthQuery(req){
    //拦截认证请求
    if(req.url.indexOf('/connect/token')>-1 && req.body.indexOf('grant_type=password')>-1){
        req.body += '&prj='+document.querySelector('#oauth_prj').value;//增加参数，项目prj
        //根据认证服务下拉框及手动填写的认证服务地址，确定请求的url地址
        var url = document.querySelector('#server_val').value;
        if(document.querySelector('#server_val').hidden)url = document.querySelector('#oauth_server').value;
        req.url = url + '/connect/token';
    }
    return req;
}
</script>";
            builder.UseSwaggerUI(c =>
            {
                c.RoutePrefix = swaggerPath;
                //拦截认证请求，在其中改变目标地址，增加参数等
                c.Interceptors.RequestInterceptorFunction = "function (req) { return ModifOAuthQuery(req); }";
                //注入js代码，设置认证服务的几个默认值
                c.HeadContent += js;
                c.OAuthScopes("api1");
                c.OAuthClientId("client");
                c.OAuthClientSecret("secret");
                //缓存当前Swagger配置
                options = c;
            });
            return builder;
        }
    }
}