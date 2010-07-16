<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.APPMTextFileSavePopUp" CodeFile="APPMTextFileSavePopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
	<body class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr class="violetBackGround" style="height:14px">
					<td style="WIDTH: 14px; HEIGHT: 13px"><asp:Image runat="server" SkinID="fleche_1"/></td>
					<td class="txtBlanc11Bold bandeauTitreBackGround" style="WIDTH: 100%; HEIGHT: 13px">&nbsp;
						<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="1912"></cc1:adexpresstext></td>
				</tr>
				<tr>
				    <td></td>
					<td>
						<table id="SaveData" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr height="35" class="backGroundWhite">
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="FileNameLabel" runat="server" Code="1746"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxFileName" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr class="backGroundWhite">
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="MailLabel" runat="server" Code="1136"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxMail" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr class="backGroundWhite">
								<td colspan="2">&nbsp;</td>
							</tr>
							<tr class="backGroundWhite">
								<td class="txtViolet11Bold" colspan="2"><asp:CheckBox id="cbxRegisterMail" runat="server" Visible="False"></asp:CheckBox></td>
							</tr>
							<tr class="backGroundWhite">
								<td colspan="2">&nbsp;</td>
							</tr>
							<tr>
								<td align="right" colspan="2"><cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" SkinID="validateButton" runat="server" onclick="validateRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
									<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" SkinID="fermerButton" runat="server" onclick="closeRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
