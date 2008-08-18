<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.CompetitorMediaPlanResults" CodeFile="CompetitorMediaPlanResults.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<%=advertiserScript%>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body>
		<form id="Form2" method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="/images/Common/logo_cote_gauche.gif" width="10"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" width="1%" background="/images/Common/dupli_fond.gif">
						<table id="tableMenu" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TBODY>
								<tr vAlign="top" bgColor="#ffffff">
									<!-- Logo -->
									<td><IMG height=90 src="/images/<%=_siteLanguage%>/logo/logo.gif" width=185 ></td>
								</tr>
								<tr>
									<td><IMG height="5" src="/images/Common/plus_sous_logo.gif" width="185"></td>
								</tr>
								<tr>
									<td><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
								<tr vAlign="top">
									<td><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr vAlign="top">
									<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" ResultOption="False"></cc2:resultsoptionswebcontrol></TD>
								</tr>
								<tr vAlign="top">
									<td id="premeriEspece"><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server" Visible="False"></cc2:modulebridgewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr>
									<td>
										<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
								</tr>
							</TBODY>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="/images/Common/logo_cote_droit.gif" width="5"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
					<td width="10" background="/images/Common/dupli_fond.gif">&nbsp;</td>
					<td width="10" bgColor="#ffffff">&nbsp;</td>
					<!-- droite-->
					<td vAlign="top" background="/images/Common/dupli_fond.gif">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td>
									<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left"><IMG height="5" src="/images/Common/croix.gif" width="5"></td>
											<td><IMG height="17" src="/images/Common/pixel.gif" width="1"></td>
											<td vAlign="top" align="right"><IMG height="5" src="/images/Common/croix.gif" width="5"></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
									<table cellSpacing="0" cellPadding="0" border="0" width="100%">
										<!--	<tr>
											<td class="txtGris11Bold" bgColor="#ffffff">
												<p style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><cc3:adexpresstext language="33" id="mainDescAdexpresstext" runat="server" Code="901"></cc3:adexpresstext></p>
											</td>
										</tr>
										<tr>
											<td><IMG height="10" src="/images/Common/pixel.gif"></td>
										</tr>-->
										<!--	<tr vAlign="top">
											<td bgColor="#ffffff">
												debut tableau des annonceurs sélectionnés
												<table>
													<tr vAlign="top">
														<td>
															<table style="BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid"
																cellSpacing="0" cellPadding="0" width="200" border="0">
																<tr>
																	<td class="txtViolet11Bold">&nbsp;<cc3:adexpresstext language="33" id="AdExpressText1" runat="server" Code="855"></cc3:adexpresstext>&nbsp;
																	</td>
																	<td align="right"><IMG style="CURSOR: hand" onclick="showHideContent2('listAdvertiser');" src="/Images/Common/button/bt_arrow_down.gif"
																			align="absMiddle">
																	</td>
																</tr>
															</table>
															<div id="listAdvertiserContent2" style="BORDER-RIGHT: #644883 1px solid; DISPLAY: none; BORDER-LEFT: #644883 1px solid; WIDTH: 620px; BORDER-BOTTOM: #644883 1px solid">
																<table cellSpacing="0" cellPadding="0" width="100%" align="center" bgColor="#ffffff" style="BORDER-TOP: #644883 1px solid">
																	<!--	<tr>
																		
																		<td style="BORDER-TOP: #644883 1px solid" ><IMG height="1" src="/images/Common/pixel.gif"></td>
																		
																	</tr>
																	<tr class="txtGris10">
																		<td style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px"
																			height="5"><asp:linkbutton id="firstSubSelectionLinkButton" runat="server" CssClass="roll02"></asp:linkbutton>&nbsp;
																		</td>
																	</tr>
																	<cc4:advertiserselectionwebcontrol id="AdvertiserSelectionWebControl1" runat="server"></cc4:advertiserselectionwebcontrol>
																	<tr>
																		<td style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 5px; PADDING-TOP: 2px; TEXT-ALIGN: right"><cc1:imagebuttonrolloverwebcontrol id="validSubSelectionImageButtonRollOverWebControl" runat="server"></cc1:imagebuttonrolloverwebcontrol></td>
																	</tr>
																</table>
															</div>
														</td>
													</tr>
												</table>
												fin tableau des annonceurs sélectionnés</td>
										</tr>-->
										<tr>
											<td><IMG height="10" src="images/pixel.gif" width="1"></td>
										</tr>
										<tr>
											<td bgColor="#ffffff" height="5"></td>
										</tr>
										<tr>
											<!-- Tableau de Résultat -->
											<td style="HEIGHT: 19px" bgColor="#ffffff"><%=result%></td>
										</tr>
										<tr>
											<td bgColor="#ffffff" height="5"></td>
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
				<TR height="5">
					<TD></TD>
					<TD vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD id="dellCel" vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD>
									<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
