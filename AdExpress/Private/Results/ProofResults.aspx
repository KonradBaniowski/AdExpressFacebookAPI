<%@ Page language="c#" Inherits="AdExpress.Private.Results.ProofResults" CodeFile="ProofResults.aspx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="dcwc" Namespace="TNS.AdExpress.Web.UI.Results.APPM" Assembly="TNS.AdExpress.Web" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc6" Namespace="TNS.AdExpress.Web.Controls.Results.Appm" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>		
	</head>
	
	<body class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
				
					<tr>
						<!-- marge de gauche-->
						<td valign="top">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" width="10"/></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td valign="top" width="1%" class="imageBackGround">
							<table id="tableMenu" cellspacing="0" cellpadding="0" width="10" border="0">
							
									<tr valign="top" class="whiteBackGround">
										<!-- Logo -->
										<td class="logoCoteDroitBackGround"><asp:Image runat="server" height="90" SkinID="logo" /></td>
									</tr>
									<tr>
										<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" width="100%"/></td>
									</tr>
									<tr>
										<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
									</tr>
									<tr valign="top">
										<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
									</tr>
					<!--</tr>--><!--2005MI-->
					<tr valign="top">
						<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
					</tr>
					<tr valign="top" class="whiteBackGround">
						<td id="premeriEspece"><asp:Image runat="server" height="10" SkinID="pixel" width="1"/>
						</td>
					</tr>
					<tr class="whiteBackGround">
						<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="true" InitializeSlogans="True"
								InitializeProduct="False"></cc2:initializeproductwebcontrol></td>
					</tr>

					
				
					<!-- Info bt droit -->
					<tr>
						<td><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
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
						<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5"/></td>
					</tr>
					<tr>
						<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
					</tr>
				</table>
			</td>
			<td class="violetBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
			<td width="10" class="imageBackGround">&nbsp;</td>
			<td width="10" class="whiteBackGround">&nbsp;</td>
			<!-- droite-->
			<td valign="top" class="imageBackGround">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<!-- Menu du haut-->
						<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
					</tr>
					<tr>
						<!-- ligne du haut a droite -->
						<td>
							<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="top" align="left"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
									<td><asp:Image runat="server" height="17" SkinID="pixel" width="1"/></td>
									<td valign="top" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								</tr>
							</table>
						</td>
					</tr>
					<!-- Centre -->
					<tr>
						<td>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td><asp:Image runat="server" height="10" SkinID="pixel"/></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
								</tr>
								<tr>
									<td style="HEIGHT: 5px" class="whiteBackGround"></td>
								</tr>
								<tr>
									<td align="center" class="whiteBackGround">
										<!--RESULTAT -->
										<cc5:resultwebcontrol id="_resultWebControl" runat="server" JavascriptFilePath="/scripts/WebResult.js" SkinID="resultwebcontrol1"></cc5:resultwebcontrol>
									</td>
								</tr>
								<tr>
									<td class="whiteBackGround" height="5"></td>
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
			<tr style="height:5px">
				<td></td>
				<td valign="top" class="imageBackGround">
					<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td valign="top"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							<td></td>
							<td valign="top" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
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
							<td valign="top"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							<td>
								<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
							<td valign="top" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
						</tr>
					</table>
				</td>
			</tr>
			</table></form>
		<!--<P></P>
		</tr></TBODY></TABLE></tr></TBODY></TABLE></FORM>--><!--2005MI-->
		<%=divClose%>
	</body>
</html>
