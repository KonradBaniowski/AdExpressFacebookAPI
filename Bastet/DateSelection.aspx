
<%@ Page language="c#" Inherits="BastetWeb.DateSelection" CodeFile="DateSelection.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<%@ Register TagPrefix="cc2" Namespace="OboutInc.Calendar2" Assembly="obout_Calendar2_Net" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Bastet</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
		<script language="JavaScript" type="text/JavaScript">
		<!--
			function selectedItem(i){
				ok=false;
				switch(i){
					case 0:
						if(Form1.monthDateList.selectedIndex!=0){
							Form1.weekDateList.options[0].selected=true;
							ok=true;
						}
						break;
					case 1:
						if(Form1.weekDateList.selectedIndex!=0){
							Form1.monthDateList.options[0].selected=true;
							ok=true;
						}
						break;
				}
				if (ok==true){
					Form1.selectedItemIndex.value=i;
					Form1.currentYear.checked=false;
				}
			}
			function selectedCheck(i){
				Form1.monthDateList.options[0].selected=true;
				Form1.weekDateList.options[0].selected=true;
				Form1.selectedItemIndex.value=i;
				Form1.currentYear.checked=false;
				switch(i){
					case 2:
						Form1.currentYear.checked=true;
						break;
				}
			}
		-->
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" cellSpacing="0" cellPadding="0" width="100%" border="0" height="50%">
				<tr height="90" bgcolor="#644883"  style="font-size: 0px;">
					<td width="185"  style="font-size: 0px;"><a href="/Index.aspx"  style="font-size: 0px;"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%"  style="font-size: 0px;">
						<cc1:HeaderWebControl id="HeaderWebControl1" runat="server" SkinID="HeaderWebControl"></cc1:HeaderWebControl>
					</td>
				</tr>
				<tr>
					<td colspan="2" valign="top">
						<!--<p class="navigation">&nbsp;<%=_email_manage%>&gt; <font color="#ff0099"><%=_period_manage%></font>&gt;<%=_login_manage%>&amp;<%=_validation%></p>-->
						<p class="txtViolet12BoldTitle">&nbsp;<%=_period_manage%></p>
						<p>
							<table cellSpacing="0" cellPadding="5" border="0" class="txtViolet10" align="center" width="400">
								<tr>
									<td colspan="3" class="txtViolet12Bold"><%=_label_calendar_period_selection%></td>
								</tr>
								<tr>
									<td><cc2:Calendar id="dateBeginCalendar" Rows="1" Columns="1" EnableViewState="true" runat="server" CSSCalendar="txtNoir12" AllowDeselect="False" AutoPostBack="true" YearSelectorType="HtmlList" ShowYearSelector="true" TitleText="" ShowOtherMonthDays="true"></cc2:Calendar></td>
									<td>&nbsp;</td>
									<td><cc2:Calendar id="dateEndCalendar" Rows="1" Columns="1" EnableViewState="true" runat="server" CSSCalendar="txtNoir12" AllowDeselect="False" AutoPostBack="true" YearSelectorType="HtmlList" ShowYearSelector="true" TitleText="" ShowOtherMonthDays="true" BeginDateCalendarId="dateBeginCalendar"></cc2:Calendar></td>
								</tr>
								<tr>
									<td class="txtViolet11">&nbsp;<asp:label id="dateBeginLabel" runat="server"></asp:label></td>
									<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
									<td class="txtViolet11">&nbsp;<asp:label id="dateEndLabel" runat="server"></asp:label></td>
								</tr>
								<tr>
									<td align="center" colspan="3" class="imageBackGround"><asp:Button id="validateCalendarButton" runat="server" Width="110px" CssClass="input" onclick="validateCalendarButton_Click"></asp:Button></td>
								</tr>
							</table>
						<p>&nbsp;</p>
						<table cellSpacing="0" cellPadding="5" border="0" class="txtViolet10" align="center" width="400">
							<tr>
								<td class="txtViolet12Bold"><%=_label_rolling_period_selection%></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><%=_label_in_the_last%></td>
							</tr>
							<tr>
								<td class="txtNoir11"><span onclick="javascript:selectedItem(0)"><asp:DropDownList id="monthDateList" runat="server" CssClass="txtNoir11"></asp:DropDownList>&nbsp;<%=_label_n_last_month%>
									</span>
								</td>
							</tr>
							<tr>
								<td class="txtNoir11"><span onclick="javascript:selectedItem(1)"><asp:DropDownList id="weekDateList" runat="server" CssClass="txtNoir11"></asp:DropDownList>&nbsp;<%=_label_n_last_week%>
									</span>
								</td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><%=_label_or_in_the_last%></td>
							</tr>
							<tr>
								<td class="txtNoir11"><input type="checkbox" onclick="javascript:selectedCheck(2)" id="currentYear"><%=_label_current_year%></td>
							</tr>
							<tr>
								<td><input id="selectedItemIndex" type="hidden" value="-1" name="selectedItemIndex"></td>
							</tr>
							<tr>
								<td align="center" class="imageBackGround"><asp:Button id="validateButton" runat="server" Width="110px" CssClass="input" onclick="validateButton_Click"></asp:Button></td>
							</tr>
						</table>
						<P></P>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
