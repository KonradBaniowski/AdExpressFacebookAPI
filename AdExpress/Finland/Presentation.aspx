<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Presentation.aspx.cs" Inherits="Finland_Presentation" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Page sans titre</title>
</head>
<body>
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" align="center">
        <tr>
	        <td>
			    <cc3:FlashWebControl ID="flashWebControl1" SkinID="AdexpressPresentation" runat="server" Width="800" Height="600"></cc3:FlashWebControl>
		    </td>
		</tr>
	</table>
    </form>
</body>
</html>
