using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NucleicAcidPickStatistics
{
    public static class ControllerExpress
    {
        public static ZEHOU.PM.DB.dbLabelInfo.User GetUser(this Controller controller) {
            var cookie = HttpContext.Current?.User?.Identity?.Name ?? "";
            if (cookie == "") {
                return null;
            }
            var arr = cookie.Split('|');
            if (arr.Length != 4)
            {
                return null;
            }
            return new ZEHOU.PM.DB.dbLabelInfo.User() { ID = arr[0], LoginName = arr[1], TrueName = arr[2], RoleID = arr[3] };
        }
        public static (ZEHOU.PM.DB.dbLabelInfo.User user, List<ZEHOU.PM.DB.dbLabelInfo.Role> roles, ZEHOU.PM.DB.dbLabelInfo.Department depart, ZEHOU.PM.DB.dbLabelInfo.Post post) GetUserFull(this Controller controller)
        {
            var user = controller.GetUser();
            if (user == null)
            {
                return (null,null,null,null);
            }

            var loginBll = new ZEHOU.PM.Bll.Login();
            return loginBll.GetUserAllInfoByUserId(user.ID);
        }
    }

    public static class PageHelper {
        public static string DetailPageJsAndCss()
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            html.AppendLine(" <meta content=\"width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=0\" name=\"viewport\" />");
            html.AppendLine("<meta http-equiv=\"Cache-Control\" content=\"no-siteapp\" />");

            html.AppendLine("<link href=\"/script/ligerUI/skins/Aqua/css/ligerui-all.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/script/ligerUI/skins/Gray/css/grid.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/script/ligerUI/skins/Gray/css/all.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/script/tab/jquery-ui.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/script/tab/jquery-ui.structure.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/script/tab/jquery-ui.theme.min.css\" rel=\"stylesheet\" type=\"text/css\" />");

            html.AppendLine("<link href=\"/script/ligerUI/skins/Gray/css/common.css\" rel=\"stylesheet\" type=\"text/css\" />");

            html.AppendLine("<link href=\"/script/jbox/themes/TooltipBorder.css\" rel=\"stylesheet\" />");
            html.AppendLine("<link href=\"/script/jbox/jBox.css\" rel=\"stylesheet\" />");

            html.AppendLine("<link href=\"/css/edit_page.css\" rel=\"stylesheet\" type=\"text/css\" />");
            html.AppendLine("<link href=\"/css/common_page.css\" rel=\"stylesheet\" type=\"text/css\" />");

            html.AppendLine("<script src=\"/script/jquery.js\"></script>");
            html.AppendLine("<script src=\"/script/json2.js\"></script>");
            html.AppendLine("<script src=\"/script/tab/jquery-ui.js\"></script>");
            html.AppendLine("<script src=\"/script/WebServiceHelper.js\"></script>");
            html.AppendLine("<script src=\"/script/My97DatePicker/WdatePicker.js\"></script>");
            html.AppendLine("<script src=\"/script/lhgdialog/lhgdialog/lhgdialog.js?skin=mac\"></script>");
            html.AppendLine("<script src=\"/script/ligerUI/js/core/base.js\"></script>");
            html.AppendLine("<script src=\"/script/ligerUI/js/plugins/ligerGrid.js\"></script>");
            html.AppendLine("<script src=\"/script/jquery.form.js\"></script>");
            html.AppendLine("<script src=\"/script/jq.select.js\"></script>");
            html.AppendLine("<script src=\"/script/layer-v2.2/layer.js\"></script>");
            html.AppendLine("<script src=\"/script/jquery.helper.js\"></script>");
            html.AppendLine("<script src=\"/script/CnToPinYin.js\"></script>");
            html.AppendLine("<script src=\"/script/jbox/jBox.js\"></script>");
            html.AppendLine("<script src=\"/pages/js/edit.js\"></script>");
            return html.ToString();
        }
    }
}