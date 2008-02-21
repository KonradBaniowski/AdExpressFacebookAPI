<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" CodeFile="SelectionError.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.SelectionError" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">

		<meta http-equiv="expires" content="0">

		<meta http-equiv="pragma" content="no-cache">

		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="800" border="0">
				<TR>
					<!-- marge de gauche-->
					<TD vAlign="top">
						<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG height="100" src="/Images/Common/logo_cote_gauche.gif" width="10"></TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
						</TABLE>
					</TD>
					<!-- menu -->
					<TD vAlign="top" align="center" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TR vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<TD><IMG height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" width=185 ></TD>
							</TR>
							<TR>
								<TD><IMG height="5" src="/Images/Common/plus_sous_logo.gif" width="185"></TD>
							</TR>
							<TR>
								<TD><IMG height="10" src="/Images/pixel.gif" width="1"></TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff"><IMG height="5" src="/Images/pixel.gif" width="1"></TD>
							</TR>
							<!-- titre et descriptif-->
							<TR>
								<TD bgColor="#ffffff">
									<TABLE id="Table4" cellSpacing="0" cellPadding="0" border="0">
										<TR>
											<TD><IMG height="2" src="/Images/Common/pixel.gif" width="5"></TD>
										</TR>
										<TR>
										<%if(!excel){ %>
											<TD bgColor="#ffffff"><A onmouseover="bouton.src='/Images/<%=_siteLanguage%>/button/back_down.gif';" onmouseout="bouton.src = '/Images/<%=_siteLanguage%>/button/back_up.gif';" href="/Private/selectionModule.aspx?idSession=<%=idSession%>" ><IMG src="/Images/<%=_siteLanguage%>/button/back_up.gif" border=0 name=bouton >
												</A>
											</TD>
											<%}%>
											<%if(excel){ %>
											<TD bgColor="#ffffff"><A onmouseover="bouton.src='/Images/<%=_siteLanguage%>/button/fermer_down.gif';" onmouseout="bouton.src = '/Images/<%=_siteLanguage%>/button/fermer_up.gif';" ><IMG  STYLE="CURSOR:hand" src="/Images/<%=_siteLanguage%>/button/fermer_up.gif" onclick=javascript:window.close(); border=0 name=bouton >
												</A>
											</TD>
											<%}%>
											
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
					<!-- Séparateur -->
					<TD vAlign="top">
						<TABLE id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG id="imageDrt" height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD bgColor="#644883"><IMG height="1" src="/Images/Common/pixel.gif" width="1">
					</TD>
					<TD width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</TD>
					<TD width="10" bgColor="#ffffff">&nbsp;</TD>
					<!-- droite-->
					<TD vAlign="top">
						<TABLE id="Table10" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<td>
									<!-- Menu du haut-->
									<table class="header" height="31" cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<TD>
												<OBJECT codeBase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"
													height="31" width="733" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" VIEWASTEXT>
													<PARAM NAME="_cx" VALUE="19394">
													<PARAM NAME="_cy" VALUE="820">
													<PARAM NAME="FlashVars" VALUE="">
													<PARAM NAME="Movie" VALUE="/Flash/Common/bandeau_violet.swf">
													<PARAM NAME="Src" VALUE="/Flash/Common/bandeau_violet.swf">
													<PARAM NAME="WMode" VALUE="Window">
													<PARAM NAME="Play" VALUE="-1">
													<PARAM NAME="Loop" VALUE="-1">
													<PARAM NAME="Quality" VALUE="High">
													<PARAM NAME="SAlign" VALUE="">
													<PARAM NAME="Menu" VALUE="0">
													<PARAM NAME="Base" VALUE="">
													<PARAM NAME="AllowScriptAccess" VALUE="always">
													<PARAM NAME="Scale" VALUE="ShowAll">
													<PARAM NAME="DeviceFont" VALUE="0">
													<PARAM NAME="EmbedMovie" VALUE="0">
													<PARAM NAME="BGColor" VALUE="">
													<PARAM NAME="SWRemote" VALUE="">
													<PARAM NAME="MovieData" VALUE="">
													<PARAM NAME="SeamlessTabbing" VALUE="1">
													<embed src="/Flash/Common/bandeau_violet.swf" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer"
														type="application/x-shockwave-flash" width="733" height="31"> </embed>
												</OBJECT>
											</TD>
											<td></td>
										</tr>
									</table>
								</td>
							</TR>
							<TR>
								<!-- ligne du haut a droite -->
								<TD background="/Images/Common/dupli_3bis.gif">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR>
											<TD vAlign="top" align="left"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
											<TD><IMG height="17" src="/Images/Common/pixel.gif" width="1"></TD>
											<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<!-- Centre -->
							<TR>
								<TD id="sertARien">
									<TABLE id="data" style="BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x"
										height="600" cellPadding="0" width="100%" align="center" border="0">
										<TR>
											<TD vAlign="top" align="center">
												<TABLE id="Table11" cellSpacing="0" cellPadding="0" width="410" border="0" height="200">
													<TR>
														<!-- fleche -->
														<TD style="WIDTH: 15px" vAlign="top" rowSpan="2"><IMG id="f1" height="14" src="/Images/Common/fleche_1.gif" border="0"></TD>
														<!-- message -->
														<TD vAlign="top" width="410" bgColor="#ffffff">
															<TABLE id="tableDetails" cellSpacing="5" border="0">
																<TR>
																	<TD vAlign="top" noWrap width="100"><IMG src="/Images/Common/bande.gif" width="400">
																	</TD>
																</TR>
																<tr>
																	<td class="txtGris11Bold"><asp:Label id="msg" runat="server"></asp:Label></td>
																</tr>
																<tr>
																	<td height="20">
																	</td>
																</tr>
															</TABLE>
															<table style="BORDER-RIGHT: #8872a0 thin solid; BORDER-TOP: #8872a0 thin solid; BORDER-LEFT: #8872a0 thin solid; BORDER-BOTTOM: #8872a0 thin solid" width="98%" align="center">
																<tr>
																	<td width=5%>
																		<img src="/Images/Common/button/help_up.gif">
																		
																	</td>
																	<td class="txtViolet11Bold" width=95%>
																	<cc1:AdExpressText language="33" id="titleHelpAdExpressText" runat="server" Code="0"></cc1:AdExpressText>
																	</td>
																</tr>
																<tr>
																	<td class="txtGris11Bold" colspan=2 style="TEXT-JUSTIFY: auto; TEXT-ALIGN: justify">
																		<cc1:AdExpressText language="33" id="helpAdExpressText" runat="server" Code="0"></cc1:AdExpressText>
																	</td>
																</tr>
															</table>
														</TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
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
					<TD id="dellCel" valign="top">
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
