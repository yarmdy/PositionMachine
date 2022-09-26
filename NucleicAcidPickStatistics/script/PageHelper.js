//-------------分页帮助类--------------

/*定义分页基类*/
function PageHelper(formId, postUrl) {
    this.formId = formId;
    this.postUrl = postUrl;
    this.CreateFormParam();
}


//----------------------------查询条件

/*定义查询条件对象*/
PageHelper.prototype.QueryCondition = {
    pageIndex: 1,
    pageSize: 10,
    pageCount: 0,
    totalCount: 0,
    orderBy: "",//desc,asc 排序方式
    orderCoulumn: ''//--排序字段
}

//---------------------------

/*==============================定义公共方法start============================*/

//创建表单元素
PageHelper.prototype.CreateFormParam = function () {
    var html = "";
    if ($("#formId").length <= 0) {
        html += "<input type=\"hidden\" id=\"formId\" name=\"formId\" value=\"" + this.formId + "\" />";
    }
    if ($("#" + this.formId + "_pageIndex").length <= 0) {
        html += "<input type=\"hidden\" id=\"" + this.formId + "_pageIndex\" name=\"" + this.formId + "_pageIndex\" value=\""+this.QueryCondition.pageIndex+"\" />";
    }
    if ($("#" + this.formId + "_pageSize").length <= 0) {
        html += "<input type=\"hidden\" id=\"" + this.formId + "_pageSize\" name=\"" + this.formId + "_pageSize\" value=\"" + this.QueryCondition.pageSize + "\" />";
    }
    if ($("#" + this.formId + "_pageCount").length <= 0) {
        html += "<input type=\"hidden\" id=\"" + this.formId + "_pageCount\" name=\"" + this.formId + "_pageCount\" value=\"0\" />";
    }
    if ($("#" + this.formId + "_orderBy").length <= 0) {
        html += "<input type=\"hidden\" id=\"" + this.formId + "_orderBy\" name=\"" + this.formId + "_orderBy\" value=\"" + this.QueryCondition.orderBy + "\" />";
    }
    if ($("#" + this.formId + "_orderCoulumn").length <= 0) {
        html += "<input type=\"hidden\" id=\"" + this.formId + "_orderCoulumn\" name=\"" + this.formId + "_orderCoulumn\" value=\"" + this.QueryCondition.orderCoulumn + "\" />";
    }
    $("#" + this.formId).append(html);
}

PageHelper.prototype.Refresh = function (callback, salutation,isAppend)
{
    this.QueryCondition.pageIndex = 1;
    this.QueryCondition.totalCount = 0;
    this.QueryData(callback, salutation);
}

PageHelper.prototype.QueryData = function (callback, salutation, isAppend) {
    if (this.QueryCondition.totalCount > 0) {
        if (this.QueryCondition.totalCount % this.QueryCondition.pageSize == 0) {
            this.QueryCondition.pageCount = this.QueryCondition.totalCount / this.QueryCondition.pageSize;
        }
        else {
            this.QueryCondition.pageCount = parseInt(this.QueryCondition.totalCount / this.QueryCondition.pageSize) + 1;
        }

        if (this.QueryCondition.pageIndex < this.QueryCondition.pageCount) {
            this.QueryCondition.pageIndex++;
        }
        else {
            this.QueryCondition.pageIndex = this.QueryCondition.pageCount+1;
            var json = "{\"Total\":0,\"Rows\":[]}";
            salutation = JSON.parse(json);
            callback(salutation, isAppend);
            return;
        }
    }
    $("#" + this.formId + "_pageIndex").val(this.QueryCondition.pageIndex);
    $("#" + this.formId + "_pageSize").val(this.QueryCondition.pageSize);
    $("#" + this.formId).ajaxSubmit({
        success: function (str) {
            if (typeof callback == "function") {
                salutation = JSON.parse(str);
                
                callback(salutation, isAppend);
            }
        },
        error: function (error) {
        },
        url: this.postUrl,  /*设置post提交到的页面*/
        type: "post", /*设置表单以post方法提交*/
        dataType: "text" /*设置返回值类型为文本*/
    });
}

//PageHelper.prototype.onScrollBootom = function (callback, salutation) {
//    $("#" + this.containId).scroll(function () {
//        var $this = $(this),
//        viewH = $(this).height(),//可见高度  
//        contentH = $(this).get(0).scrollHeight,//内容高度  
//        scrollTop = $(this).scrollTop();//滚动高度  
//        //if(contentH - viewH - scrollTop <= 100) { //到达底部100px时,加载新内容  
//        if (scrollTop / (contentH - viewH) >= 0.95) { //到达底部100px时,加载新内容 

//        }
//    });
//}
/*==============================定义公共方法end============================*/
