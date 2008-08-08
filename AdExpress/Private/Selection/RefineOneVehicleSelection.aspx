<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.RefineOneVehicleSelection" CodeFile="RefineOneVehicleSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta HTTP-EQUIV="imagetoolbar" CONTENT="no"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" width="10">
						<table id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" width="185" class="imageBackGround">
						<table id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height=90 SkinID="logo" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo"/></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle">
									<cc2:ModuleTitleWebControl id="ModuleTitleWebControl1" runat="server" CodeDescription="1064"></cc2:ModuleTitleWebControl>
								</td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
							</tr>
							<tr>
								<td>
									<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td id="sertARien" vAlign="top">
						<table id="Table10" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<!-- Menu du haut-->
								<td vAlign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td id="lineAVIrer" vAlign="top" class="imageBackGround" height="1%">
									<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
											<td height="1%"><asp:Image runat="server" height="17" SkinID="pixel" width="1"/></td>
											<td vAlign="top" align="right" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr valign="top">
								<td align="center">
									<table id="data" height="100%" cellPadding="3" cellspacing="0" width="100%" class="imageBackGround"
										border="0">
										<tr valign="top" class="whiteBackGround">
											<td width="10">&nbsp;</td>
											<td>
												<table cellpadding="0" cellspacing="1" class="violetBackGround" width="90%">
													<tr>
														<td class="whiteBackGround txtViolet11Bold">&nbsp;<cc1:AdExpressText language="33" id="titreAdexpresstext" runat="server" Code="1065"></cc1:AdExpressText>
														</td>
													</tr>
													<tr>
														<td>
															<table id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0"
																class="paleVioletBackGround">
																<tr valign="top">
																	<td>
																		<cc3:OneVehicleSelectionWebControl id="OneVehicleSelectionWebControl1" runat="server" CssClass="txtViolet11Bold"></cc3:OneVehicleSelectionWebControl></td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr valign="top" class="whiteBackGround" align="right">
											<td colspan="2"><cc4:imagebuttonrolloverwebcontrol id="validateButton" SkinID="validateButton" runat="server" onclick="validateButton_Click"></cc4:imagebuttonrolloverwebcontrol><asp:Image runat="server" SkinID="pixel" width="100" height="1"/></td>										</tr>
										<tr valign="top" height="100%">
											<td colspan="2">&nbsp;
												<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
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
				<tr height="5">
					<td></td>
					<td valign="top" class="imageBackGround">
						<table id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td></td>
								<td vAlign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</table>
					</td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td id="dellCel" valign="top" class="imageBackGround">
						<table id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td></td>
								<td vAlign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</FORM>
	</body>
</html>
