<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Universe.SaveUniverse" CodeFile="SaveUniverse.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<base target="_self"/>
		<script language="javascript">
		function cancel(){
			window.returnValue=true;
			window.close();
		}
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body class="imageBackGround">
		<form id="Form1" method="post" runat="server">
			<table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr class="violetBackGround" height="14">
					<td width="14"><asp:Image runat="server" SkinID="fleche_1" /></td>
					<td class="txtBlanc11Bold bandeauTitreBackGround">&nbsp;
						<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="769"></cc2:adexpresstext></td>
				</tr>
				<tr>
					<td></td>
					<td class="whiteBackGround"></td>
				</tr>
				<tr>
					<td></td>
					<td class="txtViolet11Bold whiteBackGround" height="20">&gt;
						<cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="917"></cc2:adexpresstext></td>
				</tr>
				<tr height="40">
					<td></td>
					<td valign="top" class="whiteBackGround">&nbsp;
						<asp:DropDownList id="directoryDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></td>
				</tr>
				<tr height="10">
					<td></td>
					<td></td>
				</tr>
				<!--Debut liste des univers sauvegardés-------------->
                <tr>
	                <td></td>
	                <td class="txtViolet11Bold whiteBackGround" height="20">&gt;
		                <cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2261"></cc2:adexpresstext></td>
                </tr>
                <tr height="40">
	                <td></td>
	                <td vAlign="top" class="whiteBackGround">&nbsp;
		                <asp:DropDownList id="universeDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></td>
                </tr>
                <tr height="10">
	                <td></td>
	                <td></td>
                </tr>
                <!--Fin listes univers sauvegardés------------------->
				<tr>
					<td></td>
					<td class="txtViolet11Bold whiteBackGround">&gt;
						<cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="2268"></cc2:adexpresstext>&nbsp;</td>
				</tr>
				<tr height="40">
					<td></td>
					<td vAlign="top" class="whiteBackGround">&nbsp;
						<asp:TextBox id="universeTextBox" runat="server" CssClass="txtNoir11" Width="200px"></asp:TextBox></td>
				</tr>
				<tr height="20">
					<td></td>
					<td valign="top" align="right">
						<cc1:ImageButtonRollOverWebControl id="okButton" runat="server" onclick="okButton_Click" SkinID="validateButton"></cc1:ImageButtonRollOverWebControl>&nbsp
						<a href="javascript:cancel();"
						onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif';" 
						onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_down.gif';">
						<img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif" border="0" name="bouton">
						</a>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
