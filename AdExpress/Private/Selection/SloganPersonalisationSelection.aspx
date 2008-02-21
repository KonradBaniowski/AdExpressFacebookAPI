<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.SloganPersonalisationSelection" CodeFile="SloganPersonalisationSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				
					<TR>
						<!-- marge de gauche-->
						<TD vAlign="top" width="10">
							<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
								<TR>
									<TD><IMG height="100" src="/Images/Common/logo_cote_gauche.gif"></TD>
								</TR>
								<TR>
									<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
								</TR>
							</TABLE>
						</TD>
						<!-- menu -->
						<TD vAlign="top" width="185" background="/Images/Common/dupli_fond.gif">
							<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
								<TR vAlign="top" bgColor="#ffffff">
									<!-- Logo -->
									<TD><IMG height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" ></TD>
								</TR>
								<TR>
									<TD><IMG height="5" src="/Images/Common/plus_sous_logo.gif"></TD>
								</TR>
								<TR>
									<TD><IMG height="10" src="/Images/Common/pixel.gif" width="1"></TD>
								</TR>
								<!-- éléments variables du menus de gauche -->
								<tr vAlign="top">
									<td id="pageTitle">
										<cc2:ModuleTitleWebControl id="ModuleTitleWebControl1" runat="server" CodeDescription="1925"></cc2:ModuleTitleWebControl>
									</td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
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
									<TD><IMG height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></TD>
								</TR>
								<TR>
									<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
								</TR>
							</TABLE>
						</TD>
						<TD bgColor="#644883"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
						<TD width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</TD>
						<TD width="10" bgColor="#ffffff">&nbsp;</TD>
						<!-- droite-->
						<TD id="sertARien" vAlign="top">
							<TABLE id="Table10" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TBODY>
									<TR>
										<!-- Menu du haut-->
										<TD vAlign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></TD>
									</TR>
									<TR>
										<!-- ligne du haut a droite -->
										<TD id="lineAVIrer" vAlign="top" background="/Images/Common/dupli_fond.gif" height="1%">
											<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
												<TR>
													<TD vAlign="top" align="left" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
													<TD height="1%"><IMG height="17" src="/Images/Common/pixel.gif" width="1"></TD>
													<TD vAlign="top" align="right" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<!-- Centre -->
									<TR valign="top">
										<TD align="center">
											<TABLE id="data" height="100%" cellPadding="3" cellspacing="0" width="100%" background="/Images/Common/dupli_fond.gif"
												border="0">
												<tr valign="top" height="5" bgcolor="#ffffff">
													<td></td>
												</tr>
												<!--<tr style="HEIGHT: 10px" valign="top">
													<td width="478" background="/images/Common/dupli_fond.gif" colSpan="6" style="WIDTH: 478px"></td>
												</tr>-->
												<!--fin chargement univers-->
												<!--Debut code généré-->
												<tr bgcolor="#ffffff" valign="top">
													<TD>
														<table cellSpacing="0" cellPadding="0" width="90%" bgColor="#644883" align="center">
															<tr>
																<td>
																	<TABLE id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" bgColor="#ffffff"
																		border="0">
																		<!--Debut code généré personnalisation des versions-->
																		<cc3:SloganPersonalisationWebControl id="SloganPersonalisationWebControl1" runat="server" CssClass="txtViolet11Bold"></cc3:SloganPersonalisationWebControl>
																		<!--fin code généré personnalisation des versions-->
																	</TABLE>
																</td>
															</tr>
														</table>
													</TD>
												</tr>
												<!--fin code généré-->
												<tr valign="top" bgcolor="#ffffff">
													<td>&nbsp;</td>
												</tr>
												<tr height="100%" valign="top">
													<td background="/Images/Common/dupli_fond.gif" align="right">
														<cc4:ImageButtonRollOverWebControl id="validateButton" runat="server" onclick="validateButton_Click"></cc4:ImageButtonRollOverWebControl>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
												</tr>
											</TABLE>
										</TD>
									</TR>
								</TBODY>
							</TABLE>
						</TD>
						<!-- la fin -->
						<TD></TD>
					</TR>
					<!-- ligne du bas a droite -->
					<TR height="5">
						<TD></TD>
						<TD valign="top" background="/Images/Common/dupli_fond.gif">
							<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TR>
									<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
									<TD></TD>
									<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								</TR>
							</TABLE>
						</TD>
						<TD></TD>
						<TD></TD>
						<TD></TD>
						<TD></TD>
						<TD id="dellCel" valign="top" background="/Images/Common/dupli_fond.gif">
							<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TR>
									<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
									<TD>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
									<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				
			</TABLE>
			<!--</TR></TBODY></TABLE></TR></TBODY></TABLE></TR></TBODY></TABLE>-->
		</FORM>
	</body>
</HTML>
