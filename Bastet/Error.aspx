<%@ Page language="c#" Inherits="BastetWeb.Error" CodeFile="Error.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>Bastet</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" Content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" cellSpacing="0" cellPadding="0" width="100%" border="0" height="50%">
				<tr height="90" bgcolor="#644883">
					<td width="185"><a href="/Index.aspx"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%">
						<table class="txtViolet11" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td><asp:Image ID="Image1" runat="server" height="31" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td class="dupli1BackGround"><asp:Image ID="Image2" runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
							<tr class="headerBackGround">
								<td><asp:Image ID="Image3" runat="server" height="58" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2" valign="middle">
						<p align="center" class="txtRouge12Bold"><%=_messageError%></p>
						<p align="center"><cc1:BastetText language="33" id="AdExpressText1" runat="server" Code="49"></cc1:BastetText>&nbsp; <a href="<%=_page%>"><cc1:BastetText language="33" id="AdExpressText2" runat="server" Code="50"></cc1:BastetText></a></p>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
