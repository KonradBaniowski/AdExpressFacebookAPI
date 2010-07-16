<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.MySessionSavePopUp" CodeFile="MySessionSavePopUp.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="Server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr class="violetBackGround" height="14">
					<td width="14"><asp:Image ID="Image153" runat="server" SkinID="fleche_1" /></td>
					<td class="txtBlanc11Bold bandeauTitreBackGround">&nbsp;
						<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="908"></cc2:adexpresstext></td>
				</tr>
				<tr>
					<td></td>
					<td class="backGroundWhite"></td>
				</tr>
				<tr>
					<td></td>
					<td class="txtViolet11Bold backGroundWhite" height="20">&gt;
						<cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="702"></cc2:adexpresstext></td>
				</tr>
				<tr height="40">
					<td></td>
					<td valign="top" class="backGroundWhite">&nbsp;
						<asp:dropdownlist id="directoryDropDownList" runat="server" Width="200px" CssClass="txtNoir11">												
						</asp:dropdownlist></td>
				</tr>
				<tr height="10">
					<td></td>
					<td></td>
				</tr>
				<!---Debut liste des résultats-->
				<tr>
	                <td></td>
	                <td class="txtViolet11Bold backGroundWhite" height="20">&gt;
		                <cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2261"></cc2:adexpresstext></td>
                </tr>
                <tr height="40">
	                <td></td>
	                <td valign="top" class="backGroundWhite">&nbsp;
		                <asp:dropdownlist id="sessionDropDownList" runat="server" Width="200px" CssClass="txtNoir11">		                
		                </asp:dropdownlist></td>
                </tr>
                <tr height="10">
	                <td></td>
	                <td></td>
                </tr>
				<!---Fin liste des résultats-->
				<tr>
					<td></td>
					<td class="txtViolet11Bold backGroundWhite" >&gt;
						<cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="2263"></cc2:adexpresstext></td>
				</tr>
				<tr height="40">
					<td></td>
					<td valign="top" class="backGroundWhite">&nbsp;
						<asp:textbox id="mySessionTextBox" runat="server" Width="200px" CssClass="txtNoir11"></asp:textbox></td>
				</tr>
				<tr height="20">
					<td></td>
					<td valign="top" align="right"><cc1:imagebuttonrolloverwebcontrol id="oKImageButtonRollOverWebControl" runat="server" onclick="oKImageButtonRollOverWebControl_Click" SkinID="okButton"></cc1:imagebuttonrolloverwebcontrol>
					&nbsp;<cc1:imagebuttonrolloverwebcontrol id="cancelImageButtonRollOverWebControl" runat="server" onclick="cancelImageButtonRollOverWebControl_Click" SkinID="annulerButton"></cc1:imagebuttonrolloverwebcontrol></td>
				</tr>
			</table>
		</form>
	</body>
</html>
