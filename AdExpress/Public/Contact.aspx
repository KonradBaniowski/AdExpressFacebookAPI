<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" CodeFile="Contact.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.Contact" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
    </HEAD>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2"  method="post" runat="server"> <!--onsubmit="checkForm()"-->
			<table cellSpacing="0" cellPadding="0" width="800" border="0" height="600">
			    <!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
					<!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="backGroundBlack">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									
									<table id="Table6" cellspacing="0" cellpadding="0" width="185" border="0">
										<tr>
											<td class="backGroundBlack"><asp:Image ID="Image4" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										
										<!-- éléments variables du menus de gauche -->
										<tr valign="top">
											<td id="pageTitle">
												<cc3:PageTitleWebControl id="PageTitleWebControl1" runat="server" CodeDescription="898" CodeTitle="764"></cc3:PageTitleWebControl>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
											<table id="Table4" cellSpacing="0" cellPadding="0" border="0">
												<tr>
													<td><asp:Image ID="Image6" runat="server" SkinID="pixel" height="3" width="5" /></td>
												</tr>
												<tr>
													<td class="whiteBackGround">
													<a onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></a>
													</td>
												</tr>
											</table>
											</td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image7" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image8" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image11" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<!-- Menu haut -->
							<tr>
								<td><cc3:headerwebcontrol language="33" id="HeaderWebControl1" runat="server"></cc3:headerwebcontrol></td>
							</tr>
							<!-- Centre -->
							<tr>
								<td align="center">
									
									<table id="data" cellPadding="0" border="0">
										<tr>
											<td>
												<table style="MARGIN-LEFT: 5px" cellSpacing="0" cellPadding="0" width="410" border="0">
													<tr>
														<!-- coordonnées client -->
														<td style="WIDTH: 394px" vAlign="top" class="backGroundWhite">
															<table id="tableDetails" cellSpacing="5" border="0">
																<tr>
																	<td noWrap colSpan="2" width="100"><asp:Image ID="Image14" runat="server" SkinID="bande" width="400" /></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="nameAdTxt" runat="server" Code="67" Enabled="False"></cc1:adexpresstext><asp:requiredfieldvalidator id="nameValid" runat="server" ErrorMessage="non nonn on" ControlToValidate="nameTxt">*</asp:requiredfieldvalidator></td>
																	<td><asp:textbox id="nameTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" style="HEIGHT: 25px" width="100"><cc1:adexpresstext language="33" id="companyAdTxt" runat="server" Code="68"></cc1:adexpresstext><asp:requiredfieldvalidator id="companyValid" runat="server" ControlToValidate="companyTxt">*</asp:requiredfieldvalidator></td>
																	<td style="HEIGHT: 25px"><asp:textbox id="companyTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="adrAdTxt" runat="server" Code="69"></cc1:adexpresstext></td>
																	<td><asp:textbox id="adrTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="cpAdTxt" runat="server" Code="72"></cc1:adexpresstext></td>
																	<td><asp:textbox id="cpTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="townAdTxt" runat="server" Code="73"></cc1:adexpresstext></td>
																	<td><asp:textbox id="townTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="countryAdTxt" runat="server" Code="70"></cc1:adexpresstext></td>
																	<td><asp:textbox id="countryTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="telAdTxt" runat="server" Code="71"></cc1:adexpresstext></td>
																	<td style="HEIGHT: 26px"><asp:textbox id="telTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="txtViolet11" width="100"><asp:label class="txtViolet11Bold" id="mailLab" runat="server">E-mail</asp:label><asp:requiredfieldvalidator id="mailValid" runat="server" ControlToValidate="mailTxt">*</asp:requiredfieldvalidator><asp:regularexpressionvalidator id="mailFormatValid" runat="server" ControlToValidate="mailTxt" Width="1px" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:regularexpressionvalidator></td>
																	<td style="HEIGHT: 26px"><asp:textbox id="mailTxt" runat="server" Width="100%"></asp:textbox></td>
																</tr>
																<tr vAlign="top">
																	<td class="txtViolet11Bold" width="100"><cc1:adexpresstext language="33" id="commentAdTxt" runat="server" Code="74"></cc1:adexpresstext></td>
																	<td><asp:textbox id="commentTxt" runat="server" Width="296px" Rows="15" TextMode="MultiLine"></asp:textbox></td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
											<td vAlign="top">
												<table style="MARGIN-LEFT: 5px; MARGIN-RIGHT: 19px" cellSpacing="0" cellPadding="0">
													<tr>
														<!-- renseignements contacts secodip -->
														<td style="HEIGHT: 429px" vAlign="top" class="backGroundWhite">
															<table id="tableSecodip" cellSpacing="5" border="0" >
																<tr>
																	<td class="txtNoir11Bold" style="HEIGHT: 8px" vAlign="top" rowSpan="1"><asp:Image ID="Image16" runat="server" SkinID="bande300" height="8" width="300" align="top" />
																	</td>
																</tr>
																<tr>
																	<td class="txtNoir11Bold"><cc1:adexpresstext language="33" id="contactAdTxt" runat="server" Code="75"></cc1:adexpresstext></td>
																</tr>

																
																<tr>
																	<td><asp:label class="txtNoir11Bold" id="name2Lab" runat="server">Sophie Le Barazer</asp:label></td>
																</tr>
																<tr>
																	<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job2AdTxt" runat="server" Code="77"></cc1:adexpresstext></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="tel2AdTxt" runat="server">01 30 74 80 35</asp:label></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="mail12AdTxt" runat="server">sophie.lebarazer@kantarmedia.com</asp:label></td>
																</tr>
																
																
																<tr>
																	<td><asp:label class="txtNoir11Bold" id="name3Lab" runat="server">Valérie Rombaut</asp:label></td>
																</tr>
																<tr>
																	<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job3AdTxt" runat="server" Code="77"></cc1:adexpresstext></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="tel3AdTxt" runat="server">01 30 74 80 33</asp:label></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="mail3AdTxt" runat="server">valerie.rombaut@kantarmedia.com</asp:label></td>
																</tr>
																
																
																<tr>
																	<td><asp:label class="txtNoir11Bold" id="name1Lab" runat="server">Isabelle Guegan</asp:label></td>
																</tr>
																<tr>
																	<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job1AdTxt" runat="server" Code="77"></cc1:adexpresstext></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="tel1AdTxt" runat="server">01 30 74 80 82</asp:label></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="mail1AdTxt" runat="server">isabelle.guegan@kantarmedia.com</asp:label></td>
																</tr>
																
																
																<tr>
																	<td><asp:label class="txtNoir11Bold" id="name4Lab" runat="server">Corinne In Albon</asp:label></td>
																</tr>
																<tr>
																	<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job4AdTxt" runat="server" Code="884"></cc1:adexpresstext></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="tel4AdTxt" runat="server">01 30 74 87 97</asp:label></td>
																</tr>
																<tr>
																	<td><asp:label class="txtGris11Bold" id="mail4AdTxt" runat="server">corinne.inalbon@kantarmedia.com</asp:label></td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<!-- bouton de validation -->
										<tr>
											<td align="right"><cc2:imagebuttonrolloverwebcontrol id="validateButton" runat="server"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;</td>
											<td>&nbsp;</td>
										</tr>
									</table>
									
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
