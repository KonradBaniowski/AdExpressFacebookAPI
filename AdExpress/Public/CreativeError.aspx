<%@ Page Language="C#" AutoEventWireup="false" CodeFile="CreativeError.aspx.cs" Inherits="AdExpress.CreativeError" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>

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
			</TABLE>
		</FORM>
	</body>
</HTML>
