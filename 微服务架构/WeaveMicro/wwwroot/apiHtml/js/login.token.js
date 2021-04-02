(function ($) {
    class loginSys {
        constructor() {

        }
    }
    //获取token
    loginSys.prototype.gettoken = function () {
        var _loginname = document.getElementById("username").value;
        var _password = document.getElementById("password").value;
        var _IdentityUrl = document.getElementById("IdentityUrl").value;
        var _project = document.getElementById("Project").value;
        if (_loginname == "") {
            $("#username").addClass("error");
            return;
        }
        if (_password == "") {
            $("#password").addClass("error");
            return;
        }
        $("#loginSubmit").addClass("loading");
        $.ajax({
            url: _IdentityUrl + "connect/token",
            type: "post",
            contentType: "application/x-www-form-urlencoded",//默认
            data: {
                "username": _loginname, "password": _password, "prj": _project, "grant_type": "password", "scope": "api1", "client_id": "client", "client_secret": "secret"
            },
            success: function (data) {
                console.log(data)
            },
            complete: function (xhr, data) {
                try {
                    var response = xhr.responseJSON;
                    if (response != null && response.access_token) {
                        var token = response.access_token;
                        localStorage.setItem("authtoken", token);
                        localStorage.setItem("userInfo", JSON.stringify(response));
                        $(".parameter[name='Authorization']").val(token);

                        $("#errorTip").html("用户授权成功");
                        setTimeout(function () { layer.closeAll(); }, 200);
                    } else {
                        $("#errorTip").html("授权失败：" + response.error_description);
                    }
                } catch (e) {
                    $("#errorTip").html("用户授权失败");                   
                } finally {
                    $("#loginSubmit").removeClass("loading");
                }
            }
        });
    }
    window.login = new loginSys();
})(jQuery);
$(function () {
    //弹窗
    $("#LoginBtn").click(function (e) {
        e.preventDefault();
        layer.open({
            type: 1,
            title: "",
            area: ['650px', '550px'],
            content: $("#templlogin").html(),
            success: function () {
                $("input.form-control").attr("autocomplete", "off");
                $("input.form-control").focus(function (e) {
                    $(this).removeClass("error");
                });
                $("#loginSubmit").click(function () {
                    $("#errorTip").html("Login to stay connected.");
                    login.gettoken();
                })
            }
        });
    });
})
