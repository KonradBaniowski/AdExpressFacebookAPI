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
	<body id="contenu" class="popUpbody" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
		    <table cellSpacing="0" cellPadding="0" width="100%" height="100%" border="0">
		        <!-- Header -->
		        <tr>
			        <td class="popUpHeaderBackground popUpTextHeader">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="857"></cc1:adexpresstext>&nbsp;:&nbsp;<asp:label id="advertiserLabel" runat="server"></asp:label></td>
		        </tr>

		        <!-- Content -->
		        <tr>
			        <td style="height:100%;background-color:#FFF;padding:10;" valign="top">
			        
			            <table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				            <!--Société-->
				            <tr valign="top">
					            <td align="center" rowspan="6" width="170">
					                <img src="/App_Themes/<%= this.Theme %>/Images/Common/Gad.jpg"><BR>
						            <br>
						            <a class="roll02" href="<%=linkGad%>" target="_blank"><asp:Label id="linkGadLabel" runat="server"></asp:Label></a><br>
						            <br>
						            <a class="roll02" href="<%=emailGad%>"><asp:Label id="emailGadLabel" runat="server"></asp:Label></a><br>
					            </td>
					            <td class="txtViolet11Bold" valign="top" width="1%" noWrap>&nbsp;<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="1132"></cc1:adexpresstext></td>
					            <td class="txtViolet11Bold" valign="top" width="1%">&nbsp;:&nbsp;</td>
					            <td class="txtViolet11" valign="top"><asp:label id="companyLabel" runat="server"></asp:label></td>
				            </tr>
				            
				            <!--Adresse complete-->
				            <tr valign="top">
					            <td class="txtViolet11Bold" nowrap>&nbsp;<cc1:adexpresstext language="33" id="AdExpressText3" runat="server" Code="1133"></cc1:adexpresstext></td>
					            <td valign="top" class="txtViolet11Bold">&nbsp;:&nbsp;</td>
					            <td class="txtViolet11" valign="top"><asp:label id="streetLabel" runat="server"></asp:label><br><asp:label id="street2Label" runat="server"></asp:label><br><asp:label id="codePostalLabel" runat="server"></asp:label>&nbsp;&nbsp;<asp:label id="townLabel" runat="server"></asp:label></td>
				            </tr>
				            
				            <!--Telephone-->
				            <tr valign="top">
					            <td class="txtViolet11Bold" nowrap>&nbsp;<cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="1134"></cc1:adexpresstext></td>
					            <td valign="top" class="txtViolet11Bold">&nbsp;:&nbsp;</td>
					            <td class="txtViolet11"><asp:label id="phoneLabel" runat="server"></asp:label></td>
				            </tr>
				            
				            <!--Fax-->
				            <tr valign="top">
					            <td class="txtViolet11Bold" nowrap>&nbsp;<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="1135"></cc1:adexpresstext></td>
					            <td valign="top" class="txtViolet11Bold">&nbsp;:&nbsp;</td>
					            <td class="txtViolet11"><asp:label id="faxLabel" runat="server"></asp:label></td>
				            </tr>
				            
				            <!--Email-->
				            <tr valign="top">
					            <td class="txtViolet11Bold" nowrap>&nbsp;<cc1:adexpresstext language="33" id="AdExpressText6" runat="server" Code="1136"></cc1:adexpresstext></td>
					            <td valign="top" class="txtViolet11Bold">&nbsp;:&nbsp;</td>
					            <td class="txtViolet11"><asp:label id="emailLabel" runat="server"></asp:label>&nbsp;</td>
				            </tr>
				            
				            <!--Empty-->
				            <tr>
				                <td colspan="3">&nbsp;</td>
				            </tr>
			            </table>
			
			        </td>
		        </tr>

		        <!-- Footer -->
		        <tr>
			        <td class="popUpFooterBackground" align="right"><%=_docMarketingTarget%>&nbsp;&nbsp;</td>
		        </tr>
	        </table>
		</form>
	</body>
</HTML>
