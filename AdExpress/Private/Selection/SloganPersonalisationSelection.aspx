<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.SloganPersonalisationSelection" CodeFile="SloganPersonalisationSelection.aspx.cs" %>
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
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				
					<tr>
						<!-- marge de gauche-->
						<td valign="top" width="10">
							<table id="Table3" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td valign="top" width="185" class="imageBackGround">
							<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
								<tr valign="top" class="whiteBackGround">
									<!-- Logo -->
									<td><asp:Image runat="server" height="90" SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<!-- éléments variables du menus de gauche -->
								<tr valign="top">
									<td id="pageTitle">
										<cc2:ModuleTitleWebControl id="ModuleTitleWebControl1" runat="server" CodeDescription="1925"></cc2:ModuleTitleWebControl>
									</td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
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
									<td><asp:Image runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
								</tr>
							</table>
						</td>
						<td class="violetBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
						<td width="10" class="imageBackGround">&nbsp;</td>
						<td width="10" class="whiteBackGround">&nbsp;</td>
						<!-- droite-->
						<td id="sertARien" valign="top">
							<table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tbody>
									<tr>
										<!-- Menu du haut-->
										<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
									</tr>
									<tr>
										<!-- ligne du haut a droite -->
										<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
											<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td valign="top" align="left" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
													<td height="1%"><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></td>
													<td valign="top" align="right" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
												</tr>
											</table>
										</td>
									</tr>
									<!-- Centre -->
									<tr valign="top">
										<td align="center">
											<table id="data" height="100%" cellpadding="3" cellspacing="0" width="100%" class="imageBackGround"
												border="0">
												<tr valign="top" height="5" class="whiteBackGround">
													<td></td>
												</tr>
												<!--<tr style="HEIGHT: 10px" valign="top">
													<td width="478" background="/images/Common/dupli_fond.gif" colSpan="6" style="WIDTH: 478px"></td>
												</tr>-->
												<!--fin chargement univers-->
												<!--Debut code généré-->
												<tr class="whiteBackGround" valign="top">
													<td>
														<table cellspacing="0" cellpadding="0" width="90%" class="violetBackGround" align="center">
															<tr>
																<td>
																	<table id="centerTable" height="100%" cellspacing="0" cellpadding="0" width="100%" class="whiteBackGround"
																		border="0">
																		<!--Debut code généré personnalisation des versions-->
																		<cc3:SloganPersonalisationWebControl id="SloganPersonalisationWebControl1" runat="server" CssClass="txtViolet11Bold"></cc3:SloganPersonalisationWebControl>
																		<!--fin code généré personnalisation des versions-->
																	</table>
																</td>
															</tr>
														</table>
													</td>
												</tr>
												<!--fin code généré-->
												<tr valign="top" class="whiteBackGround">
													<td>&nbsp;</td>
												</tr>
												<tr height="100%" valign="top">
													<td class="imageBackGround" align="right">
														<cc4:ImageButtonRollOverWebControl id="validateButton" runat="server" onclick="validateButton_Click" SkinID="validateButton"></cc4:ImageButtonRollOverWebControl>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
												</tr>
											</table>
										</td>
									</tr>
								</TBODY>
							</table>
						</td>
						<!-- la fin -->
						<td></td>
					</tr>
					<!-- ligne du bas a droite -->
					<tr height="5">
						<td></td>
						<td valign="top" class="imageBackGround">
							<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
									<td></td>
									<td valign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
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
									<td valign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
									<td>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
									<td valign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
								</tr>
							</table>
						</td>
					</tr>
				
			</table>
			<!--</tr></TBODY></table></tr></TBODY></table></tr></TBODY></table>-->
		</form>
	</body>
</html>
