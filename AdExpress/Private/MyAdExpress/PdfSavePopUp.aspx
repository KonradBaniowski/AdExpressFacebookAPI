<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.PdfSavePopUp" CodeFile="PdfSavePopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
	<body class="popUpbody" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
		    <table cellSpacing="0" cellPadding="0" width="100%" height="100%" border="0">
		        <!-- Header -->
		        <tr>
			        <td class="popUpHeaderBackground popUpTextHeader">&nbsp;<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="1747"></cc1:adexpresstext></td>
		        </tr>

		        <!-- Content -->
		        <tr>
			        <td style="height:100%;background-color:#FFF;padding:10;" valign="top">
			        
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
								<td colSpan="2">&nbsp;</td>
							</tr>
							<tr class="backGroundWhite">
								<td class="txtViolet11Bold" colspan="2"><asp:CheckBox id="cbxRegisterMail" runat="server" Visible="False"></asp:CheckBox></td>
							</tr>
						</table>
						
			        </td>
		        </tr>

		        <!-- Footer -->
		        <tr>
			        <td class="popUpFooterBackground" align="right"><cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" runat="server" onclick="validateRollOverWebControl_Click" SkinID="validateButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" runat="server" onclick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;</td>
		        </tr>
	        </table>

		</form>
	</body>
</html>
