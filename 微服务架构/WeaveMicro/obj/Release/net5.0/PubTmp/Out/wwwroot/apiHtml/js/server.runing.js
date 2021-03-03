//服务中心
var ServRuning = {
    //服务列表
    serverlist() {
        var e = this;
        e.readTextFile("/bin/Debug/net5.0/temp.json", function (text) {
            var data = JSON.parse(text);
            console.log(data);
            var source = "";
            $("div[name=\"服务\"] .server-items").remove();
            if (data && data != null) {
                $.each(data, function (i, item) {
                    source += `<div class="col-lg-4 col-md-6 col-sm-6 server-items">
                    <div class="card card-block card-stretch card-height">
                        <div class="card-body image-thumb">
                            <a href="index.html?api=${item.IP}:${item.Port}" data-toggle="modal" data-target="#index">
                                <div class="mb-4 text-center p-3 rounded iq-thumb">
                                    <div class="iq-image-overlay"></div><img src="image/server.png" class="img-fluid" alt="${item.Name}">
                                </div>
                                <h5 class="card-title mt-1 text-center IP">${item.Name}</h5>
                                <p class="mb-2 port text-success">IP<span class="float-right">${item.IP}</span></p>
                                <p class="mb-2 port text-success">端口<span class="float-right">${item.Port}</span></p>
                                <p class="mb-0 status text-success">状态<span class="float-right">运行中</span></p>
                            </a>
                        </div>
                    </div>
                </div>`;
                });
                $("div[name=\"服务\"]").append(source).find(".view-number").html(data.length + "个");
            }
        });
    },
    //网关
    geteway() {
        var e = this;
        e.readTextFile("/bin/Debug/net5.0/gateway.json", function (text) {
            var data = JSON.parse(text);
            console.log(data);
            var source = "";
            $("div[name=\"网关\"] .server-items").remove();
            if (data && data != null) {
                $.each(data, function (i, item) {
                    source += `<div class="col-lg-4 col-md-6 col-sm-6 server-items">
                    <div class="card card-block card-stretch card-height">
                        <div class="card-body image-thumb">
                            <a href="#" data-toggle="modal">
                                <div class="mb-4 text-center p-3 rounded iq-thumb">
                                    <div class="iq-image-overlay"></div><img src="image/server.png" class="img-fluid" alt="${item.IP}">
                                </div>
                                <p class="mb-2 port text-success">IP<span class="float-right">${item.IP}</span></p>
                                <p class="mb-2 port text-success">端口<span class="float-right">${item.Port}</span></p>
                                <p class="mb-0 status text-success">状态<span class="float-right">运行中</span></p>
                            </a>
                        </div>
                    </div>
                </div>`;
                });
                $("div[name=\"网关\"]").append(source).find(".view-number").html(data.length + "个");
            }
        });
    },
    //读取本地文件的回调函数
    readTextFile(file, callback) {
        var d, v = {};
        "undefined" != typeof window ? d = window : "undefined" != typeof self ? d = self : (console.warn("Using browser-only version of superagent in non-browser environment"),
            d = this);
        getXHR = function () {
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
        }
        var rawFile = getXHR();// new XMLHttpRequest();

        rawFile.overrideMimeType("application/json");
        rawFile.open("GET", file, true);
        rawFile.onreadystatechange = function () {
            if (rawFile.readyState === 4 && rawFile.status == "200") {
                callback(rawFile.responseText);
            }
        }
        rawFile.send(null);
    }
}
ServRuning.serverlist();
ServRuning.geteway();