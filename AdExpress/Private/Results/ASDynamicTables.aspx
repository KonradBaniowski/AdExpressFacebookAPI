<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.ASDynamicTables" CodeFile="ASDynamicTables.aspx.cs" %>
<%@ Register TagPrefix="dcwc" Namespace="Dundas.Charting.WebControl" Assembly="DundasWebChart" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body onclick="<%=scriptBody%>" class="bodyStyle">
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
									<td><asp:Image runat="server" height=90 width=185 SkinID="logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="5" width="185" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr vAlign="top">
									<td><cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol></td>
								</tr>
								<tr vAlign="top">
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</tr>
								<tr vAlign="top">
									<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" EvolutionOption="True" PdvOption="True"
											PdmOption="True" Width="120px" ImageWidth="139" ImageHeight="26" PreformatedTableOption="True" ShowPictures="True" ResultOption="False"
											AutoPostBackOption="False" ProductDetailOption="True" MediaDetailOption="True"
											UnitOption="False" PersonalizedElementsOption="True" InializeAdVertiserOption="True" SkinID="resultsOptionsASDynamicTables"></cc2:resultsoptionswebcontrol></TD><!--MediaOptionOption="True" --><!--2005MI-->
								</tr>
				<TR>
					<TD style="HEIGHT: 12px" vAlign="top" class="whiteBackGround"><cc1:imagebuttonrolloverwebcontrol id="ValidateSelectionButton" runat="server" 
					                                                                onclick="ValidateSelectionButton_Click" SkinID="okButton"></cc1:imagebuttonrolloverwebcontrol></TD>
				<tr vAlign="top" class="whiteBackGround">
					<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
				</tr>
				<tr vAlign="top">
					<td id="premeriEspece"><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
				</tr>
				<tr vAlign="top">
					<td><cc2:modulebridgewebcontrol id="ModuleBridgeWebControl1" runat="server"></cc2:modulebridgewebcontrol></td>
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
					<tr>
						<!-- ligne du haut a droite -->
						<td>
							<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td vAlign="top" align="left"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
									<td><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></td>
									<td vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
								</tr>
							</table>
						</td>
					</tr>
					<!-- Centre -->
					<tr>
						<td class="whiteBackGround" height="10"></td>
					</tr>
					<tr width="100%">
						<!-- Tableau de Résultat -->
						<td style="HEIGHT: 19px" align="center" class="whiteBackGround">
							<%=result%>
						</td>
					</tr>
					<tr>
						<td class="whiteBackGround" height="10"></td>
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
			</TABLE>
		</form>
		<%=divClose%>
	</body>
</HTML>
