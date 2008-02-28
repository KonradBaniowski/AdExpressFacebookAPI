<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.OneVehicleSelection" CodeFile="OneVehicleSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="imagetoolbar" content="no"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" class="bodyStyle">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<TR>
					<!-- marge de gauche-->
					<TD vAlign="top" width="10">
						<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" /></TD>
							</TR>
							<TR>
								<TD class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></TD>
							</TR>
						</TABLE>
					</TD>
					<!-- menu -->
					<TD vAlign="top" width="185" class="imageBackGround">
						<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TR vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<TD><asp:Image runat="server" height="90" SkinID="logo" /></TD>
							</TR>
							<TR>
								<TD><asp:Image runat="server" height="5" SkinID="plus_sous_logo" /></TD>
							</TR>
							<TR>
								<TD><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></TD>
							</TR>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle">
									<cc2:ModuleTitleWebControl id="ModuleTitleWebControl1" runat="server" CodeDescription="1064"></cc2:ModuleTitleWebControl>
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
						</TABLE>
					</TD>
					<!-- Séparateur -->
					<TD vAlign="top">
						<TABLE id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><asp:Image runat="server" height="100" width="5" SkinID="logo_cote_droit" /></TD>
							</TR>
							<TR>
								<TD class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD class="violetBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></TD>
					<TD width="10" class="imageBackGround">&nbsp;</TD>
					<TD width="10" class="whiteBackGround">&nbsp;</TD>
					<!-- droite-->
					<TD id="sertARien" vAlign="top">
						<TABLE id="Table10" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<!-- Menu du haut-->
								<TD vAlign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></TD>
							</TR>
							<TR>
								<!-- ligne du haut a droite -->
								<TD id="lineAVIrer" vAlign="top" class="imageBackGround" height="1%">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR>
											<TD vAlign="top" align="left" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
											<TD height="1%"><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></TD>
											<TD vAlign="top" align="right" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<!-- Centre -->
							<TR valign="top">
								<TD align="center">
									<TABLE id="data" height="100%" cellPadding="3" cellspacing="0" width="100%" class="imageBackGround" border="0">
										<TR valign="top" class="whiteBackGround">
											<TD width="10">&nbsp;</TD>
											<TD>
												<table cellpadding="0" cellspacing="1" width="90%" class="violetBackGround">
													<tr>
														<td class="txtViolet11Bold whiteBackGround">&nbsp;<cc1:AdExpressText language="33" id="titreAdexpresstext" runat="server" Code="1065"></cc1:AdExpressText>
														</td>
													</tr>
													<tr>
														<td>
															<TABLE id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0" class="paleVioletBackGround">
																<TR valign="top">
																	<TD>
																		<cc3:OneVehicleSelectionWebControl id="OneVehicleSelectionWebControl1" runat="server"></cc3:OneVehicleSelectionWebControl></TD>
																</TR>
															</TABLE>
														</td>
													</tr>
												</table>
											</TD>
										</TR>
										<tr valign="top" class="whiteBackGround" align="right">
											<td colspan="2"><cc4:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol>
											<asp:Image runat="server" width="100" height="1" SkinID="pixel" /></td>
										</tr>
										<tr valign="top" height="100%">
											<td colspan="2">&nbsp;
												<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
										</tr>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
					<!-- la fin -->
					<TD></TD>
				</TR>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD valign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD id="dellCel" valign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</FORM>
	</body>
</HTML>
