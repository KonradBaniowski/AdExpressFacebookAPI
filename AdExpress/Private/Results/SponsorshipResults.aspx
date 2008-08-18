<%@ Page language="c#" Inherits="AdExpress.Private.Results.SponsorshipResults" CodeFile="SponsorshipResults.aspx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc6" Namespace="TNS.FrameWork.WebResultUI" Assembly="TNS.FrameWork.WebResultUI" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body onclick="<%=scriptBody%>" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td valign="top">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" width="10" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" width="1%" class="imageBackGround">
						<table id="tableMenu" cellspacing="0" cellpadding="0" width="10" border="0">
							
								<tr valign="top" class="whiteBackGround">
									<!-- Logo -->
									<td class="logoCoteDroitBackGround"><asp:Image runat="server" height=90 SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" width="100%" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" SkinID="pixel" width="1" /></td>
								</tr>
								<tr valign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
				<!--</tr>-->
				<tr valign="top">
					<td><asp:Image runat="server" height="10" SkinID="pixel" width="1" /></td>
				</tr>
				<tr valign="top">
					<td valign="top"><cc2:resultsoptionswebcontrol SkinID="resultsOptionsASDynamicTables" id="ResultsOptionsWebControl1" runat="server" PercentageTypeOption="True" ShowPictures="False"
							InsertOption="False" Percentage="False" AutoPostBackOption="False" ResultOption="False" PreformatedTableOption="False"
							Width="120px" ImageHeight="26" ImageWidth="139"></cc2:resultsoptionswebcontrol></td>
				</tr>
				<!--Niveaux Produits-->
				<tr class="whiteBackGround" height="3">
					<td></td>
				</tr>
				<tr class="whiteBackGround">
					<td><cc2:genericmedialeveldetailselectionwebcontrol SkinID="genericmedialeveldetailselectionwebcontrol" id="_genericMediaLevelDetailSelectionWebControl" runat="server" Width="200px" NbDetailLevelItemList="4"
							SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"
							GenericDetailLevelComponentProfile="media" GenericDetailLevelType="devicesAnalysis"></cc2:genericmedialeveldetailselectionwebcontrol></td>
				</tr>
				<tr class="whiteBackGround" height="5">
					<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="False" InitializeProduct="True"></cc2:initializeproductwebcontrol></td>
				</tr>
				<tr class="whiteBackGround" height="5">
					<td><cc2:InitializeMediaWebControl id="InitializeMediaWebcontrol1" runat="server" AutoPostBackOption="False" InitializeMedia="True"></cc2:InitializeMediaWebControl></td>
				</tr>
				<!--Fin Niveaux Produits-->
				<!--Debut Type de tableau-->
				<tr valign="top">
					<td valign="top"><cc2:resultstabletypeswebcontrol id="ResultsTableTypesWebControl1" runat="server" ShowPictures="True" AutoPostBackOption="False"
							Width="120px" SkinID = "ResultsTableTypesWebControl1" ImageHeight="26" ImageWidth="139"></cc2:resultstabletypeswebcontrol></td>
				</tr>
				<!--Fin Type de tableau-->
				<tr class="whiteBackGround">
					<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="okButton"></cc1:imagebuttonrolloverwebcontrol></td>
				</tr>
				<tr class="whiteBackGround" height="5">
					<td></td>
				</tr>
				<tr valign="top">
					<td id="premeriEspece"><asp:Image runat="server" height="10" SkinID="pixel" width="1" /></td>
				</tr>
				<tr valign="top">
					<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server" Visible="False"></cc2:modulebridgewebcontrol></td>
				</tr>
				<!-- Info bt droit -->
				<tr>
					<td><asp:Image runat="server" height="5" SkinID="pixel" width="1" /></td>
				</tr>
				<tr>
					<td><cc2:informationwebcontrol id="InformationWebControl1" runat="server"></cc2:informationwebcontrol></td>
				</tr>
			</table>
			</td> 
			<!-- Séparateur -->
			<td valign="top">
				<table id="Table5" cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5" /></td>
					</tr>
					<tr>
						<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
					</tr>
				</table>
			</td>
			<td bgColor="#644883"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
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
									<td valign="top" align="left"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
									<td><asp:Image runat="server" height="17" SkinID="pixel" width="1" /></td>
									<td valign="top" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
								</tr>
							</table>
						</td>
					</tr>
					<!-- Centre -->
					<tr>
						<td>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr valign="top">
									<td></td>
								<tr>
								<tr>
									<td><asp:Image runat="server" height="10" src="images/pixel.gif" width="1" /></td>
								</tr>
								<tr>
									<td style="HEIGHT: 5px" class="whiteBackGround"></td>
								</tr>
								<tr class="whiteBackGround">
									<!-- Test Tableau de Résultat -->
									<td style="HEIGHT: 37px" class="whiteBackGround"><br>
										<cc5:resultwebcontrol id="resultwebcontrol1" runat="server" SkinID="resultwebcontrol1" JavascriptFilePath="/scripts/WebResult.js" ></cc5:resultwebcontrol></td>
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
			<tr height="5">
				<td></td>
				<td valign="top" class="imageBackGround">
					<TABLE id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
							<td></td>
							<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
						</tr>
					</TABLE>
				</td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
				<td id="dellCel" valign="top" class="imageBackGround">
					<TABLE id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
							<td><cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol></td>
							<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
						</tr>
					</TABLE>
				</td>
			</tr>
			</TABLE></form>
		<%=divClose%>
	</body>
</HTML>
