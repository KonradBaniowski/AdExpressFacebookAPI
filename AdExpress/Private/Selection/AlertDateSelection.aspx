<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.AlertDateSelection" CodeFile="AlertDateSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<script language="JavaScript" type="text/JavaScript">
			function selectedItem(i){
				ok=false;
				switch(i){
					case 0:
						if(Form2.yearDateList.selectedIndex!=0){
							Form2.monthDateList.options[0].selected=true;
							Form2.weekDateList.options[0].selected=true;
							Form2.dayDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 1:
						if(Form2.monthDateList.selectedIndex!=0){
							Form2.weekDateList.options[0].selected=true;
							Form2.dayDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 2:
						if(Form2.weekDateList.selectedIndex!=0){
							Form2.monthDateList.options[0].selected=true;
							Form2.dayDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 6:
						Form2.monthDateList.options[0].selected=true;
						Form2.weekDateList.options[0].selected=true;
						Form2.dayDateList.options[0].selected=true;
						ok = true;
						break;
					case 7:
						Form2.monthDateList.options[0].selected=true;
						Form2.weekDateList.options[0].selected=true;
						Form2.dayDateList.options[0].selected=true;
						ok = true;
						break;
					case 8:
						if(Form2.dayDateList.selectedIndex!=0){
							Form2.monthDateList.options[0].selected=true;
							Form2.weekDateList.options[0].selected=true;
							ok=true;
						}
						break;
				}
				if (ok==true){
					Form2.selectedItemIndex.value=i;
					Form2.previousMonth.checked=false;
					Form2.previousWeek.checked=false;
					Form2.previousDay.checked=false;
				}
			}
			function selectedCheck(i){
				Form2.monthDateList.options[0].selected=true;
				Form2.weekDateList.options[0].selected=true;
				Form2.dayDateList.options[0].selected=true;
				Form2.selectedItemIndex.value=i;
				Form2.previousMonth.checked=false;
				Form2.previousWeek.checked=false;
				Form2.previousDay.checked=false;
				switch(i){
					case 4:
						Form2.previousMonth.checked=true;
						break;
					case 5:
						Form2.previousWeek.checked=true;
						break;
					case 9:
						Form2.previousDay.checked=true;
						break;
				}
			}


		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript:selectedItem(7);">
		<FORM id="Form2" method="post" runat="server">
			<TABLE id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<TR>
					<!-- marge de gauche-->
					<TD vAlign="top" width="10">
						<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG height="100" src="/Images/Common/logo_cote_gauche.gif"></TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
						</TABLE>
					</TD>
					<!-- menu -->
					<TD vAlign="top" width="185" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<TR vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<TD><IMG height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" ></TD>
							</TR>
							<TR>
								<TD><IMG height="5" src="/Images/Common/plus_sous_logo.gif"></TD>
							</TR>
							<TR>
								<TD><IMG height="10" src="images/pixel.gif" width="1"></TD>
							</TR>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="800"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td>
									<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
							</tr>
						</TABLE>
					</TD>
					<!-- Séparateur -->
					<TD vAlign="top">
						<TABLE id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD bgColor="#644883"><IMG height="1" src="/Images/Common/pixel.gif" width="1"></TD>
					<TD width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</TD>
					<TD width="10" bgColor="#ffffff">&nbsp;</TD>
					<!-- droite-->
					<TD id="sertARien" vAlign="top">
						<TABLE id="Table10" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TBODY>
								<TR>
									<!-- Menu du haut-->
									<TD vAlign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></TD>
								</TR>
								<TR>
									<!-- ligne du haut a droite -->
									<TD id="lineAVIrer" vAlign="top" background="/Images/Common/dupli_fond.gif" height="1%">
										<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<TR>
												<TD vAlign="top" align="left" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
												<TD height="1%"><IMG height="17" src="/Images/Common/pixel.gif" width="1"></TD>
												<TD vAlign="top" align="right" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								<!-- Centre -->
								<TR>
									<!--debut elements-->
									<td background="/Images/Common/dupli_fond.gif">
										<TABLE id="Table4" cellSpacing="0" cellPadding="0" width="100%" border="0" bgcolor="#ffffff">
											<TR>
												<TD>
													<TABLE id="Table9" cellSpacing="0" cellPadding="3" width="100%" border="0">
														<TR>
															<td></td>
															<TD class="txtViolet12Bold" colSpan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD>
																<table cellPadding="0" width="100%" bgColor="#ffffff" border="0">
																	<tr>
																		<td>
																			<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarBeginWebControl" runat="server" CalendarType="dateBegin"></cc3:daycalendarwebcontrol></p>
																		</td>
																		<td>
																			<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarEndWebControl" runat="server" CalendarType="dateEnd"></cc3:daycalendarwebcontrol></p>
																		</td>
																	</tr>
																</table>
															</TD>
														</TR>
														<tr height="5">
															<td colSpan="3"></td>
														</tr>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" background="/Images/Common/dupli_fond.gif" height="35">
													<TABLE id="Table11" cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD width="150"></TD>
															<TD><cc4:imagebuttonrolloverwebcontrol id="validateButton1" runat="server" onclick="validateButton1_Click"></cc4:imagebuttonrolloverwebcontrol></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD>
													<TABLE id="Table13" cellSpacing="0" cellPadding="3" width="100%" border="0">
														<TR>
															<td></td>
															<TD class="txtViolet12Bold" colSpan="2"><cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server" Code="777"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td></td>
															<TD class="txtGris11Bold" colSpan="2"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server" Code="785"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td style="HEIGHT: 23px"></td>
															<TD style="HEIGHT: 23px" width="15"></TD>
															<TD style="HEIGHT: 23px">
																<p class="txtNoir11" onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server" ListTypeDisplay="month" CssClass="txtNoir11">
																		<asp:ListItem Value="0">----</asp:ListItem>
																		<asp:ListItem Value="1">1</asp:ListItem>
																		<asp:ListItem Value="2">2</asp:ListItem>
																		<asp:ListItem Value="3">3</asp:ListItem>
																		<asp:ListItem Value="4">4</asp:ListItem>
																	</cc3:datelistwebcontrol>&nbsp;
																	<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="783"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<TR>
															<td style="HEIGHT: 24px"></td>
															<TD width="15" style="HEIGHT: 24px"></TD>
															<TD style="HEIGHT: 24px">
																<p class="txtNoir11" onclick="javascript:selectedItem(2)"><cc3:datelistwebcontrol id="weekDateList" runat="server" ListTypeDisplay="week" CssClass="txtNoir11">
																		<asp:ListItem Value="0">----</asp:ListItem>
																		<asp:ListItem Value="1">1</asp:ListItem>
																		<asp:ListItem Value="2">2</asp:ListItem>
																		<asp:ListItem Value="3">3</asp:ListItem>
																		<asp:ListItem Value="4">4</asp:ListItem>
																		<asp:ListItem Value="5">5</asp:ListItem>
																		<asp:ListItem Value="6">6</asp:ListItem>
																		<asp:ListItem Value="7">7</asp:ListItem>
																		<asp:ListItem Value="8">8</asp:ListItem>
																		<asp:ListItem Value="9">9</asp:ListItem>
																		<asp:ListItem Value="10">10</asp:ListItem>
																		<asp:ListItem Value="11">11</asp:ListItem>
																		<asp:ListItem Value="12">12</asp:ListItem>
																		<asp:ListItem Value="13">13</asp:ListItem>
																		<asp:ListItem Value="14">14</asp:ListItem>
																		<asp:ListItem Value="15">15</asp:ListItem>
																		<asp:ListItem Value="16">16</asp:ListItem>
																	</cc3:datelistwebcontrol>&nbsp;
																	<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server" Code="784"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<!--Debut N derniers jours-->
														<TR>
															<td style="HEIGHT: 24px"></td>
															<TD width="15" style="HEIGHT: 24px"></TD>
															<TD style="HEIGHT: 24px">
																<p class="txtNoir11" onclick="javascript:selectedItem(8)"><cc3:datelistwebcontrol id="dayDateList" runat="server" CssClass="txtNoir11">
																		<asp:ListItem Value="0">----</asp:ListItem>
																		<asp:ListItem Value="1">1</asp:ListItem>
																		<asp:ListItem Value="2">2</asp:ListItem>
																		<asp:ListItem Value="3">3</asp:ListItem>
																		<asp:ListItem Value="4">4</asp:ListItem>
																		<asp:ListItem Value="5">5</asp:ListItem>
																		<asp:ListItem Value="6">6</asp:ListItem>
																		<asp:ListItem Value="7">7</asp:ListItem>
																		<asp:ListItem Value="8">8</asp:ListItem>
																		<asp:ListItem Value="9">9</asp:ListItem>
																		<asp:ListItem Value="10">10</asp:ListItem>
																		<asp:ListItem Value="11">11</asp:ListItem>
																		<asp:ListItem Value="12">12</asp:ListItem>
																		<asp:ListItem Value="13">13</asp:ListItem>
																		<asp:ListItem Value="14">14</asp:ListItem>
																		<asp:ListItem Value="15">15</asp:ListItem>
																		<asp:ListItem Value="16">16</asp:ListItem>
																	</cc3:datelistwebcontrol>&nbsp;
																	<cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="1974"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<!--Fin N derniers jours-->
														<TR>
															<td></td>
															<TD class="txtGris11Bold" colSpan="2">
																<cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext>
															</TD>
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11">
																<input type="checkbox" onclick="javascript:selectedCheck(4)" id="previousMonth"><label for="previousMonth"><%=previousMonth%></label>
															</TD>
														</TR>
														<TR>
															<td style="HEIGHT: 26px"></td>
															<TD width="15" style="HEIGHT: 26px"></TD>
															<TD class="txtNoir11" style="HEIGHT: 26px">
																<input type="checkbox" onclick="javascript:selectedCheck(5)" id="previousWeek"><label for="previousWeek"><%=previousWeek%></label>
															</TD>
														</TR>
														<TR>
															<td style="HEIGHT: 26px"></td>
															<TD width="15" style="HEIGHT: 26px"></TD>
															<TD class="txtNoir11" style="HEIGHT: 26px">
																<input type="checkbox" onclick="javascript:selectedCheck(9)" id="previousDay"><label for="previousDay"><%=previousDay%></label>
															</TD>
														</TR>
														<tr height="5">
															<td colSpan="3"></td>
														</tr>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" height="35">
													<TABLE id="Table12" cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD width="150"></TD>
															<TD><cc4:imagebuttonrolloverwebcontrol id="validateButton2" runat="server" onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</td>
									<!--fin elements--></TR>
							</TBODY>
						</TABLE>
					</TD>
					<!-- la fin -->
					<TD></TD>
				</TR>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD bgColor="#644883"></TD>
					<TD background="/Images/Common/dupli_fond.gif"></TD>
					<TD></TD>
					<TD id="dellCel" vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD><cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex">
			</TABLE>
		</FORM>
	</body>
</HTML>
