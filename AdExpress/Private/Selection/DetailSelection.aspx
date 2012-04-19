<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DetailSelection" CodeFile="DetailSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<META http-equiv="Content-Type" content="text/html;">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</head>
	<body class="popUpbody" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
		    <cc3:menuwebcontrol id="MenuWebControl2" runat="server"></cc3:menuwebcontrol>
			<table cellspacing="0" cellpadding="0" border="0" style="width:100%;height:100%;">
				<!-- Header -->
				<tr>
					<td class="popUpHeaderBackground popUpTextHeader">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="870"></cc1:adexpresstext></td>
				</tr>
				<!-- Content -->
				<tr>
					<td style="height:100%;background-color:#FFF;padding:10;" valign="top">
				        <TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				            <!--Choix de l'etude-->
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="842"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="moduleLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <!--Etude comparative-->
				            <%if(comparativeStudy){%>
				            <TR height="5">
					            <TD class="txtViolet11Bold" vAlign="top" align="left">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Période-->
				            <%if(displayPeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2275"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="infoDateLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Période de l'étude-->
				            <%if(displayStudyPeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="2291"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="StudyPeriod" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Période comparative-->
				            <%if(displayComparativePeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="2292"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="comparativePeriod" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Type de la période comparative-->
				            <%if (displayComparativePeriodType) {%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText15" runat="server" Code="2293"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="ComparativePeriodType" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Type de la disponibilté des données-->
				            <%if (displayPeriodDisponibilityType) {%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText16" runat="server" Code="2296"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="PeriodDisponibilityType" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Agence Média-->
				            <%if(displayMediaAgency){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1580"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="MediaAgency" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Média-->
				            <%if(displayMedia){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top" align="center">
						            <div style="MARGIN-LEFT: 3px"><%=mediaText%></div>
					            </TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Colonnes génériques-->
				            <%if(displayGenericlevelDetailColumnLabel){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="2300"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="genericlevelDetailColumnLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Niveaux génériques-->
				            <%if(displayGenericlevelDetailLabel){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext12" runat="server" Code="1886"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="genericlevelDetailLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Détail média -->
				            <%if(displayDetailMedia){%>
				            <TR height="5">
					            <TD class="txtViolet11" vAlign="top" align="center"><%=mediaDetailText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
                            <!--TODO: A vérifier fusion dev trunk-->
                            <%if(displayListOfVehicles){%>
				            <TR height="5">					
					         <TD class="txtViolet11" vAlign="top" align="center"><%=listOfVehicleText%></TD>
				          </TR>
				           <TR height="10"><TD></TD></TR>
				          <%}%>
				            <!--Reference média -->
				            <%if(displayReferenceDetailMedia){%>
				            <TR height="5">
					            <TD class="txtViolet11" vAlign="top" align="center"><%=referenceMediaDetailText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Genres d'émissions-->
				            <%if(displayProgramType){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="2066"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top" align="center"><%=programTypeText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Formes de parrainage-->
				            <%if(displaySponsorshipForm){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="2067"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top" align="center"><%=sponsorshipFormText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Type de pourcante-->
				            <%if(displayPercentageAlignment){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="2153"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="percentageAlignmentLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Vagues-->
				            <%if(displayWave){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="1762"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top" align="center">
						            <div style="MARGIN-LEFT: 3px"><%=waveText%></div>
					            </TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Cibles-->
				            <%if(displayTargets){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="1763"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top" align="center"><div style="margin-left: 3px"><%=targetsText%></div></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Unité-->
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="849"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11" vAlign="top">&nbsp;<asp:label id="unitLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
                            <!--Encart-->
                            <%if (displayInset){ %>
                            <tr>
                                <td class="txtViolet11Bold">&nbsp;<cc1:AdExpressText ID="AdExpressText18" runat="server" Code="1400" Language="33" /></td>
                            </tr>
                            <tr height="20">
                                <td class="txtViolet11" valign="top">&nbsp;<cc1:AdExpressText ID="AdExpressText20" runat="server" Language="33" /></td>
                            </tr>
                            <TR height="10"><TD></TD></TR>
                            <%} %>

                             <!--Campaign type-->
                <%if (displayCampaignTypeSelection){ %>
                <tr>                   
                    <td class="txtViolet11Bold">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressTextCampaintypeLabel" runat="server" Code="2671" Language="33" />
                    </td>
                </tr>   
                <tr height="20">
                    
                    <td class="txtViolet11" valign="top">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressTextCampaintypeValue" runat="server" Language="33" />
                    </td>
                </tr>
                 <TR height="10"><TD></TD></TR>
                <%} %>
                            <!--Auto promo-->
                            <%if (displayAutoPromo){ %>
                            <tr>
                                <td class="txtViolet11Bold">&nbsp;<cc1:AdExpressText ID="AdExpressText19" runat="server" Code="2552" Language="33" /> :</td>
                            </tr>
                            <tr height="20">
                                <td class="txtViolet11" valign="top">&nbsp;<cc1:AdExpressText ID="AdExpressText21" runat="server" Language="33" /></td>
                            </tr>
                            <TR height="10"><TD></TD></TR>
                            <%} %>
                            <!--Banners Format-->
                            <%if (_displayBannersFormat){ %>
                            <tr>
                                <td class="txtViolet11Bold">&nbsp;<cc1:AdExpressText ID="AdExpressText22" runat="server" Code="2928" Language="33" /> :</td>
                            </tr>
                            <tr height="20">
                                <td class="txtViolet11" valign="top"><%=_bannersFormatText%></td>
                            </tr>
                            <TR height="10"><TD></TD></TR>
                            <%} %>
                            <!--Advertising Agnecy-->
				            <%if(displayAdvertisingAgency){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="advertisingAgencyAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=advertisingAgencyText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
                            <!--Produits-->
				            <%if(displayProduct){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="productAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=productText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
<!--TODO : A tester fusion dev trunk-->
                            <!-- Sélection des médias (affiner dans les résultats) -->
				<%if (displayMediaSelection) {%>
				<TR>				
					<TD class="txtViolet11Bold">&nbsp;
						<cc1:adexpresstext language="33" id="mediaSelectiondWebText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">					
					<TD vAlign="top" align="center" class="txtViolet11Bold"><%=mediaSelectiondText%></TD>
				</TR>				
				 <TR height="10"><TD></TD></TR>
				<%}%>
				            <!-- Personnalisation des supports (affiner dans les résultats) -->
				            <%if(displayMediaPersonnalized){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="mediaPersonnalizedWebText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=mediaPersonnalizedText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!-- Référence produits -->
				            <%if(displayReferenceAdvertiser){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="referenceProductAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=referenceAdvertiserText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Annonceurs / Références -->
				            <%if(displayAdvertiser){%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="advertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=advertiserText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!--Competitor Annonceur/Références -->
				            <%if(displayCompetitorAdvertiser){%>
				            <%=competitorAdvertiserText%>
				            <%}%>
<!--TODO: vérifier pour fusion dev trunk-->
                            	<!--Ad type-->
				<%if(displayAdTypeSelection){%>
				<TR>					
					<TD class="txtViolet11Bold">&nbsp;
						<cc1:adexpresstext language="33" id="AdTypeWebText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">					
					<TD vAlign="top" align="center" class="txtViolet11Bold"><%=AdTypeText%></TD>
				</TR>
				<TR height="10"><TD></TD></TR>
				<%}%>
			            </TABLE>
					</td>
				</tr>
				<!-- Footer -->
				<tr>
					<td class="popUpFooterBackground" align="right"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
			
		</form>
	</body>
</html>
