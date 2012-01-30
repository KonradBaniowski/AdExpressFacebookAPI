<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DetailSelectionExcel" CodeFile="DetailSelectionExcel.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" border="0">
				<TR>
					<td width="5"></td>
					<TD  bgColor="#ffffff">
				    <%=logo%>
					</TD>
				</TR>	
				<tr>
					<td width="5"></td>
					<td>
						&nbsp;
					</td>
				</tr>		
				<!--Choix de l'etude-->
				<tr>
					<td width="5"></td>
					<td class="txtViolet14Bold" align="center">&nbsp;<%=detailSelection%>
					</td>
				</tr>
				<tr>
					<td width="5"></td>
					<td>
						&nbsp;
					</td>
				</tr>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=typeEtude%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="moduleLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<!--Etude comparative-->
				<%if(comparativeStudy){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" vAlign="top" bgColor="#ffffff">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Période-->
				<%if(displayPeriod){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2369"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="infoDateLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Période de l'étude-->
				<%if(displayStudyPeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="2291"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="StudyPeriod" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Période comparative-->
				<%if(displayComparativePeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="2292"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="comparativePeriod" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de la période comparative-->
				<%if (displayComparativePeriodType) {%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText15" runat="server" Code="2293"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="ComparativePeriodType" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de la disponibilté des données-->
				<%if (displayPeriodDisponibilityType) {%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText16" runat="server" Code="2296"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="PeriodDisponibilityType" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Média-->
				<%if(displayMedia){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=mediaText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Colonnes génériques-->
				<%if(displayGenericlevelDetailColumnLabel){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=genericlevelDetailColumnLabelTitle%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="genericlevelDetailColumnLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Niveaux génériques-->
				<%if(displayGenericlevelDetailLabel){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=genericlevelDetailLabelTitle%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="genericlevelDetailLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<%if(displayDetailMedia){%>
				<TR height="5">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=mediaDetailText%></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<%if(displayListOfVehicles){%>
				<TR height="5">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=listOfVehicleText%></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Reference média -->
				<%if(displayReferenceDetailMedia){%>
				<TR height="5">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=referenceMediaDetailText%></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Genres d'émissions-->
				<%if(displayProgramType){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=programTypeLabel%></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=programTypeText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Formes de parrainage-->
				<%if(displaySponsorshipForm){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="2067"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=sponsorshipFormText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Campaign type-->
				<%if(displayCampaignTypeSelection){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="2671"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;&nbsp;<%=campaignTypeSelectionText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de pourcentage-->
				<%if(displayPercentageAlignment){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=percentageAlignmentTitle%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="percentageAlignmentLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Vagues-->
				<%if(displayWave){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="1762"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=waveText%></TD>
				</TR>
				<!--<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>-->
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Cibles-->
				<%if(displayTargets){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="1763"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=targetsText%></TD>
				</TR>
				<!--<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>-->
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Unité-->
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=unitTitle%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="unitLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<!--Annonceurs / Références ...-->
				<%if(displayAdvertiser){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=advertiserAdexpresstext%>
					</TD>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=advertiserText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Produits-->
				<%if(displayProduct){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=productAdExpressText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=productText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				
				<!--Advertising Agency-->
				<%if(displayAdvertisingAgency){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=advertisingAgencyAdExpressText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=advertisingAgencyText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				
				<!-- Personnalisation des supports (affiner dans les résultats) -->
				<%if(displayMediaSelection){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=mediaSelectionWebText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=mediaSelectionText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				
				<!-- Personnalisation des supports (affiner dans les résultats) -->
				<%if(displayMediaPersonnalized){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=mediaPersonnalizedWebText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=mediaPersonnalizedText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				
				
				<!-- Référence produits -->
				<%if(displayReferenceAdvertiser){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=referenceProductAdExpressText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=referenceAdvertiserText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Ad type-->
				<%if(displayAdType){%>
				<TR>
					<td width="5"></td>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<%=adTypeText%>
					</TD>
				</TR>
				<TR height="20">
					<td width="5"></td>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=productText%></TD>
				</TR>
				<TR height="5">
					<td width="5"></td>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<%}%>
				<!--Competitor Annonceur/Références -->
				<%if(displayCompetitorAdvertiser){%>
				<%=competitorAdvertiserText%>
				<%}%>
				<TR height="7">
					<td width="5"></td>
					<TD></TD>
				</TR>
				<TR>
					<td width="5"></td>
					<TD  bgColor="#ffffff">
				    <%=copyRight%>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
