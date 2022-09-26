/*jquer websocket框架插件*/
//网页链接采用 ?fuid=lucy&touid=lily&group=分组编号&sentype=发送类型

/*
@ip：ip
@port:端口
@open_callback:打开回调函数
@close_callback：关闭回调函数
@error_callback：错误回调函数
@msg_callback：消息接收回调函数
@msg:接收消息
*/
function WebSocketHelper(ip, port,urlParam, open_callback, close_callback, error_callback, msg_callback, msg_json) {
    var url = "ws://" + ip + ":" + port + "/chat"+urlParam;

    this.Socket = { webSocket: null, msg: "", error_code: true };

    if ("WebSocket" in window) {
        this.Socket.webSocket = new WebSocket(url);

    }
    else if ("MozWebSocket" in window) {
        this.Socket.webSocket = new MozWebSocket(url);
    } else {
        this.Socket.msg = "浏览器不支持WebSocket";
        this.Socket.error_code = false;
    }

    this.Socket.webSocket.onopen = function () {
        if (typeof open_callback == "function") {
            open_callback();
        }
    }

    this.Socket.webSocket.onclose = function () {
        if (typeof close_callback == "function") {
            close_callback();
        }
    }
    this.Socket.webSocket.onerror = function () {
        if (typeof error_callback == "function") {
            error_callback();
        }
    }
    this.Socket.webSocket.onmessage = function (msg) {
        if (typeof msg_callback == "function") {
            msg_json = msg;
            msg_callback(msg_json);
        }
    }
}

/*发送消息*/
WebSocketHelper.prototype.SentMsg = function (msgObj) {
    if (this.Socket.error_code && msgObj.data != "") {
        this.Socket.webSocket.send(JSON.stringify(msgObj));
    }

}


/*发送消息
@sentType:发送类型【signal=发送单个目标用户，group=发送分组用户，all=发给所有用户】
@fuid:发送者
@touid:接收者
@msg:消息内容
*/
WebSocketHelper.prototype.SentMsgParam = function (sentType,fuid,touid,msg) {
    if (this.Socket.error_code) {

        /*消息发送格式*/
        var MsgSentObj =
        {
            sentType: sentType,//发送类型【signal=发送单个目标用户，group=发送分组用户，all=发给所有用户】
            time: new Date().format("yyyy-MM-dd hh:mm:ss"),
            fuid: fuid,
            touid: touid,
            data:this.ObjToUrlParam(msg),//消息内容 json格式
        };
        this.Socket.webSocket.send(JSON.stringify(MsgSentObj));
    }

}

/*对象转出url参数格式*/
WebSocketHelper.prototype.ObjToUrlParam = function (obj)
{
    var arr = ["?1=1"];
    for (var n in obj) {
        arr.push(n + "=" + obj[n]);
    }
    var urlPrams = arr.join("&");
    return urlPrams;
}

