<%@ Page language="c#" Inherits="AdExpress.Private.Helps.MediaInsertionsCreationsResultsHelp" CodeFile="MediaInsertionsCreationsResultsHelp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
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
								<!--td align="right"><img src="/Images/Common/button/help_up.gif" border="0"></td>--></tr>
						</table>
					</td>
				</TR>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Aide -->
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><IMG src="/Images/Common/button/print_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text15Adexpresstext" runat="server" Code="1964"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><IMG src="/Images/<%=_siteLanguage%>/Help/insertionsCustomizedLevels.gif" >&nbsp;<cc1:adexpresstext language="33" id="text1AdExpressText" runat="server" Code="1965"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td align="center" colSpan="2"><IMG src="/Images/<%=_siteLanguage%>/Help/insertionsCustomizedLevelsOpen.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1966"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="15" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<!--corbeille-->
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
				</tr>
				<tr>
					<td width="10"><IMG src="/Images/Common/pixel.gif" width="10"></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40" valign="top"><IMG src="/Images/Common/button/delete_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="1967"></cc1:adexpresstext></td>
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
								<td align="center" width="40" valign="top"><IMG src="/Images/Common/button/restore_up.gif"></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="1968"></cc1:adexpresstext><br>
									<cc1:adexpresstext language="33" id="Adexpresstext4" runat="server" Code="1969"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><IMG height="12" src="/Images/Common/pixel.gif" width="100%"></td>
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
