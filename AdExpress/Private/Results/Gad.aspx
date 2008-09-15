<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.Gad" CodeFile="Gad.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Gad</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
		<script language="javascript" type="text/javascript">
			function OpenGad(address){
				contenu.innerHTML = "<IFRAME style=\"border-style:none;\" SRC=\"" + address + "\" HEIGHT=\"665\" WIDTH=\"890\">Désolé mais votre navigateur ne supporte pas les cadres locaux.</IFRAME>";
				contenu.leftMargin = 0;
				contenu.topMargin = 0;
				contenu.background="";
				window.resizeTo(900,700);
				
			}
		</script>
	</HEAD>
	<body id="contenu" class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR class="violetBackGround" height="14">
					<TD style="HEIGHT: 10px" width="14"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/fleche_1.gif"></TD>
					<TD class="txtBlanc11Bold" style="HEIGHT: 10px" background="/App_Themes/<%= this.Theme %>/Images/Common/bandeau_titre.gif"
						colSpan="4">&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="857"></cc1:adexpresstext>
						<asp:label id="advertiserLabel" runat="server"></asp:label></TD>
				</TR>
				<!--Société-->
				<TR style="HEIGHT: 20px">
					<TD></TD>
					<TD align="center" width="150" class="whiteBackGround"></TD>
					<TD class="txtViolet11Bold whiteBackGround" vAlign="top" noWrap width="1%" colSpan="4">&nbsp;
					<TD class="txtViolet11 whiteBackGround" vAlign="top"></TD>
				</TR>
				<TR style="HEIGHT: 20px" vAlign="top">
					<TD></TD>
					<td class="whiteBackGround" align="center" rowSpan="6" width="150"><IMG src="/App_Themes/<%= this.Theme %>/Images/Common/Gad.jpg"><BR>
						<BR>
						<A class=roll02 href="<%=linkGad%>" 
      target=_blank>
							<asp:Label id="linkGadLabel" runat="server"></asp:Label></A><BR>
						<BR>
						<A 
      class=roll02 href="<%=emailGad%>">
							<asp:Label id="emailGadLabel" runat="server"></asp:Label></A><BR>
					</td>
					<TD class="txtViolet11Bold whiteBackGround" vAlign="top" width="1%" noWrap>&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="1132"></cc1:adexpresstext>
					<td class="whiteBackGround txtViolet11Bold" valign="top" width="1%">&nbsp;:&nbsp;</td>
					</TD>
					<TD class="txtViolet11 whiteBackGround" vAlign="top"><asp:label id="companyLabel" runat="server"></asp:label></TD>
				</TR>
				<!--Adresse complete-->
				<TR vAlign="top">
					<TD></TD>
					<!--Adresse 1 / Adresse 2 / Code Postal / Ville-->
					<TD class="txtViolet11Bold whiteBackGround" noWrap>&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="1133"></cc1:adexpresstext>
					<td valign="top" class="whiteBackGround txtViolet11Bold">&nbsp;:&nbsp;</td>
					</TD>
					<TD class="txtViolet11 whiteBackGround" vAlign="top"><asp:label id="streetLabel" runat="server"></asp:label><br>
						<asp:label id="street2Label" runat="server"></asp:label><br>
						<asp:label id="codePostalLabel" runat="server"></asp:label>&nbsp;&nbsp;<asp:label id="townLabel" runat="server"></asp:label><br>
					</TD>
				</TR>
				<!--Telephone-->
				<TR vAlign="top">
					<TD></TD>
					<TD class="txtViolet11Bold whiteBackGround" noWrap>&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="1134"></cc1:adexpresstext>
					<td valign="top" class="whiteBackGround txtViolet11Bold">&nbsp;:&nbsp;</td>
					</TD>
					<TD class="txtViolet11 whiteBackGround"><asp:label id="phoneLabel" runat="server"></asp:label></TD>
				</TR>
				<!--Fax-->
				<TR vAlign="top">
					<TD></TD>
					<TD class="txtViolet11Bold whiteBackGround" noWrap>&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="1135"></cc1:adexpresstext>
					<td valign="top" class="whiteBackGround txtViolet11Bold">&nbsp;:&nbsp;</td>
					</TD>
					<TD class="txtViolet11 whiteBackGround"><asp:label id="faxLabel" runat="server"></asp:label></TD>
				</TR>
				<!--Email-->
				<TR vAlign="top">
					<TD></TD>
					<TD class="txtViolet11Bold whiteBackGround" noWrap>&nbsp;
						<cc1:adexpresstext language="33" id="AdExpressText6" runat="server" Code="1136"></cc1:adexpresstext>
					<td valign="top" class="whiteBackGround txtViolet11Bold">&nbsp;:&nbsp;</td>
					</TD>
					<TD class="txtViolet11 whiteBackGround"><asp:label id="emailLabel" runat="server"></asp:label></TD>
				</TR>
				<!--vide-->
				<TR style="HEIGHT: 50px">
					<TD></TD>
					<TD class="whiteBackGround"></TD>
					<TD class="whiteBackGround"></TD>
					<TD class="whiteBackGround" align=right valign=bottom>
						<%=_docMarketingTarget%>
					</TD>
					<!--</TD>--><!--2005MI-->
				</TR>
				<TR height="7">
					<TD colSpan="5"></TD>
				</TR>
				<!--Fermer-->
				<TR>
					<TD align="right" colSpan="5"><cc2:imagebuttonrolloverwebcontrol id="closeImageButtonRollOverWebControl" runat="server"></cc2:imagebuttonrolloverwebcontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
