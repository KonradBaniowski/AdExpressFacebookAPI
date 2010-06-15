<%@ Page language="c#" Inherits="BastetWeb.Index" CodeFile="Index.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<%@ Register TagPrefix="cc1" Namespace="TNS.Isis.WebControls.Authentification" Assembly="TNS.Isis.WebControls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>Bastet</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" cellSpacing="0" cellPadding="0" width="100%" border="0" height="50%">
				<tr height="90" bgcolor="#644883">
					<td width="185"><a href="/Index.aspx"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%">
						<cc2:HeaderWebControl id="HeaderWebControl1" runat="server" SkinID="HeaderWebControl"></cc2:HeaderWebControl>
					</td>
				</tr>
				<tr>
					<td colspan="2" valign="top">
						<p>&nbsp;</p>
						<p>&nbsp;</p>
						<p>&nbsp;</p>
						<p align="center">
						    <cc1:LoginWebControl id="loginWebControl" runat="server" 
						        WebServiceMethod="canAccessToBastet"
						        CssTable="LoginWebControlTable" >
						    </cc1:LoginWebControl></p>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
