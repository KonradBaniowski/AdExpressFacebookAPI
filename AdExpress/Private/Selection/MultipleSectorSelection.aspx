<%@ Page language="c#" Inherits="AdExpress.Private.Selection.MultipleSectorSelection" CodeFile="MultipleSectorSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
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
	<body onload="<%=Script%>javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
					<tr>
						<!-- marge de gauche-->
						<td valign="top" width="10">
							<table id="Table3" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server"  height="100" SkinID="logo_cote_gauche" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server"  height="1" SkinID="pixel" width="1" /></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td valign="top" width="185" class="imageBackGround">
							<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
								<tr valign="top" class="whiteBackGround">
									<!-- Logo -->
									<td><asp:Image runat="server"  height="90" SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server"  height="5" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server"  height="10" SkinID="pixel" width="1" /></td>
								</tr>
								<!-- éléments variables du menus de gauche -->
								<tr valign="top">
									<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="1538"></cc2:moduletitlewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><asp:Image runat="server"  height="5" SkinID="pixel" width="1" /></td>
								</tr>
								<tr>
									<td>
										<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
								</tr>
							</table>
						</td>
						<!-- Séparateur -->
						<td valign="top">
							<table id="Table5" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server"  height="100" SkinID="logo_cote_droit" width="5" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server"  height="1" SkinID="pixel" width="1" /></td>
								</tr>
							</table>
						</td>
						<td class="violetBackGround"><asp:Image runat="server"  height="1" SkinID="pixel" width="1" /></td>
						<td width="10" class="imageBackGround">&nbsp;</td>
						<td width="10" class="whiteBackGround">&nbsp;</td>
						<!-- droite-->
						<td id="sertARien" valign="top">
							<table id="Table10" style="height:100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								
									<tr>
										<!-- Menu du haut-->
										<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
									</tr>
									<tr>
										<!-- ligne du haut a droite -->
										<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
											<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td valign="top" align="left" height="1%"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
													<td height="1%"><asp:Image runat="server"  height="17" SkinID="pixel" width="1" /></td>
													<td valign="top" align="right" height="1%"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
												</tr>
											</table>
										</td>
									</tr>
									<!-- Centre -->
									<tr valign="top">
										<td align="center" class="imageBackGround">
											<table id="data" cellspacing="0" cellpadding="0" width="100%" class="whiteBackGround">
												<tr style="HEIGHT: 10px">
													<td style="WIDTH: 478px" width="478" class="imageBackGround" colspan="6"></td>
												</tr>
												<tr>
													<td colspan="5" class="whiteBackGround">
												
														<cc3:LoadableUniversWebControl id="LoadableUniversWebControl1" runat="server"></cc3:LoadableUniversWebControl>
													</td>
												</tr>
												<!--<tr style="HEIGHT: 5px">
													<td colspan="6"></td>
												</tr>-->
												<tr style="HEIGHT: 10px">
													<td style="WIDTH: 478px" width="478" class="imageBackGround" colspan="6"></td>
												</tr>
												<!--fin chargement univers-->
												<!--Debut code généré-->
												<cc3:sectorselectionwebcontrol id="sectorSelectionWebControl" runat="server"></cc3:sectorselectionwebcontrol>
												<!--fin code généré-->
												<tr valign="top">
													<td align="right">
														<cc4:imagebuttonrolloverwebcontrol id="saveUniverseImageButtonRollOverWebControl" runat="server" SkinID="saveButton" onclick="saveUniverseImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol>&nbsp;
														<cc4:imagebuttonrolloverwebcontrol id="validateButton" runat="server" CommandName="next" onclick="validateButton_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol><asp:Image runat="server"  SkinID="pixel" width="100" height="1" /></td>
												</tr>
											</table>
										</td>
									</tr>								
							</table>
						</td>
						<!-- la fin -->
						<td></td>
					</tr>
					<!-- ligne du bas a droite -->
					<tr style="height:5px">
						<td></td>
						<td valign="top" class="imageBackGround">
							<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
									<td></td>
									<td valign="bottom" align="right"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
								</tr>
							</table>
						</td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td id="dellCel" valign="top" class="imageBackGround">
							<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
									<td>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
									<td valign="bottom" align="right"><asp:Image runat="server"  height="5" SkinID="croix" width="5" /></td>
								</tr>
							</table>
						</td>
					</tr>				
			</table>
			<!--</tr></TBODY></table></tr></TBODY></table></tr></TBODY></table>--><!--2005MI-->
			</form>
		<%=divClose%>
	</body>
</html>
