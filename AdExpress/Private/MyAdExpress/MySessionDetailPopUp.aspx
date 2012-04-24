<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.MySessionDetailPopUp" CodeFile="MySessionDetailPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
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
	<body class="popUpbody" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
		    <table cellSpacing="0" cellPadding="0" width="100%" height="100%" border="0">
	            <!-- Header -->
	            <tr>
		            <td class="popUpHeaderBackground popUpTextHeader">&nbsp;
		                <cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="852"></cc1:adexpresstext>&nbsp;
						<asp:Label id="mySessionLabel" runat="server"></asp:Label>
		            </td>
	            </tr>

	            <!-- Content -->
	            <tr>
		            <td style="height:100%;background-color:#FFF;padding:10;" valign="top">
		            
			            <TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				            <!--Choix de l'etude-->
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="842"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="moduleLabel" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            
				            <!--Etude comparative-->
				            <%if(comparativeStudy){%>
				            <TR height="5">
					            <TD class="txtViolet11Bold backGroundWhite" vAlign="top">&nbsp;&nbsp;<%=comparativeStudyText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--P�riode-->
				            <%if(displayPeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2369"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="infoDateLabel1" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--P�riode de l'�tude-->
				            <%if(displayStudyPeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="2291"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="StudyPeriod" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--P�riode comparative-->
				            <%if(displayComparativePeriod){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="2292"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="comparativePeriod" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Type de la p�riode comparative-->
				            <%if (displayComparativePeriodType) {%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText15" runat="server" Code="2293"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="ComparativePeriodType" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Type de la disponibilt� des donn�es-->
				            <%if (displayPeriodDisponibilityType) {%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText16" runat="server" Code="2296"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="PeriodDisponibilityType" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Agence M�dia-->
				            <%if(displayMediaAgency){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1580"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="MediaAgency" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--M�dia-->
				            <%if(displayMedia){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="845"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20"><TD align="center" vAlign="top" class="txtViolet11 backGroundWhite"><%=mediaText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				              <!--display Generic Media Detail Level-->
				            <%if(displayGenericMediaDetailLevel){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText7" runat="server" Code="1886"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="genericMediaDetailLevelLabel1" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				             <!--display perso level-->
				            <%if (displayPersonnalizedLevel){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText8" runat="server" Code="1896"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:label id="personnalizedLevelLevelLabel1" runat="server"></asp:label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <!-- D�tail M�dia -->
				            <%if(displayDetailMedia){%>
				            <TR height="5">
					            <TD align="center" vAlign="top" class="txtViolet11 backGroundWhite"><%=mediaDetailText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <% }%>
<!--TODO: V�rifier fusion Dev=>Trunk-->
				             	<%if(displayListOfVehicles){%>
				 <TR height="5">
					<TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=listOfVehicleText%></TD>
				</TR>
				 <TR height="10"><TD></TD></TR>
				<%}%>
                            <!-- D�tail product -->
				            <%if(displayDetailProduct){%>
				            <TR height="5">
					            <TD align="center" vAlign="top" class="txtViolet11 backGroundWhite"><%=productDetailText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <% }%>
				            
				            <!--Reference m�dia -->
				            <%if(displayReferenceDetailMedia){%>
				            <TR height="5">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top" align="center"><%=referenceMediaDetailText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Unit�-->
				             <%if(displayUnit){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext5" runat="server" Code="849"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:Label id="unitLabel" runat="server"></asp:Label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				               <%}%>
				              
				             <%if(displayCampaignTypeSelection){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:adexpresstext language="33" id="AdExpressTextCampaintypeLabel" runat="server" Code="2671"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD class="txtViolet11 backGroundWhite" vAlign="top">&nbsp;<asp:Label id="AdExpressTextCampaintypeValue" runat="server"></asp:Label></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				               <%}%>
				            <!--Produits-->
				            <%if(displayProduct){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:AdExpressText language="33" id="productAdExpressText" runat="server" Code="0"></cc1:AdExpressText></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center" class="backGroundWhite"><%=productText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Professions -->
				            <%if (displayProfessions) {%>
				            <TR>
					            <TD class="txtViolet11Bold">&nbsp;<cc1:adexpresstext language="33" id="professionAdExpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center"><%=professionText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Advertising Agency-->
				            <%if(displayAdvertisingAgency){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">&nbsp;<cc1:AdExpressText language="33" id="advertisingAgencyAdExpressText" runat="server" Code="0"></cc1:AdExpressText></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" align="center" class="backGroundWhite"><%=advertisingAgencyText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <!--Annonceurs / R�f�rences ...-->
				            <%if(displayAdvertiser){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite" style="HEIGHT: 18px">&nbsp;<cc1:adexpresstext language="33" id="advertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD align="center" vAlign="top" class="backGroundWhite"><%=advertiserText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            
				            <%if(referenceAdvertiserDisplay){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite" style="HEIGHT: 18px">&nbsp;<cc1:adexpresstext language="33" id="referenceAdvertiserAdexpresstext" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD align="center" vAlign="top" class="backGroundWhite"><%=referenceAdvertiserText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				             <%if(displayMediaSelection){%>
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite" style="HEIGHT: 18px">&nbsp;<cc1:adexpresstext language="33" id="mediaSelectiondWebText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD align="center" vAlign="top" class="backGroundWhite"><%=mediaSelectiondText%></TD>
				            </TR>
				            <TR height="10"><TD></TD></TR>
				            <%}%>
				            <%if(competitorAdvertiserDisplay){%>
				            <%=competitorAdvertiserText%>
				            <%}%>
				            <TR height="10"><TD></TD></TR>
		                 </TABLE>

		            </td>
	            </tr>
            	
	            <!-- Footer -->
	            <tr>
		            <td class="popUpFooterBackground" align="right"><cc2:ImageButtonRollOverWebControl id="closeImageButtonRollOverWebControl" runat="server" onclick="closeImageButtonRollOverWebControl_Click" SkinID="fermerButton"></cc2:ImageButtonRollOverWebControl>&nbsp;&nbsp;</td>
	            </tr>
            </table>
		</form>
	</body>
</HTML>
