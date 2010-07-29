<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.ProofDateSelection" CodeFile="ProofDateSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
		<meta http-equiv="expires" content="0" />
		<meta http-equiv="pragma" content="no-cache" />
		<meta content="no-cache" name="Cache-control" />
	</head>
	<body style="margin-bottom:0px; margin-left:0px; margin-top:0px;" onload="javascript:selectedItem(7);javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
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
												<cc2:informationwebcontrol id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:informationwebcontrol></td>
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
							<!-- Centre -->
							<tr>
								<td>
									
									<table id="Table4" cellspacing="0" cellpadding="0" width="100%" border="0">
									    <tr>
											<td class="txtViolet12Bold" style="padding-left:20px;"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></td>
										</tr>
										<tr>
											<td>
												<table id="Table9" cellspacing="0" cellpadding="5" width="100%" border="0">
													<tr>
														<td width="10">&nbsp;</td>
														<td>
															<table cellpadding="0" width="100%" border="0">
																<tr>
																	<td>
																		<p id="dateBegin" onclick="javascript:selectedItem(6)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarBeginWebControl" runat="server" CalendarType="dateBegin"
																				PeriodLength="23"></cc3:daycalendarwebcontrol></p>
																	</td>
																	<td>
																		<p id="dateEnd" onclick="javascript:selectedItem(7)"><cc3:daycalendarwebcontrol language="33" id="dayCalendarEndWebControl" runat="server" CalendarType="dateEnd"
																				PeriodLength="23"></cc3:daycalendarwebcontrol></p>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table id="Table11" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td width="27">&nbsp;</td>
														<td><cc4:imagebuttonrolloverwebcontrol id="validateButton1" runat="server" SkinID="validateButton" onclick="validateButton1_Click"></cc4:imagebuttonrolloverwebcontrol></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr style="height:10px;"><td></td></tr>
										
										<tr class="imageBackGround" style="height:10px;"><td></td></tr>
									    <tr style="height:10px;"><td></td></tr>
									    <tr>
										    <td class="txtViolet12Bold" style="padding-left:20px;"><cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server" Code="777"></cc1:adexpresstext></td>
										</tr>
										
										<tr>
											<td>
												<table id="Table13" cellspacing="0" cellpadding="5" width="100%" border="0">
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server" Code="785"></cc1:adexpresstext></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><span onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server" ListTypeDisplay="month" ModuleType="tvSponsorship"
																	CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
															<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="783"></cc1:adexpresstext></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><span onclick="javascript:selectedItem(2)"><cc3:datelistwebcontrol id="weekDateList" runat="server" ListTypeDisplay="week" ModuleType="tvSponsorship"
																	CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
															<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server" Code="784"></cc1:adexpresstext></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><input id="currentYear" onclick="javascript:selectedCheck(8)" type="checkbox"/><label for="currentYear"><%=currentYear%></label></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><input id="previousYear" onclick="javascript:selectedCheck(3)" type="checkbox"/><label for="previousYear"><%=previousYear%></label></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><input id="previousMonth" onclick="javascript:selectedCheck(4)" type="checkbox"/><label for="previousMonth"><%=previousMonth%></label></td>
													</tr>
													<tr>
														<td width="20">&nbsp;</td>
														<td class="txtNoir11"><input id="previousWeek" onclick="javascript:selectedCheck(5)" type="checkbox"/><label for="previousWeek"><%=previousWeek%></label></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td >
												<table id="Table12" cellspacing="0" cellpadding="5" width="100%" border="0">
													<tr>
														<td width="20">&nbsp;</td>
														<td><cc4:imagebuttonrolloverwebcontrol id="validateButton2" SkinID="validateButton" runat="server" onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex" />
		</form>
	</body>
</html>
