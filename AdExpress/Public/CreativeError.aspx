<%@ Page Language="C#" AutoEventWireup="false" CodeFile="CreativeError.aspx.cs" Inherits="AdExpress.CreativeError" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
	<head>
		<title>AdExpress</title>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" style="background-color:#EFF3F4">
		<form id="Form2" method="post" runat="server">
		
			<table id="messageBox" border="0" cellpadding="0" cellspacing="0" width="500px" style="border:solid 2px; border-color:#5C5C5C; background-color:#FFFFFF; margin-top:15%; margin-left:25%;">
            <tr align="center">
                <td align="center">
                        <asp:Label id="msg" runat="server" Font-Names="Arial" Font-Size="12px"></asp:Label>
                </td>
            </tr>
        </table>
		</form>
	</body>
</html>
