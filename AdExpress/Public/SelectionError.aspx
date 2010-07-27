<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" CodeFile="SelectionError.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.SelectionError" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
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
			    <!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<TR>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image8" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="backGroundBlack">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							
							
							<!-- titre et descriptif-->
							<tr>
								<td>
								    <table id="Table6" cellspacing="0" cellpadding="0" width="185" border="0">
										<tr>
											<td class="backGroundBlack"><asp:Image ID="Image1" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										<TR>
										    <%if(!excel){ %>
											<TD class="whiteBackGround">
											    <A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="/Private/selectionModule.aspx?idSession=<%=idSession%>" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></A>
											</TD>
											<%}%>
											<%if(excel){ %>
											<TD class="whiteBackGround">
											    <A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>
											</TD>
											<%}%>
										</TR>
									</table>
								</td>
							</tr>
						</TABLE>
					</TD>
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image4" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image5" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
                        <table cellSpacing="0" cellPadding="0" border="0">
                            <!-- Menu haut -->
                            <tr>
                                <td style="background-color:Black;">
									<OBJECT codeBase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"
										height="60" width="648" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" VIEWASTEXT>
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
											type="application/x-shockwave-flash" width="648" height="60"> </embed>
									</OBJECT>
								</td>
							</TR>
							<tr class="headerBackGround">
							    <td height="58" ></td>
							</tr>

							<!-- Centre -->
							<TR>
								<TD>
									<TABLE id="data" height="600" cellPadding="0" width="100%" align="center" border="0">
										<TR>
											<TD vAlign="top" align="center">
												<TABLE id="Table11" cellSpacing="0" cellPadding="0" width="410" border="0" height="200">
													<TR>
														<!-- fleche -->
														<TD style="WIDTH: 15px" vAlign="top" rowSpan="2"><asp:Image ID="Image13" runat="server" SkinID="fleche_1" height="14" /></TD>
														<!-- message -->
														<TD vAlign="top" width="410">
															<TABLE id="tableDetails" cellSpacing="5" border="0">
																<TR>
																	<TD vAlign="top" noWrap width="100"><asp:Image ID="Image14" runat="server" SkinID="bande" width="400" /></TD>
																</TR>
																<tr>
																	<td class="txtGris11Bold"><asp:Label id="msg" runat="server"></asp:Label></td>
																</tr>
																<tr>
																	<td height="20">
																	</td>
																</tr>
															</TABLE>
															<table class="selectionErrorBorder" width="98%" align="center">
																<tr>
																	<td width=5%>
																		<asp:Image ID="Image19" runat="server" SkinID="help" />
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
			</TABLE>
		</FORM>
	</body>
</HTML>
