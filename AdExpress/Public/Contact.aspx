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
	<body class="bodyStyle">
		<form id="Form2"  method="post" runat="server"> <!--onsubmit="checkForm()"-->
			<table cellSpacing="0" cellPadding="0" width="800" border="0" height="600">
				<TBODY>
					<tr>
						<!-- marge de gauche-->
						<td vAlign="top">
							<table cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<td><asp:Image ID="Image153" runat="server" SkinID="logo_cote_gauche" height="100" width="10" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="1" width="1" /></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td vAlign="top" class="imageBackGround">
							<table cellSpacing="0" cellPadding="0" width="10" border="0">
								<tr vAlign="top" class="whiteBackGround">
									<!-- Logo -->
									<td><asp:Image ID="Image3" runat="server" SkinID="logo" height="90" width="185"/></td>
								</tr>
								<tr>
									<td><asp:Image ID="Image2" runat="server" SkinID="plus_sous_logo" height="5" width="185" /></td>
								</tr>
								<tr>
									<td><asp:Image ID="Image4" runat="server" SkinID="pixel" height="10" width="1" /></td>
								</tr>
								<tr>
									<td id="debutMenu">
										<cc3:PageTitleWebControl id="PageTitleWebControl1" runat="server" CodeDescription="898" CodeTitle="764"></cc3:PageTitleWebControl>
									</td>
								</tr>
								<tr>
									<td><asp:Image ID="Image5" runat="server" SkinID="pixel" height="10" width="1" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround">
										<table id="Table4" cellSpacing="0" cellPadding="0" border="0">
											<tr>
												<td><asp:Image ID="Image6" runat="server" SkinID="pixel" height="3" width="5" /></td>
											</tr>
											<tr>
												<td class="whiteBackGround">
												<A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></A>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
							<asp:validationsummary id="ValidationSummary1" BackColor="White" ShowMessageBox="True" ShowSummary="False"
								Height="40px" runat="server" Width="168px"></asp:validationsummary>
						</td>
						<!-- Séparateur -->
						<td vAlign="top">
							<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<td><asp:Image ID="Image7" runat="server" SkinID="logo_cote_droit" height="100" width="5" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image ID="Image8" runat="server" SkinID="pixel" height="1" width="1" /></td>
								</tr>
							</table>
						</td>
						<td class="violetBackGround"><asp:Image ID="Image9" runat="server" SkinID="pixel" height="1" width="1" /></td>
						<td width="10" class="imageBackGround">&nbsp;</td>
						<td width="10" class="whiteBackGround">&nbsp;</td>
						<!-- droite-->
						<td vAlign="top">
							<table cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<!-- Menu du haut-->
									<td><cc3:headerwebcontrol language="33" id="HeaderWebControl1" runat="server"></cc3:headerwebcontrol></td>
								</tr>
								<tr>
									<!-- ligne du haut a droite -->
									<td class="imageBackGround3bis">
										<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<tr>
												<td vAlign="top" align="left"><asp:Image ID="Image10" runat="server" SkinID="croix" height="5" width="5" /></td>
												<td><asp:Image ID="Image12" runat="server" SkinID="pixel" height="17" width="1" /></td>
												<td vAlign="top" align="right"><asp:Image ID="Image11" runat="server" SkinID="croix" height="5" width="5" /></td>
											</tr>
										</table>
									</td>
								</tr>
								<!-- Centre -->
								<tr>
									<td align="center">
										<table id="data" class="darkBackGround" cellPadding="0" border="0">
											<tr>
												<td>
													<table style="MARGIN-LEFT: 5px" cellSpacing="0" cellPadding="0" width="410" border="0">
														<tr>
															<!-- fleche -->
															<td style="WIDTH: 15px" vAlign="top" rowSpan="10"><asp:Image ID="Image13" runat="server" SkinID="fleche_1" height="14" />
															</td>
															<!-- coordonnées client -->
															<td style="WIDTH: 394px" vAlign="top" class="whiteBackGround">
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
															<!-- fleche -->
															<td style="HEIGHT: 429px" vAlign="top" rowSpan="10"><asp:Image ID="Image15" runat="server" SkinID="fleche_1" height="14" />
															</td>
															<!-- renseignements contacts secodip -->
															<td style="HEIGHT: 429px" vAlign="top" class="whiteBackGround">
																<table id="tableSecodip" cellSpacing="5" border="0">
																	<tr>
																		<td class="txtNoir11Bold" style="HEIGHT: 8px" vAlign="top" rowSpan="1"><asp:Image ID="Image16" runat="server" SkinID="bande300" height="8" width="300" align="top" />
																		</td>
																	</tr>
																	<tr>
																		<td class="txtNoir11Bold"><cc1:adexpresstext language="33" id="contactAdTxt" runat="server" Code="75"></cc1:adexpresstext></td>
																	</tr>
																	<tr>
																	</tr>
																	<tr>
																	</tr>
																	<tr>
																		<td><asp:label class="txtNoir11Bold" id="name1Lab" runat="server">Pierre Marty</asp:label></td>
																	</tr>
																	<tr>
																		<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job1AdTxt" runat="server" Code="76"></cc1:adexpresstext></td>
																	</tr>
																	<tr>
																		<td><asp:label class="txtGris11Bold" id="tel1AdTxt" runat="server">01 30 74 80 27</asp:label></td>
																	</tr>
																	<tr>
																		<td><asp:label class="txtGris11Bold" id="mail1AdTxt" runat="server">pierre.marty@tns-global.com</asp:label></td>
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
																		<td><asp:label class="txtGris11Bold" id="mail12AdTxt" runat="server">sophie.lebarazer@tns-global.com</asp:label></td>
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
																		<td><asp:label class="txtGris11Bold" id="mail3AdTxt" runat="server">valerie.rombaut@tns-global.com</asp:label></td>
																	</tr>
																	<tr>
																		<td><asp:label class="txtNoir11Bold" id="name4Lab" runat="server">Eric Trousset</asp:label></td>
																	</tr>
																	<tr>
																		<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="job4AdTxt" runat="server" Code="884"></cc1:adexpresstext></td>
																	</tr>
																	<tr>
																		<td><asp:label class="txtGris11Bold" id="tel4AdTxt" runat="server">01 30 74 87 70</asp:label></td>
																	</tr>
																	<tr>
																		<td><asp:label class="txtGris11Bold" id="mail4AdTxt" runat="server">eric.trousset@tns-global.com</asp:label></td>
																	</tr>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<!-- bouton de validation -->
											<tr>
												<td align="center" colSpan="2"><cc2:imagebuttonrolloverwebcontrol id="validateButton" runat="server"></cc2:imagebuttonrolloverwebcontrol></td>
											</tr>
										</table>
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
									<TD vAlign="top"><asp:Image ID="Image17" runat="server" SkinID="croix" height="5" width="5" /></TD>
									<TD></TD>
									<TD vAlign="top" align="right"><asp:Image ID="Image18" runat="server" SkinID="croix" height="5" width="5" /></TD>
								</TR>
							</TABLE>
						</TD>
						<TD></TD>
						<TD class="violetBackGround"></TD>
						<TD class="imageBackGround"></TD>
						<TD></TD>
						<TD id="dellCel" valign="top">
							<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TR>
									<TD vAlign="top"><asp:Image ID="Image19" runat="server" SkinID="croix" height="5" width="5" /></TD>
									<TD></TD>
									<TD vAlign="top" align="right"><asp:Image ID="Image20" runat="server" SkinID="croix" height="5" width="5" /></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TBODY>
			</table>
		</form>
	</body>
</HTML>
