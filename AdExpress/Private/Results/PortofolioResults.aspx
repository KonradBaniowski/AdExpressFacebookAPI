<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="dcwc" Namespace="TNS.AdExpress.Web.UI.Results" Assembly="TNS.AdExpress.Web" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.PortofolioResults" CodeFile="PortofolioResults.aspx.cs" %>
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
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
				<!--</tr>--><!--2005MI-->
				<tr vAlign="top">
					<td><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
				</tr>
				<tr vAlign="top">
					<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" ProductDetailOption="False" InsertOption="True"
							MediaDetailOption="False" ResultOption="True" AutoPostBackOption="False" Percentage="True" UnitOption="True" ShowPictures="False"
							></cc2:resultsoptionswebcontrol></TD><!--InitializeProduct="True"-->
				</tr>
				<!--Niveaux Produits-->
				<tr bgColor="#ffffff" height="5">
					<td></td>
				</tr>
				<%if(novelty){%>
				<tr bgColor="#ffffff">
					<td class="txtGris11Bold"><%=newProductText%></td>
				</tr>
				<tr bgColor="#ffffff">
					<td><asp:radiobuttonlist id="newProductRadioButtonList" runat="server"></asp:radiobuttonlist></td>
				</tr>
				<%}%>
				<%if(detailProductLevel){%>
				<tr bgColor="#ffffff" height="5">
				</tr>
				<tr bgColor="#ffffff" height="5">
					<td style="HEIGHT: 9px"><cc2:genericmedialeveldetailselectionwebcontrol id="_genericMediaLevelDetailSelectionWebControl" runat="server" GenericDetailLevelComponentProfile="product"
							GenericDetailLevelType="dynamicCompetitorPotential" NbDetailLevelItemList="3" Width="200px" SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"
							RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"></cc2:genericmedialeveldetailselectionwebcontrol></td>
				</tr>
				<%if(displayMediaAgencyList){%>
				<tr bgColor="#ffffff" height="5">
					<td></td>
				</tr>
				<tr bgColor="#ffffff">
					<td class="txtGris11Bold" style="HEIGHT: 18px"><cc3:adexpresstext language="33" id="mediaAgencyText" runat="server" Code="1580"></cc3:adexpresstext></td>
				</tr>
				<tr>
					<td bgColor="#ffffff" height="5"><cc5:mediaagencyyearwebcontrol id="MediaAgencyYearWebControl1" runat="server"></cc5:mediaagencyyearwebcontrol></td>
				</tr>
				<tr bgColor="#ffffff" height="3">
					<td></td>
				</tr>
				<%}%>
				<tr bgColor="#ffffff" height="5">
					<td></td>
				</tr>
				<%}%>
				<tr bgColor="#ffffff" height="5">
					<td style="HEIGHT: 9px"><cc2:initializeproductwebcontrol id="InitializeProductWebControl" runat="server" InitializeProduct="True" AutoPostBackOption="False"></cc2:initializeproductwebcontrol></td>
				</tr>
				<tr vAlign="top" bgColor="#ffffff">
					<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" ImageUrl="/Images/Common/Button/ok_up.gif" RollOverImageUrl="/Images/Common/Button/ok_down.gif"></cc1:imagebuttonrolloverwebcontrol></td>
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
			<!-- SÚparateur -->
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
								<tr>
									<td><IMG height="10" src="/images/Common/pixel.gif"></td>
								</tr>
								<TR>
									<td><IMG height="10" src="/Images/Common/pixel.gif" width="1"></td>
								</TR>
								<tr>
									<td style="HEIGHT: 5px" bgColor="#ffffff"></td>
								</tr>
								<tr>
									<td align="center" bgColor="#ffffff">
										<!-- Resultat -->
										<%=result%>
										<DCWC:PORTOFOLIOCHARTUI id="portofolioChart" runat="server"></DCWC:PORTOFOLIOCHARTUI><cc5:resultwebcontrol id="_resultWebControl" runat="server" JavascriptFilePath="/scripts/WebResult.js"
											CssDetailSelectionBordelLevel="dsBorderLevel" CssDetailSelectionL1="dsLv1" CssDetailSelectionL2="dsLv2" CssDetailSelectionL3="dsLv3" CssDetailSelectionTitle="dsTitle" CssDetailSelectionTitleData="dsTitleData"
											CssDetailSelectionTitleGlobal="dsTitleGlobal" CssLHeader="h2" BackgroudColorL1="#B1A3C1" CssL1="lv1" HighlightBackgroundColorL1="#FFFFFF" HighlightBackgroundColorL3="#FFFFFF" BackgroudColorL2="#D0C8DA"
											CssL2="lv2" CssL3="lv3" HighlightBackgroundColorL2="#FFFFFF" BackgroudColorL3="#E1E0DA" AllowPaging="True" ImgBtnCroiOverPath="/Images/Common/fl_tri_croi3_in.gif" ImgBtnCroiPath="/Images/Common/fl_tri_croi3.gif"
											ImgBtnDeCroiOverPath="/Images/Common/fl_tri_decroi3_in.gif" ImgBtnDeCroiPath="/Images/Common/fl_tri_decroi3.gif" CssLTotal="lv0"></cc5:resultwebcontrol></td>
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
