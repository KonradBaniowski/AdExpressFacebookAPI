<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DynamicDateSelection" CodeFile="DynamicDateSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
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
					case 1:
						if(Form2.monthDateList.selectedIndex!=0){							
							if(Form2.weekDateList != null)Form2.weekDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 2:
						if(Form2.weekDateList!=null && Form2.weekDateList.selectedIndex!=0){							
							Form2.monthDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 6:						
						Form2.monthDateList.options[0].selected=true;
						if(Form2.weekDateList != null)Form2.weekDateList.options[0].selected=true;
						ok = true;
						break;
					case 7:						
						Form2.monthDateList.options[0].selected=true;
						if(Form2.weekDateList != null)Form2.weekDateList.options[0].selected=true;
						ok = true;
						break;
				}
				if (ok==true){
					Form2.selectedItemIndex.value=i;
					Form2.currentYearCheckbox.checked=false;
					Form2.previousMonthCheckbox.checked=false;
					Form2.previousYearCheckbox.checked=false;
					if(Form2.previousWeekCheckBox!=null)Form2.previousWeekCheckBox.checked=false;
				}
			}
			function selectedCheck(i){
				Form2.monthDateList.options[0].selected=true;
				if(Form2.weekDateList != null)Form2.weekDateList.options[0].selected=true;
				Form2.selectedItemIndex.value=i;
				Form2.currentYearCheckbox.checked=false;
				Form2.previousYearCheckbox.checked=false;
				Form2.previousMonthCheckbox.checked=false;
				if(Form2.previousWeekCheckBox!=null)Form2.previousWeekCheckBox.checked=false;
				switch(i){
				case 0:
						Form2.currentYearCheckbox.checked=true;
						break;
					case 3:
						Form2.previousYearCheckbox.checked=true;
						break;
					case 4:
						Form2.previousMonthCheckbox.checked=true;
						break;
					case 5:
						if(Form2.previousWeekCheckBox!=null)Form2.previousWeekCheckBox.checked=true;
						break;
				}
			}

		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
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
									<td>
										<TABLE id="Table4" cellSpacing="0" cellPadding="0" width="100%" border="0">
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
																			<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:monthweekcalendarwebcontrol language="33" id="monthWeekCalendarBeginWebControl" runat="server" CalendarType="dateBegin"
																					ShowOnlyCompleteDate="True"></cc3:monthweekcalendarwebcontrol></p>
																		</td>
																		<td>
																			<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:monthweekcalendarwebcontrol language="33" id="monthWeekCalendarEndWebControl" runat="server" CalendarType="dateEnd"
																					ShowOnlyCompleteDate="True"></cc3:monthweekcalendarwebcontrol></p>
																		</td>
																	</tr>
																	<tr>
																		<td colspan="2" class="txtViolet11">
																			<cc1:AdExpressText language="33" id="AdExpressText1" runat="server" Code="2043"></cc1:AdExpressText></td>
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
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11"><span onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server" ListTypeDisplay="month" ModuleType="analysis"
																		CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="783"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td style="HEIGHT: 30px"></td>
															<TD style="HEIGHT: 30px" width="15"></TD>
															<TD class="txtNoir11" style="HEIGHT: 30px"><span onclick="javascript:selectedItem(2)"><cc3:datelistwebcontrol id="weekDateList" runat="server" ListTypeDisplay="week" ModuleType="analysis" CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server" Code="784"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td></td>
															<TD class="txtGris11Bold" colSpan="2"><cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11">
															<cc4:adexpresscheckbox id="currentYearCheckbox"   Code="1119" TextAlign="Right"  onClick="javascript:selectedCheck(0)" runat="server"></cc4:adexpresscheckbox>															
															</TD>	
															
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11">
															<cc4:adexpresscheckbox id="previousYearCheckbox"   Code="787" TextAlign="Right"  onClick="javascript:selectedCheck(3)" runat="server"></cc4:adexpresscheckbox>															
															</TD>															
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11">
															<cc4:adexpresscheckbox id="previousMonthCheckbox"   Code="788" TextAlign="Right"  onClick="javascript:selectedCheck(4)" runat="server"></cc4:adexpresscheckbox>															
															</TD>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD class="txtNoir11">
															<cc4:adexpresscheckbox id="previousWeekCheckBox"   Code="789" TextAlign="Right"  onClick="javascript:selectedCheck(5)" runat="server"></cc4:adexpresscheckbox>																
															</TD>
														<tr height="5">
															<td colSpan="4"></td>
														</tr>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" background="/Images/Common/dupli_fond.gif" height="35">
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
								<TD>
									<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex">
		</FORM>
	</body>
</HTML>
