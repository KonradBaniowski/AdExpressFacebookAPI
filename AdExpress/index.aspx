<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.index" CodeFile="index.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/favicon.ico" rel="SHORTCUT ICON"/>
		<script language="javascript" type="text/javascript" src="/scripts/CookiesJScript.js"></script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<script language="javascript" type="text/javascript">
				var language = <%=_siteLanguage%>;
				<%=_setLanguage%>
				var cook = GetCookie('language');
				if (cook != null){
					if (language != cook){
						document.location="/index.aspx?sitelanguage="+cook;
					}
				}
			</script>
			<table cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" width="10" SkinID="logo_cote_gauche" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" class="imageBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height="90" width="185" SkinID="logoTNShome" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" width="185" SkinID="plus_sous_logo" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="40" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td class="txtNoir11Bold whiteBackGround" width="185" height="9">
									<p class="paragraphePadding tableFont">
										<cc1:AdExpressText language="33" id="AdExpressText3" runat="server" Code="821"></cc1:AdExpressText>
									</p>
								</td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround">
									<table id="Table4" cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td colspan="2"><asp:Image runat="server" height="5" width="6" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td><asp:Image runat="server" height="1" width="5" SkinID="pixel" /></td>
											<td><asp:textbox id="loginTextbox" runat="server" CssClass="txtNoir11Bold"></asp:textbox></td>
										</tr>
										<tr>
											<td colspan="2"><asp:Image runat="server" height="5" width="6" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td><asp:Image runat="server" height="1" width="5" SkinID="pixel" /></td>
											<td><asp:textbox id="passwordTextbox" runat="server" CssClass="txtNoir11Bold" TextMode="Password"></asp:textbox></td>
										</tr>
										<tr>
											<td colspan="2"><asp:Image runat="server" height="10" width="6" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td><asp:Image runat="server" height="1" width="5" SkinID="pixel" /></td>
											<td><cc2:imagebuttonrolloverwebcontrol id="ImageButtonRollOverWebControl1" runat="server" SkinID="validateButton" onclick="ImageButtonRollOverWebControl1_Click"></cc2:imagebuttonrolloverwebcontrol></td>
										</tr>
										<tr>
											<td colspan="2"><asp:Image runat="server" height="5" width="6" SkinID="pixel" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td  width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td>
									<cc3:HeaderWebControl id="HeaderWebControl1" runat="server"></cc3:HeaderWebControl></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td class="dupli3BackGround">
									<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
											<td><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></td>
											<td vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
								    <cc3:FlashWebControl ID="flashWebControl1" runat="server"></cc3:FlashWebControl>
								</td>
							</tr>
						</table>
					</td>
					<!-- la fin -->
					<td></td>
				</tr>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD valign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD class="violetBackGround"></TD>
					<TD class="imageBackGround"></TD>
					<TD></TD>
					<TD id="dellCel" valign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
