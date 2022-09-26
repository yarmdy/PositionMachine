//调用跨域Web Service
function invokeServiceByProxy(wsdl, method, params, successFunc, errorFunc, completeFunc) {
    var proxyParams = { "wsdl": wsdl, "method": method, "parameters": JSON.stringify(params) };
    var op = {
        url: "/Services/ProxyService.svc/Request",
        data: proxyParams,
        success: successFunc,
        error: errorFunc,
        complete:completeFunc
    };
    invokeService(op.url, op.data, op.success, op.error, op.complete);
}

//跨域获取HTML
function getRemotePageByProxy(url, method, params, successFunc, errorFunc, completeFunc) {
    var proxyParams = { "url": url, "method": method, "parameters": JSON.stringify(params) };
    var op = {
        url: "/Services/ProxyService.svc/GetRemotePage",
        data: proxyParams,
        success: successFunc,
        error: errorFunc,
        complete: completeFunc
    };
    invokeService(op.url, op.data, op.success, op.error, op.complete);
}

//跨域获取远程内容
function getRemoteContentByProxy(url, method, params, successFunc, errorFunc, completeFunc) {
    var proxyParams = { "url": url, "method": method, "parameters": JSON.stringify(params) };
    var op = {
        url: "/Services/ProxyService.svc/GetRemoteContent",
        data: proxyParams,
        success: successFunc,
        error: errorFunc,
        complete: completeFunc
    };
    invokeService(op.url, op.data, op.success, op.error, op.complete);
}
 
//调用本地Web Service或WCF服务
function invokeService(url, params, successFunc, errorFunc, completeFunc) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: JSON.stringify(params),
        processData: false,
        contentType: "application/json; charset=utf-8",
        success: successFunc,
        error: errorFunc,
        complete: completeFunc
    });
}

; $.extend({
    getJSONX: function (url, sucfunc, errfunc) {
        var proxyParams = { "url": url, "method": "", "parameters": "" }; //.replace(/\&callback=\?/g, '')
        var proxyUrl = "/Services/ProxyService.svc/GetRemoteContent";
        return $.ajax({
            type: "POST",
            url: proxyUrl,
            dataType: "json",
            data: JSON.stringify(proxyParams),
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                var obj = JSON.parse(result.d);
                if (obj.code == 1) { sucfunc(obj.data); } else { if (errfunc) { errfunc(this, obj.error, obj.code); } }
            } /*,
            error: function (jqXHR, textStatus, errorThrown) { alert("error:"+textStatus)} ,
            complete: function () { }*/
        });
    }
});

var jutils = {};
jutils.ajaxGet = function(url, params, successFunc, errorFunc) {
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        data: params,
        //contentType: 'application/x-www-form-urlencoded',
        success: successFunc,
        error: errorFunc
    });
};
jutils.ajaxPost = function(url, params, successFunc, errorFunc, completeFunc) {
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        cache: false,
        data: params,
        contentType: 'application/x-www-form-urlencoded',
        success: successFunc,
        error: errorFunc
    });
};
jutils.getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r !== null) return unescape(r[2]); return null; //返回参数值
};

jutils.confirm = function(title, successFunc) {
    layer.confirm(title,
        {
            btn: ['是', '否'] //按钮
        },
        function(index) {
            layer.close(index);
            successFunc();

        },
        function() {

        });
};

jutils.loadFormData = function (form, data) {
    if (form === null || !data)
        return;
    for (var name in data) {
        var val = data[name];
        //!_checkField(name, val&& !_npCombotreeField(name, val)
        if (!_checkField(name, val)) {
            form.find("input[id=\"" + name + "\"]").val(val);
            form.find("textarea[id=\"" + name + "\"]").val(val);
            form.find("select[id=\"" + name + "\"]").val(val);
        }
    }

    function _checkField(pName, pValue) {
        var cc = $(form).find("input[id=\"" + pName + "\"][type=radio], input[name=\"" + pName + "\"][type=checkbox]");
        if (cc.length) {
            //cc._propAttr('checked', false);
            cc.each(function () {
                var f = $(this);
                f.prop("checked", false);
                if (f.val() === String(pValue) || $.inArray(f.val(), $.isArray(pValue) ? pValue : [pValue]) >= 0) {
                    f.prop("checked", true);
                }
            });
            return true;
        }
        return false;
    }

};