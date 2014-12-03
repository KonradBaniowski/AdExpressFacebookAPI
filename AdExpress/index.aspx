<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.index" CodeFile="index.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/favicon.ico" rel="SHORTCUT ICON"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" border="0">
			    <!-- Gradient -->
			    <tr>
			        <td colspan="7" height="3" class="gradientBar"></td>
			    </tr>
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image4" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height="90" width="185" SkinID="logoTNShome" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="40" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold whiteBackGround" width="185" height="9">
									<p class="paragraphePadding tableFont">
										<cc1:AdExpressText language="33" id="AdExpressText3" runat="server" Code="2844"/>&nbsp;<cc1:AdExpressText language="33" id="companyNameText" runat="server"/>&nbsp;<cc1:AdExpressText language="33" id="AdExpressText111" runat="server" Code="2845"/>
									</p>
								</td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
							
							<!-- HR -->
							<tr><td><hr/></td></tr>
							<tr>
								<td class="whiteBackGround">
								    <!-- login / password / button -->
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
							<!-- HR -->
							<tr><td><hr/></td></tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image3" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image1" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image2" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
						    <!-- Menu haut -->
							<tr>
								<td><cc3:HeaderWebControl id="HeaderWebControl1" runat="server" 
                                        Type_de_page="index"></cc3:HeaderWebControl></td>
							</tr>
							<!-- Flash Homepage -->
							<tr>
								<td style="background-color:#000000;">
								    <cc3:FlashWebControl ID="flashWebControl1" runat="server" Width="733" Height="484"></cc3:FlashWebControl>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
