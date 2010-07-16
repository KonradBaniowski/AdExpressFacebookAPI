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
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body onclick="<%=scriptBody%>" class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				<!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="whiteBackGround logoCoteDroitBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									
									<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
										<tr>
											<td><asp:Image ID="Image3" runat="server" height="5" SkinID="plus_sous_logo" /></td>
										</tr>
										<tr>
											<td><asp:Image ID="Image4" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										
										<!-- éléments variables du menus de gauche -->
										<tr valign="top">
											<td id="pageTitle">
												<cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Options -->
										<tr valign="top">
											<td><asp:Image ID="Image5" runat="server" height="10" SkinID="pixel" width="1" /></td>
										</tr>
										<tr valign="top">
											<td valign="top"><cc2:resultsoptionswebcontrol SkinID="resultsOptionsASDynamicTables" id="ResultsOptionsWebControl1" runat="server" PercentageTypeOption="True" ShowPictures="False"
													InsertOption="False" Percentage="False" AutoPostBackOption="False" ResultOption="False" PreformatedTableOption="False"
													Width="120px" ImageHeight="26" ImageWidth="139"></cc2:resultsoptionswebcontrol></td>
										</tr>
										<!--Niveaux Produits-->
										<tr height="3">
											<td></td>
										</tr>
										<tr>
											<td><cc2:genericmedialeveldetailselectionwebcontrol SkinID="genericmedialeveldetailselectionwebcontrol" 
                                                    id="_genericMediaLevelDetailSelectionWebControl" runat="server" Width="200px" NbDetailLevelItemList="4"
													SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"
													GenericDetailLevelComponentProfile="media" GenericDetailLevelType="devicesAnalysis" BackGroundColor="" 
                                                    CssDefaultListLabel="txtBlanc11Bold"></cc2:genericmedialeveldetailselectionwebcontrol></td>
										</tr>
										<tr height="5">
											<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" 
                                                    runat="server" AutoPostBackOption="False" InitializeProduct="True" 
                                                    CommonCssClass="txtOrange11Bold"></cc2:initializeproductwebcontrol></td>
										</tr>
										<tr height="5">
											<td><cc2:InitializeMediaWebControl id="InitializeMediaWebcontrol1" runat="server" 
                                                    AutoPostBackOption="False" InitializeMedia="True" 
                                                    CommonCssClass="txtOrange11Bold"></cc2:InitializeMediaWebControl></td>
										</tr>
										<!--Fin Niveaux Produits-->
										
										<!--Debut Type de tableau-->
										<tr valign="top">
											<td valign="top"><cc2:resultstabletypeswebcontrol id="ResultsTableTypesWebControl1" runat="server" ShowPictures="True" AutoPostBackOption="False"
													Width="120px" SkinID = "ResultsTableTypesWebControl1" ImageHeight="26" ImageWidth="139"></cc2:resultstabletypeswebcontrol></td>
										</tr>
										<!--Fin Type de tableau-->
										
										<tr>
											<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="okButton"></cc1:imagebuttonrolloverwebcontrol></td>
										</tr>
										<tr height="5">
											<td></td>
										</tr>
										<tr valign="top">
											<td><asp:Image ID="Image6" runat="server" height="10" SkinID="pixel" width="1" /></td>
										</tr>
										<tr valign="top">
											<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server" Visible="False"></cc2:modulebridgewebcontrol></td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image7" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc2:informationwebcontrol id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:informationwebcontrol></td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image8" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image9" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image11" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image12" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
			        <!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
					        
					        <!-- Centre -->
					        <tr>
						        <td>
							        <table cellspacing="0" cellpadding="0" width="100%" border="0">
								        <tr>
									        <!-- Test Tableau de Résultat -->
									        <td><br>
										        <cc5:resultwebcontrol id="resultwebcontrol1" runat="server" SkinID="resultwebcontrol1" JavascriptFilePath="/scripts/WebResult4.js" ></cc5:resultwebcontrol></td>
								        </tr>
								        <tr>
									        <td height="5"><cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol></td>
								        </tr>
							        </table>
						        </td>
					        </tr>
				        </table>
			        </td>
                </tr> 
            </TABLE>
        </form>
		<%=divClose%>
	</body>
</HTML>
