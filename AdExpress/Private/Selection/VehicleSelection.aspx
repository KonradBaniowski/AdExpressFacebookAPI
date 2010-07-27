<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.VehicleSelection" CodeFile="VehicleSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta HTTP-EQUIV="imagetoolbar" CONTENT="no"/>
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" class="bodyStyle" onload="javascript:activateActiveX();">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<TR>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="backGroundBlack">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									
									<table id="Table6" cellspacing="0" cellpadding="0" width="185" border="0">
										<tr>
											<td class="backGroundBlack"><asp:Image ID="Image4" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										
										<!-- éléments variables du menus de gauche -->
										<tr valign="top">
											<td id="pageTitle">
												<cc2:ModuleTitleWebControl id="ModuleTitleWebControl1" runat="server" CodeDescription="775"></cc2:ModuleTitleWebControl>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc2:InformationWebControl id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:InformationWebControl></td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image6" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image7" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image8" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<!-- Centre -->
							<TR valign="top">
								<TD align="center">
								
									<TABLE id="data" height="100%" cellPadding="3" cellspacing="0" width="100%" border="0">
										<TR valign="top">
											<TD width="10">&nbsp;</TD>
											<TD>
												<table cellpadding="0" cellspacing="1" class="violetBackGround" width="90%">
													<tr class="backGroundWhite">
														<td class="txtViolet11Bold">&nbsp;<cc1:AdExpressText language="33" id="titreAdexpresstext" runat="server" Code="1017"></cc1:AdExpressText></td>
													</tr>
													<tr>
														<td>
															<TABLE id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0"
																class="paleVioletBackGround">
																<TR valign="top">
																	<TD><a class="roll04" style="TEXT-DECORATION: none" href="javascript:selectAllVehicles();">
																			&nbsp;<cc1:AdExpressText language="33" id="AdExpressText2" runat="server" Code="944"></cc1:AdExpressText>
																		</a>
																	</TD>
																</TR>
																<TR valign="top">
																	<TD><cc3:vehicleselectionwebcontrol id="VehicleSelectionWebControl2" runat="server" CssClass="txtViolet11Bold"></cc3:vehicleselectionwebcontrol></TD>
																</TR>
															</TABLE>
														</td>
													</tr>
												</table>
											</TD>
										</TR>
										<tr valign="top" align="right">
											<td colspan="2"><cc4:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol><img src="/Images/Common/pixel.gif" width="100" height="1"></td>
										</tr>
										<tr valign="top" height="100%">
											<td colspan="2">&nbsp;<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
										</tr>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
					<!-- la fin -->
					<TD></TD>
				</TR>
			</TABLE>
		</FORM>
	</body>
</HTML>
