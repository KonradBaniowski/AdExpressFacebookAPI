
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
		<script language="javascript" type="text/javascript" src="/scripts/WebResult.js"></script>
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
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" width="10" SkinID="logo_cote_gauche" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" width="1%" class="imageBackGround">
						<table id="tableMenu" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TBODY>
								<tr valign="top" class="whiteBackGround">
									<!-- Logo -->
									<td class="logoCoteDroitBackGround"><asp:Image runat="server" height="90" width="185" SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" width="200" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr valign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
								<tr valign="top">
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr valign="top">
									<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" AutoPostBackOption="False" ResultOption="False"
											MediaDetailOption="False"></cc2:resultsoptionswebcontrol></TD>
								</tr>
								<tr valign="top" class="whiteBackGround">
									<td><cc2:PeriodDetailWebControl ID="PeriodDetailWebControl1" runat="server" SkinID="PeriodDetailWebControl">
                                        </cc2:PeriodDetailWebControl></td>
								</tr>
                                <tr valign="top" class="whiteBackGround">
									<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
								</tr>
								<tr valign="top">
									<td vAlign="top"><cc2:genericmedialeveldetailselectionwebcontrol id="GenericMediaLevelDetailSelectionWebControl1" runat="server" Width="200px" NbDetailLevelItemList="4"
											RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx" SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"></cc2:genericmedialeveldetailselectionwebcontrol></td>
								</tr>
								<tr  class="whiteBackGround">
									<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="False" InitializeSlogans="True"
											InitializeProduct="False"></cc2:initializeproductwebcontrol></td>
								</tr>
								<tr class="whiteBackGround" height="5">
					                <td><cc2:InitializeMediaWebControl id="InitializeMediaWebcontrol1" runat="server" AutoPostBackOption="False" InitializeMedia="True"></cc2:InitializeMediaWebControl></td>
				                </tr>
                                <tr valign="top" class="whiteBackGround">
									<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
								</tr>

								<tr vAlign="top">
									<td id="Espace"><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr vAlign="top">
									<td align="left"><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="validateButton"></cc1:imagebuttonrolloverwebcontrol></td>
								</tr>
								<tr vAlign="top">
									<td id="premeriEspece"><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server" Visible="False"></cc2:modulebridgewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
								</tr>
								<tr>
									<td>
										<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
								</tr>
							</TBODY>
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
											<td valign="top" align="left"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
											<td><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></td>
											<td valign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td><asp:Image runat="server" height="10" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td class="whiteBackGround" height="5"></td>
										</tr>
										<%=zoomButton%>
										
                                        <tr class="whiteBackGround" >
                                            <td><cc2:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="whiteBackGround" height="10">
                                            </td>
                                        </tr>
                                        <tr>
											<!-- Tableau de Résultat -->
											<td style="HEIGHT: 19px" class="whiteBackGround"><cc5:genericmediaschedulewebcontrol id="GenericMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240" ShowVersion="True"></cc5:genericmediaschedulewebcontrol></td>
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
				<TR height="5">
					<TD></TD>
					<TD vAlign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD></TD>
					<TD id="dellCel" vAlign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD>
									<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
								<TD vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
