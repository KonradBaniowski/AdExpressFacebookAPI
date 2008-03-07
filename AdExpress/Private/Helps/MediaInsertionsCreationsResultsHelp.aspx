<%@ Page language="c#" Inherits="AdExpress.Private.Helps.MediaInsertionsCreationsResultsHelp" CodeFile="MediaInsertionsCreationsResultsHelp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
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
					<td colSpan="2"><asp:Image ID="Image4" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Aide -->
				<tr>
					<td width="10"><asp:Image ID="Image9" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40"><asp:Image ID="Image153" runat="server" SkinID="print" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="text15Adexpresstext" runat="server" Code="1964"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image13" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image5" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/insertionsCustomizedLevels.gif" >&nbsp;<cc1:adexpresstext language="33" id="text1AdExpressText" runat="server" Code="1965"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!-- Exemple -->
				<tr>
					<td align="center" colSpan="2"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Help/insertionsCustomizedLevelsOpen.gif" ></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image6" runat="server" SkinID="pixel" width="10" /></td>
					<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1966"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image3" runat="server" SkinID="pixel" height="15" width="100%" /></td>
				</tr>
				<!--corbeille-->
				<tr>
					<td colSpan="2"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image7" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40" valign="top"><asp:Image ID="Image14" runat="server" SkinID="deleteLevels" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="1967"></cc1:adexpresstext></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image11" runat="server" SkinID="pixel" height="12" width="100%" /></td>
				</tr>
				<tr>
					<td width="10"><asp:Image ID="Image8" runat="server" SkinID="pixel" width="10" /></td>
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" width="40" valign="top"><asp:Image ID="Image15" runat="server" SkinID="restoreLevels" /></td>
								<td class="txtViolet11"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="1968"></cc1:adexpresstext><br>
									<cc1:adexpresstext language="33" id="Adexpresstext4" runat="server" Code="1969"></cc1:adexpresstext>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><asp:Image ID="Image12" runat="server" SkinID="pixel" height="12" width="100%" /></td>
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
					<td align="right" class="whiteBackGround" colSpan="2"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
