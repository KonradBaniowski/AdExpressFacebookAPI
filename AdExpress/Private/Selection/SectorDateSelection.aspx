<%@ Page language="c#" Inherits="AdExpress.Private.Selection.SectorDateSelection" CodeFile="SectorDateSelection.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
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
			}
			function selectedCheck(i){
							
				switch(i){
					case 2 :
						Form2.monthDateList.options[0].selected=true;
						Form2.selectedItemIndex.value=i;	
						break;
					case 3 :
						Form2.monthDateList.options[0].selected=true;
						Form2.selectedItemIndex.value=i;	
						break;
					case 4:
						Form2.CompetitorSudy2Ckbx.checked=false;
						Form2.CompetitorSudy1Ckbx.checked=false;
						Form2.monthDateList.options[0].selected=true;
						Form2.selectedItemIndex.value=i;													
						break;
					case 8:
						Form2.selectedComparativeStudy.value=i;	
						Form2.CompetitorSudy2Ckbx.checked=false;
						Form2.CurrentYearRadioButton.checked=false;
						Form2.PreviousYearRadioButton.checked=false;
						Form2.TwoYearAgoRadioButton.checked=false;
						Form2.monthDateList.options[0].selected=true;
						Form2.selectedItemIndex.value=i;													
						break;
					case 5:
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
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript:selectedItem(7);javascript:activateActiveX();" class="bodyStyle">
		<FORM id="Form2" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" border="0">
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
												<cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="800"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc2:InformationWebControl id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:InformationWebControl></td>
										</tr>
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image6" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image7" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image8" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<!-- Center -->
							<tr>
								<td>
									
									<!--debut elements-->
									<TABLE id="Table4" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TBODY>
											<TR>
												<TD>
													<TABLE id="Table9" cellSpacing="0" cellPadding="3" width="100%" border="0">
														<TBODY>
															<TR>
																<td style="HEIGHT: 25px"></td>
																<TD class="txtViolet12Bold" style="HEIGHT: 25px" colSpan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></TD>
															</TR>
															<TR>
																<td></td>
																<TD width="15"></TD>
																<TD>
																	<table cellPadding="0" width="100%" border="0">
																		<tr>
																			<td>
																				<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:monthcalendarwebcontrol language="33" id="monthCalendarBeginWebControl" runat="server" CalendarType="dateBegin" SkinID="monthcalendarwebcontrol1"></cc3:monthcalendarwebcontrol></p>
																			</td>
																			<td>
																				<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:monthcalendarwebcontrol language="33" id="monthCalendarEndWebControl" runat="server" CalendarType="dateEnd" SkinID="monthcalendarwebcontrol1"></cc3:monthcalendarwebcontrol></p>
																			</td>
																		</tr>
																	</table>
																</TD>
															</TR>
															<tr>
																<td></td>
																<TD width="15"></TD>
																<td class="txtGris11Bold" onclick="javascript:selectedCheck(8)" colSpan="3"><cc1:adexpresstext language="33" id="CompetitorStudy1Adexpresstext" runat="server" Code="1118" Width="56px"></cc1:adexpresstext>
																	:
																	<asp:checkbox id="CompetitorSudy1Ckbx" runat="server"></asp:checkbox></td>
															</tr>
														
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 37px" vAlign="top" class="imageBackGround" height="37">
													<TABLE id="Table11" cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD width="150"></TD>
															<TD><cc4:imagebuttonrolloverwebcontrol id="validateButton1" runat="server" onclick="validateButton1_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol></TD>
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
															<td style="HEIGHT: 24px"></td>
															<TD class="txtGris11Bold" style="HEIGHT: 24px" colSpan="2"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server" Code="785"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td style="HEIGHT: 2px"></td>
															<TD style="HEIGHT: 2px" width="15"></TD>
															<TD style="HEIGHT: 2px">
																<p class="txtNoir11" onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server" ModuleType="recap" CssClass="txtNoir11" ListTypeDisplay="month">
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
																	</cc3:datelistwebcontrol>&nbsp;
																	<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="1846"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<TR>
															<td></td>
															<TD class="txtGris11Bold" colSpan="2"><cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext></TD>
														</TR>
														<TR>
															<td style="HEIGHT: 23px"></td>
															<TD style="HEIGHT: 23px" width="15"></TD>
															<TD style="HEIGHT: 23px">
																<p class="txtNoir11" onclick="javascript:selectedCheck(2)"><asp:radiobutton id="CurrentYearRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																	<cc1:adexpresstext language="33" id="CurrentYearAdExpressText" runat="server" Code="1119"></cc1:adexpresstext>&nbsp;(<%=downloadDate%>)</p>
															</TD>
														</TR>
														<TR>
															<td style="HEIGHT: 26px"></td>
															<TD style="HEIGHT: 26px" width="15"></TD>
															<TD style="HEIGHT: 26px">
																<p class="txtNoir11" onclick="javascript:selectedCheck(3)"><asp:radiobutton id="PreviousYearRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																	<cc1:adexpresstext language="33" id="PreviousYearAdExpressText" runat="server" Code="1121"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD>
																<p class="txtNoir11" onclick="javascript:selectedCheck(4)"><asp:radiobutton id="TwoYearAgoRadioButton" runat="server" GroupName="SectorYearChoice"></asp:radiobutton>&nbsp;
																	<cc1:adexpresstext language="33" id="TwoYearAgoAdexpresstext" runat="server" Code="1120"></cc1:adexpresstext></p>
															</TD>
														</TR>
														<TR>
															<td></td>
															<TD class="txtGris11Bold" style="HEIGHT: 17px" onclick="javascript:selectedCheck(5)"
																colSpan="2"><cc1:adexpresstext language="33" id="CompetitorStudy2Adexpresstext" runat="server" Code="1118" Width="24px"></cc1:adexpresstext>
																:
																<asp:checkbox id="CompetitorSudy2Ckbx" runat="server" CssClass="txtNoir11"></asp:checkbox></TD>
														</TR>
														<TR>
															<td></td>
															<TD width="15"></TD>
															<TD></TD>
														</TR>
														<tr height="5">
															<td colSpan="3"></td>
														</tr>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" class="imageBackGround" height="35">
													<TABLE id="Table12" cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD width="150"></TD>
															<TD><cc4:imagebuttonrolloverwebcontrol id="validateButton2" runat="server" onclick="validateButton2_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<tr><td><cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td></tr>
										</TBODY>
									</TABLE>
									
								</td>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex">
			<input id="selectedComparativeStudy" type="hidden" value="-1" name="selectedComparativeStudy">
		</FORM>
	</body>
</HTML>
