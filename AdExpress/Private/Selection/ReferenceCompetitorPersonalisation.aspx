<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.ReferenceCompetitorPersonalisation" CodeFile="ReferenceCompetitorPersonalisation.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 transitional//EN" >
<html>
	<head>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="<%=errorScript%>">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" height="600" cellspacing="0" cellpadding="0" width="800" border="0">
			
					<tr>
						<!-- marge de gauche-->
						<td valign="top" width="10">
							<table id="Table3" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><img height="100" src="/Images/Common/logo_cote_gauche.gif"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td valign="top" width="185" background="/Images/Common/dupli_fond.gif">
							<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
								<tr valign="top" bgColor="#ffffff">
									<!-- Logo -->
									<td><img height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" ></td>
								</tr>
								<tr>
									<td><img height="5" src="/Images/Common/plus_sous_logo.gif"></td>
								</tr>
								<tr>
									<td><img height="10" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
								<!-- éléments variables du menus de gauche -->
								<tr valign="top">
									<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="1093"></cc2:moduletitlewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><img height="5" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr>
									<td>
										<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
								</tr>
							</table>
						</td>
						<!-- Séparateur -->
						<td valign="top">
							<table id="Table5" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><img height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
							</table>
						</td>
						<td bgColor="#644883"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
						<td width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</td>
						<td width="10" bgColor="#ffffff">&nbsp;</td>
						<!-- droite-->
						<td id="sertARien" valign="top">
							<table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								
									<tr>
										<!-- Menu du haut-->
										<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
									</tr>
									<tr>
										<!-- ligne du haut a droite -->
										<td id="lineAVIrer" valign="top" background="/Images/Common/dupli_fond.gif" height="1%">
											<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td valign="top" align="left" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
													<td height="1%"><img height="17" src="/Images/Common/pixel.gif" width="1"></td>
													<td valign="top" align="right" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
												</tr>
											</table>
										</td>
									</tr>
									<!-- Centre -->
									<tr valign="top">
										<td align="center">
											<table id="data" height="100%" cellspacing="0" cellpadding="3" width="100%" background="/Images/Common/dupli_fond.gif"
												border="0">
												<tr style="HEIGHT: 10px">
													<td style="WIDTH: 478px" width="478" background="/images/Common/dupli_fond.gif" colSpan="6"></td>
												</tr>
												<!--fin chargement univers-->
												<!--Debut code généré--><cc3:recapadvertiserselectionwebcontrol id="recapAdvertiserSelectionWebControl" runat="server"></cc3:recapadvertiserselectionwebcontrol>
												<!--fin code généré-->
												<tr valign="top" bgColor="#ffffff" height="1%">
													<td>&nbsp;</td>
												</tr>
												<tr valign="top" height="50">
													<td align="right" background="/Images/Common/dupli_fond.gif">&nbsp;
														<%=previousButton%>
														<cc4:imagebuttonrolloverwebcontrol id="validateButton" runat="server" CommandName="next" onclick="validateButton_Click"></cc4:imagebuttonrolloverwebcontrol></td>
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
					<tr height="5">
						<td></td>
						<td valign="top" background="/Images/Common/dupli_fond.gif"><table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
									<td></td>
									<td valign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
								</tr>
							</table>
						</td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td id="dellCel" valign="top" background="/Images/Common/dupli_fond.gif">
							<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
									<td>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
									<td valign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
								</tr>
							</table>
						</td>
					</tr>
				
			</table>
			<!--</tr></TBODY></table></tr></TBODY></table></tr></TBODY></table>--><!--2005MI-->
			</form>
		<%=divClose%>
	</body>
</html>
