<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.AEPMWaveSelection" CodeFile="AEPMWaveSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta HTTP-EQUIV="imagetoolbar" CONTENT="no">
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
							<!-- �l�ments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle">
                                    <cc2:ModuleTitleWebControl ID="ModuleTitleWebControl1" runat="server" CodeDescription="1651"/>
								</td>
							</tr>
							<TR>
								<TD><IMG height="10" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
							<TR>
								<TD>
									<cc2:RecallWebControl id="recallWebControl" runat="server"></cc2:RecallWebControl>
								</TD>
							</TR>
							<!-- Help -->
							<TR>
								<TD><IMG height="10" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
							<tr>
								<td>
									<cc2:HelpWebControl id="helpWebControl" runat="server"></cc2:HelpWebControl>
								</td>
							</tr>
						</TABLE>
					</TD>
					<!-- S�parateur -->
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
										<TR valign="top" bgcolor="#ffffff">
											<TD width="10">&nbsp;</TD>
											<TD>
												<!--<table cellpadding="0" cellspacing="1" bgColor="#644883" width="90%">
													<tr>
														<td bgcolor="#ffffff" class="txtViolet11Bold">&nbsp;<cc1:AdExpressText language="33" id="titreAdexpresstext" runat="server" Code="1650"></cc1:AdExpressText>
														</td>
													</tr>
													<tr>
														<td>-->
															<!--<TABLE id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0" bgcolor="#ded8e5">
																<TR valign="top">
																	<TD>-->
																		<cc3:WaveSelectionWebControl id="waveSelectionWebControl1" runat="server" CssClass="txtViolet11Bold"></cc3:WaveSelectionWebControl>																		
																		<!--</TD>
																</TR>
															</TABLE>-->
														<!--</td>
													</tr>
												</table>-->
											</TD>
										</TR>
										<tr valign="top" bgColor="#ffffff" align="right">
											<td colspan="2"><cc4:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click"></cc4:imagebuttonrolloverwebcontrol><img src="/Images/Common/pixel.gif" width="100" height="1"></td>
										</tr>
										<tr valign="top" height="100%">
											<td colspan="2"><img src="/Images/Common/pixel.gif" width="100%" height="100%"></td>
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
								<TD></TD>
								<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>			
		</FORM>
	</body>
</HTML>