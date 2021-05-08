(function (jQuery, C, e) {
    "use strict";
    function u(e) {
        return void 0 !== e.ref
    }
    function c(e) {
        return void 0 !== e.key
    }
    function _isHost(e) {
        var t = {}.toString.call(e);
        switch (t) {
            case "[object File]":
            case "[object Blob]":
            case "[object FormData]":
                return !0;
            default:
                return !1
        }
    }
    const isPathParam = word => word.includes('{') || word.includes('}');
    // 去除括号
    const delBrackets = word => word.replace(/{|}/g, '');
    // 替换横杠
    const replaceHorizontal = word => word.replace(/-/g, '_');
    var serialize = {
        "application/x-www-form-urlencoded": function (e) {
            if (typeof e == "string") e = JSON.parse(e); var t = []; for (var n in e) t.push(n + "=" + e[n]); return t.join("&")
        },
        "application/json": JSON.stringify
    }
    var l = {};
    l.createElement = function (e, t, n) {
        // debugger
        var reg = new RegExp(/^(onClick|onCopyCapture)/);
        var NS = new RegExp(/(svg|use)/);

        var r, a = null, f = null;
        if (null != e) {
            var dom = !NS.test(e) ? C.createElement(e) : C.createElementNS('http://www.w3.org/2000/svg', e);
            if ("object" == typeof t && null !== t)
                for (r in u(t) && (f = t.ref),
                    c(t) && (p = "" + t.key), t) {
                    !reg.test(r) ? dom.setAttribute(r, t[r]) : (a = r.toLowerCase().replace("on", ""), dom.addEventListener(a, t[r], false));
                }
            ((typeof n === "object") && n !== null) ? ((n.constructor === Array) && (n instanceof Array)) ? $.each(n, function (ii, te) { dom.appendChild(te) }) : dom.appendChild(n)
                : ("string" == typeof n && n !== null) ? dom.innerHTML = n : "";
            return dom;
        }
    }

    var et = {
        "operations-tag": function (r, f) {
            var E = ["operations-tag", r]
                , x = "full" === f || "list" === f ? !0 : !1//that.isShown(E, "full" === f || "list" === f);
            return l.createElement("div", {
                class: "opblock-tag-section is-open"
            }, l.createElement("h4", {
                onClick: function (e) {
                    e.preventDefault();
                    var _parent = $(e.currentTarget).parent();
                    if (_parent.hasClass("is-open")) {
                        $(e.currentTarget).attr("data-is-open", false); _parent.removeClass("is-open").find("svg > use").attr("xlink:href", "#large-arrow").attr("href", "#large-arrow") && _parent.find(".resource").hide();
                    } else {
                        $(e.currentTarget).attr("data-is-open", true); _parent.addClass("is-open").find("svg > use").attr("xlink:href", "#large-arrow-down").attr("href", "#large-arrow-down") && _parent.find(".resource").show();
                    }
                },
                class: "opblock-tag no-desc",
                id: E.map((function (e) {
                    return e
                })).join("-"),
                "data-tag": r,
                "data-is-open": x
            }, [l.createElement("a", {
                class: "nostyle",
                enabled: x,
                isShown: x,
            }, "<span>" + r + "</span>"), l.createElement("small", null), l.createElement("div", null),
            l.createElement("button", {
                class: "expand-operation",
                title: x ? "Collapse operation" : "Expand operation",
            }, l.createElement("svg", {
                class: "arrow",
                width: "20",
                height: "20"
            }, l.createElement("use", {
                href: x ? "#large-arrow-down" : "#large-arrow",
                "xlink:href": x ? "#large-arrow-down" : "#large-arrow"
            })))]));
        },
        "method": function (e) {
            return l.createElement("span", {
                class: "opblock-summary-method"
            }, e.toUpperCase())
        },
        "path": function (i, r) {
            return l.createElement("span", {
                class: r ? "opblock-summary-path__deprecated" : "opblock-summary-path",
                onCopyCapture: ui.onCopyCapture,
                "data-path": i
            }, l.createElement("a", {
                class: "nostyle", href: "#" + i,
                enabled: true,
                isShown: true,
            }, "<span>" + i.replace(/\//g, "​/") + "</span>"))
        },
        "desc": function (t) {
            return l.createElement("div", {
                class: "opblock-summary-description",
            }, "<span class=\"float-right\">" + t + "</span>")
        },
        post_table: function (t) {
            return l.createElement("table", {
                class: "table responses-table paramsTable",
            }, "<thead><tr class=\"responses-header\"><th class=\"col_header response-col_status\" style=\"width:20%;\">名称</th><th class=\"col_header response-col_description\" style=\"width:30%;\">数据值</th><th class=\"col col_header response-col_links\" style=\"width:20%;\">参数类型</td><td class=\"col col_header response-col_links\" style=\"width:30%;\">示例</th></tr></thead><tbody>" + t + "</tbody>")
        },
        get_table: function (t) {
            return l.createElement("table", {
                class: "table responses-table paramsTable",
            }, "<thead><tr class=\"responses-header\"><th class=\"col_header response-col_status\" style=\"width:20%;\">名称</th><th class=\"col_header response-col_description\" style=\"width:30%;\">数据值</th><th class=\"col col_header response-col_links\" style=\"width:20%;\">描述</td><td class=\"col col_header response-col_links\" style=\"width:20%;\">参数类型</th><th class=\"col col_header response-col_links\" style=\"width:20%;\">数据类型</th></tr></thead><tbody>" + t + "</tbody>")
        },
        //验证table
        auth_table: function () {
            var _token = sessionStorage.getItem("authtoken") || "";
            return l.createElement("table", {
                class: "table responses-table Authorization",
            }, "<thead><tr class=\"responses-header\"><th class=\"col_header response-col_status\">名称</th><th class=\"col_header response-col_description\" style=\"width:90%\">数据值</th></tr></thead><tbody><tr><td class=\"response-col_status\">Header Prefix</td><td class=\"response-col_description\"><div><input type=\"text\" class=\"parameter required\" value=\"Bearer\" required placeholder=\"Bearer\" name=\"HeaderPrefix\" style=\"width:300px\" /></div></td></tr><tr><td class=\"response-col_status\">Access Token</td><td class=\"response-col_description\"><div><textarea name=\"Authorization\" class=\"parameter required\" required placeholder=\"(required)\">" + _token + "</textarea></div></td></tr></tbody>")
        },
        curl: function (i) {
            return l.createElement("div", {
                class: "curl-command"
            }, [l.createElement("h4", null, "Curl"), l.createElement("div", {
                class: "copy-to-clipboard"
            }, l.createElement("button", null)), l.createElement("div", null, i)])
        },
        ocommand: function (t, url, cmd, r) {
            return l.createElement("div", {
                class: "other-command"
            }, [l.createElement("h4", { innerHTML: t }), l.createElement("div", { class: cmd }, r)]);
        },
        loading: function (b) {
            return l.createElement("div", { class: "loading-container" }, l.createElement("div", { class: "loading" }));
        },
        x_www_form_urlencoded: '<section class="response-controls"><div class="response-control-media-type response-control-media-type--accept-controller"><small class="response-control-media-type__title">Parameter content type</small><div class="content-type-wrapper "><select class="content-type"><option value="application/x-www-form-urlencoded">application/x-www-form-urlencoded</option></select></div></div></section>',
        application_json: '<section class="response-controls"><div class="response-control-media-type response-control-media-type--accept-controller"><small class="response-control-media-type__title">Parameter content type</small><div class="content-type-wrapper "><select class="content-type"><option value="application/json">application/json</option></select></div></div></section>'

    }
    //初始化ui
    var UIDocument = function () {
        return {
            et: et,
            _timeout: 0,
            loadingStatus: null,
            init: function (jsonData, serviceName) {
                var that = this;
                var bodyUI = C.getElementById("apiList");
                bodyUI && (bodyUI.innerHTML = "");
                if (jsonData && jsonData != null) {
                    $.each(jsonData, function (i, api) {
                        if (api.Name == serviceName || serviceName == "all") {
                            var tag, el = l.createElement("div", { class: "block col-12 block-desktop col-12-desktop" }, l.createElement("div", null, tag = that.et["operations-tag"](api.Name, "list")));
                            bodyUI.appendChild(el);
                            $.each(api.services, function (s, server) {
                                var opblock = l.createElement("div", { class: "opblock opblock-" + server.Method.toLocaleLowerCase() + "", Authorize: server.Authorize, id: "operations-" + server.Method.toLocaleLowerCase() + "_" + server.Route.replace("/", "_") },
                                    l.createElement("div", { class: "opblock-summary opblock-summary-" + server.Method.toLocaleLowerCase(), onClick: ui.toggleShown }, [that.et.method(server.Method), that.et.path(server.Route), that.et.desc(server.annotation)]));

                                var eldiv = l.createElement("div", { class: "no-margin resource" }, l.createElement("span", null, opblock)),
                                    section = null;

                                var opbody = l.createElement("div", { class: "no-margin request-section" }, l.createElement("div", { class: "opblock-body" },
                                    //需要验证
                                    [server.Authorize == true ? l.createElement("div", { class: "opblock-section" }, [l.createElement("div", { class: "opblock-section-header" }, [l.createElement("div", { class: "tab-header" }, l.createElement("div", { class: "tab-item active" }, "<h4 class=\"opblock-title\"><span>Authorization</span></h4>"))]),
                                    l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, that.et.auth_table()))]) : l.createElement("div")
                                        , section = l.createElement("div", { class: "opblock-section" }, l.createElement("div", { class: "opblock-section-header" }, [l.createElement("div", { class: "tab-header" }, l.createElement("div", { class: "tab-item active" }, "<h4 class=\"opblock-title\"><span>Parameters</span></h4>")), l.createElement("div", { class: "try-out" }, l.createElement("button", { class: "btn try-out__btn", onClick: ui.send }, "Try it out "))]))]
                                ));
                                opblock.appendChild(opbody), tag.appendChild(eldiv);

                                //参数
                                var Params_body = new Object(), Params_get = "";
                                if (server.parameter && !server.parameter.length) {
                                    var p = l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, "<p>No parameters</p>"));
                                    section.appendChild(p);
                                } else {
                                    var parameters = server.parameter;
                                    var parameterstr = server.parameterexplain;
                                    $.each(parameters, function (pi, pp) {
                                        var descText = parameterstr[pi].split("|")[0].replace("@", "");
                                        var _fieldtype = descText//.split(',')[1];
                                        if (_fieldtype && _fieldtype.toLocaleLowerCase().indexOf("int32") != -1) {
                                            _fieldtype = 0;
                                        }
                                        Params_body[pp] = _fieldtype;
                                        Params_get += "<tr class=\"response\"><td class=\"response-col_links\">" + pp + "</td><td class=\"col_description\"><div class=\"model-example\"><div><div><input type=\"text\" class=\"parameter required\" autocomplete=\"off\" required placeholder=\"(required)\" name=\"" + pp + "\" /></div></div></div></td><td class=\"response-col_links\"><i>" + _fieldtype + "</i></td><td class=\"response-col_links\"><i>query</i></td><td class=\"response-col_links\"><i>" + descText.split(',')[0] + "</i></td></tr>";
                                    })
                                    var html = '<tr class="response"><td class="response-col_status">#td1#</td><td class="response-col_description"><div class="model-example"><div><div><div class="highlight-code"><textarea name="mode" class="body-textarea required" placeholder="(required)"></textarea></div></div></div></div>#ContentType#</td>#td#<td class="response-col_links"><i>body</i></td><td class="response-col_links"> <div><div class="highlight-code"><pre class="example microlight" style="display: block; overflow-x: auto; padding: 0.5em; background: rgb(51, 51, 51); color: white;"><code>' + JSON.stringify(Params_body, null, 4) + '</code></pre></div></div></td></tr>'

                                    if (server.Method.toLocaleLowerCase() == "none") {
                                        html = html.replace("#td1#", "model").replace("#td#", '').replace("#ContentType#", ui.et.application_json);
                                        var p = l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, that.et.get_table(html)));
                                        section.appendChild(p);
                                    }
                                    if (server.Method.toLocaleLowerCase() == "post") {
                                        var p;
                                        if (parameters.length > 1) {
                                            Params_get += '<tr><td></td><td colspan="4">' + ui.et.x_www_form_urlencoded + '</td></tr>';
                                            p = l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, that.et.get_table(Params_get)));
                                        } else {
                                            html = html.replace("#td1#", parameters[0]).replace("#td#", '<td class="response-col_links"><i>' + parameterstr[0] + '</i></td>').replace("#ContentType#", ui.et.x_www_form_urlencoded);;
                                            p = l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, that.et.get_table(html)));
                                        }
                                        p && section.appendChild(p);
                                    }
                                    if (server.Method.toLocaleLowerCase() == "get") {
                                        var p = l.createElement("div", { class: "parameters-container" }, l.createElement("div", { class: "opblock-description-wrapper" }, that.et.get_table(Params_get)));
                                        section.appendChild(p);
                                    }
                                }
                            })
                        }
                    })
                }
            },
            //toggleShown
            toggleShown: function (e) {
                var next = $(e.currentTarget).next(".request-section");
                if (next.is(":visible")) {
                    next.slideUp();
                    $(e.currentTarget).closest(".opblock").removeClass("is-open").attr("data-is-open", false);
                } else {
                    next.slideDown();
                    $(e.currentTarget).closest(".opblock").addClass("is-open").attr("data-is-open", true);
                }
            },
            getXHR: function () {
                var d, v = {};
                "undefined" != typeof window ? d = window : "undefined" != typeof self ? d = self : (console.warn("Using browser-only version of superagent in non-browser environment"), d = this);

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
            //数据请求
            send: function (ev) {
                ev.preventDefault();
                ui.tryItOutEnabled = !0;
                var e = !0;
                var _parent = $(ev.currentTarget).closest(".opblock");
                var a = {
                    parent: _parent,
                    url: $("#Url").val(),
                    headers: {}
                }
                _parent.find(".loading-container").remove();

                a.method = _parent.find(".opblock-summary-method").text(),
                    a.path = _parent.find(".opblock-summary-path").attr("data-path"),
                    _parent.find(".content-type").length ? a.headers["Content-Type"] = _parent.find(".content-type").val() : "";
                //接口是否需要验证token
                var tokenAuth = new Object();
                if (_parent.find("table.Authorization").length) {
                    var _table = _parent.find("table.Authorization");
                    tokenAuth.auth = true;
                    _table.find(".parameter.required").each(function () {
                        $(this).removeClass("error"), "" === $(this).val() && ($(this).addClass("error"), e = !1);
                        tokenAuth[$(this).attr("name")] = $(this).val();
                    });
                    if (tokenAuth.auth) a.headers["Authorization"] = tokenAuth.HeaderPrefix + " " + tokenAuth.Authorization;
                }

                if (a.method.toUpperCase() == "GET") {
                    a.query = new Array();
                    _parent.find(".paramsTable input.required").each(function () {
                        $(this).removeClass("error"), "" === $(this).val() && ($(this).addClass("error"), e = !1);
                        a.query.push($(this).attr("name") + "=" + $(this).val());
                    });
                }
                if (a.method.toUpperCase() == "POST" || a.method.toUpperCase() == "NONE") {
                    a.method = "POST";
                    var t = _parent.find(".paramsTable textarea.required");
                    if (t.length > 0) {
                        t.removeClass("error"), ("" === t.val() || null === t.val()) && (t.addClass("error"), e = !1);
                        a.formData = t.val();
                    } else {
                        var body = {};
                        _parent.find(".paramsTable input.required").each(function () {
                            $(this).removeClass("error"), "" === $(this).val() && ($(this).addClass("error"), e = !1);
                            body[$(this).attr("name")] = $(this).val();
                        });
                        a.formData = JSON.stringify(body);
                    }
                }
                if (e == !1) {
                    return;
                }
                ui.execute(a);
            },
            appendQueryString: function () {
                if (this.props.query) {
                    var e = this.props.query.join("&");
                    e && (this.url += ~this.url.indexOf("?") ? "&" + e : "?" + e)
                }
            },
            execute: function (p) {
                var a = this
                    , s = this.xhr = this.getXHR()
                    , i = !this._timeout
                    , data = p.query || p.formData;
                this.props = p;
                this.url = p.url + p.path;
                this.method = p.method;
                this.headers = p.headers;
                if ("GET" != this.method && "HEAD" != this.method && "string" == typeof data && !_isHost(data)) {
                    var u = this.headers["Content-Type"]
                        , c = serialize[u ? u.split(";")[0] : ""];
                    !c && (/[\/+]json\b/.test(u)) && (c = serialize["application/json"]), c && (data = c(data))
                }
                a.loadingStatus = "loading" && a.props.parent.find(".opblock-section")[0].after(ui.et.loading(a.loadingStatus));

                function u() { s.abort() }
                s.onload = function () {
                    var e, t, n = { status: s.status, statusText: s.statusText, headers: (e = s.getAllResponseHeaders() || "", t = new Object(), e.replace(/\r?\n[\t ]+/g, " ").split(/\r?\n/).forEach((function (e) { var n = e.split(":"), r = n.shift().trim(); if (r) { var o = n.join(":").trim(); t[r] = o } })), t) };
                    n.url = "responseURL" in s ? s.responseURL : n.headers.get("X-Request-URL"); n.responseText = "response" in s ? s.response : s.responseText; ui.sendResponse(n, t);
                }, s.onerror = function () { ui.loadingStatus = "failed"; ui.showError("Network request failed"); throw new TypeError("Network request failed") }
                    , s.ontimeout = function () { ui.loadingStatus = "failed"; ui.showError("Network request failed"); throw new TypeError("Network request failed") }
                    , s.onabort = function () { ui.loadingStatus = "failed"; ui.showError("AbortError"); throw new DOMException("Aborted", "AbortError") }
                    , a.appendQueryString(), s.open(a.method, a.url, !0), "include" === a.credentials ? s.withCredentials = !0 : "omit" === a.credentials && (s.withCredentials = !1), a.signal && (a.signal.addEventListener("abort", u))
                    , s.onreadystatechange = function () { if (4 === s.readyState) { ui.loadingStatus = "success"; a.signal && a.signal.removeEventListener("abort", u); } }
                for (var ph in this.headers) {
                    null != this.headers[ph] && s.setRequestHeader(ph, this.headers[ph]);
                }
                s.send(void 0 === data ? null : data)
            },
            //数据响应
            sendResponse: function (e) {
                try {
                    var resbody = this.props.parent.find(".opblock-body");
                    resbody.find(".responses-wrapper").remove();
                    resbody.find(".loading-container").remove();
                    var f = e.responseText || this.xhr.responseText;

                    var t = f != null && !_isHost(f) ? JSON.stringify(JSON.parse(f), null, "\t").replace(/\n/g, "<br>") : f;
                    var _h = $.extend({}, e.headers);
                    var resheader_text = "";
                    for (var it in _h) {
                        resheader_text += "<span class=\"headerline\">\"" + it + "\":\"" + _h[it] + "\"</span>"
                    }
                    //response
                    var response = l.createElement("div", { class: "responses-wrapper" }, l.createElement("div", { class: "opblock-section-header" }, "<h4>Responses</h4>")),
                        res_inner = l.createElement("div", { class: "responses-inner" }, [ui.et.curl('<pre class="curl microlight" style="display: block; overflow-x: auto; padding: 0.5em; background: rgb(51, 51, 51); color: white;"><code class="language-bash"><span><!-- react-text: 215 -->curl -X ' + this.method + ' </span><span style="color: rgb(162, 252, 162);">"' + e.url + '"</span><span> -H  </span><span style="color: rgb(162, 252, 162);"> "accept: */*"</span><span> -d </span><span style="color: rgb(162, 252, 162);">""</span></code></pre>')
                            , l.createElement("div", null, '<h4>Request URL</h4><div class="request-url"><pre class="microlight">' + e.url + '</pre></div>')
                            , l.createElement("div", null, '<h4>响应码</h4><div class="response_code"><pre class="microlight">' + e.status + '</pre></div>')
                            , l.createElement("div", null, '<h4>响应体</h4><div class="response_body"><pre class="microlight">' + t + '</pre></div>')
                            , l.createElement("div", null, '<h4>响应头</h4><div class="response_headers"><pre class="microlight">' + resheader_text + '</pre></div>')])
                    response.appendChild(res_inner) && resbody[0].appendChild(response);

                } catch (d) {
                    layer.msg("unable to parse JSON content")
                }

            },
            showError(msg) {
                try {
                    var resbody = this.props.parent.find(".opblock-body");
                    resbody.find(".responses-wrapper").remove();
                    resbody.find(".loading-container").remove();
                    //response
                    var response = l.createElement("div", { class: "responses-wrapper" }, l.createElement("div", { class: "opblock-section-header" }, "<h4>Responses</h4>")),
                        res_inner = l.createElement("div", { class: "responses-inner" }, [l.createElement("div", null, '<h4>响应码</h4><div class="response_code"><pre class="microlight">' + this.xhr.status + '</pre></div>')
                            , l.createElement("div", null, '<h4>响应体</h4><div class="response_body"><pre class="microlight"><span style="color: rgb(162, 252, 162);">' + msg + '</span></pre></div>')])
                    response.appendChild(res_inner) && resbody[0].appendChild(response);
                } catch (d) {
                    layer.msg("error:" + e)
                }
            },
            //数据粘贴
            onCopyCapture: function (t) {
                var c = "clipboardData" in e ? e.clipboardData : window.clipboardData;
                c.setData("text/plain", $(t).data("path")), t.preventDefault()
            }
        }
    }();
    window.ui = UIDocument;
    var ServiceName = "all";
    var search = location.search;
    if (search && search.length > 1) {
        ServiceName = search.split("&")[1];
    }

    jQuery(function () {
        $.getJSON("temp.json", function (data) {
            ui.init(data, ServiceName);
        });

        ////网关列表
        var htmladd = "<ul class=\"gatewaylist\"><li class=\"item-gateway\"><a href=\"#\">http://127.0.0.1:1221/</a></li> </ul>";
        var htmladdObject = $(htmladd);

        $.getJSON("gateway.json", function (data) {
            $.each(data, function (i, gt) {
                var url = "http://" + gt.IP + ":" + gt.port + "/";
                i == 0 && $("#Url").val(url), htmladdObject.append("<li class=\"item-gateway\"><a href=\"#\">" + url + "</a></li>");
            });
        });

        $(".customeUrl").focus(function () {
            $(".gatewaylist").remove();//清楚底部内容
            
            $(this).after(htmladdObject.css("left", $(this).offset().left + "px"));
            $(htmladdObject).slideDown(300);

            $(".gatewaylist li a").click(function () {
                $("#Url").val($(this).html()); $(htmladdObject).hide();
            }); 

            $(document).on("click", function (event) {//点击空白处，设置的弹框消失
                event.stopPropagation();
                if ($(event.target).find(".gatewaylist").length !== 0) {
                    $(htmladdObject).hide();
                }
                if ($(event.target).find(".gatewaylist").length == 0 && (!$(event.target).hasClass("customeUrl") && !$(event.target).hasClass("gatewaylist"))) {
                    $(htmladdObject).hide();
                }
            });
        });             

        jQuery(document).on("click", ".example.microlight", function (e) {
            e.preventDefault();
            $(this).closest("tr").find("textarea").val($(this).text());
        })
        jQuery("#btnSearch").click(function (e) {
            e.preventDefault();
            var keyword = $("#query_keyword").val();
            if (keyword == "" || keyword == null) {
                return;
            }
            var node = $(".document-ui .opblock[id$='" + keyword + "']");
            if (node.length) {
                $(".document-ui .resource").find(".opblock").removeClass("is-open").attr("data-is-open", false).find(".request-section").slideUp();
                node.addClass("is-open").attr("data-is-open", true).closest(".resource").show(); node.find(".request-section").slideDown();
            } else {
                layer.msg("未查到相关接口信息");
            }
        })
    })

})(jQuery, document, window);