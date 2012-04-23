<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CelebritiesResults.aspx.cs" Inherits="AdExpress.Private.Results.CelebritiesResults" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
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
												<cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Options -->
										<tr valign="top">
											<td><asp:Image ID="Image5" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										<tr valign="top">
											<td valign="top">
                                                <cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" 
                                                    runat="server" AutoPostBackOption="False" ResultOption="False"
													MediaDetailOption="False" GenericDetailLevelComponentProfile="media" GenericDetailLevelType="mediaSchedule" 
                                                    GenericMediaLevelDetailSelectionOptions="True" NbDetailLevelItemList="4" 
                                                    RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx" 
                                                    SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx" 
                                                    InializeSlogansOption="True" InializeMediaOption="True" 
                                                    PeriodDetailOptions="True"
                                                    ComparativeStudyOption="True"
                                                    comparativeStudyDateTypeOption="True"></cc2:resultsoptionswebcontrol></td>
										</tr>
										<tr class="backGroundOptions">
											<td style="padding:0px 5px 5px 0px;" align="right"><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="validButton"></cc1:imagebuttonrolloverwebcontrol></td>
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
											<td><cc5:genericmediaschedulewebcontrol id="GenericMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240" ShowVersion="False"></cc5:genericmediaschedulewebcontrol></td>
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

