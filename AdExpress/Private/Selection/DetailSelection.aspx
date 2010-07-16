<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DetailSelection" CodeFile="DetailSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
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
	</HEAD>
	<body class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<cc3:menuwebcontrol id="MenuWebControl2" runat="server"></cc3:menuwebcontrol>
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR class="violetBackGround" height="14">
					<TD style="HEIGHT: 13px" width="14"><asp:Image ID="Image153" runat="server" SkinID="fleche_1" /></TD>
					<TD class="txtBlanc11Bold bandeauTitreBackGround" style="HEIGHT: 13px">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="870"></cc1:adexpresstext></TD>
				</TR>
				<!--Choix de l'etude-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="842"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="moduleLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!--Etude comparative-->
				<%if(comparativeStudy){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite" vAlign="top" align="left">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--P�riode-->
				<%if(displayPeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2275"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="infoDateLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--P�riode de l'�tude-->
				<%if(displayStudyPeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="2291"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="StudyPeriod" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--P�riode comparative-->
				<%if(displayComparativePeriod){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="2292"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="comparativePeriod" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de la p�riode comparative-->
				<%if (displayComparativePeriodType) {%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText15" runat="server" Code="2293"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="ComparativePeriodType" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de la disponibilt� des donn�es-->
				<%if (displayPeriodDisponibilityType) {%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText16" runat="server" Code="2296"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="PeriodDisponibilityType" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Agence M�dia-->
				<%if(displayMediaAgency){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1580"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="MediaAgency" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--M�dia-->
				<%if(displayMedia){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center">
						<div style="MARGIN-LEFT: 3px"><%=mediaText%></div>
					</TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Colonnes g�n�riques-->
				<%if(displayGenericlevelDetailColumnLabel){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="2300"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="genericlevelDetailColumnLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Niveaux g�n�riques-->
				<%if(displayGenericlevelDetailLabel){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext12" runat="server" Code="1886"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="genericlevelDetailLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<%if(displayDetailMedia){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=mediaDetailText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				
				<!--Reference m�dia -->
				<%if(displayReferenceDetailMedia){%>
				<TR height="5">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=referenceMediaDetailText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>								
				<!--Genres d'�missions-->
				<%if(displayProgramType){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="2066"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=programTypeText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Formes de parrainage-->
				<%if(displaySponsorshipForm){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="2067"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=sponsorshipFormText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Type de pourcante-->
				<%if(displayPercentageAlignment){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="2153"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
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
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="1762"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center">
						<div style="MARGIN-LEFT: 3px"><%=waveText%></div>
					</TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Cibles-->
				<%if(displayTargets){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="1763"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><div style="margin-left: 3px"><%=targetsText%></div></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Unit�-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="849"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;
						<asp:label id="unitLabel" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
                <!--Encart-->
                <%if (displayInset){ %>
                <tr>
                    <td>
                    </td>
                    <td class="txtViolet11Bold backGroundWhite">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressText18" runat="server" Code="1400" Language="33" />
                    </td>
                </tr>
                <tr height="20">
                    <td>
                    </td>
                    <td class="txtViolet11 backGroundWhite" valign="top">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressText20" runat="server" Language="33" />
                    </td>
                </tr>
                <tr height="7">
                    <td colspan="2">
                    </td>
                </tr>
                <%} %>
                <!--Auto promo-->
                <%if (displayAutoPromo){ %>
                <tr>
                    <td>
                    </td>
                    <td class="txtViolet11Bold backGroundWhite">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressText19" runat="server" Code="2552" Language="33" /> :
                    </td>
                </tr>
                <tr height="20">
                    <td>
                    </td>
                    <td class="txtViolet11 backGroundWhite" valign="top">
                        &nbsp;
                        <cc1:AdExpressText ID="AdExpressText21" runat="server" Language="33" />
                    </td>
                </tr>
                <tr height="7">
                    <td colspan="2">
                    </td>
                </tr>
                <%} %>
                <!--Produits-->
				<%if(displayProduct){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="productAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" class="backGroundWhite"><%=productText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				
				<!-- Personnalisation des supports (affiner dans les r�sultats) -->
				<%if(displayMediaPersonnalized){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="mediaPersonnalizedWebText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" class="backGroundWhite"><%=mediaPersonnalizedText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				
				
				<!-- R�f�rence produits -->
				<%if(displayReferenceAdvertiser){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="referenceProductAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" class="backGroundWhite"><%=referenceAdvertiserText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Annonceurs / R�f�rences ...-->
				<%if(displayAdvertiser){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold backGroundWhite">&nbsp;
						<cc1:adexpresstext language="33" id="advertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" class="backGroundWhite"><%=advertiserText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Competitor Annonceur/R�f�rences -->
				<%if(displayCompetitorAdvertiser){%>
				<%=competitorAdvertiserText%>
				<%}%>
				<!--Fermer-->
				<TR>
					<TD align="right" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
