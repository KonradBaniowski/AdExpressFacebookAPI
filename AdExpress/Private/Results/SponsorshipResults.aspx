<%@ Page language="c#" Inherits="AdExpress.Private.Results.SponsorshipResults" CodeFile="SponsorshipResults.aspx.cs" %>
<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc6" Namespace="TNS.FrameWork.WebResultUI" Assembly="TNS.FrameWork.WebResultUI" %>
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
		<LINK href="/Css/GenericUI.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<%=scripts%>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body onclick="<%=scriptBody%>">
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
							
								<tr vAlign="top" bgColor="#ffffff">
									<!-- Logo -->
									<td background="/images/Common/logo_cote_droit.gif"><IMG height=90 src="/images/<%=_siteLanguage%>/logo/logo.gif" ></td>
								</tr>
								<tr>
									<td><IMG height="5" src="/images/Common/plus_sous_logo.gif" width="100%"></td>
								</tr>
								<tr>
									<td><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
				<!--</tr>-->
				<tr vAlign="top">
					<td><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
				</tr>
				<tr vAlign="top">
					<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" PercentageTypeOption="True" ShowPictures="False"
							InsertOption="False" Percentage="False" AutoPostBackOption="False" ResultOption="False" PreformatedTableOption="False"
							Width="120px" BorderColor="#644883" ImageHeight="26" ImageWidth="139"></cc2:resultsoptionswebcontrol></TD>
				</tr>
				<!--Niveaux Produits-->
				<tr bgColor="#ffffff" height="3">
					<td></td>
				</tr>
				<tr bgColor="#ffffff">
					<td><cc2:genericmedialeveldetailselectionwebcontrol id="_genericMediaLevelDetailSelectionWebControl" runat="server" Width="200px" NbDetailLevelItemList="4"
							SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"
							GenericDetailLevelComponentProfile="media" GenericDetailLevelType="devicesAnalysis"></cc2:genericmedialeveldetailselectionwebcontrol></td>
				</tr>
				<tr bgColor="#ffffff" height="5">
					<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="False" InitializeProduct="True"></cc2:initializeproductwebcontrol></td>
				</tr>
				<tr bgColor="#ffffff" height="5">
					<td><cc2:InitializeMediaWebControl id="InitializeMediaWebcontrol1" runat="server" AutoPostBackOption="False" InitializeMedia="True"></cc2:InitializeMediaWebControl></td>
				</tr>
				<!--Fin Niveaux Produits-->
				<!--Debut Type de tableau-->
				<tr vAlign="top">
					<TD vAlign="top"><cc2:resultstabletypeswebcontrol id="ResultsTableTypesWebControl1" runat="server" ShowPictures="True" AutoPostBackOption="False"
							Width="120px" BorderColor="#644883" ImageHeight="26" ImageWidth="139"></cc2:resultstabletypeswebcontrol></TD>
				</tr>
				<!--Fin Type de tableau-->
				<tr bgColor="#ffffff">
					<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" ImageUrl="/Images/Common/Button/ok_up.gif" RollOverImageUrl="/Images/Common/Button/ok_down.gif"></cc1:imagebuttonrolloverwebcontrol></td>
				</tr>
				<tr bgColor="#ffffff" height="5">
					<td></td>
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
					<td><cc2:informationwebcontrol id="InformationWebControl1" runat="server"></cc2:informationwebcontrol></td>
				</tr>
			</table>
			</TD> 
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
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr vAlign="top">
									<td></td>
								<tr>
								<TR>
									<td><IMG height="10" src="images/pixel.gif" width="1"></td>
								</TR>
								<tr>
									<td style="HEIGHT: 5px" bgColor="#ffffff"></td>
								</tr>
								<tr bgColor="#ffffff">
									<!-- Test Tableau de Résultat -->
									<td style="HEIGHT: 37px" bgColor="#ffffff"><br>
										<cc5:resultwebcontrol id="resultwebcontrol1" runat="server" CssL1="lv1" CssL2="lv2" CssL3="lv3" CssLHeader="h2"
											CssLTotal="lv0" CssCHeader="h2" JavascriptFilePath="/scripts/WebResult.js" BackgroudColorL1="#B1A3C1"
											HighlightBackgroundColorL1="#FFFFFF" HighlightBackgroundColorL2="#FFFFFF" HighlightBackgroundColorL3="#FFFFFF"
											BackgroudColorL2="#D0C8DA" BackgroudColorL3="#E1E0DA" ImgBtnCroiOverPath="/Images/Common/fl_tri_croi3_in.gif"
											ImgBtnCroiPath="/Images/Common/fl_tri_croi3.gif" ImgBtnDeCroiOverPath="/Images/Common/fl_tri_decroi3_in.gif"
											ImgBtnDeCroiPath="/Images/Common/fl_tri_decroi3.gif" PageSizeOptions="100,200,500,1000" AllowPaging="True"
											BackgroudColorL4="#F1F1F1" CssL4="lv4" HighlightBackgroundColorL4="#FFFFFF" CssDetailSelectionBordelLevel="dsBorderLevel"
											CssDetailSelectionL1="dsLv1" CssDetailSelectionL2="dsLv2" CssDetailSelectionL3="dsLv3" CssDetailSelectionTitle="dsTitle"
											CssDetailSelectionTitleData="dsTitleData" CssDetailSelectionTitleGlobal="dsTitleGlobal"></cc5:resultwebcontrol></td>
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
			</TR> 
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
							<TD><cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol></TD>
							<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
			</TABLE></form>
		<%=divClose%>
	</body>
</HTML>
