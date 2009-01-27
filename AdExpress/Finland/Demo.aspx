<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Demo.aspx.cs" Inherits="Finland_Demo" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Page sans titre</title>
</head>
<body bgcolor="#FFFFFF">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" align="center" width="1024px" height="768px">
        <tr>
	        <td>
			    <cc3:FlashWebControl ID="flashWebControl1" SkinID="AdexpressDemo" runat="server" Width="1024" Height="768"></cc3:FlashWebControl>
		    </td>
		</tr>
	</table>
    </form>
</body>
</html>
