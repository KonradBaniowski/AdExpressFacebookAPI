<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Helps.APPMResultsHelp" CodeFile="APPMResultsHelp.aspx.cs" %>
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
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body class="darkBackGround whiteBackGround" bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="98%" align="center" class="whiteBackGround" border="0">
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
					<td colSpan="2"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte Option d'analyse -->
				<tr>
					<td width="10"><asp:Image ID="Image52" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1437"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image16" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/APPMResultOptions.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image17" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Validation OK -->
				<tr>
					<td width="10"><asp:Image ID="Image53" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext33" runat="server" Code="1005"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image153" runat="server" SkinID="ok" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext34" runat="server" Code="1336"></cc1:adexpresstext>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image18" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Synthese -->
				<tr>
					<td width="10"><asp:Image ID="Image54" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="text5AdExpressText" runat="server" Code="1664"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image19" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- option : agence media-->
				<tr>
					<td width="10"><asp:Image ID="Image55" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/mediaAgenciesOption.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="1804"></cc1:adexpresstext></td>
				</tr>
				<!-- option : produits-->
				<tr>
					<td width="10"><asp:Image ID="Image56" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/productsOptions.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="1805"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image20" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Calendrier d'action -->
				<tr>
					<td width="10"><asp:Image ID="Image57" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext4" runat="server" Code="1773"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image21" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image58" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext5" runat="server" Code="1806"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image22" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image59" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext6" runat="server" Code="1807"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image23" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image60" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="1808"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image24" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image61" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="1809"></cc1:adexpresstext><br>
						<cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="1810"></cc1:adexpresstext><br>
						<cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/lienCheminDeFer.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="1811"></cc1:adexpresstext>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image25" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image62" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext12" runat="server" Code="1005"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image109" runat="server" SkinID="back" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext15" runat="server" Code="1812"></cc1:adexpresstext>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image26" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Analyse par titre -->
				<tr>
					<td width="10"><asp:Image ID="Image63" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext13" runat="server" Code="1680"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image27" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image64" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext14" runat="server" Code="1813"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image28" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Analyse des parts de voix -->
				<tr>
					<td width="10"><asp:Image ID="Image65" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext16" runat="server" Code="1728"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image29" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image66" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="1814"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image30" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Analyse par périodicité -->
				<tr>
					<td width="10"><asp:Image ID="Image67" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext18" runat="server" Code="1665"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image31" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image68" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext19" runat="server" Code="1815"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image32" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Analyse par famille de presse -->
				<tr>
					<td width="10"><asp:Image ID="Image69" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext20" runat="server" Code="1740"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image33" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image70" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext21" runat="server" Code="1816"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image34" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Affinités -->
				<tr>
					<td width="10"><asp:Image ID="Image71" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext22" runat="server" Code="1687"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image35" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image72" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext23" runat="server" Code="1817"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image36" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Titre : Icônes d'option présentation  -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image73" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext24" runat="server" Code="1350"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image74" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image110" runat="server" SkinID="chart" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext25" runat="server" Code="1351"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image102" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image75" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image111" runat="server" SkinID="table" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext26" runat="server" Code="1352"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image103" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<!-- option Unités-->
				<tr>
					<td width="10"><asp:Image ID="Image76" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/tabBordResultOptionUnite.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext35" runat="server" Code="1818"></cc1:adexpresstext></td>
				</tr>
				<!-- Validation OK -->
				<tr>
					<td width="10"><asp:Image ID="Image77" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext31" runat="server" Code="1005"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image112" runat="server" SkinID="ok" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext32" runat="server" Code="1336"></cc1:adexpresstext>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image37" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Calendrier d'action par version -->
				<tr>
					<td width="10"><asp:Image ID="Image78" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11Bold"><u><cc1:adexpresstext language="33" id="Adexpresstext47" runat="server" Code="1998"></cc1:adexpresstext>&nbsp;:</u></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image38" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image79" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext48" runat="server" Code="2037"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image39" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image80" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext49" runat="server" Code="2038"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image40" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image81" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext50" runat="server" Code="1810"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image82" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image113" runat="server" SkinID="result2" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext51" runat="server" Code="2024"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image83" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/alertPMbackCalendar.gif">&nbsp;</td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext52" runat="server" Code="2025"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				
				
				<tr>
					<td colSpan="2"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image84" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/APPMSynthesis.gif">&nbsp;</td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext53" runat="server" Code="2039"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				
				
				
				
				
				<!-- Sauvegarde de la recherche -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext27" runat="server" Code="771"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image8" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image85" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<cc1:AdExpressText language="33" id="AdExpressText11rightClick" runat="server" Code="2021"></cc1:AdExpressText></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image41" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image86" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext28" runat="server" Code="1337"></cc1:adexpresstext></td>
				</tr>
				
				
				<!--Sauvegarde session-->
				<tr>
					<td colSpan="2"><asp:Image ID="Image9" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image87" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image114" runat="server" SkinID="save" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext29" runat="server" Code="2451"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image42" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResult.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image43" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image88" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext67" runat="server" Code="2452"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultNewName.gif">&nbsp;&nbsp;<IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultFolderSelection.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image89" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext68" runat="server" Code="2453"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveUniverseSaveAs.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image11" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image90" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext69" runat="server" Code="2450"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image12" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				
				
				
				
				
				
				<!-- Excel print -->
				<tr>
					<td width="10"><asp:Image ID="Image91" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image115" runat="server" SkinID="print" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text4AdExpressText" runat="server" Code="1662"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image44" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Text mail -->
				<tr>
					<td width="10"><asp:Image ID="Image92" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image116" runat="server" SkinID="textExport" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext44" runat="server" Code="2032"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image45" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Excel mail -->
				<tr>
					<td width="10"><asp:Image ID="Image93" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image117" runat="server" SkinID="excelExport" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext45" runat="server" Code="2033"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image46" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- PDF export-->
				<tr>
					<td width="10"><asp:Image ID="Image94" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" valign="top" width="40"><asp:Image ID="Image118" runat="server" SkinID="pdfExport" /></td>
								<td class="txtViolet11">
									<cc1:adexpresstext language="33" id="Adexpresstext30" runat="server" Code="2034"></cc1:adexpresstext><br>
									<cc1:adexpresstext language="33" id="Adexpresstext46" runat="server" Code="2035"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext36" runat="server" Code="1820"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext37" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image119" runat="server" SkinID="valider" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext38" runat="server" Code="1821"></cc1:adexpresstext><br>
									<cc1:adexpresstext language="33" id="Adexpresstext39" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image120" runat="server" SkinID="close" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext40" runat="server" Code="1822"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image13" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- PopUp sauvegarder pdf -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image47" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/savePDF.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image48" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image95" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" valign="top" width="40"><asp:Image ID="Image104" runat="server" SkinID="pixel" width="40" /></td>
								<td class="txtViolet11">
									<cc1:adexpresstext language="33" id="Adexpresstext41" runat="server" Code="1823"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext42" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image121" runat="server" SkinID="ouvrir" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext43" runat="server" Code="1824"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image14" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Titre 3: Icônes de navigation  -->
				<TR>
					<td colSpan="2" class="txtViolet12Bold paleVioletBackGround">&nbsp;
						<cc1:AdExpressText language="33" id="titre3Adexpresstext" runat="server" Code="1010"></cc1:AdExpressText>
					</td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image15" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image96" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<cc1:AdExpressText language="33" id="text14Adexpresstext" runat="server" Code="2020"></cc1:AdExpressText>
					</td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image97" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image122" runat="server" SkinID="selected" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text15Adexpresstext" runat="server" Code="1012"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image105" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image98" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image123" runat="server" SkinID="product" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text16Adexpresstext" runat="server" Code="1013"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image106" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image99" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image124" runat="server" SkinID="media" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text17Adexpresstext" runat="server" Code="1014"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image107" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image100" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image125" runat="server" SkinID="calendar" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text18Adexpresstext" runat="server" Code="1015"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image108" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image101" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image126" runat="server" SkinID="result" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text19Adexpresstext" runat="server" Code="1016"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Bouton de fermeture -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image49" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td class="violetBackGround" colSpan="2"><asp:Image ID="Image50" runat="server" SkinID="pixel" height="1" width="100%" /></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image51" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="right" class="whiteBackGround" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
