<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DashBoardDateSelection" CodeFile="DashBoardDateSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="JavaScript" type="text/JavaScript">
			function selectedItem(i){							
				Form2.selectedItemIndex.value=i;
				Form2.CurrentYearRadioButton.checked=false;
				Form2.PreviousYearRadioButton.checked=false;	
				Form2.TwoYearAgoRadioButton.checked=false;	
				Form2.CompetitorSudy1Ckbx.checked=false;
				Form2.LastLoadedMonthRadiobutton.checked=false;	
				Form2.LastLoadedWeekRadioButton.checked=false;							
			}
			
			function selectedCheck(i){
							
				switch(i){
					case 1 :
						Form2.CompetitorSudy1Ckbx.checked=false;	
						Form2.selectedItemIndex.value=i;
						break;
					case 9 :
						Form2.CompetitorSudy1Ckbx.checked=false;	
						Form2.selectedItemIndex.value=i;
						break;	
					case 2 :					
						Form2.selectedItemIndex.value=i;	
						break;
					case 3 :					
						Form2.selectedItemIndex.value=i;	
						break;
					case 4 :
						Form2.CompetitorSudy2Ckbx.checked=false;
						Form2.CompetitorSudy1Ckbx.checked=false;						
						Form2.selectedItemIndex.value=i;													
						break;
					case 8 :
						Form2.selectedComparativeStudy.value=i;	
						Form2.CompetitorSudy2Ckbx.checked=false;
						Form2.CurrentYearRadioButton.checked=false;
						Form2.PreviousYearRadioButton.checked=false;
						Form2.TwoYearAgoRadioButton.checked=false;
						Form2.LastLoadedMonthRadiobutton.checked=false;	
						Form2.LastLoadedWeekRadioButton.checked=false;						
						Form2.selectedItemIndex.value=i;													
						break;
					case 5 :
						Form2.TwoYearAgoRadioButton.checked=false;
						Form2.CompetitorSudy1Ckbx.checked=false;
						Form2.selectedComparativeStudy.value=i;																
						break;
					
				}
			}

		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body style="margin-bottom:0px; margin-left:0px; margin-top:0px;" onload="javascript:selectedItem(7);" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
					<tr>
						<!-- marge de gauche-->
						<td valign="top" width="10">
							<table id="Table3" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
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
									<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" /></td>
								</tr>
								<tr>
									<td><asp:Image runat="server" height="10" SkinID="pixel" width="1" /></td>
								</tr>
								<!-- éléments variables du menus de gauche -->
								<tr valign="top">
									<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="800"></cc2:moduletitlewebcontrol></td>
								</tr>
								<!-- Info bt droit -->
								<tr>
									<td><asp:Image runat="server" height="5" SkinID="pixel" width="1" /></td>
								</tr>
								<tr>
									<td>
										<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
								</tr>
							</table>
						</td>
						<!-- Séparateur -->
						<td valign="top">
							<table id="Table5" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5" /></td>
								</tr>
								<tr>
									<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
								</tr>
							</table>
						</td>
						<td class="violetBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1" /></td>
						<td width="10" class="imageBackGround">&nbsp;</td>
						<td width="10" class="whiteBackGround">&nbsp;</td>
						<!-- droite-->
						<td id="sertARien" valign="top">
							<table id="Table10" style="height:100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								
									<tr>
										<!-- Menu du haut-->
										<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
									</tr>
									<tr>
										<!-- ligne du haut a droite -->
										<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
											<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td valign="top" align="left" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
													<td height="1%"><asp:Image runat="server" height="17" SkinID="pixel" width="1" /></td>
													<td valign="top" align="right" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
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
																<tbody>
																	<tr>
																		<td style="HEIGHT: 25px"></td>
																		<td class="txtViolet12Bold" style="HEIGHT: 25px" colspan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></td>
																	</tr>
																	<tr>
																		<td></td>
																		<td width="15"></td>
																		<td>
																			<table cellpadding="0" width="100%" class="whiteBackGround" border="0">
																				
																					<tr>
																						<td>
																							<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:monthweekcalendarwebcontrol language="33" id="monthWeekCalendarBeginWebControl" runat="server" CalendarType="dateBegin" SkinID="MonthWeekCalendarWebControl1" 
																									ShowOnlyCompleteDate="True"></cc3:monthweekcalendarwebcontrol></p>
																						</td>
																						<td>
																							<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:monthweekcalendarwebcontrol language="33" id="monthWeekCalendarEndWebControl" runat="server" CalendarType="dateEnd" SkinID="MonthWeekCalendarWebControl1" 
																									ShowOnlyCompleteDate="True"></cc3:monthweekcalendarwebcontrol></p>
																						</td>
																					</tr>
																				
																			</table>
																		</td>
																	</tr>
																	<tr>
																		<td></td>
																		<td width="15"></td>
																		<td class="txtGris11Bold" onclick="javascript:selectedCheck(8)" colspan="3"><cc1:adexpresstext language="33" id="CompetitorStudy1Adexpresstext" runat="server" Code="1118" Width="56px"></cc1:adexpresstext>:
																			<asp:checkbox id="CompetitorSudy1Ckbx" runat="server"></asp:checkbox></td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
													<tr>
														<td style="HEIGHT: 37px" valign="top" class="imageBackGround" height="37">
															<table id="Table11" cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr>
																	<td width="150"></td>
																	<td><cc4:imagebuttonrolloverwebcontrol id="validateButton1" SkinID="validateButton" runat="server" onclick="validateButton1_Click"></cc4:imagebuttonrolloverwebcontrol></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td class="txtViolet12Bold">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server" Code="777"></cc1:adexpresstext></td>
													</tr>
													<tr>
														<td>
															<table id="Table13" cellspacing="0" cellpadding="2" width="100%" border="0">
																<tr>
																	<td style="HEIGHT: 26px" width="50"></td>
																	<!--<td style="HEIGHT: 26px" width="15"></td>-->
																	<td style="HEIGHT: 26px" colspan="2">
																		<p class="txtNoir11" onclick="javascript:selectedCheck(9)"><asp:radiobutton id="LastLoadedWeekRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																			<cc1:adexpresstext language="33" id="LastLoadedWeekText" runat="server" Code="1618"></cc1:adexpresstext></p>
																	</td>
																</tr>
																<tr>
																	<td style="HEIGHT: 26px"></td>
																	<!--<td style="HEIGHT: 26px" width="15"></td>-->
																	<td style="HEIGHT: 26px" colspan="2">
																		<p class="txtNoir11" onclick="javascript:selectedCheck(1)"><asp:radiobutton id="LastLoadedMonthRadiobutton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																			<cc1:adexpresstext language="33" id="LastLoadedMonthText" runat="server" Code="1619"></cc1:adexpresstext></p>
																	</td>
																</tr>
																<tr>
																	<td style="HEIGHT: 23px"></td>
																	<!--<td style="HEIGHT: 23px" width="15"></td>-->
																	<td style="HEIGHT: 23px" colspan="2">
																		<p class="txtNoir11" onclick="javascript:selectedCheck(2)"><asp:radiobutton id="CurrentYearRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																			<cc1:adexpresstext language="33" id="CurrentYearAdExpressText" runat="server" Code="1119"></cc1:adexpresstext>&nbsp;(<%=DateTime.Now.Year%>
																			)</p>
																	</td>
																</tr>
																<tr>
																	<td style="HEIGHT: 26px"></td>
																	<!--<td style="HEIGHT: 26px" width="15"></td>-->
																	<td style="HEIGHT: 26px" colspan="2">
																		<p class="txtNoir11" onclick="javascript:selectedCheck(3)"><asp:radiobutton id="PreviousYearRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																			<cc1:adexpresstext language="33" id="PreviousYearAdExpressText" runat="server" Code="1121"></cc1:adexpresstext></p>
																	</td>
																</tr>
																<tr>
																	<td></td>
																	<!--<td width="5"></td>-->
																	<td colspan="2">
																		<p class="txtNoir11" onclick="javascript:selectedCheck(4)"><asp:radiobutton id="TwoYearAgoRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																			<cc1:adexpresstext language="33" id="TwoYearAgoAdexpresstext" runat="server" Code="1120"></cc1:adexpresstext></p>
																	</td>
																</tr>
																<tr>
																	<td></td>
																	<td class="txtGris11Bold" style="HEIGHT: 17px" onclick="javascript:selectedCheck(5)"
																		colspan="2"><cc1:adexpresstext language="33" id="CompetitorStudy2Adexpresstext" runat="server" Code="1118" Width="24px"></cc1:adexpresstext>:
																		<asp:checkbox id="CompetitorSudy2Ckbx" runat="server" CssClass="txtNoir11"></asp:checkbox></td>
																</tr>
																<tr>
																	<td></td>
																	<td width="15"></td>
																	<td></td>
																</tr>
																<tr style="height:5px">
																	<td colspan="3"></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td valign="top" class="imageBackGround" height="35">
															<table id="Table12" cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr>
																	<td width="150"></td>
																	<td><cc4:imagebuttonrolloverwebcontrol id="validateButton2" SkinID="validateButton" runat="server" onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol></td>
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
					<tr style="height:5px">
						<td></td>
						<td valign="top" class="imageBackGround">
							<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
									<td></td>
									<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
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
									<td valign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
									<td>
										<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
									<td valign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5" /></td>
								</tr>
							</table>
						</td>
					</tr>
					
			  </table>
			  <input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex" />
			  <input id="selectedComparativeStudy" type="hidden" value="-1" name="selectedComparativeStudy" />
		</form>
		<!--<P></P>
		</tr></TBODY></table></tr></TBODY></table></tr></TBODY></table></tr></TBODY></table></tr></TBODY></table></FORM>--><!--2005MI-->
	</body>
</html>
