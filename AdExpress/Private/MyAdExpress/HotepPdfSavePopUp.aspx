<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.HotepPdfSavePopUp" CodeFile="HotepPdfSavePopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
  </HEAD>
	<body background="/images/Common/dupli_fond.gif">
		<form id="Form1" method="post" runat="server">
			<table id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr bgColor="#644883" height="14">
					<td style="WIDTH: 14px; HEIGHT: 13px"><IMG src="/Images/Common/fleche_1.gif"></td>
					<td class="txtBlanc11Bold" style="WIDTH: 100%; HEIGHT: 13px" background="/Images/Common/bandeau_titre.gif">&nbsp;
						<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="1747"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2">
						<table id="SaveData" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="35">
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="FileNameLabel" runat="server" Code="1746"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxFileName" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr>
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="MailLabel" runat="server" Code="1136"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxMail" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr>
								<td colSpan="2">&nbsp;</td>
							</tr>
							<tr>
								<td align="right" colSpan="2"><cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" runat="server" onclick="validateRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
									<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" runat="server" onclick="closeRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
