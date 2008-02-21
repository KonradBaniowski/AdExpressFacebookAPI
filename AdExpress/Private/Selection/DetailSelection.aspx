<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DetailSelection" CodeFile="DetailSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<META http-equiv="Content-Type" content="text/html; charset=windows-1252">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/Universe.css" type="text/css" rel="stylesheet">
		<%=script%>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body background="/images/Common/dupli_fond.gif">
		<form id="Form1" method="post" runat="server">
			<cc3:menuwebcontrol id="MenuWebControl2" runat="server"></cc3:menuwebcontrol>
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
			
				<TR bgColor="#644883" height="14">
					<TD style="HEIGHT: 13px" width="14"><IMG src="/Images/Common/fleche_1.gif"></TD>
					<TD class="txtBlanc11Bold" style="HEIGHT: 13px" background="/Images/Common/bandeau_titre.gif">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="870"></cc1:adexpresstext></TD>
				</TR>
				<!--Choix de l'etude-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="842"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="moduleLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!--Etude comparative-->
				<%if(comparativeStudy){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11Bold" vAlign="top" align="left" bgColor="#ffffff">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Période-->
				<%if(displayPeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2275"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="infoDateLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
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
				<!--Agence Média-->
				<%if(displayMediaAgency){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1580"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="MediaAgency" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Média-->
				<%if(displayMedia){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff">
						<div style="MARGIN-LEFT: 3px"><%=mediaText%></div>
					</TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Colonnes génériques-->
				<%if(displayGenericlevelDetailColumnLabel){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="2300"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="genericlevelDetailColumnLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Niveaux génériques-->
				<%if(displayGenericlevelDetailLabel){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext12" runat="server" Code="1886"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="genericlevelDetailLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<%if(displayDetailMedia){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=mediaDetailText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				
				<!--Reference média -->
				<%if(displayReferenceDetailMedia){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><%=referenceMediaDetailText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>								
				<!--Genres d'émissions-->
				<%if(displayProgramType){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="2066"></cc1:adexpresstext></TD>
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
				<!--Type de pourcante-->
				<%if(displayPercentageAlignment){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="2153"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="percentageAlignmentLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
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
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff">
						<div style="MARGIN-LEFT: 3px"><%=waveText%></div>
					</TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
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
					<TD class="txtViolet11" vAlign="top" align="center" bgColor="#ffffff"><div style="margin-left: 3px"><%=targetsText%></div></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Unité-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="849"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="unitLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>				
				<!--Produits-->
				<%if(displayProduct){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="productAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=productText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!-- Référence produits -->
				<%if(displayReferenceAdvertiser){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="referenceProductAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=referenceAdvertiserText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Annonceurs / Références ...-->
				<%if(displayAdvertiser){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="advertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" bgColor="#ffffff"><%=advertiserText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Competitor Annonceur/Références -->
				<%if(displayCompetitorAdvertiser){%>
				<%=competitorAdvertiserText%>
				<%}%>
				<!--Fermer-->
				<TR>
					<TD align="right" colSpan="2"><cc2:imagebuttonrolloverwebcontrol id="closeImageButtonRollOverWebControl" runat="server" onclick="closeImageButtonRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
