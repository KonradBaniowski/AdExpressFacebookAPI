<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Alerts.AlertSavePopUp" CodeFile="AlertSavePopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.1 Strict //EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
	<body class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr class="violetBackGround" height="14">
					<td style="WIDTH: 14px; HEIGHT: 13px"><asp:Image ID="Image153" runat="server" SkinID="fleche_1" /></td>
					<td class="txtBlanc11Bold bandeauTitreBackGround" style="WIDTH: 100%; HEIGHT: 13px">&nbsp;
						<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="2609"></cc1:adexpresstext></td>
				</tr>
				<tr>
					<td colspan="2">
						<table id="SaveData" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr height="35">
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="FileNameLabel" runat="server" Code="1746"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxFileName" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr>
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="MailLabel" runat="server" Code="1136"></cc1:adexpresstext></td>
								<td>
								    <asp:textbox id="tbxMail" runat="server" Width="300px"></asp:textbox><br />
								    <asp:CheckBox id="cbxRegisterMail" runat="server" Visible="False"></asp:CheckBox>
								</td>
							</tr>
							<tr>
							    <td class="txtViolet11Bold" width="150" style="vertical-align: top">
							        <br />
							        <cc1:adexpresstext language="33" id="PeriodicityLabel" runat="server" Code="1293"></cc1:adexpresstext>
							    </td>
							    <td>
							        <br />
							        <script type="text/javascript">
							            var oldDay = undefined;
							        
							            function onPeriodicityChanged(e) {
						                    document.getElementById('divPeriodicityWeekly').style.display = 'none';
						                    document.getElementById('divPeriodicityMonthly').style.display = 'none';
						                    if (e.value == 20) {
						                        document.getElementById('divPeriodicityWeekly').style.display = '';
						                    }
						                    else {
						                        if (e.value == 30) {
						                            document.getElementById('divPeriodicityMonthly').style.display = '';
						                        }
							                }
							            }
							            
							            function onPeriodicityParameterClicked(e, obj)
							            {
							                document.getElementById('hiddenPeriodicityValue').value = e;
							                if (oldDay != undefined) {
							                    oldDay.style.backgroundColor = '#FFFFFF';
							                    oldDay.style.color = "#AAAAAA";
							                }
							                obj.style.backgroundColor = "#DED8E5";
							                obj.style.color = "#FFFFFF";
							                oldDay = obj;
							                return (false);
							            }
							        </script>
							        <asp:DropDownList runat="server" ID="ddlPeriodicityType">
							        </asp:DropDownList>
							        <br />
                                    <div class="periodicity-type-day" id="divPeriodicityWeekly" style="display: none">
							            <table>
							                <tr>
							                    <th class="alert-dayofmonth-selection-title">
    							                    <asp:Label ID="lblIntroWeekly" runat="server"></asp:Label>
							                    </th>
							                </tr>
							                <tr>
							                    <td>
							                        <asp:HyperLink ID="lnkMonday" runat="server">1</asp:HyperLink>
							                        <asp:HyperLink ID="lnkTuesday" runat="server">2</asp:HyperLink>
							                        <asp:HyperLink ID="lnkWednesday" runat="server">3</asp:HyperLink>
							                        <asp:HyperLink ID="lnkThursday" runat="server">4</asp:HyperLink>
							                        <asp:HyperLink ID="lnkFriday" runat="server">5</asp:HyperLink>
							                        <asp:HyperLink ID="lnkSaturday" runat="server">6</asp:HyperLink>
							                        <asp:HyperLink ID="lnkSunday" runat="server">7</asp:HyperLink>
							                    </td>
							                </tr>
							            </table>
							        </div>
							        <div class="periodicity-type" id="divPeriodicityMonthly" style="display: none">
							            <table class="alert-dayofmonth-selection-table">
							                <tr>
							                    <th colspan="7" class="alert-dayofmonth-selection-title">
							                        <asp:Label runat="server" ID="lblIntroMonthly"></asp:Label>
							                    </th>
							                </tr>
							                <tr>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">1</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">2</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">3</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">4</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">5</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">6</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">7</a></td>
						                    </tr>
						                    <tr>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">8</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">9</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">10</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">11</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">12</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">13</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">14</a></td>
						                    </tr>
						                    <tr>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">15</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">16</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">17</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">18</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">19</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">20</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">21</a></td>
						                    </tr>
						                    <tr>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">22</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">23</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">24</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">25</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">26</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">27</a></td>
						                        <td><a href="#" onclick="onPeriodicityParameterClicked(this.innerHTML, this);">28</a></td>
						                   </tr>
						                </table>
							        </div>
							        <br style="clear:both" />
							        <input type="hidden" runat="server" id="hiddenPeriodicityValue" value="-1" />
							    </td>
							</tr>							
							<tr>
								<td colSpan="2">&nbsp;</td>
							</tr>
							<tr>
								<td colspan="2">&nbsp;</td>
							</tr>
							<tr>
								<td align="right" colspan="2"><cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" runat="server" onclick="validateRollOverWebControl_Click" SkinID="validateButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
									<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" runat="server" onclick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
