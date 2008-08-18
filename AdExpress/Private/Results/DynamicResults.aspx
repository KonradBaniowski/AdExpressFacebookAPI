<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.DynamicResults" CodeFile="DynamicResults.aspx.cs" %>
<%@ Register TagPrefix="cc6" Namespace="TNS.FrameWork.WebResultUI" Assembly="TNS.FrameWork.WebResultUI" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</HEAD>
	<body class="bodyStyle">
		<form id="Form2" method="post" runat="server">
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
					<td vAlign="top" width="1%" class="imageBackGround">
						<table id="tableMenu" cellSpacing="0" cellPadding="0" width="10" border="0">
							
								<tr vAlign="top" class="whiteBackGround">
									<!-- Logo -->
									<td class="logoCoteDroitBackGround"><asp:Image runat="server" height="90" SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" width="100%" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
				<!--</tr>--><!--2005MI-->
				<tr vAlign="top">
					<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
				</tr>
				<tr vAlign="top">
					<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" InsertOption="True" Percentage="True"
							AutoPostBackOption="False" ResultOption="True"></cc2:resultsoptionswebcontrol></TD>
				</tr>
				<!--Niveaux Produits-->
				<tr class="whiteBackGround" height="3">
					<td></td>
				</tr>
				<!--<tr bgColor="#ffffff">
					<td class="txtGris11Bold"><cc3:adexpresstext language="33" id="detailProductAdExpressText" runat="server" Code="1124"></cc3:adexpresstext></td>
				</tr>-->
				<tr vAlign="top">
					<TD vAlign="top"><cc2:GenericColumnLevelDetailSelectionWebControl id="_genericColumnLevelDetailSelectionWebControl1" runat="server" Width="200px" GenericColumnDetailLevelType="dynamicCompetitorPotential"
							NbColumnDetailLevelItemList="1"></cc2:GenericColumnLevelDetailSelectionWebControl></TD>
				</tr>
				<tr class="whiteBackGround">
					<td><cc2:genericmedialeveldetailselectionwebcontrol id="_genericMediaLevelDetailSelectionWebControl" runat="server" NbDetailLevelItemList="3"
							SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"
							GenericDetailLevelComponentProfile="product" GenericDetailLevelType="dynamicCompetitorPotential" Width="200px"></cc2:genericmedialeveldetailselectionwebcontrol></td>
				</tr>
				
				<tr class="whiteBackGround" height="5">
					<td><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="False" InitializeProduct="True"></cc2:initializeproductwebcontrol></td>
				</tr>
				<tr class="whiteBackGround">
					<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="okButton"></cc1:imagebuttonrolloverwebcontrol></td>
				</tr>
				<tr class="whiteBackGround" height="5">
					<td></td>
				</tr>
				<!--Fin Niveaux Produits-->
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
			</table>
			</TD> 
			<!-- Séparateur -->
			<td vAlign="top">
				<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
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
			<td vAlign="top" class="imageBackGround">
				<table cellSpacing="0" cellPadding="0" border="0">
					<tr>
						<!-- Menu du haut-->
						<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
					</tr>
					<!-- Centre -->
					<tr>
						<td>
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td style="HEIGHT: 5px" class="whiteBackGround"></td>
								</tr>
								<tr class="whiteBackGround">
									<!-- Test Tableau de Résultat -->
									<td class="whiteBackGround" style="HEIGHT: 37px"><br>
										<cc5:resultwebcontrol id="resultwebcontrol1" runat="server" JavascriptFilePath='/scripts/WebResult.js' 
											PageSizeOptions="100,200,500,1000" AllowPaging="True" SkinID="portofolioResultTable"></cc5:resultwebcontrol></td>
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
			</TR> 
			<!-- ligne du bas a droite -->
			<TR height="5">
				<TD></TD>
				<TD vAlign="top" class="imageBackGround">
					<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
						<TR>
							<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							<TD></TD>
							<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
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
							<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							<TD><cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol></TD>
							<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
			</TABLE></form>
		<%=divClose%>
	</body>
</HTML>
