<%@ Page language="c#" CodeFile="Configuration.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.Configuration" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
    </HEAD>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="800" border="0" height="600">
			    <!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									
									<table id="Table6" cellspacing="0" cellpadding="0" width="185" border="0">
										<tr>
											<td class="backGroundBlack"><asp:Image ID="Image4" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										
										<!-- éléments variables du menus de gauche -->
										<tr valign="top">
											<td id="pageTitle">
												<cc3:PageTitleWebControl id="PageTitleWebControl1" runat="server" CodeDescription="982" CodeTitle="990"></cc3:PageTitleWebControl>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
											<table id="Table4" cellSpacing="0" cellPadding="0" border="0">
												<tr>
													<td><asp:Image ID="Image6" runat="server" SkinID="pixel" height="3" width="5" /></td>
												</tr>
												<tr>
													<td class="whiteBackGround">
													<a onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></a>
													</td>
												</tr>
											</table>
											</td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image7" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image8" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image11" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<!-- Menu haut -->
							<tr>
								<td><cc3:headerwebcontrol language="33" id="HeaderWebControl1" runat="server"></cc3:headerwebcontrol></td>
							</tr>
							<!-- Centre -->
							<tr>
								<td align="center">
									
								    <table cellSpacing="0" cellPadding="0" width="98%" align="center" class="backGroundWhite" border="0">
				                        <TR>
					                        <td class="violetBackGround" colSpan="2"><font class="txtBlanc14Bold">&nbsp;<cc1:adexpresstext language="33" id="titreAdExpressText" runat="server" Code="982"></cc1:adexpresstext>
						                        </font><font class="txtBlanc11Bold">&nbsp;<cc1:adexpresstext language="33" id="versionAdExpressText" runat="server" Code="983"></cc1:adexpresstext>
						                        </font>
					                        </td>
				                        </TR>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/computer.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="computerAdExpressText" runat="server" Code="981"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image12" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/iexplorer.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="iexplorerAdexpresstext" runat="server" Code="980"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image13" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/screen.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="screenAdexpresstext" runat="server" Code="984"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image14" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/excel.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="excelAdexpresstext" runat="server" Code="985"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image15" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/adobe.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="adobeAdexpresstext" runat="server" Code="986"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image16" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/flash.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="flashAdexpresstext" runat="server" Code="987"></cc1:adexpresstext></td>
				                        </tr>
				                        <tr>
					                        <td colSpan="2"><asp:Image ID="Image17" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				                        </tr>
				                        <tr>
					                        <td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/realOne.gif">&nbsp;<br>
						                        <br>
						                        <IMG src="/App_Themes/<%= this.Theme %>/Images/Common/ConfigurationLogo/mediaPlayer.gif">&nbsp;
					                        </td>
					                        <td class="txtViolet11"><cc1:adexpresstext language="33" id="videoAdexpresstext" runat="server" Code="988"></cc1:adexpresstext></td>
				                        </tr>
			                        </table>
									
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
