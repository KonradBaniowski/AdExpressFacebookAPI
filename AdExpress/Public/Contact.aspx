<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" CodeFile="Contact.aspx.cs" AutoEventWireup="false" Inherits="AdExpress.Public.Contact" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
	</style>
		<script language="JavaScript" type="text/JavaScript">
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">

		<meta http-equiv="expires" content="0">

		<meta http-equiv="pragma" content="no-cache">

		<meta name="Cache-control" content="no-cache">
</HEAD>
	<body>
		<form id="Form2"  method="post" runat="server"> <!--onsubmit="checkForm()"-->
			<table cellSpacing="0" cellPadding="0" width="800" border="0" height="600">
				<TBODY>
					<tr>
						<!-- marge de gauche-->
						<td vAlign="top">
							<table cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<td><IMG height="100" src="/Images/Common/logo_cote_gauche.gif" width="10"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
							</table>
						</td>
						<!-- menu -->
						<td vAlign="top" background="/Images/Common/dupli_fond.gif">
							<table cellSpacing="0" cellPadding="0" width="10" border="0">
								<tr vAlign="top" bgColor="#ffffff">
									<!-- Logo -->
									<td><IMG height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" width=185 ></td>
								</tr>
								<tr>
									<td><IMG height="5" src="/Images/Common/plus_sous_logo.gif" width="185"></td>
								</tr>
								<tr>
									<td><IMG height="10" src="/Images/pixel.gif" width="1"></td>
								</tr>
								<tr>
									<td id="debutMenu">
										<cc3:PageTitleWebControl id="PageTitleWebControl1" runat="server" CodeDescription="898" CodeTitle="764"></cc3:PageTitleWebControl>
									</td>
								</tr>
								<tr>
									<td><IMG height="10" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff">
										<table id="Table4" cellSpacing="0" cellPadding="0" border="0">
											<tr>
												<td><IMG height="3" src="/Images/Common/pixel.gif" width="5">                                                    
                                                </td>
											</tr>
											<tr>
												<td bgColor="#ffffff">
													<a href="javascript:history.back()" onmouseover="bouton.src='/Images/<%=_siteLanguage%>/button/back_down.gif';" onmouseout="bouton.src = '/Images/<%=_siteLanguage%>/button/back_up.gif';">
														<img src="/Images/<%=_siteLanguage%>/button/back_up.gif" border=0 name=bouton></a>
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
									<td><IMG height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></td>
								</tr>
							</table>
						</td>
						<td bgColor="#644883"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></td>
						<td width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</td>
						<td width="10" bgColor="#ffffff">&nbsp;</td>
						<!-- droite-->
						<td vAlign="top">
							<table cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<!-- Menu du haut-->
									<td><cc3:headerwebcontrol language="33" id="HeaderWebControl1" runat="server"></cc3:headerwebcontrol></td>
								</tr>
								<tr>
									<!-- ligne du haut a droite -->
									<td background="/Images/Common/dupli_3bis.gif">
										<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<tr>
												<td vAlign="top" align="left"><IMG height="5" src="/Images/Common/croix.gif" width="5"></td>
												<td><IMG height="17" src="/Images/Common/pixel.gif" width="1"></td>
												<td vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></td>
											</tr>
										</table>
									</td>
								</tr>
								<!-- Centre -->
								<tr>
									<td align="center">
										<table id="data" style="BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x"
											cellPadding="0" border="0">
											<tr>
												<td>
													<table style="MARGIN-LEFT: 5px" cellSpacing="0" cellPadding="0" width="410" border="0">
														<tr>
															<!-- fleche -->
															<td style="WIDTH: 15px" vAlign="top" rowSpan="10"><IMG id="f1" height="14" src="/Images/Common/fleche_1.gif" border="0">
															</td>
															<!-- coordonnées client -->
															<td style="WIDTH: 394px" vAlign="top" bgColor="#ffffff">
																<table id="tableDetails" cellSpacing="5" border="0">
																	<tr>
																		<td noWrap colSpan="2" width="100"><IMG src="/Images/Common/bande.gif" width="400"></td>
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
															<td style="HEIGHT: 429px" vAlign="top" rowSpan="10"><IMG id="f2" height="14" src="/Images/Common/fleche_1.gif" border="0">
															</td>
															<!-- renseignements contacts secodip -->
															<td style="HEIGHT: 429px" vAlign="top" bgColor="#ffffff">
																<table id="tableSecodip" cellSpacing="5" border="0">
																	<tr>
																		<td class="txtNoir11Bold" style="HEIGHT: 8px" vAlign="top" rowSpan="1"><IMG height="8" src="/Images/Common/bande300.gif" width="300" align="top">
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
						<TD valign="top" background="/Images/Common/dupli_fond.gif">
							<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TR>
									<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
									<TD></TD>
									<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								</TR>
							</TABLE>
						</TD>
						<TD></TD>
						<TD bgColor="#644883"></TD>
						<TD background="/Images/Common/dupli_fond.gif"></TD>
						<TD></TD>
						<TD id="dellCel" valign="top">
							<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<TR>
									<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
									<TD></TD>
									<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TBODY>
			</table>
		</form>
	</body>
</HTML>
