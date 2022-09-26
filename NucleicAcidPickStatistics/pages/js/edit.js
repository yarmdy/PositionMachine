function ShowElementObj() {
    var objs = document.getElementsByClassName("div_title");
    for (var i = 1; i < objs.length; i++) {
        var obj = objs[i];
        var div_id = obj.attributes['id'].value;
        var tb_id = div_id.replace("div_", "tb_");
        $("#" + tb_id).hide();
    }
}
