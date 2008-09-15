<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.ReferenceCompetitorPersonalisation" CodeFile="ReferenceCompetitorPersonalisation.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="<%=errorScript%>javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" height="600" cellspacing="0" cellpadding="0" width="800" border="0">
			
					<tr>
						<!-- marge de gauche-->
						<td valign="top" width="10">
							<table id="Table3" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td valign="top" width="185" class="imageBackGround">
							<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
								<tr valign="top" class="whiteBackGround">
									<!-- Logo -->
									<td><asp:Image runat="server" height="90" SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<!-- éléments variables du menus de gauche -->
								<tr valign="top">
									<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="1093"></cc2:moduletitlewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
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
									<td><asp:Image runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
								</tr>
							</table>
						</td>
						<td class="violetBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
						<td width="10" class="imageBackGround">&nbsp;</td>
						<td width="10" class="whiteBackGround">&nbsp;</td>
						<!-- droite-->
						<td id="sertARien" valign="top">
							<table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								
									<tr>
										<!-- Menu du haut-->
										<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
									</tr>
									<tr>
										<!-- ligne du haut a droite -->
										<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
											<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td valign="top" align="left" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
													<td height="1%"><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></td>
													<td valign="top" align="right" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
												</tr>
											</table>
										</td>
									</tr>
									<!-- Centre -->
									<tr valign="top">
										<td align="center">
											<table id="data" height="100%" cellspacing="0" cellpadding="3" width="100%" class="imageBackGround"
												border="0">
												<tr style="HEIGHT: 10px">
													<td style="WIDTH: 478px" width="478" class="imageBackGround" colSpan="6"></td>
												</tr>
												<!--fin chargement univers-->
												<!--Debut code généré--><cc3:recapadvertiserselectionwebcontrol id="recapAdvertiserSelectionWebControl" runat="server"></cc3:recapadvertiserselectionwebcontrol>
												<!--fin code généré-->
												<tr valign="top" class="whiteBackGround" height="1%">
													<td>&nbsp;</td>
												</tr>
												<tr valign="top" height="50">
													<td align="right" class="imageBackGround">&nbsp;
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
						<td valign="top" class="imageBackGround"><table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
									<td></td>
									<td valign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
								</tr>
							</table>
						</td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td id="dellCel" valign="top" class="imageBackGround">
							<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
									<td>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
									<td valign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
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
