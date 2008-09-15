<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" CodeFile="Message.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Message" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript:activateActiveX();">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="800" border="0">
				<TR>
					<!-- marge de gauche-->
					<TD vAlign="top">
						<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><asp:Image ID="Image153" runat="server" SkinID="logo_cote_gauche" height="100" width="10" /></TD>
							</TR>
							<TR>
								<TD class="whiteBackGround"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="1" width="1" /></TD>
							</TR>
						</TABLE>
					</TD>
					<!-- menu -->
					<TD vAlign="top" align="center" class="imageBackGround">
						<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TR vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<TD><asp:Image ID="Image8" runat="server" SkinID="logo" height="90" width="185" /></TD>
							</TR>
							<TR>
								<TD><asp:Image ID="Image9" runat="server" SkinID="plus_sous_logo" height="5" width="185" /></TD>
							</TR>
							<TR>
								<TD><asp:Image ID="Image2" runat="server" SkinID="pixel" height="10" width="1" /></TD>
							</TR>
							<TR>
								<TD class="whiteBackGround"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="5" width="1" /></TD>
							</TR>
							<!-- titre et descriptif-->
							<TR>
								<TD class="whiteBackGround">
									<TABLE id="Table4" cellSpacing="0" cellPadding="0" border="0">
										<TR>
											<TD><asp:Image ID="Image4" runat="server" SkinID="pixel" height="2" width="5" /></TD>
										</TR>
										<TR>
											<TD class="whiteBackGround">
											<A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back(-<%=backPageNb%>);" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></A>
											</TD>
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
								<TD><asp:Image ID="Image5" runat="server" SkinID="logo_cote_droit" height="100" width="5" /></TD>
							</TR>
							<TR>
								<TD class="whiteBackGround"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="1" width="1" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD bgColor="#644883"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="1" width="1" />
					</TD>
					<TD width="10" class="imageBackGround">&nbsp;</TD>
					<TD width="10" class="whiteBackGround">&nbsp;</TD>
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
													<PARAM NAME="Movie" VALUE="/App_Themes/<%= this.Theme %>/Flash/Common/HeaderStrip.swf">
													<PARAM NAME="Src" VALUE="/App_Themes/<%= this.Theme %>/Flash/Common/HeaderStrip.swf">
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
													<embed src="/App_Themes/<%= this.Theme %>/Flash/Common/HeaderStrip.swf" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer"
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
								<TD class="imageBackGround3bis">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR>
											<TD vAlign="top" align="left"><asp:Image ID="Image10" runat="server" SkinID="croix" height="5" width="5" /></TD>
											<TD><asp:Image ID="Image12" runat="server" SkinID="pixel" height="17" width="1" /></TD>
											<TD vAlign="top" align="right"><asp:Image ID="Image11" runat="server" SkinID="croix" height="5" width="5" /></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<!-- Centre -->
							<TR>
								<TD id="sertARien">
									<TABLE id="data" class="darkBackGround" height="600" cellPadding="0" width="100%" align="center" border="0">
										<TR>
											<TD vAlign="top" align="center">
												<TABLE id="Table11" cellSpacing="0" cellPadding="0" width="410" border="0" height="200">
													<TR>
														<!-- fleche -->
														<TD style="WIDTH: 15px" vAlign="top" rowSpan="2"><asp:Image ID="Image13" runat="server" SkinID="fleche_1" height="14" /></TD>
														<!-- message -->
														<TD vAlign="top" width="410" class="whiteBackGround">
															<TABLE id="tableDetails" cellSpacing="5" border="0">
																<TR>
																	<TD vAlign="top" noWrap width="100"><asp:Image ID="Image14" runat="server" SkinID="bande" width="400" />
																	</TD>
																</TR>
																<tr>
																	<td class="txtGris11Bold"><asp:Label id="msg" runat="server"></asp:Label></td>
																</tr>
															</TABLE>
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
					<TD valign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image ID="Image15" runat="server" SkinID="croix" height="5" width="5" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image ID="Image16" runat="server" SkinID="croix" height="5" width="5" /></TD>
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
								<TD vAlign="top"><asp:Image ID="Image17" runat="server" SkinID="croix" height="5" width="5" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image ID="Image18" runat="server" SkinID="croix" height="5" width="5" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</FORM>
	</body>
</HTML>
