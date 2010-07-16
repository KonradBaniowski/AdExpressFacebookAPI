
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.MediaPlanResults" CodeFile="MediaPlanResults.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" type="text/javascript" src="/scripts/WebResult4.js"></script>
		<script type="text/javascript" src="/scripts/dom-drag.js"></script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
		<asp:HiddenField ID="zoomParam" runat="server" EnableViewState="true"/>
			<table height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
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
											<td><asp:Image ID="Image5" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										<tr valign="top">
											<td valign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" AutoPostBackOption="False" ResultOption="False"
													MediaDetailOption="False"></cc2:resultsoptionswebcontrol></td>
										</tr>
										<tr valign="top" class="whiteBackGround">
											<td><cc2:PeriodDetailWebControl ID="PeriodDetailWebControl1" runat="server" SkinID="PeriodDetailWebControl">
																						</cc2:PeriodDetailWebControl></td>
										</tr>
										<tr valign="top" class="whiteBackGround">
											<td><asp:Image ID="Image6" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr valign="top">
											<td vAlign="top"><cc2:genericmedialeveldetailselectionwebcontrol id="GenericMediaLevelDetailSelectionWebControl1" runat="server" Width="200px" NbDetailLevelItemList="4"
													RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx" SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"></cc2:genericmedialeveldetailselectionwebcontrol></td>
										</tr>
										<tr  class="whiteBackGround">
											<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" 
                                                    runat="server" AutoPostBackOption="False" InitializeSlogans="True"
													InitializeProduct="False" CommonCssClass="txtOrange11Bold"></cc2:initializeproductwebcontrol></td>
										</tr>
										<tr class="whiteBackGround" height="5">
											<td><cc2:InitializeMediaWebControl id="InitializeMediaWebcontrol1" runat="server" 
                                                    AutoPostBackOption="False" InitializeMedia="True" 
                                                    CommonCssClass="txtOrange11Bold"></cc2:InitializeMediaWebControl></td>
										</tr>
										<tr valign="top" class="whiteBackGround">
											<td><asp:Image ID="Image7" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr vAlign="top">
											<td id="Espace"><asp:Image ID="Image8" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										<tr vAlign="top">
											<td align="left"><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="validateButton"></cc1:imagebuttonrolloverwebcontrol></td>
										</tr>
										<tr vAlign="top">
											<td id="premeriEspece"><asp:Image ID="Image9" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										<tr vAlign="top">
											<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server" Visible="False"></cc2:modulebridgewebcontrol></td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image10" runat="server" height="5" width="1" SkinID="pixel" /></td>
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
								<td><asp:Image ID="Image11" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image12" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image13" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image14" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image15" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
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
										<%=zoomButton%>
                                        <tr>
                                            <td><cc2:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td height="10"></td>
                                        </tr>
                                        <tr>
											<!-- Tableau de Résultat -->
											<td><cc5:genericmediaschedulewebcontrol id="GenericMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240" ShowVersion="True"></cc5:genericmediaschedulewebcontrol></td>
										</tr>
										<tr>
											<td><cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
