<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GlobalDateSelection.aspx.cs" Inherits="AdExpress.Private.Selection.GlobalDateSelection" %>
<%@ Register Assembly="obout_SlideMenu3_Pro_NET" Namespace="OboutInc.SlideMenu" TagPrefix="osm" %>
<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Selections" TagPrefix="cc5" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head id="Head1" runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script type="text/javascript" src="/scripts/jquery.js"></script> 
		<script type="text/javascript" src="/scripts/thickbox.js"></script>
		<script language="JavaScript" type="text/JavaScript">
		    var buttonName;
		    var buttonNameValue='';
		    
		    function selectedItem(i){
				ok=false;
				Form2.selectionType.value = "dateWeekComparative"; 
				Form2.disponibilityType.value = "lastDay";
				switch(i){
					case 0:
					    if(Form2.yearDateList!=null){
						    if(Form2.yearDateList.selectedIndex!=0){
							    Form2.monthDateList.options[0].selected=true;
							    if(Form2.weekDateList!=null)Form2.weekDateList.options[0].selected=true;
							    Form2.dayDateList.options[0].selected=true;
							    ok=true;
						    }
						}
						break;
					case 1:
						if(Form2.monthDateList.selectedIndex!=0){
							if(Form2.yearDateList!=null) Form2.yearDateList.options[0].selected=true;
							if(Form2.weekDateList!=null) Form2.weekDateList.options[0].selected=true;
							Form2.dayDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 2:
						if(Form2.weekDateList.selectedIndex!=0){
							if(Form2.yearDateList!=null) Form2.yearDateList.options[0].selected=true;
							Form2.monthDateList.options[0].selected=true;
							Form2.dayDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 3:
						if(Form2.dayDateList.selectedIndex!=0){
							if(Form2.yearDateList!=null) Form2.yearDateList.options[0].selected=true;
							Form2.monthDateList.options[0].selected=true;
							if(Form2.weekDateList!=null)Form2.weekDateList.options[0].selected=true;
							ok=true;
						}
						break;
				}
				if (ok==true){
					Form2.selectedItemIndex.value=i;
					Form2.previousMonthCheckbox.checked=false;
					Form2.previousYearCheckbox.checked=false;
					Form2.previousDayCheckBox.checked=false;
					if (Form2.currentYearCheckbox!=null) Form2.currentYearCheckbox.checked=false;
					if(Form2.previousWeekCheckBox!=null) Form2.previousWeekCheckBox.checked=false;
				}
			}
			function selectedCheck(i){
				if(Form2.yearDateList!=null) Form2.yearDateList.options[0].selected=true;
				Form2.monthDateList.options[0].selected=true;
				if(Form2.weekDateList!=null) Form2.weekDateList.options[0].selected=true;
				Form2.dayDateList.options[0].selected=true;
				Form2.selectedItemIndex.value=i;
				Form2.previousYearCheckbox.checked=false;
				Form2.previousMonthCheckbox.checked=false;
				Form2.previousDayCheckBox.checked=false;
				if (Form2.currentYearCheckbox!=null) Form2.currentYearCheckbox.checked=false;
				if(Form2.previousWeekCheckBox!=null) Form2.previousWeekCheckBox.checked=false;
				switch(i){
					case 4:
						Form2.previousYearCheckbox.checked=true;
						break;
					case 5:
						Form2.previousMonthCheckbox.checked=true;
						break;
					case 6:
						if(Form2.previousWeekCheckBox!=null)Form2.previousWeekCheckBox.checked=true;
						break;
					case 7:
						Form2.previousDayCheckBox.checked=true;
						break;
					case 8:
					    if (Form2.currentYearCheckbox!=null)
						    Form2.currentYearCheckbox.checked=true;
						break;
				}
			}
            function selectedRadio(i){
				switch(i){
					case 'dateWeekComparative':
						dateToDate.checked=false;
						dateWeekComparative.checked=true;
						Form2.selectionType.value = "dateWeekComparative"; 
						break;
					case 'dateToDate':
						dateWeekComparative.checked=false;
						dateToDate.checked=true;
						Form2.selectionType.value = "dateToDate"; 
						break;
					case 'lastDay':
						lastPeriod.checked=false;
						lastDay.checked=true;
						Form2.disponibilityType.value = "lastDay"; 
						break;
					case 'lastPeriod':
						lastPeriod.checked=true;
						lastDay.checked=false;
						Form2.disponibilityType.value = "lastPeriod"; 
						break;	
				}
			}
		</script>
		<script type="text/javascript" src="/scripts/DateFormat.js"></script>
	</head>
	<body onload="javascript:selectedItem(7);javascript:activateActiveX();" bottomMargin="0" leftMargin="0" topMargin="0" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table id="Table1" height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				<tr>
					<!-- marge de gauche-->
					<td valign="top" width="10">
						<table id="Table3" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image ID="Image1" runat="server" height="100" SkinID="logo_cote_gauche" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image2" runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" width="185" class="imageBackGround">
						<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
							<tr valign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image3" runat="server" height=90 SkinID="logo" /></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image4" runat="server" height="5" SkinID="plus_sous_logo" /></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image5" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr valign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="800"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image ID="Image6" runat="server" height="5" width="1" SkinID="pixel" /></td>
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
								<td><asp:Image ID="Image7" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image8" runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image ID="Image9" runat="server" height="1" width="1" SkinID="pixel" /></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td id="sertARien" valign="top">
						<table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tbody>
								<tr>
									<!-- Menu du haut-->
									<td valign="top" height="1%"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
								</tr>
								<tr>
									<!-- ligne du haut a droite -->
									<td id="lineAVIrer" valign="top" class="imageBackGround" height="1%">
										<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td valign="top" align="left" height="1%"><asp:Image ID="Image10" runat="server" height="5" width="5" SkinID="croix" /></td>
												<td height="1%"><asp:Image ID="Image11" runat="server" height="17" width="1" SkinID="pixel" /></td>
												<td valign="top" align="right" height="1%"><asp:Image ID="Image12" runat="server" height="5" width="5" SkinID="croix" /></td>
											</tr>
										</table>
									</td>
								</tr>
								<!-- Centre -->
								<tr valign="top">
									<!--debut elements-->
									<td>
										<table id="Table4" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td>
													<table id="Table9" cellspacing="0" cellpadding="3" width="100%" border="0">
														<tr>
															<td></td>
															<td class="txtViolet12Bold" colspan="2"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></td>
														</tr>
														<tr>
															<td></td>
															<td width="15"></td>
															<td>
															    <table cellpadding="0" cellspacing="0" border="0">
                                                                    <tr>
                                                                        <td>
                                                                            <cc5:GlobalCalendarWebControl ID="GlobalCalendarWebControl1" runat="server"/>
                                                                        </td>    
                                                                    </tr>
                                                                 </table>
															</td>
														</tr>
														<tr height="5">
															<td colspan="3">
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<table id="Table13" cellspacing="0" cellpadding="3" width="100%" border="0">
													
													<tr><td><table id="Table11" cellspacing="0" cellpadding="3" width="100%" border="0">
													
														<tr>
															
															<td class="txtViolet12Bold" colspan="2"><cc1:adexpresstext language="33" id="title2AdExpTxt" runat="server" Code="777"></cc1:adexpresstext></td>
														</tr>
														</table></td></tr>
														<tr><td><table id="Table14" cellspacing="0" cellpadding="3" border="0">
														<!--<tr>
														    <td></td>
														    <td>
														        <table id="Table113" cellspacing="0" cellpadding="3" width="100%" border="0">
														        <tr>
														        <td valign=top><table id="Table11" cellspacing="0" cellpadding="3" width="100%" border="0"><tr valign=top><td>
														-->
														<tr valign=top>
															<td style="HEIGHT: 18px"></td>
															<td class="txtGris11Bold" colspan="2" style="HEIGHT: 18px"><cc1:adexpresstext language="33" id="comment2AdExpressText" runat="server" Code="785"></cc1:adexpresstext></td>
														</tr>
														<tr valign=top>
														<%if (!isDynamicModule) { %>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<span id="item0" onclick="javascript:selectedItem(0)">
																	<cc3:datelistwebcontrol id="yearDateList" runat="server" CssClass="txtNoir11" ModuleType="analysis" ListTypeDisplay="year"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="yearAdExpressText" runat="server" Code="781"></cc1:adexpresstext>
															</td>
														
														<%} %>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<span onclick="javascript:selectedItem(1)">
																	<cc3:datelistwebcontrol id="monthDateList" runat="server" CssClass="txtNoir11" ModuleType="analysis" ListTypeDisplay="month"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="monthAdExpressText" runat="server" Code="783"></cc1:adexpresstext>
															</td>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<span onclick="javascript:selectedItem(2)">
																	<cc3:datelistwebcontrol id="weekDateList" runat="server" CssClass="txtNoir11" ModuleType="analysis" ListTypeDisplay="week"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="weekAdExpressText" runat="server" Code="784"></cc1:adexpresstext>
															</td>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<span onclick="javascript:selectedItem(3)">
																	<cc3:datelistwebcontrol id="dayDateList" runat="server" CssClass="txtNoir11" ModuleType="alert" ListTypeDisplay="day"></cc3:datelistwebcontrol></span>&nbsp;
																<cc1:adexpresstext language="33" id="dayAdExpressText" runat="server" Code="1974"></cc1:adexpresstext>
															</td>
														</tr>
														
														</table></td></tr>
														<!--</td></tr></table>
														</td>
														<td>
														<table id="Table14" cellspacing="0" cellpadding="3" width="100%" border="0">
														<tr><td>-->
														<tr><td><table id="Table15" cellspacing="0" cellpadding="3" border="0">
														
														<tr>
															<td></td>
															<td class="txtGris11Bold" colspan="2">
																<cc1:adexpresstext language="33" id="lastPeriodAdexpresstext" runat="server" Code="786"></cc1:adexpresstext></td>
														</tr>
														<tr>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
															<cc4:adexpresscheckbox id="currentYearCheckbox"   Code="1119" TextAlign="Right"  onClick="javascript:selectedCheck(8)" runat="server"></cc4:adexpresscheckbox>
															</td>	
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<cc4:adexpresscheckbox id="previousYearCheckbox"   Code="787" TextAlign="Right"  onClick="javascript:selectedCheck(4)" runat="server"></cc4:adexpresscheckbox>															
															</td>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<cc4:adexpresscheckbox id="previousMonthCheckbox"   Code="788" TextAlign="Right"  onClick="javascript:selectedCheck(5)" runat="server"></cc4:adexpresscheckbox>															
															</td>
														
															<td></td>
															<td width="15"></td>
															<td class="txtNoir11">
																<cc4:adexpresscheckbox id="previousWeekCheckBox"   Code="789" TextAlign="Right"  onClick="javascript:selectedCheck(6)" runat="server"></cc4:adexpresscheckbox>																																
															</td>
														
														    <td></td>
														    <td width="15"></td>
														    <td class="txtNoir11">
														        <cc4:adexpresscheckbox ID="previousDayCheckBox" Code="1975" TextAlign="Right" OnClick="javascript:selectedCheck(7)" runat="server"></cc4:adexpresscheckbox>
														    </td>
														</tr>
														</table></td></tr>
														<tr height="5">
															<td colspan="4"></td>
														</tr>
														<!--</td></tr></table>
														</td></tr></table>
														</td>
														</tr>-->
													</table>
												</td>
											</tr>
											<tr>
												<td valign="top" class="imageBackGround" height="20">
													<table id="Table12" cellspacing="0" cellpadding="0" width="100%" border="0">
														<tr>
															<td width="150"></td>
															<td>
															    <!--<cc4:imagebuttonrolloverwebcontrol id="validateButton2" runat="server" onclick="validateButton2_Click"></cc4:imagebuttonrolloverwebcontrol>-->
															    <img id="validateButton2" src="/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_up.gif" onmouseover="validateButton2.src='/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_down.gif';" onmouseout="validateButton2.src='/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_up.gif';" onclick="PostBack('validateButton2');" style="cursor:pointer"/>
															</td>
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
				<tr height="5" valign="top">
					<td></td>
					<td valign="top" class="imageBackGround">
						<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><asp:Image ID="Image13" runat="server" height="5" width="5" SkinID="croix" /></td>
								<td></td>
								<td valign="bottom" align="right"><asp:Image ID="Image14" runat="server" height="5" width="5" SkinID="croix" /></td>
								<td>
			                    </td>
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
								<td valign="bottom"><asp:Image ID="Image15" runat="server" height="5" width="5" SkinID="croix" /></td>
								<td><cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
								<td valign="bottom" align="right"><asp:Image ID="Image16" runat="server" height="5" width="5" SkinID="croix" /></td>
							</tr>
						</table>
					</td>
				</tr>				
			</table>
			<input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex"/>
			<input id="dateSelectedItem" type="hidden" value="-1" name="dateSelectedItem"/>
			<input id="selectionType" type="hidden" value="-1" name="selectionType"/>
			<input id="disponibilityType" type="hidden" value="-1" name="disponibilityType"/>
			<input style="visibility:hidden" type="hidden" id="comparativeLink" alt="#TB_inline?height=200&width=400&inlineId=myOnPageContent" title="Sélection comparative" class="thickbox" type="button" value="Show" />   
			<%=testSelection%>
			<div style="display:none;"  id="myOnPageContent" >
		         <table id="Table67" cellspacing="0" cellpadding="0" border="0" width="100%">
		            <tr>
		                <td>&nbsp;</td>
		            </tr>
		            <tr>
		                <td>
		                    <div id="comparativeDiv">
		                    <table id="comparativeTable" cellspacing="0" cellpadding="0" border="0" width="100%">
		         
		                      <tr>
		                          <td class="txtGris11Bold" colspan="2">
                                        <cc1:adexpresstext language="33" id="ComparativePeriodType1" runat="server" Code="2293"></cc1:adexpresstext></td>
                                  </td>
                              </tr>
                              <tr>
                              	<td>
		                          &nbsp;<input id="dateWeekComparative" type="radio" onClick="javascript:selectedRadio('dateWeekComparative');" checked="checked">
		                                    <font class="txtNoir11"><cc1:adexpresstext language="33" id="ComparativePeriodType1_2" runat="server" Code="2295"></cc1:adexpresstext>
		                                    </font>
		                                </input>
		                        </td>
		                      </tr>
		                      <tr>
                                <td>
		                            &nbsp;<input id="dateToDate" type="radio" onClick="javascript:selectedRadio('dateToDate');">
		                                    <font class="txtNoir11"><cc1:adexpresstext language="33" id="ComparativePeriodType1_1" runat="server" Code="2294"></cc1:adexpresstext>
		                                    </font>
		                                  </input>
		                        </td>
		                      </tr>
		            
		                    </table>
		                    </div>
		                    <div id="espaceDiv1"><br/></div>
		                    <div id="disponibilityDiv">
		                    <table id="disponibilityTable" cellspacing="0" cellpadding="0" border="0" width="100%">
		            
		                        <tr>  
		                            <td class="txtGris11Bold" colspan="2">
                                         <cc1:adexpresstext language="33" id="DisponibiltyPeriodType1" runat="server" Code="2296"></cc1:adexpresstext>
                                    </td>
                                </tr>
                                <tr>
		                            <td>
		                                &nbsp;<input id="lastDay" type="radio" onClick="javascript:selectedRadio('lastDay');" checked="checked">
		                                            <font class="txtNoir11"><cc1:adexpresstext language="33" id="DisponibiltyPeriodType1_1" runat="server" Code="2297"></cc1:adexpresstext>
		                                            </font>
		                                      </input>
		                            </td>
		                        </tr>
		                        <tr>
		                            <td>
		                             &nbsp;<input id="lastPeriod" type="radio" onClick="javascript:selectedRadio('lastPeriod');">
		                                        <font class="txtNoir11"><cc1:adexpresstext language="33" id="DisponibiltyPeriodType1_2" runat="server" Code="2298"></cc1:adexpresstext>
		                                        </font>
		                                   </input>
		                            </td>
		                        </tr>
		            
		                    </table>
		                    </div>
		                    <div id="espaceDiv2"  style="display:none;"><br/></div>
		                </td>
		            </tr>
		            <tr>
                        <td>&nbsp;</td>
		            </tr>
		            <tr>
		                <td>
		                    <table id="Table16" cellspacing="0" cellpadding="0" border="0" width="100%">
		                        <tr>
		                            <td>        
		                                <img align=right id="selectionTypeButton" src="/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_up.gif" onmouseover="selectionTypeButton.src='/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_down.gif';" onmouseout="selectionTypeButton.src='/App_Themes/<%= this.Theme %>/Images/Culture/button/valider_up.gif';" onclick="__doPostBack(''+buttonName+'',''+buttonNameValue+'');" style="cursor:pointer"/>
		                            </td>
		                        </tr>    
		            
		             
		                    </table>
		                </td>
		            </tr>
		              
		        </table>   
		    </div>
		</form>

	</body>
</html>

