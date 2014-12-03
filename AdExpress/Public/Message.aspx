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
		<form id="Form2" method="post" runat="server">
			<table id="Table1" cellSpacing="0" cellPadding="0" border="0">
			    <!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<tr>
					<!-- marge de gauche-->
					<td valign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
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
											<td class="backGroundBlack"><asp:Image ID="Image4" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td><a onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back(-<%=backPageNb%>);" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></a></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image5" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image6" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image7" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image8" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0" class="errorPageWidth">
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
							</tr>
							<tr class="headerBackGround">
							    <td height="58" ></td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
									<table id="data" height="600" cellPadding="0" width="100%" align="center" border="0">
										<tr>
											<td vAlign="top" align="center">
												<table id="Table11" cellSpacing="0" cellPadding="0" width="410" border="0" height="200">
													<tr>
														<!-- fleche -->
														<td style="WIDTH: 15px" vAlign="top" rowSpan="2"><asp:Image ID="Image9" runat="server" SkinID="fleche_1" height="14" /></TD>
														<!-- message -->
														<td vAlign="top" width="410">
															<table id="tableDetails" cellSpacing="5" border="0">
																<tr>
																	<td vAlign="top" noWrap width="100"><asp:Image ID="Image10" runat="server" SkinID="bande" width="400" /></td>
																</tr>
																<tr>
																	<td class="txtGris11Bold"><asp:Label id="msg" runat="server"></asp:Label></td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
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
