<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="dcwc" Namespace="TNS.AdExpress.Web.UI.Results" Assembly="TNS.AdExpress.Web" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.MediaPortofolioResults" CodeFile="MediaPortofolioResults.aspx.cs" %>
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
					<TD vAlign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" runat="server" ShowPictures="False" UnitOption="False"
							Percentage="False" AutoPostBackOption="False" ResultOption="True" MediaDetailOption="False" InsertOption="True" ProductDetailOption="False"></cc2:resultsoptionswebcontrol></TD>
				</tr>
				<%if(_genericProductLevel){%>
				<tr class="whiteBackGround" height="5">
					<td class="txtGris11Bold" style="HEIGHT: 14px"></td>
				</tr>
				<tr class="whiteBackGround" height="5">
					<td><cc2:genericmedialeveldetailselectionwebcontrol id="_genericMediaLevelDetailSelectionWebControl" runat="server" SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"
							RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx" GenericDetailLevelType="dynamicCompetitorPotential"
							GenericDetailLevelComponentProfile="product" Width="200px" NbDetailLevelItemList="3"></cc2:genericmedialeveldetailselectionwebcontrol></td>
				</tr>
				<%}%>				
				<%if(press){%>
				<!--Fin Niveaux Produits-->
				<tr vAlign="top" class="whiteBackGround">
					<td id="premeriEspece">&nbsp;<A onmouseover="table.src = '/Images/Common/Button/table_down.gif';" onclick="tableRadioButton.checked=true;"
							onmouseout="table.src = '/Images/Common/Button/table_up.gif';" href="#"></A>
					</td>
				</tr>
				<%}%>
				<tr class="whiteBackGround" height="5">
					<td style="HEIGHT: 9px"><cc2:initializeproductwebcontrol id="InitializeProductWebControl1" runat="server" AutoPostBackOption="False" InitializeProduct="True"></cc2:initializeproductwebcontrol></td>
				</tr>
				<tr vAlign="top" class="whiteBackGround">
					<td><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" RollOverImageUrl="/Images/Common/Button/ok_down.gif"
							ImageUrl="/Images/Common/Button/ok_up.gif"></cc1:imagebuttonrolloverwebcontrol></td>
				</tr>
				<!-- Info bt droit -->
				<tr>
					<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
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
			<td tabIndex="4" vAlign="top" class="imageBackGround">
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
						<td>
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td><asp:Image runat="server" height="10" SkinID="pixel" /></td>
								</tr>
								<TR>
									<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
								</TR>
								<tr>
									<td style="HEIGHT: 5px" class="whiteBackGround"></td>
								</tr>
								<tr>
									<td align="center" class="whiteBackGround" height="5"><cc5:resultwebcontrol id="_ResultWebControl" runat="server" JavascriptFilePath="/scripts/WebResult.js"
											AllowPaging="True" SkinID="portofolioResultTable"></cc5:resultwebcontrol>
										<%=result%>
									</td>
								</tr>
								<tr>
									<td style="HEIGHT: 5px" class="whiteBackGround"></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
			<!--</TR></TBODY></TABLE></TD> -->
			<!-- la fin -->
			<td></td>
			</TR> 
			<!-- ligne du bas a droite -->
			<TR height="5">
				<TD></TD>
				<TD class="imageBackGround" vAlign="top">
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
				<TD id="dellCel" class="imageBackGround" vAlign="top">
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
