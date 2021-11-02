//加载json
function handleRequest() {
    $(".moduleList").empty();
    var search = location.search;
    var controllName = "all";
    if (search && search.length > 1) {
        controllName = search.split("&")[1];
    }
    try {
        $.ajax({
            type: "get",
            async: true,
            url: "/temp.json",
            dataType: "json",
            success: function (jsonData) {
                var apiUrl = "http://" + jsonData[0].IP + ":" + jsonData[0].Port + "/";
                $("#url").val(apiUrl);//"http://116.255.241.138:1221/"
                var itemArr = [];
                $.each(jsonData, function (i, api) {
                    if (api.Name == controllName || controllName == "all") {
                        for (var item of api.services) {
                            var name = item.Route.split("/")[0], ind = $.inArray(name, itemArr);
                            if (ind == -1) {
                                itemArr.push(name);
                                ind = itemArr.length - 1;
                                $(".moduleList").append(`<li class="resource" id="resource_${name}"><div><span class="subTit routename">${name}</span><p class="operaBtnPart inBlock fRig"><button class="showOrhide mr-1">显示/隐藏</button><button class="showOperaOrhideOpera">展开/隐藏操作</button></p></div><ul class="slideList">  </ul></li>`);
                            }
                            var liHtml = "";
                            //请求 参数
                            var Params_post = new Object(), Params_get = "";
                            var parameters = item.parameter;
                            var parameterstr = item.parameterexplain;

                            $.each(parameters, function (i, pp) {
                                var descText = parameterstr[i].split("|")[0].replace("@", "");
                                var _fieldtype = descText.split(',')[1];
                                if (_fieldtype && _fieldtype.indexOf("Int32") != -1) {
                                    _fieldtype = 0;
                                }
                                Params_get += ` <tr> <td>${pp}</td><td><input type="text" placeholder="(required)" class="parameter required" minlenth="1" name="${pp}"></td><td>${_fieldtype}</td><td>query</td><td>${descText.split(',')[0]}</td></tr>`
                                Params_post[pp] = _fieldtype;
                            });

                            var contentTypes = { post: '<option value="application/x-www-form-urlencoded">application/x-www-form-urlencoded</option>', none: '<option value="application/json">application/json</option>' }
                            if (["post", "POST", "NONE"].includes(item.Method.toUpperCase())) {
                                liHtml = `<li class="${item.Method} operation" id="${item.Route.replace("/", "_")}" Authorize="${item.Authorize}">
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
                                                    <p>Parameter content type:</p><select name="paramContentType">${contentTypes[item.Method.toLowerCase()]}</select>
                                                </td>
                                                <td>参数</td>
                                                <td>body</td>
                                                <td>
                                                    <p>Example Value</p>
                                                    <div class="examBox"><pre><code> ${JSON.stringify(Params_post, null, 4)}</code></pre></div>
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
                                liHtml = ` <li class="${className} operation" id="${item.Route.replace("/", "_")}" Authorize="${item.Authorize}">
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
                            <tbody>${Params_get}</tbody>
                        </table>
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
                    }
                });
                var params = location.href.split("#")[1];
                if (params != null && params != undefined) {
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
    "undefined" != typeof window ? d = window : "undefined" != typeof self ? d = self : (console.warn("Using browser-only version of superagent in non-browser environment"), d = this);

    function s(e) {
        var t, n, r, i, a = e.split(/\r?\n/), o = {};
        a.pop();
        for (var s = 0, l = a.length; s < l; ++s)
            n = a[s],
                t = n.indexOf(":"),
                r = n.slice(0, t).toLowerCase(),
                i = n.slice(t + 1),
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
        _timeoutError: function () {
            var e = this._timeout
                , t = new Error("timeout of " + e + "ms exceeded");
            t.timeout = e, this._timeout = 0, clearTimeout(this._timer)
        },
        crossDomainError: function () {
            var e = new Error("Request has been terminated\nPossible causes: the network is offline, Origin is not allowed by Access-Control-Allow-Origin, the page is being unloaded, etc.");
            e.crossDomain = !0,
                e.status = this.status,
                e.method = this.method,
                e.url = this.url,
                this._timeout = 0, clearTimeout(this._timer)
        },
        serialize: {
            "application/x-www-form-urlencoded": function (e) {
                if (typeof e == "string") e = JSON.parse(e); var t = []; for (var n in e) t.push(n + "=" + e[n]); return t.join("&")
            },
            "application/json": JSON.stringify
        },
        parse: {
            "application/x-www-form-urlencoded": JSON.stringify,
            "application/json": JSON.parse
        },
        _isHost: function (e) {
            var t = {}.toString.call(e);
            switch (t) {
                case "[object File]":
                case "[object Blob]":
                case "[object FormData]":
                    return !0;
                default:
                    return !1
            }
        },
        send: function (p) {
            var t = this
                , n = this.xhr = this.getXHR()
                , i = !this._timeout

            this.parameters = p;
            this.url = p.apiUrl + p.url;
            this.method = p.method;
            this.headers = p.headers;

            n.onreadystatechange = function () {
                if (4 == n.readyState) {
                    var e;
                    try {
                        e = n.status
                    } catch (r) {
                        e = 0
                    }
                    // 接收响应数据
                    t.showResponse();
                    if (0 == e) {
                        if (t.timedout) {
                            return t._timeoutError();
                        }
                        if (t._aborted)
                            return;
                        return t.crossDomainError();
                    }
                }
            }
            //debugger
            if (i && !this._timer && (this._timer = setTimeout(function () {
                t.timedout = !0, this._aborted ? this : (this._aborted = !0,
                    this.xhr && this.xhr.abort(), t._timeout = 0, clearTimeout(t._timer))
            }, i)),
                this.appendQueryString(), n.open(this.method, this.url, !0),
                //this._withCredentials = !0 && (n.withCredentials = !0),
                "GET" != this.method && "HEAD" != this.method && "string" == typeof p.data && !this._isHost(p.data)) {
                var u = this.headers["Content-Type"]
                    , c = this.serialize[u ? u.split(";")[0] : ""];

                !c && (/[\/+]json\b/.test(u)) && (c = t.serialize["application/json"]),
                    c && (p.data = c(p.data))
            };
            for (var ph in this.headers) {
                null != this.headers[ph] && n.setRequestHeader(ph, this.headers[ph]);
            }
            return this.parameters.responseType && (n.responseType = this.parameters.responseType),

                n.send("undefined" != typeof p.data ? p.data : null), this
        },
        //* query参数
        appendQueryString: function () {
            if (this.method.toUpperCase() == "GET") {
                var pp = this.parameters.data;
                var e = pp.join("&");
                e && (this.url += ~this.url.indexOf("?") ? "&" + e : "?" + e);
            }
        },
        showResponse() {
            var e = this;
            this.responseText = "HEAD" != this.method && ("" === this.xhr.responseType || "text" === this.xhr.responseType) || "undefined" == typeof this.xhr.responseType ? this.xhr.responseText : null,
                this.responseHeader = s(this.xhr.getAllResponseHeaders()), this.responseHeader["content-type"] = this.xhr.getResponseHeader("content-type");
            var dom = this.parameters.parent;
            e.headers = $.extend({}, this.headers, this.responseHeader);
            try {
                var f = e.responseText || e.response;
                r = "string" == typeof f ? {} : f;
                dom.find(".reponse_body").show();
                dom.find(".response_code").html("<pre></pre>");
                dom.find(".response_code pre").html(e.xhr.status);

                dom.find(".request_url").html("<pre></pre>");
                dom.find(".request_url pre").html(e.url);

                dom.find(".response_body").html("<pre></pre>");
                var t = f != null && !this._isHost(f) ? JSON.stringify(JSON.parse(f), null, "\t").replace(/\n/g, "<br>") : f;
                dom.find(".response_body pre").html(t);

            } catch (d) {
                alert("unable to parse JSON content")
            }
        }
    }
    window.ajaxRequest = new AjaxRequest();
}
$(function () {
    //查找
    $(document).on("click", "#searchBtn", function (e) {
        var keyword = $("#apiKey").val();
        if (keyword == "" || keyword == null) {
            return;
        }
        var node = $(".slideList li[id$='" + keyword + "']");
        if (node.length) {
            $(".slideList .operaDetails").hide();
            node.closest(".resource").find(".slideList").show(); node.find(".operaDetails").slideDown();
        } else {
            layer.msg("未查到相关接口信息");
        }
    })
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
        var t = $("textarea", $(this).parent().parent());
        var txt = $(this).text();
        "" !== $.trim(t.val()) && t.prop("placeholder") !== t.val() || t.val(txt);
    })
    //隐藏响应
    $(document).on("click", ".hideResBtn", function () {
        $(this).hide().parent().next(".resPart").slideUp(300);
    })
    $(document).on("click", ".tryBtn", function (ev) {
        null !== ev && ev.preventDefault();
        var parent = $(ev.currentTarget).closest(".operation");
        var e = !0;
        var body = [];//路径参数
        parent.find("input.required").each(function () {
            $(this).removeClass("error"), "" === $(this).val() && ($(this).addClass("error"), e = !1);

            body.push($(this).attr("name") + "=" + $(this).val());
        });
        parent.find("textarea.required:visible").each(function () {
            $(this).removeClass("error"), "" === jQuery.trim($(this).val()) && ($(this).addClass("error"), e = !1);

            body = $(this).val();
        });
        a = {
            parent: parent,
            apiUrl: $("#url").val(),
            data: body,
            headers: {}
        };
        if (e == !1) {
            return;
        }
        a.method = parent.find(".httpMethod").text(),
            a.url = parent.find(".httpPath").text(),
            parent.find("[name='paramContentType']").length ? a.headers["Content-Type"] = parent.find("[name='paramContentType']").val() : "",
            parent.find("[name='respContentType']").length ? a.headers["Accept"] = parent.find("[name='respContentType']").val() : "";

        ajaxRequest.send(a);
    })
    handleRequest();
})