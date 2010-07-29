<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProofResultsHelp.aspx.cs" Inherits="Private_Helps_ProofResultsHelp" %>
<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Translation" TagPrefix="cc1" %>
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
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body class="darkBackGround backGroundWhite" bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
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
					<td colSpan="2"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Explication -->
				<tr>
					<td width="10"><asp:Image ID="Image17" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="2228"></cc1:adexpresstext><br>
						<cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="1810"></cc1:adexpresstext><br>
						<cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/lienCheminDeFer.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="1811"></cc1:adexpresstext>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image40" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Titre : Pagination des résultats -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="2131"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image18" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="2136"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image41" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/nbPage.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image19" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="2137"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image42" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/repeatOption.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image20" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext13" runat="server" Code="2138"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Titre : Rappel de la sélection -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext14" runat="server" Code="2132"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image21" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image153" runat="server" SkinID="bt_detail" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext16" runat="server" Code="2135"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image43" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/detailSelectionDiv.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image8" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Sauvegarde de la recherche -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext27" runat="server" Code="771"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image9" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image22" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<cc1:AdExpressText language="33" id="AdExpressText11rightClick" runat="server" Code="2021"></cc1:AdExpressText></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image44" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image23" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext28" runat="server" Code="1337"></cc1:adexpresstext></td>
				</tr>
				
				
				<!--Sauvegarde session-->
				<tr>
					<td colSpan="2"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image24" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image55" runat="server" SkinID="save" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext31" runat="server" Code="2451"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image45" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResult.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image46" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image28" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext67" runat="server" Code="2452"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultNewName.gif">&nbsp;&nbsp;<IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveResultFolderSelection.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image11" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image29" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext68" runat="server" Code="2453"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/PopUpSaveUniverseSaveAs.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image12" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><asp:Image ID="Image30" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext69" runat="server" Code="2450"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image13" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				
				
				<!-- Excel print -->
				<tr>
					<td width="10"><asp:Image ID="Image31" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image56" runat="server" SkinID="print" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text4AdExpressText" runat="server" Code="1662"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image47" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- PDF export-->
				<tr>
					<td width="10"><asp:Image ID="Image32" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" valign="top" width="40"><asp:Image ID="Image57" runat="server" SkinID="pdfExport" /></td>
								<td class="txtViolet11">
									<cc1:adexpresstext language="33" id="Adexpresstext30" runat="server" Code="2229"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext36" runat="server" Code="1820"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext37" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image58" runat="server" SkinID="valider" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext38" runat="server" Code="1821"></cc1:adexpresstext><br>
									<cc1:adexpresstext language="33" id="Adexpresstext39" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image59" runat="server" SkinID="close" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext40" runat="server" Code="1822"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image14" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- PopUp sauvegarder pdf -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image48" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/savePDF.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image49" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image33" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" valign="top" width="40"><asp:Image ID="Image50" runat="server" SkinID="pixel" width="40" /></td>
								<td class="txtViolet11">
									<cc1:adexpresstext language="33" id="Adexpresstext41" runat="server" Code="1823"></cc1:adexpresstext><br>
									<br>
									<cc1:adexpresstext language="33" id="Adexpresstext42" runat="server" Code="997"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image60" runat="server" SkinID="ouvrir" />&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext43" runat="server" Code="1824"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image15" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Titre 3: Icones de navigation  -->
				<TR>
					<td colSpan="2" class="txtViolet12Bold paleVioletBackGround">&nbsp;
						<cc1:AdExpressText language="33" id="titre3Adexpresstext" runat="server" Code="1010"></cc1:AdExpressText>
					</td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image16" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image34" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11">
						<cc1:AdExpressText language="33" id="text14Adexpresstext" runat="server" Code="2020"></cc1:AdExpressText>
					</td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image35" runat="server" SkinID="pixel" width="10" /></td>
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
					<td colSpan="2"><asp:Image ID="Image51" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image36" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image61" runat="server" SkinID="product" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text16Adexpresstext" runat="server" Code="1013"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image52" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image37" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image62" runat="server" SkinID="calendar" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text18Adexpresstext" runat="server" Code="1015"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				
				<tr>
					<td colSpan="2"><asp:Image ID="Image53" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image38" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image63" runat="server" SkinID="slogan" /></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="AdExpressText4" runat="server" Code="1961"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				
				<tr>
					<td colSpan="2"><asp:Image ID="Image54" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image39" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><asp:Image ID="Image64" runat="server" SkinID="result" /></td>
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
</HTML>