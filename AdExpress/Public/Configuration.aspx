<%@ Page language="c#" CodeFile="Configuration.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.Configuration" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Configuration</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body class="darkBackGround whiteBackGround" bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="98%" align="center" class="whiteBackGround" border="0">
				<TR>
					<td class="violetBackGround" colSpan="2"><font class="txtBlanc14Bold">&nbsp;<cc1:adexpresstext language="33" id="titreAdExpressText" runat="server" Code="982"></cc1:adexpresstext>
						</font><font class="txtBlanc11Bold">&nbsp;<cc1:adexpresstext language="33" id="versionAdExpressText" runat="server" Code="983"></cc1:adexpresstext>
						</font>
					</td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/computer.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="computerAdExpressText" runat="server" Code="981"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/iexplorer.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="iexplorerAdexpresstext" runat="server" Code="980"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/screen.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="screenAdexpresstext" runat="server" Code="984"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/excel.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="excelAdexpresstext" runat="server" Code="985"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/adobe.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="adobeAdexpresstext" runat="server" Code="986"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/flash.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="flashAdexpresstext" runat="server" Code="987"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/realOne.gif">&nbsp;<br>
						<br>
						<IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/mediaPlayer.gif">&nbsp;
					</td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="videoAdexpresstext" runat="server" Code="988"></cc1:adexpresstext></td>
				</tr>
				<!-- Bouton de retour -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image25" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td class="violetBackGround" colSpan="2"><asp:Image ID="Image27" runat="server" SkinID="pixel" height="1" width="100%" /></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image26" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="right" class="whiteBackGround" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
