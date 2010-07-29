<%@ Page language="c#" Inherits="AdExpress.Private.Helps.ZoomMediaPlanAnalysisResultsHelp" CodeFile="ZoomMediaPlanAnalysisResultsHelp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
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
				<!-- Titre 0 (Léfende) -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="titre0Adexpresstext" runat="server" Code="1076"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image11" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/Help/legendePresent.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="textLegende1AdExpressText" runat="server" Code="1077"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image23" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image12" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/Help/legendePeriodicite.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="textLegende2AdExpressText" runat="server" Code="1078"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image24" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image13" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/Help/legendeNonPresent.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="textLegende3AdExpressText" runat="server" Code="1079"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image25" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<!-- Titre 1 -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Titre1AdExpressText" runat="server" Code="792"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image14" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text1AdExpressText" runat="server" Code="1033"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/unite.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Sauvegarde -->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="Titre2AdExpressText" runat="server" Code="1034"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Excel print -->
				<tr>
					<td width="10"><asp:Image ID="Image15" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image153" runat="server" SkinID="print" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1662"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image26" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Excel export-->
				<tr>
					<td width="10"><asp:Image ID="Image16" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image33" runat="server" SkinID="excel" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext11" runat="server" Code="1663"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Titre 3  Résultats-->
				<TR>
					<td class="txtViolet12Bold paleVioletBackGround" colSpan="2">&nbsp;
						<cc1:adexpresstext language="33" id="titre3Adexpresstext" runat="server" Code="1038"></cc1:adexpresstext></td>
				</TR>
				<tr>
					<td colSpan="2"><asp:Image ID="Image7" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image17" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text3AdExpressText" runat="server" Code="1047"></cc1:adexpresstext></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image27" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/monthExample.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image28" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<!-- Déploiement -->
				<tr>
					<td width="10"><asp:Image ID="Image18" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image34" runat="server" SkinID="mpOpen" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text4AdExpressText" runat="server" Code="1042"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image29" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image19" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image35" runat="server" SkinID="mpClose" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text5AdExpressText" runat="server" Code="1043"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image8" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image20" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text6AdExpressText" runat="server" Code="1044"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image9" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image21" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text7AdExpressText" runat="server" Code="1048"></cc1:adexpresstext>&nbsp;<img src="/App_Themes/<%= this.Theme %>/Images/Common/picto_plus.gif"><br>
						<cc1:adexpresstext language="33" id="text8AdExpressText" runat="server" Code="1049"></cc1:adexpresstext>:
					</td>
				</tr>
				<tr>
					<td colSpan="2" align="center"><asp:Image ID="Image36" runat="server" SkinID="print" /></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image22" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="text9AdExpressText" runat="server" Code="1050"></cc1:adexpresstext>&nbsp;<asp:Image ID="Image37" runat="server" SkinID="back" />&nbsp;<cc1:adexpresstext language="33" id="text10AdExpressText" runat="server" Code="1051" />
					</td>
				</tr>
				<!-- Bouton de fermeture -->
				<tr>
					<td colSpan="2"><asp:Image ID="Image30" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td class="violetBackGround" colSpan="2"><asp:Image ID="Image31" runat="server" SkinID="pixel" height="1" width="100%" /></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image32" runat="server" SkinID="pixel" height="10" width="100%" /></td>
				</tr>
				<tr>
					<td align="right" class="backGroundWhite" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
