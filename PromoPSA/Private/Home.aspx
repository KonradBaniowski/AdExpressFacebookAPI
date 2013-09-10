<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Private_Home" %>
<%@ Register TagPrefix="cc1" Namespace="KMI.PromoPSA.Web.Controls.Header" Assembly="KMI.PromoPSA.Web.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;"/>
	<meta content="C#" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
	<meta http-equiv="expires" content="0"/>
	<meta http-equiv="pragma" content="no-cache"/>
	<meta content="no-cache" name="Cache-control"/>
    <link rel="stylesheet" type="text/css" media="screen" href="path_to_ui_css_file/jquery-ui-1.7.1.custom.css" />
</head>
<body class="bodyStyle">
    <form id="form1" runat="server">
        <div class="header" >
            <div style="float:left;">
                <asp:Image ID="Image1" runat="server" SkinID="logo_tns_home" />
            </div>
        </div>
        <div class="promotionsInformationDiv">
            <table cellspacing="0" cellpadding="0" border="0" height="100%">
                <tr>
                    <td><cc1:DisconnectUserWebControl runat="server" id="DisconnectUserWebControl1" SkinID="DisconnectUserWebControl"/></td>
                    <td><cc1:LoginInformationWebControl runat="server" id="LoginInformationWebControl1" SkinID="LoginInformationWebControl"/></td>
                    <td><cc1:PromotionInformationWebControl runat="server" id="PromotionInformationWebControl1" SkinID="PromotionInformationWebControl"/></td>
                </tr>
            </table>
        </div>
        <div style="margin-top:10px; margin-left:10px;">
            
        </div>
    </form>
</body>
</html>


