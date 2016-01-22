<%@ Page language="c#" Inherits="AdExpress.Private.Selection.AEPMTargetSelection" CodeFile="AEPMTargetSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="imagetoolbar" content="no" />
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
		<meta http-equiv="expires" content="0" />
		<meta http-equiv="pragma" content="no-cache" />
		<meta name="Cache-control" content="no-cache" />
	</head>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
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
							<tr vAlign="top" class="backGroundBlack">
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
												<cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="1654"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc2:InformationWebControl id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:InformationWebControl></td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image6" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image7" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image8" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>

							<!-- Centre -->
							<tr valign="top">
								<td align="center">
									<table id="data" style="height:100%" cellspacing="0" cellpadding="3" width="100%" border="0">
										<tr valign="top">
											<td width="10">&nbsp;</td>
											<td>
												<table cellspacing="1" cellpadding="0" width="100%" class="violetBackGround" style="text-align:left">
													<cc3:targetselectionwebcontrol id="targetSelectionWebControl1" runat="server" CssClass="txtBlanc11Bold"></cc3:targetselectionwebcontrol>
													<cc3:targetselectionwebcontrol id="targetSelectionWebControl2" runat="server" CssClass="txtBlanc11Bold"></cc3:targetselectionwebcontrol>
												</table>
											</td>
										</tr>
										<tr valign="top" align="right" >
											<td colspan="2"><cc4:imagebuttonrolloverwebcontrol SkinID="validateButton" id="validateButton" runat="server" onclick="validateButton_Click"></cc4:imagebuttonrolloverwebcontrol></td>
										</tr>
										<tr valign="top" style="height:100%">
											<td colspan="2">&nbsp;<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
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
</html>
