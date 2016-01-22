<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GlobalDynamicDateSelection.aspx.cs" Inherits="AdExpress.Private.Selection.GlobalDynamicDateSelection" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<link href="/Css/Calendar.css" type="text/css" rel="stylesheet"/>
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
</head>
<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript:selectedItem(7);javascript:activateActiveX();">
    <form id="Form2" method="post" runat="server">
        <table id="Table1" height="600" cellSpacing="0" cellPadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top" width="10">
						<table id="Table3" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><img height="100" src="/Images/Common/logo_cote_gauche.gif"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" width="185" background="/Images/Common/dupli_fond.gif">
						<table id="Table6" cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<td><img height=90 src="/Images/<%=_siteLanguage%>/logo/logo.gif" ></td>
							</tr>
							<tr>
								<td><img height="5" src="/Images/Common/plus_sous_logo.gif"></td>
							</tr>
							<tr>
								<td><img height="10" src="images/pixel.gif" width="1"></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="800"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><img height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td>
									<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><img height="100" src="/Images/Common/logo_cote_droit.gif" width="5"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<td bgColor="#644883"><img height="1" src="/Images/Common/pixel.gif" width="1"></td>
					<td width="10" background="/Images/Common/dupli_fond.gif">&nbsp;</td>
					<td width="10" bgColor="#ffffff">&nbsp;</td>
					<!-- droite-->
					<td id="sertARien" vAlign="top">
						<TABLE id="Table10" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TBODY>
								<tr>
									<!-- Menu du haut-->
									<td vAlign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
								</tr>
								<tr>
									<!-- ligne du haut a droite -->
									<td id="lineAVIrer" vAlign="top" background="/Images/Common/dupli_fond.gif" height="1%">
										<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<tr>
												<td vAlign="top" align="left" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
												<td height="1%"><img height="17" src="/Images/Common/pixel.gif" width="1"></td>
												<td vAlign="top" align="right" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
											</tr>
										</table>
									</td>
								</tr>
								<!-- Centre -->
								<tr>
									<!--debut elements-->
									<td>
										<table id="Table4" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<tr>
												<td>
													<table id="Table9" cellSpacing="0" cellPadding="3" width="100%" border="0">
														<tr>
															<td></td>
															<td class="txtViolet12Bold" colSpan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td>
																<table cellPadding="0" width="100%" bgColor="#ffffff" border="0">
																	<tr>
                                                                        <td>
                                                                            <cc5:GlobalCalendarWebControl ID="GlobalCalendarWebControl1" runat="server"/>
                                                                        </td>    
                                                                    </tr>
																</table>
															</td>
														</tr>
														<tr height="5">
															<td colSpan="3"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<table id="Table13" cellSpacing="0" cellPadding="3" width="100%" border="0">
														<tr>
															<td></td>
															<TD class="txtViolet12Bold" colSpan="2"><cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server" Code="777"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td class="txtGris11Bold" colSpan="2"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server" Code="785"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11"><span onclick="javascript:selectedItem(1)"><cc3:datelistwebcontrol id="monthDateList" runat="server" ListTypeDisplay="month" ModuleType="analysis"
																		CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="783"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td style="HEIGHT: 30px"></td>
															<td style="HEIGHT: 30px" width="15"></td>
															<td class="txtNoir11" style="HEIGHT: 30px"><span onclick="javascript:selectedItem(2)"><cc3:datelistwebcontrol id="weekDateList" runat="server" ListTypeDisplay="week" ModuleType="analysis" CssClass="txtNoir11"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server" Code="784"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td class="txtGris11Bold" colSpan="2"><cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
															<cc4:adexpresscheckbox id="currentYearCheckbox"   Code="1119" TextAlign="Right"  onClick="javascript:selectedCheck(0)" runat="server"></cc4:adexpresscheckbox>															
															</td>	
															
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
															<cc4:adexpresscheckbox id="previousYearCheckbox"   Code="787" TextAlign="Right"  onClick="javascript:selectedCheck(3)" runat="server"></cc4:adexpresscheckbox>															
															</td>															
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
															<cc4:adexpresscheckbox id="previousMonthCheckbox"   Code="788" TextAlign="Right"  onClick="javascript:selectedCheck(4)" runat="server"></cc4:adexpresscheckbox>															
															</td>
														<tr>
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
															<cc4:adexpresscheckbox id="previousWeekCheckBox"   Code="789" TextAlign="Right"  onClick="javascript:selectedCheck(5)" runat="server"></cc4:adexpresscheckbox>																
															</td>
														<tr height="5">
															<td colSpan="4"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td vAlign="top" background="/Images/Common/dupli_fond.gif" height="35">
													<table id="Table12" cellSpacing="0" cellPadding="0" width="100%" border="0">
														<tr>
															<TD width="150"></td>
															<td><cc4:imagebuttonrolloverwebcontrol id="validateButton2" runat="server" onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol></td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>
									<!--fin elements--></tr>
							</tbody>
						</table>
					</td>
					<!-- la fin -->
					<td></td>
				</tr>
				<!-- ligne du bas a droite -->
				<tr height="5">
					<td></td>
					<td vAlign="top" background="/Images/Common/dupli_fond.gif">
						<table id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
								<td></td>
								<td vAlign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
							</tr>
						</table>
					</td>
					<td></td>
					<td bgColor="#644883"></td>
					<td background="/Images/Common/dupli_fond.gif"></td>
					<td></td>
					<td id="dellCel" vAlign="top" background="/Images/Common/dupli_fond.gif">
						<table id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
								<td>
									<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
								<td vAlign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex">
			<input id="dateSelectedItem" type="hidden" value="-1" name="dateSelectedItem"/>
    </form>
</body>
</html>
