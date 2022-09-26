
/*显示上传进度状态*/
function ShowStatus(is_show, msg) {
    var html = "";
    if ($("#loadStatus_opendiv").length <= 0) {
        html += "<div id=\"loadStatus_opendiv\" class=\"loadStatus_opendiv\" style=\"display: none;\"> <div id=\"loadStatus_content_msg\"></div></div>";
    }
    if ($("#loadStatus_opendiv_bg").length <= 0) {
        html += "<div id=\"loadStatus_opendiv_bg\" class=\"loadStatus_opendiv_bg\" style=\"display: none;\"></div>";
    }
    if (html != "") {
        $("body").append(html);
    }

    if (is_show) {
        $("#loadStatus_opendiv").show();
        $("#loadStatus_opendiv_bg").show();
        if (msg) {
            $("#loadStatus_content_msg").html(msg);
        }
    }
    else {
        $("#loadStatus_opendiv").hide();
        $("#loadStatus_opendiv_bg").hide();
    }
}

/*检测是否加载完毕*/
function HideStatus() {
    var imgdefereds = [];
    $('img').each(function () {
        var dfd = $.Deferred();
        $(this).bind('load', function () {
            dfd.resolve();
        }).bind('error', function () {
        })
        if (this.complete) setTimeout(function () {
            dfd.resolve();
        }, 1000);
        imgdefereds.push(dfd);
    })
    $.when.apply(null, imgdefereds).done(function () {
        ShowStatus(false, "");
    });
}