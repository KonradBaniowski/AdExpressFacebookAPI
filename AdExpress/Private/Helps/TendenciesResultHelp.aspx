<%@ Page language="c#" Inherits="AdExpress.Private.Helps.TendenciesResultHelp" CodeFile="TendenciesResultHelp.aspx.cs" %>
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x"
		bottomMargin="25" bgColor="#ffffff" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="98%" align="center" bgColor="#ffffff" border="0">
				<TR>
					<td colSpan="2">
						<table cellPadding="0" width="100%" bgColor="#644883" border="0">
							<tr>
								<td align="left"><font class="txtBlanc14Bold">&nbsp;
										<cc1:adexpresstext language="33" id="titleAdExpressText" runat="server" Code="993"></cc1:adexpresstext></font></td>
							</tr>
						</table>
					</td>
				</TR>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Titre 1 : Options d'analyse-->
				<TR>
					<td class="txtViolet12Bold" bgColor="#ded8e5" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Titre1AdExpressText" runat="server" Code="792"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!--AIDE TENDANCES-->
				<!-- Texte Option d'analyse -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1620"></cc1:adexpresstext>
						<br>
						<IMG src="/Images/<%=_siteLanguage%>/Help/cumulDate.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="1621"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="1622"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/Images/<%=_siteLanguage%>/Help/tendancesPeriodes.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- pdm -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><IMG src="/Images/<%=_siteLanguage%>/Help/annDynPDM.gif" >&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext4" runat="server" Code="1441"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Sauvegarde de la recherche -->
				<TR>
					<td class="txtViolet12Bold" bgColor="#ded8e5" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Adexpresstext15" runat="server" Code="771"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11">
						<cc1:AdExpressText language="33" id="AdExpressText11rightClick" runat="server" Code="2021"></cc1:AdExpressText></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext16" runat="server" Code="1337"></cc1:adexpresstext></td>
				</tr>
				
				
				<!--Sauvegarde session-->
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/save_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext31" runat="server" Code="2451"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/Images/<%=_siteLanguage%>/Help/PopUpSaveResult.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext67" runat="server" Code="2452"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/Images/<%=_siteLanguage%>/Help/PopUpSaveResultNewName.gif">&nbsp;&nbsp;<IMG src="/Images/<%=_siteLanguage%>/Help/PopUpSaveResultFolderSelection.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext68" runat="server" Code="2453"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple Image -->
				<tr>
					<td colSpan="2" align="center"><IMG src="/Images/<%=_siteLanguage%>/Help/PopUpSaveUniverseSaveAs.gif"></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Texte -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext69" runat="server" Code="2450"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				
				
				
				<!-- Excel print -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/print_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text4AdExpressText" runat="server" Code="1662"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Titre : Icônes de navigation  -->
				<TR>
					<td class="txtViolet12Bold" bgColor="#ded8e5" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="titreNAdexpresstext" runat="server" Code="1010"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text14Adexpresstext" runat="server" Code="2020"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/loupe_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext5" runat="server" Code="1012"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/universe_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text16Adexpresstext" runat="server" Code="1013"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/media_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text17Adexpresstext" runat="server" Code="1014"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/calendar_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text18Adexpresstext" runat="server" Code="1015"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><IMG src="/Images/Common/button/result_up.gif"></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="text19Adexpresstext" runat="server" Code="1016"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td width="40" align="center"><IMG src="/Images/Common/button/universe_up.gif"></td>
								<td class="txtViolet11">
									<cc1:AdExpressText language="33" id="Adexpresstext6" runat="server" Code="1344"></cc1:AdExpressText>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Bouton de fermeture -->
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td colSpan="2" bgColor="#644883"><IMG height="1" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="10" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td bgColor="#ffffff" align="right" colspan="2">
						<a href="javascript:window.close();" onmouseover="bouton.src='/Images/<%=_siteLanguage%>/button/fermer_down.gif';" onmouseout="bouton.src = '/Images/<%=_siteLanguage%>/button/fermer_up.gif';">
							<img src="/Images/<%=_siteLanguage%>/button/fermer_up.gif" border=0 name=bouton></a>&nbsp;&nbsp;
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
