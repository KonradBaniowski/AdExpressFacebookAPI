<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<%@ Page language="c#" Inherits="BastetWeb.MailSelection" CodeFile="MailSelection.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Bastet</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" height="50%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr height="90" bgcolor="#644883"  style="font-size: 0px;">
					<td width="185"  style="font-size: 0px;"><a href="/Index.aspx"  style="font-size: 0px;"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%"  style="font-size: 0px;">
						<cc1:HeaderWebControl id="HeaderWebControl1" runat="server" SkinID="HeaderWebControl"></cc1:HeaderWebControl>
					</td>
				</tr>
				<tr>
					<td vAlign="top" colSpan="2">
						<!--<p class="navigation">&nbsp;<font color="#ff0099"><%=_email_manage%></font>&gt;<%=_period_manage%>&gt;<%=_login_manage%>&amp;<%=_validation%></p>-->
						<p class="txtViolet12BoldTitle">&nbsp;<%=_email_manage%></p>
						<P>&nbsp;</P>
						<div align="center">
							<table class="txtViolet11" cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<td class="txtViolet12Bold" colSpan="2" align="left"><%=_label_addEmail%></td>
								</tr>
								<tr>
									<td><asp:textbox id="mailTextBox" runat="server" Width="200px" CssClass="input"></asp:textbox></td>
									<td><asp:button id="addMailButton" runat="server" Width="70px" CssClass="input" Text="" onclick="addMailButton_Click"></asp:button></td>
								</tr>
								<tr>
									<td colSpan="2">&nbsp;</td>
								</tr>
								<tr>
									<td class="txtViolet12Bold" colSpan="2" align="left"><%=_label_emailList%></td>
								</tr>
								<tr vAlign="top">
									<td><asp:listbox id="mailListBox" runat="server" Width="200px" CssClass="input" Height="200px"></asp:listbox></td>
									<td><asp:button id="deleteMailButton" runat="server" Width="70px" CssClass="input" Text="" onclick="deleteMailButton_Click"></asp:button></td>
								</tr>
							</table>
						<P>&nbsp;</P>
						<P align="center"><asp:button id="validateButton" runat="server" Width="110px" CssClass="input" Text="" onclick="validateButton_Click"></asp:button></P>
						<P></div>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
