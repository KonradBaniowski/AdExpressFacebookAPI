﻿<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewCreativesResultsHelp.aspx.cs" Inherits="Private_Helps_NewCreativesResultsHelp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body class="darkBackGround backGroundWhite" bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
    <form id="form1" runat="server">
        <table cellSpacing="0" cellPadding="0" width="98%" align="center" class="backGroundWhite" border="0">
			<TR>
				<td colSpan="2">
					<table cellPadding="0" width="100%" class="violetBackGround" border="0">
						<tr>
							<td align="left"><font class="txtBlanc14Bold">&nbsp;
									<cc1:adexpresstext language="33" id="titleAdExpressText" runat="server" Code="993"></cc1:adexpresstext></font></td>
						</tr>
					</table>
				</td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Titre 1 : Options d'analyse-->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Titre1AdExpressText" runat="server" Code="792"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte Option d'analyse -->
			<tr>
				<td width="10"><asp:Image ID="Image22" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="2948"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/NewCreaResultOption.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image60" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			
			<!-- Période détaillée par -->
			<tr>
				<td width="10"><asp:Image ID="Image59" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext26" runat="server" Code="2423"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple Image -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image17" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/MediaPlanDetailPeriod.gif"></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image18" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			
			<!-- doc marketing -->
			<tr>
				<td width="10"><asp:Image ID="Image29" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1463"></cc1:adexpresstext></td>
			</tr>
			<!--exemple-->
			<tr>
				<td colSpan="2"><asp:Image ID="Image65" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/annDynGadLink.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			
			<!-- Titre : Pagination des résultats -->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext27" runat="server" Code="2131"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image32" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext29" runat="server" Code="2136"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image72" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/nbPage.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image9" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image33" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext28" runat="server" Code="2137"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image73" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/repeatOption.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image34" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="2138"></cc1:adexpresstext></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image11" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			
			<!-- Titre : Rappel de la sélection -->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext25" runat="server" Code="2132"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image2" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image153" runat="server" SkinID="bt_detail" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="2135"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image74" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/detailSelectionDivNewCrea.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image8" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Titre : Tri des colonnes -->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext23" runat="server" Code="2133"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image12" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image36" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext24" runat="server" Code="2134"></cc1:adexpresstext></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image75" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<!-- Titre : Niveaux personnalisés-->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext22" runat="server" Code="1896"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image13" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- niveau de détail produit -->
			<tr>
				<td width="10"><asp:Image ID="Image37" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="2126"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image76" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/personalizedLevels.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image14" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- niveau de détail personnalisés -->
			<tr>
				<td width="10"><asp:Image ID="Image38" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<cc1:adexpresstext language="33" id="Adexpresstext13" runat="server" Code="2127"></cc1:adexpresstext><br>
					<cc1:adexpresstext language="33" id="Adexpresstext14" runat="server" Code="1957"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image91" runat="server" SkinID="valider" />
				</td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image77" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/levels3.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image15" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- niveau enregistrés -->
			<tr>
				<td width="10"><asp:Image ID="Image39" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<cc1:adexpresstext language="33" id="Adexpresstext18" runat="server" Code="2128"></cc1:adexpresstext><br>
					<cc1:adexpresstext language="33" id="Adexpresstext19" runat="server" Code="1957"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image92" runat="server" SkinID="valider" />
				</td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image78" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/personalizedLevelsChoice.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image16" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!--Sauvegarde niveaux -->
			<tr>
				<td width="10"><asp:Image ID="Image40" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image93" runat="server" SkinID="save" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext20" runat="server" Code="2129"></cc1:adexpresstext></td>
						</tr>
						<tr>
							<td align="center" width="40"><asp:Image ID="Image94" runat="server" SkinID="delete2" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext21" runat="server" Code="2130"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image85" runat="server" SkinID="pixel" height="25" width="100%" /></td>
			</tr>
				
			<!-- sauvegarde de votre recherche-->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext35" runat="server" Code="771"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image19" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image43" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<cc1:AdExpressText language="33" id="AdExpressText11rightClick" runat="server" Code="2021"></cc1:AdExpressText></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image81" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image44" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext36" runat="server" Code="1337"></cc1:adexpresstext></td>
			</tr>
			<!--Sauvegarde session-->
			<tr>
				<td colSpan="2"><asp:Image ID="Image20" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image45" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11">
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image96" runat="server" SkinID="save" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext70" runat="server" Code="2451"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image82" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResult.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image83" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image46" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext67" runat="server" Code="2452"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple Image -->
			<tr>
				<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultNewName.gif">&nbsp;&nbsp;<IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultFolderSelection.gif"></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image21" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image47" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext68" runat="server" Code="2453"></cc1:adexpresstext></td>
			</tr>
			<!-- Exemple Image -->
			<tr>
				<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveUniverseSaveAs.gif"></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image23" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Texte -->
			<tr>
				<td width="10"><asp:Image ID="Image48" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext69" runat="server" Code="2450"></cc1:adexpresstext></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image24" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			
			<!-- Titre : Options accessibles depuis le clic droit de la souris  -->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="Adexpresstext39" runat="server" Code="2396"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image31" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<!-- Clic droit -->
			<tr>
				<td width="10"><asp:Image ID="Image66" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext40" runat="server" Code="2397"></cc1:adexpresstext></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image118" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<!-- Exemple -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image119" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/MouseRightButtonNewCrea.gif" ></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image35" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			
			<!-- Excel print -->
			<tr>
				<td width="10"><asp:Image ID="Image49" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image97" runat="server" SkinID="print" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="text4AdExpressText" runat="server" Code="1662"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image84" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<!-- Excel export-->
			<tr>
				<td width="10"><asp:Image ID="Image50" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image98" runat="server" SkinID="excel" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="1663"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image28" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			
			
			<!-- Titre : Icônes de navigation  -->
			<TR>
				<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
					<cc1:adexpresstext language="33" id="titreNAdexpresstext" runat="server" Code="1010"></cc1:adexpresstext></td>
			</TR>
			<tr>
				<td colSpan="2"><asp:Image ID="Image30" runat="server" SkinID="pixel" height="15" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image52" runat="server" SkinID="pixel" width="10" /></td>
				<td class="txtViolet11"><cc1:adexpresstext language="33" id="text14Adexpresstext" runat="server" Code="2020"></cc1:adexpresstext></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image53" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image122" runat="server" SkinID="selected" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="text15Adexpresstext" runat="server" Code="1012"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image86" runat="server" SkinID="pixel" height="12" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image55" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image101" runat="server" SkinID="media" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="text17Adexpresstext" runat="server" Code="1014"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image88" runat="server" SkinID="pixel" height="12" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image56" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellSpacing="0" cellPadding="0" border="0">
						<tr>
							<td align="center" width="40"><asp:Image ID="Image102" runat="server" SkinID="calendar" /></td>
							<td class="txtViolet11"><cc1:adexpresstext language="33" id="text18Adexpresstext" runat="server" Code="1015"></cc1:adexpresstext></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image89" runat="server" SkinID="pixel" height="12" width="100%" /></td>
			</tr>
			<tr>
				<td width="10"><asp:Image ID="Image57" runat="server" SkinID="pixel" width="10" /></td>
				<td>
					<table cellpadding="0" cellspacing="0" border="0">
						<tr>
							<td width="40" align="center"><asp:Image ID="Image103" runat="server" SkinID="result" /></td>
							<td class="txtViolet11">
								<cc1:AdExpressText language="33" id="text19Adexpresstext" runat="server" Code="1016"></cc1:AdExpressText>
							</td>
						</tr>
					</table>
				</td>
			</tr>

			
			<!-- Bouton de fermeture -->
			<tr>
				<td colSpan="2"><asp:Image ID="Image25" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td class="violetBackGround" colSpan="2"><asp:Image ID="Image27" runat="server" SkinID="pixel" height="1" width="100%" /></td>
			</tr>
			<tr>
				<td colSpan="2"><asp:Image ID="Image26" runat="server" SkinID="pixel" height="10" width="100%" /></td>
			</tr>
			<tr>
				<td align="right" class="backGroundWhite" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
			</tr>
		</table>
    </form>
</body>
</html>