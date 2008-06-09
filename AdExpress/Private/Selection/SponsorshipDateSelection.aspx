<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.SponsorshipDateSelection" CodeFile="SponsorshipDateSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server" >
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="JavaScript" type="text/JavaScript">
		
			function selectedItem(i){
				ok=false;
				switch(i){
					
					case 1:
						if(Form2.monthDateList.selectedIndex!=0){
							Form2.weekDateList.options[0].selected=true;							
							ok=true;
						}
						break;
					case 2:
						if(Form2.weekDateList.selectedIndex!=0){
							Form2.monthDateList.options[0].selected=true;							
							ok=true;
						}
						break;
					case 6:
						Form2.monthDateList.options[0].selected=true;
						Form2.weekDateList.options[0].selected=true;						
						ok = true;
						break;
					case 7:
						Form2.monthDateList.options[0].selected=true;
						Form2.weekDateList.options[0].selected=true;						
						ok = true;
						break;
											
				}
				if (ok==true){
					Form2.selectedItemIndex.value=i;
					Form2.previousMonth.checked=false;
					Form2.previousWeek.checked=false;
					Form2.previousYear.checked=false;
					Form2.currentYear.checked=false;
				}
			}
			
			function selectedCheck(i){
				Form2.selectedItemIndex.value=i;
				Form2.monthDateList.options[0].selected=true;
				Form2.weekDateList.options[0].selected=true;								
				Form2.previousMonth.checked=false;
				Form2.previousWeek.checked=false;
				Form2.previousYear.checked=false;
				Form2.currentYear.checked=false;
				switch(i){
					case 3:
						Form2.previousYear.checked=true;
						break;
					case 4:
						Form2.previousMonth.checked=true;
						break;
					case 5:
						Form2.previousWeek.checked=true;
						break;
					case 8:
						Form2.currentYear.checked=true;
						break;
				}
			}


		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript:selectedItem(7);" class="bodyStyle">
		<form id="Form2" method="post" runat="server" >
			<table id="Table1" height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td valign="top" width="10">
						<table id="Table3" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image runat="server"  height="100" SkinID="logo_cote_gauche"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server"  height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" width="185" class="imageBackGround">
						<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
							<tr valign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height="90" SkinID="logo" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo"/></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr valign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server"  CodeDescription="800"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
							</tr>
							<tr>
								<td><cc2:informationwebcontrol id="InformationWebControl1" runat="server" ></cc2:informationwebcontrol></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td valign="top">
						<table id="Table5" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td id="sertARien" valign="top">
						<table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
							
								<tr>
									<!-- Menu du haut-->
									<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server"  Type_de_page="generic"></cc2:headerwebcontrol></td>
								</tr>
								<tr>
									<!-- ligne du haut a droite -->
									<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
										<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td valign="top" align="left" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
												<td height="1%"><asp:Image runat="server" height="17" SkinID="pixel" width="1"/></td>
												<td valign="top" align="right" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
											</tr>
										</table>
									</td>
								</tr>
								<!-- Centre -->
								<tr>
									<!--debut elements-->
									<td>
										<table id="Table4" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td>
													<table id="Table9" cellspacing="0" cellpadding="3" width="100%" border="0">
														<tr>
															<td></td>
															<td class="txtViolet12Bold" colspan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server"  Code="776"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td>
																<table cellpadding="0" width="100%" class="whiteBackGround" border="0">
																	<tr>
																		<td>
																			<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarBeginWebControl" runat="server"  CalendarType="dateBegin"
																					PeriodLength="23"></cc3:daycalendarwebcontrol></p>
																		</td>
																		<td>
																			<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarEndWebControl" runat="server"  CalendarType="dateEnd"
																					PeriodLength="23"></cc3:daycalendarwebcontrol></p>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr height="5">
															<td colspan="3"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td valign="top" class="imageBackGround" height="35">
													<table id="Table11" cellspacing="0" cellpadding="0" width="100%" border="0">
														<tr>
															<td width="150"></td>
															<td><cc4:imagebuttonrolloverwebcontrol id="validateButton1" runat="server"  SkinID="validateButton"  onclick="validateButton1_Click"></cc4:imagebuttonrolloverwebcontrol></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<table id="Table13" cellspacing="0" cellpadding="3" width="100%" border="0">
														<tr>
															<td></td>
															<td class="txtViolet12Bold" colspan="2"><cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server"  Code="777"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td class="txtGris11Bold" colspan="2"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server"  Code="785"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><span onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server"  ListTypeDisplay="month" ModuleType="tvSponsorship"
																		CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server"  Code="783"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><span onclick="javascript:selectedItem(2)"><cc3:datelistwebcontrol id="weekDateList" runat="server"  ListTypeDisplay="week" ModuleType="tvSponsorship"
																		CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server"  Code="784"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td class="txtGris11Bold" colspan="2"><cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server"  Code="786"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><input id="currentYear" onclick="javascript:selectedCheck(8)" type="checkbox"/><label for="currentYear"><%=currentYear%></label></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><input id="previousYear" onclick="javascript:selectedCheck(3)" type="checkbox"/><label for="previousYear"><%=previousYear%></label></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><input id="previousMonth" onclick="javascript:selectedCheck(4)" type="checkbox"/><label for="previousMonth"><%=previousMonth%></label></td>
														</tr>	
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><input id="previousWeek" onclick="javascript:selectedCheck(5)" type="checkbox"/><label for="previousWeek"><%=previousWeek%></label></td>
														<tr height="5">
															<td colspan="4"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td valign="top" class="imageBackGround" height="35">
													<table id="Table12" cellspacing="0" cellpadding="0" width="100%" border="0">
														<tr>
															<td width="150"></td>
															<td><cc4:imagebuttonrolloverwebcontrol id="validateButton2" SkinID="validateButton" runat="server"  onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol></td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>
									<!--fin elements--></tr>
							
						</table>
					</td>
					<!-- la fin -->
					<td></td>
				</tr>
				<!-- ligne du bas a droite -->
				<tr height="5">
					<td></td>
					<td valign="top" class="imageBackGround">
						<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td></td>
								<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</table>
					</td>
					<td></td>
					<td class="violetBackGround"></td>
					<td class="imageBackGround"></td>
					<td></td>
					<td id="dellCel" valign="top" class="imageBackGround">
						<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td><cc2:menuwebcontrol id="MenuWebControl2" runat="server" ></cc2:menuwebcontrol></td>
								<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</table>
					</td>
				</tr>				
			</table>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex">
		</form>
	</body>
</html>
