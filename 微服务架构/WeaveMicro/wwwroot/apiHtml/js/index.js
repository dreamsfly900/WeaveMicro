//加载json
function handleRequest() {
    $(".moduleList").empty();
    try {
        $.ajax({
            type: "get",
            async: true,
            url: "http://127.0.0.1:5022/apiHtml/json/temp.json",
            dataType: "json",
            success: function (jsonData) {
                console.log(jsonData)

                var apiUrl = "http://" + jsonData[0].IP + ":" + jsonData[0].Port + "/";
                $("#url").val("http://116.255.241.138:1221/");
                var itemArr = [];
                $.each(jsonData, function (i, api) {
                    for (var item of api.services) {
                        var name = item.Route.split("/")[0],
                            ind = $.inArray(name, itemArr);
                        if (ind == -1) {
                            itemArr.push(name);
                            ind = itemArr.length - 1;
                            $(".moduleList").append(`<li class="resource" id="resource_${name}">
                    <div>
                        <span class="subTit routename">${name}</span>
                        <p class="operaBtnPart inBlock fRig">
                            <button class="showOrhide">显示/隐藏</button>
                            <button class="showOperaOrhideOpera">展开/隐藏操作</button>
                        </p>
                    </div>
                    <ul class="slideList">  </ul>
                    </li>`);
                        }
                        var liHtml = "";
                        //请求 参数
                        var Params_post = [], Params_get = "";
                        var parameters = item.parameter;
                        var parameterstr = item.parameterexplain;
                        $.each(parameters, function (i, pp) {
                            var descText = parameterstr[i].split("|")[0].replace("@", "");
                            Params_get += ` <tr> <td>${pp}</td><td><input type="text" placeholder="(required)" class="parameter required" minlenth="1" name="${pp}"></td><td>${descText.split(',')[1]}</td><td>query</td><td>${descText.split(',')[0]}</td></tr>`
                            Params_post.push("\"" + pp + "\":\"" + descText.split(',')[1] + "\"");
                        })
                        if (["post", "POST"].includes(item.Method)) {
                            liHtml = `<li class="${item.Method} operation" id="${item.Route.replace("/", "_")}">
                    <p class="col hander">
                        <button class="httpMethod">${item.Method}</button>
                        <span class="httpPath"><a href="#${item.Route}" class="routepath">${item.Route}</a></span>
                        <span class="fRig defColor httpOptions">${item.annotation}</span>
                    </p>
                    <div class="operaDetails">
                        <p>响应Content Type 
                            <select name="respContentType"><option value="application/json">application/json</option><option value="text/json">text/json</option></select>
                        </p>
                        <p class="defColor">参数</p>
                        <table class="tableParameters">
                            <thead><tr><td width="100px">参数</td><td width="300px">值</td><td width="200px">描述</td><td width="100px">参数类型</td><td width="200px">数据类型</td></tr></thead>
                            <tbody>
                                <tr>
                                    <td>mode</td>
                                    <td>
                                        <textarea name="mode" class="body-textarea required" cols="30" rows="10" placeholder="(required)"></textarea>
                                        <p>Parameter content type:</p>
                                        <select name="paramContentType"><option value="application/json">application/json</option><option value="text/json">text/json</option><option value="application/x-www-form-urlencoded">application/x-www-form-urlencoded</option></select>
                                    </td>
                                    <td>参数</td>
                                    <td>body</td>
                                    <td>
                                        <p>Example Value</p>
                                        <div class="examBox"><pre><code> ${"{" + Params_post.join(',') + "}"}</code></pre></div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <p class="bottBtn"><button class="tryBtn">试一下！</button></p>
                        <div class="resPart reponse_body" style="display:none;">
                            <p class="defColor">请求URL</p><div class="box request_url">${apiUrl + item.Route}</div>
                            <p class="defColor">响应体</p><div class="box response_body"></div> 
                            <p class="defColor">响应码</p><div class="box response_code"></div> 
                        </div>
                    </div>
                </li>`;
                        } else {
                            var className = "GET";
                            liHtml = ` <li class="${className} operation" id="${item.Route.replace("/", "_")}">
                    <p class="col hander">
                        <button class="httpMethod">${item.Method}</button>
                        <span class="httpPath"><a href="#${item.Route}" class="routepath">${item.Route}</a></span>
                        <span class="fRig defColor httpOptions">${item.annotation}</span>
                    </p>
                    <div class="operaDetails">
                        <p>响应Content Type <select name="paramContentType"><option value="application/json">application/json</option><option value="text/json">text/json</option></select></p>
                        <p class="defColor">参数</p>
                        <table class="tableParameters">
                            <thead><tr><td width="100px">参数</td><td width="300px">值</td><td width="200px">描述</td><td width="100px">参数类型</td><td width="200px">数据类型</td></tr></thead>
                            <tbody>`;
                            liHtml += Params_get;
                            liHtml += `</tbody></table>
                        <p class="bottBtn"><button class="tryBtn">试一下！</button> <span class="hideResBtn defColor">隐藏响应</span></p>
                        <div class="resPart reponse_body" style="display:none;">
                            <p class="defColor">请求URL</p><div class="box request_url">${apiUrl + item.Route}</div>
                            <p class="defColor">响应体</p><div class="box response_body"></div> 
                            <p class="defColor">响应码</p><div class="box response_code"></div> 
                        </div>
                    </div>
                </li>`;

                        }
                        $(".moduleList>li:eq(" + ind + ") .slideList").append(liHtml);
                    }
                });
                var params = location.href.split("#")[1];
                console.log(params)
                if (params != null) {
                    var list = params.split('/');
                    $(".moduleList > li[id='resource_" + list[0] + "']").find(".slideList").show();
                    $(".slideList li[id='" + list[0] + "_" + list[1] + "']").find(".operaDetails").slideDown();
                }
            }
        });
    } catch (e) {
        console.log(e);
    }
    const isPathParam = word => word.includes('{') || word.includes('}');
    // 去除括号
    const delBrackets = word => word.replace(/{|}/g, '');
    // 替换横杠
    const replaceHorizontal = word => word.replace(/-/g, '_');
    // 类型转换
    const transType = (type) => {
        if (type === 'string') {
            return 'string';
        } else if (type === 'number' || type === 'integer') {
            return 'number';
        } else if (type === 'boolean') {
            return 'boolean';
        } else if (type === 'array') {
            return 'any[]';
        }
        return type;
    };
    var d, v = {};
    "undefined" != typeof window ? d = window : "undefined" != typeof self ? d = self : (console.warn("Using browser-only version of superagent in non-browser environment"),
        d = this);
    v.serialize = {
        //"application/x-www-form-urlencoded": i,
        "application/json": JSON.stringify
    },
        v.parse = {
            //"application/x-www-form-urlencoded": o,
            "application/json": JSON.parse
        }
    function s(e) {
        var t, n, r, i, a = e.split(/\r?\n/), o = {};
        a.pop();
        for (var s = 0, l = a.length; s < l; ++s)
            n = a[s],
                t = n.indexOf(":"),
                r = n.slice(0, t).toLowerCase(),
                i = b(n.slice(t + 1)),
                o[r] = i;
        return o
    }
    var AjaxRequest = function () {
        this.parameters = {};//参数
        this._timeout = 0;
    }

    AjaxRequest.prototype = {
        _timeout: 0,
        getXHR: function () {
            if (!(!d.XMLHttpRequest || d.location && "file:" == d.location.protocol && d.ActiveXObject))
                return new XMLHttpRequest;
            try {
                return new ActiveXObject("Microsoft.XMLHTTP")
            } catch (e) { }
            try {
                return new ActiveXObject("Msxml2.XMLHTTP.6.0")
            } catch (e) { }
            try {
                return new ActiveXObject("Msxml2.XMLHTTP.3.0")
            } catch (e) { }
            try {
                return new ActiveXObject("Msxml2.XMLHTTP")
            } catch (e) { }
            throw Error("Browser-only verison of superagent could not find XHR")
        },
        send: function (p) {
            var t = this
                , n = this.xhr = this.getXHR()
                , i = !this._timeout

            this.parameters = p;
            this.url = p.apiUrl + p.url;
            this.method = p.method;
            this.header = p.header;
            var data = p.data;

            this.appendQueryString()
            $.ajax({

                type: this.method,

                url: this.url,

                async: true,

                data: data,

                headers: {
                    "Access-Control-Allow-Origin": "*",
                    "Access-Control-Allow-Credentials": "true",
                    "Content-Type":"application/json;charset=utf8"
                },
                ajaxGridOptions: {
                    xhrFields: {
                        withCredentials: true
                    }
                },

                crossDomain: true, // 发送Ajax时，Request header 中会包含跨域的额外信息，但不会含cookie（作用不明，不会影响请求头的携带）

                success: function (data) {

                    console.log(data);

                }

            });

            //n.onreadystatechange = function () {
            //    if (4 == n.readyState) {
            //        var e;
            //        try {
            //            e = n.status
            //        } catch (r) {
            //            e = 0
            //        }
            //        // 接收响应数据
            //        t.showResponse();
            //        if (0 == e) {
            //            //if (t.timedout) {
            //            //    t = new Error("timeout of " + e + "ms exceeded");
            //            //    return;
            //            //}
            //            //if (t._aborted)
            //            //    return;
            //            //var e = new Error("Request has been terminated\nPossible causes: the network is offline, Origin is not allowed by Access-Control-Allow-Origin, the page is being unloaded, etc.");
            //            //e.crossDomain = !0

            //        }

            //    }
            //}
            ////if (i && !this._timer && (this._timer = setTimeout(function () {
            ////    t.timedout = !0, this._aborted ? this : (this._aborted = !0,
            ////        this.xhr && this.xhr.abort(), t._timeout = 0, clearTimeout(t._timer))
            ////}, i)),
            //this.appendQueryString(), n.open(this.method, this.url, !0),
            //    this._withCredentials = !0 && (n.withCredentials = !0);
            ////    "GET" != this.method && "HEAD" != this.method && "string" != typeof a) {
            ////    var u = this.header["Content-Type"]
            ////        , c = u ? u.split(";")[0] : ""
            ////};
       
            //for (var ph in this.header)
            //    null != this.header[ph] && n.setRequestHeader(ph, this.header[ph]);
                        

            //return this.parameters.responseType && (n.responseType = this.parameters.responseType),

            //    n.send("undefined" != typeof data ? data : null), this
        },
        /**
         * query参数
         * @param parameters
         */
        appendQueryString: function () {
            var pps = [];
            var pp = this.parameters.data;
            if (pp != "" && pp != undefined) {
                pp = typeof pp != "object" ? JSON.parse(pp) : pp;
                for (var key in pp) {
                    pps.push(key + "=" + pp[key])
                }
            }
            var e = pps.join("&");
            e && (this.url += ~this.url.indexOf("?") ? "&" + e : "?" + e);
        },
        showResponse() {
            var e = this;
            this.responseText = "HEAD" != this.method && ("" === this.xhr.responseType || "text" === this.xhr.responseType) || "undefined" == typeof this.xhr.responseType ? this.xhr.responseText : null,
                this.responseHeader = s(this.xhr.getAllResponseHeaders()), this.responseHeader["content-type"] = this.xhr.getResponseHeader("content-type");
            var dom = this.parameters.parent;
            try {
                var f = e.responseText || e.response;
                r = "string" == typeof f ? {} : f;
                dom.find(".reponse_body").show();
                dom.find(".response_code").html("<pre></pre>");
                dom.find(".response_code pre").html(e.xhr.status);

                dom.find(".request_url").html("<pre></pre>");
                dom.find(".request_url pre").html(e.url);
                dom.find(".response_body").html("<pre></pre>");
                dom.find(".response_body pre").html(f);

            } catch (d) {
                alert("unable to parse JSON content")
            }
        }
    }
    window.ajaxRequest = new AjaxRequest();
}
$(function () {
    //展示/隐藏
    $(document).on("click", ".showOrhide", function (e) {
        $(e.target).parent().parent().next(".slideList").slideToggle(300);
    })
    //展开/隐藏操作
    $(document).on("click", ".showOperaOrhideOpera", function (e) {
        $(e.target).parent().parent().next(".slideList").find('.operaDetails').slideToggle(300);
    })
    //点击列收合操作详情
    $(document).on("click", ".col", function (e) {
        $(this).next().slideToggle(300);
    })
    //参数示例
    $(document).on("click", ".examBox", function () {
        var txt = $(this).text();
        $(this).parent().parent().find("textarea").val(txt);
    })
    //隐藏响应
    $(document).on("click", ".hideResBtn", function () {
        $(this).hide().parent().next(".resPart").slideUp(300);
    })
    $(document).on("click", ".tryBtn", function (ev) {
        null !== ev && ev.preventDefault();
        var parent = $(ev.currentTarget).closest(".operation");
        var e = !0;
        var body = {};//路径参数
        parent.find("input.required").each(function () {
            $(this).removeClass("error"),
                "" === $(this).val() && ($(this).addClass("error"), e = !1);

            body[$(this).attr("name")] = $(this).val();
        });
        parent.find("textarea.required:visible").each(function () {
            $(this).removeClass("error"),
                "" === jQuery.trim($(this).val()) && ($(this).addClass("error"), e = !1);

            body = $(this).val();
            //body["paramType"] = $(this).closest("tr").find("td:eq(3)").text();
        });
        a = {
            parent: parent,
            apiUrl: $("#url").val(),
            data: body,
            header: {}
        };

        if (e == !1) {
            return;
        }
        debugger
        a.method = parent.find(".httpMethod").text(),
            a.url = parent.find(".httpPath").text(),
            parent.find("[name='paramContentType']") ? a.header["Content-Type"] = parent.find("[name='paramContentType']").val() : "",
            parent.find("[name='respContentType']") ? a.header["Accept"] = parent.find("[name='respContentType']").val() : "";

        ajaxRequest.send(a);
    })
    handleRequest();

})