<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GlobalDateSelectionComparative.aspx.cs" Inherits="AdExpress.Private.Selection.GlobalDateSelectionComparative" %>
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
		    var buttonNameValue = '';

		    function selectedItem(i) {
		        ok = false;
		        Form2.selectionType.value = "dateWeekComparative";
		        Form2.disponibilityType.value = "lastDay";
		        switch (i) {
		            case 0:
		                if (Form2.yearDateList != null) {
		                    if (Form2.yearDateList.selectedIndex != 0) {
		                        Form2.monthDateList.options[0].selected = true;
		                        if (Form2.weekDateList != null) Form2.weekDateList.options[0].selected = true;
		                        Form2.dayDateList.options[0].selected = true;
		                        ok = true;
		                    }
		                }
		                break;
		            case 1:
		                if (Form2.monthDateList.selectedIndex != 0) {
		                    if (Form2.yearDateList != null) Form2.yearDateList.options[0].selected = true;
		                    if (Form2.weekDateList != null) Form2.weekDateList.options[0].selected = true;
		                    Form2.dayDateList.options[0].selected = true;
		                    ok = true;
		                }
		                break;
		            case 2:
		                if (Form2.weekDateList.selectedIndex != 0) {
		                    if (Form2.yearDateList != null) Form2.yearDateList.options[0].selected = true;
		                    Form2.monthDateList.options[0].selected = true;
		                    Form2.dayDateList.options[0].selected = true;
		                    ok = true;
		                }
		                break;
		            case 3:
		                if (Form2.dayDateList.selectedIndex != 0) {
		                    if (Form2.yearDateList != null) Form2.yearDateList.options[0].selected = true;
		                    Form2.monthDateList.options[0].selected = true;
		                    if (Form2.weekDateList != null) Form2.weekDateList.options[0].selected = true;
		                    ok = true;
		                }
		                break;
		        }
		        if (ok == true) {
		            Form2.selectedItemIndex.value = i;
		            Form2.previousMonthCheckbox.checked = false;
		            Form2.previousYearCheckbox.checked = false;
		            Form2.previousDayCheckBox.checked = false;
		            if (Form2.currentYearCheckbox != null) Form2.currentYearCheckbox.checked = false;
		            if (Form2.previousWeekCheckBox != null) Form2.previousWeekCheckBox.checked = false;
		        }
		    }
		    function selectedCheck(i) {
		        if (Form2.yearDateList != null) Form2.yearDateList.options[0].selected = true;
		        Form2.monthDateList.options[0].selected = true;
		        if (Form2.weekDateList != null) Form2.weekDateList.options[0].selected = true;
		        Form2.dayDateList.options[0].selected = true;
		        Form2.selectedItemIndex.value = i;
		        Form2.previousYearCheckbox.checked = false;
		        Form2.previousMonthCheckbox.checked = false;
		        Form2.previousDayCheckBox.checked = false;
		        if (Form2.currentYearCheckbox != null) Form2.currentYearCheckbox.checked = false;
		        if (Form2.previousWeekCheckBox != null) Form2.previousWeekCheckBox.checked = false;
		        switch (i) {
		            case 4:
		                Form2.previousYearCheckbox.checked = true;
		                break;
		            case 5:
		                Form2.previousMonthCheckbox.checked = true;
		                break;
		            case 6:
		                if (Form2.previousWeekCheckBox != null) Form2.previousWeekCheckBox.checked = true;
		                break;
		            case 7:
		                Form2.previousDayCheckBox.checked = true;
		                break;
		            case 8:
		                if (Form2.currentYearCheckbox != null)
		                    Form2.currentYearCheckbox.checked = true;
		                break;
		        }
		    }
		    function selectedRadio(i) {
		        switch (i) {
		            case 'dateWeekComparative':
		                dateToDate.checked = false;
		                dateWeekComparative.checked = true;
		                Form2.selectionType.value = "dateWeekComparative";
		                break;
		            case 'dateToDate':
		                dateWeekComparative.checked = false;
		                dateToDate.checked = true;
		                Form2.selectionType.value = "dateToDate";
		                break;
		            case 'lastDay':
		                lastPeriod.checked = false;
		                lastDay.checked = true;
		                Form2.disponibilityType.value = "lastDay";
		                break;
		            case 'lastPeriod':
		                lastPeriod.checked = true;
		                lastDay.checked = false;
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
					<td valign="top" >
						<table id="Table10" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tbody>
							    <!-- Menu du haut-->
								<tr>
									<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
								</tr>
								<!-- Centre -->
								<tr valign="top">
									<td>
										
										<table id="Table4" cellspacing="0" cellpadding="0" width="100%" border="0">
										    <tr>
											    <td class="txtViolet12Bold" style="padding-left:20px;"><cc1:adexpresstext language="33" id="title1AdExpTxt" runat="server" Code="776"></cc1:adexpresstext></td>
											</tr>
											<tr>
												<td>
													<table id="Table9" cellspacing="0" cellpadding="5" width="100%" border="0">
														<tr>
															<td width="20">&nbsp;</td>
															<td>
															    <table cellpadding="0" cellspacing="0" border="0">
                                                                    <tr>
                                                                        <td>
                                                                            <cc5:GlobalCalendarWebControl ID="GlobalCalendarWebControl1" runat="server"/>
                                                                        </td>    
                                                                    </tr>
                                                                    <tr>
                                                                    <!---------Début Boutons validation 1 -->
                                                                      <td><table id="Table17" cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                       <tr><td width="10">&nbsp</td> 
                                                                       <td> <cc4:ImageButtonRollOverWebControl runat="server" ID="validateButton1" />
				                                                                                          </td> </tr>
		                                                                                </table>
                                                                      <!---------Fin Boutons validation 1 -->
                                                                      
                                                                     </td></tr>
                                                                 </table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr class="imageBackGround" style="height:10px;"><td></td></tr>
										    <tr style="height:10px;"><td></td></tr>										   											
											<tr valign="top" height="100%">
											    <td>&nbsp;<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
											</tr>
										</table>
									</td>
								</tr>
							</tbody>
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
			
		</form>

	</body>
</html>

