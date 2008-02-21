<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.MySessionDetailPopUp" CodeFile="MySessionDetailPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/Universe.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">

		<meta http-equiv="expires" content="0">

		<meta http-equiv="pragma" content="no-cache">

		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body background="/images/Common/dupli_fond.gif">
		<%=script%>
		<form id="Form1" method="post" runat="server">
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR bgColor="#644883" height="14">
					<TD width="14"><IMG src="/Images/Common/fleche_1.gif"></TD>
					<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="852"></cc1:adexpresstext>&nbsp;
						<asp:Label id="mySessionLabel" runat="server"></asp:Label></TD>
				</TR>
				<!--Choix de l'etude-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="842"></cc1:adexpresstext></TD>
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
					<TD class="txtViolet11Bold" vAlign="top" bgColor="#ffffff">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<!--Période-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2369"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:label id="infoDateLabel1" runat="server"></asp:label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
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
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD align="center" vAlign="top" bgColor="#ffffff" class="txtViolet11"><%=mediaText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!-- Détail Média -->
				<%if(displayDetailMedia){%>
				<TR height="5">
					<TD></TD>
					<TD align="center" vAlign="top" bgColor="#ffffff" class="txtViolet11"><%=mediaDetailText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<!--<TR height="7">
					<TD colSpan="2"></TD>
				</TR>-->
				<% }%>
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
				<!-- Fin détail média -->
				<!--Unité-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext5" runat="server" Code="849"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD class="txtViolet11" vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:Label id="unitLabel" runat="server"></asp:Label></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!--Produits-->
				<%if(displayProduct){%>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:AdExpressText language="33" id="productAdExpressText" runat="server" Code="0"></cc1:AdExpressText></TD>
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
				<!--Annonceurs / Références ...-->
				<%if(displayAdvertiser){%>
				<TR>
					<TD style="HEIGHT: 18px"></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff" style="HEIGHT: 18px">&nbsp;
						<cc1:adexpresstext language="33" id="advertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD align="center" vAlign="top" bgColor="#ffffff"><%=advertiserText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<%if(referenceAdvertiserDisplay){%>
				<TR>
					<TD style="HEIGHT: 18px"></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff" style="HEIGHT: 18px">&nbsp;
						<cc1:adexpresstext language="33" id="referenceAdvertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD align="center" vAlign="top" bgColor="#ffffff"><%=referenceAdvertiserText%></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<%}%>
				<%if(competitorAdvertiserDisplay){%>
				<!--<TR>
					<TD style="HEIGHT: 18px"></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&nbsp;
						<cc1:adexpresstext language="33" id="competitorAdvertiserAdexpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>-->
				<%=competitorAdvertiserText%>
				<%}%>
				<!--	<TR height="5">
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR> -->
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!--Fermer-->
				<TR>
					<TD align="right" colspan="2">
						<cc2:ImageButtonRollOverWebControl id="closeImageButtonRollOverWebControl" runat="server" onclick="closeImageButtonRollOverWebControl_Click"></cc2:ImageButtonRollOverWebControl></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
