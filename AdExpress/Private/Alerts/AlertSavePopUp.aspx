<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Alerts.AlertSavePopUp" CodeFile="AlertSavePopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
        <style type="text/css" media="screen">
            html {
            height:100%; max-height:100%; padding:0; margin:0; border:0; background:#fff; 
            /* hide overflow:hidden from IE5/Mac */ 
            /* \*/ 
            overflow: hidden; 
            /* */ 
            }
            body {height:100%; max-height:100%; overflow:hidden; padding:0; margin:0; border:0;}
        </style>
		<script type="text/javascript">
		    function resizeWin() {
		        if (document.all) {
		            winHeight = document.all['divContent'].offsetHeight;
		            winWidth = document.all['divContent'].offsetWidth;
		        }
		        window.resizeTo(winWidth + 30, winHeight + 40);
		    }

		    var oldDay = undefined;

		    function onPeriodicityChanged(e) {
		        if (e.value == 20) {
		            document.getElementById('divPeriodicityDaily').style.display = 'none';
		            document.getElementById('divPeriodicityWeekly').style.display = '';
		            document.getElementById('divPeriodicityMonthly').style.display = 'none';
		        }
		        else if (e.value == 30) {
		            document.getElementById('divPeriodicityDaily').style.display = 'none';
		            document.getElementById('divPeriodicityWeekly').style.display = 'none';
		            document.getElementById('divPeriodicityMonthly').style.display = '';
		        }
		        else {
		            document.getElementById('divPeriodicityDaily').style.display = '';
		            document.getElementById('divPeriodicityWeekly').style.display = 'none';
		            document.getElementById('divPeriodicityMonthly').style.display = 'none';
		        }
		        if (oldDay != undefined) {
		            var name = GetImgNameWithoutExtention(oldDay.src);
		            if (name[name.length - 1] == 's')
		                name = name.substring(0, name.length - 1);

		            oldDay.src = oldDay.src.substring(0, oldDay.src.lastIndexOf("/") + 1) + name + GetExtention(oldDay.src);
		        }
		        document.getElementById('hiddenPeriodicityValue').value = -1;
		    }

		    function onPeriodicityParameterClicked(e, obj, imgPath) {
		        document.getElementById('hiddenPeriodicityValue').value = e;
		        if (oldDay != undefined) {
		            var name = GetImgNameWithoutExtention(oldDay.src);
		            if (name.lastIndexOf('s', name.length) > 0)
		                name = name.substring(0, name.length - 1);
		            //if (name[name.length - 1] == 's')
		            //    name = name.substring(0, name.length - 1);
		            oldDay.src = imgPath + name + GetExtention(oldDay.src);
		        }
		        obj.src = imgPath + GetImgNameWithoutExtention(obj.src) + "s" + GetExtention(obj.src);
		        oldDay = obj;
		        return (false);
		    }

		    function GetExtention(imgPath) {
		        return imgPath.substring(imgPath.lastIndexOf("."), imgPath.length);
		    }

		    function GetImgNameWithoutExtention(imgPath) {
		        return imgPath.substring(imgPath.lastIndexOf("/") + 1, imgPath.lastIndexOf("."));
		    }
		</script>
	</head>
	<body class="popUpbody" onload="javascript:activateActiveX();resizeWin();">
		<form id="Form2" method="post" runat="server">
					<!-- Header -->
					<div class="popUpHead popUpHeaderBackground popUpTextHeader">
						&nbsp;<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="2609"></cc1:adexpresstext>
					</div>
					<!-- Content -->
					<div class="popUpContent">
		                <div class="popUpPad2"></div>
						    <table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0" >
				                <tr>
					                <td class="txtViolet11Bold">
					                    <cc1:adexpresstext language="33" id="FileNameLabel" runat="server" Code="1746"></cc1:adexpresstext>&nbsp; :
					                </td>
				                </tr>
                                <tr>
					                <td>
					                    <asp:textbox id="tbxFileName" runat="server" width="300px"></asp:textbox>
					                </td>
				                </tr>
				                <tr><td>&nbsp;</td></tr>
				                <tr>
					                <td class="txtViolet11Bold" >
					                    <cc1:adexpresstext language="33" id="MailLabel" runat="server" Code="1136"></cc1:adexpresstext>&nbsp; :
					                </td>
				                </tr>
				                <tr>
					                <td class="txtViolet11">
					                    <asp:textbox id="tbxMail" runat="server" width="300px"></asp:textbox><br />
					                    <asp:CheckBox id="cbxRegisterMail" runat="server" Visible="False"></asp:CheckBox>
					                </td>
				                </tr>
				                <tr><td>&nbsp;</td></tr>
				                <tr>
				                    <td class="txtViolet11Bold" >
				                        <cc1:adexpresstext language="33" id="PeriodicityLabel" runat="server" Code="1293"></cc1:adexpresstext>&nbsp; :
				                    </td>
				                </tr>
				                <tr>
				                    <td>
                				        
				                        <asp:DropDownList runat="server" ID="ddlPeriodicityType">
				                        </asp:DropDownList>
                				        
				                        <div id="divPeriodicityDaily" style="padding:5px;">
				                            <table cellpadding="0" cellspacing="0">
				                                <tr>
			                                        <td>
			                                            <p class="alert-dayofmonth-selection-table-warning">
			                                                <cc1:adexpresstext language="33" id="Adexpresstext2" runat="server" Code="2616"></cc1:adexpresstext>
                                                        </p>
                                                    </td>
                                                </tr>
				                            </table>
				                        </div>
                                        <div id="divPeriodicityWeekly" style="padding:5px;display: none;">
				                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
				                                <tr>
				                                    <td align="center">
		                                                <table cellpadding="0" cellspacing="0" class="periodicity-type-day">
		                                                    <tr>
		                                                        <th colspan="7" align="center" class="alert-dayofweek-selection-title">
			                                                        <asp:Label ID="lblIntroWeekly" runat="server"></asp:Label>
		                                                        </th>
		                                                    </tr>
		                                                    <tr>
		                                                        <td>
			                                                        <table cellpadding="0" cellspacing="0" border="0" width="100%">
			                                                            <tr>
		                                                                    <td><asp:Image runat="server" id="lnkMonday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkTuesday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkWednesday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkThursday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkFriday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkSaturday" CssClass="imageCursor" /></td>
		                                                                    <td><asp:Image runat="server" id="lnkSunday" CssClass="imageCursor" /></td>
		                                                                </tr>
		                                                            </table>
		                                                        </td>
		                                                    </tr>
		                                                </table>
				                                    </td>
				                                </tr>
				                                <tr><td height="10"></td></tr>
				                                <tr>
			                                        <td>
			                                            <p class="alert-dayofmonth-selection-table-warning">
			                                                <cc1:adexpresstext language="33" id="Adexpresstext1" runat="server" Code="2616"></cc1:adexpresstext>
                                                        </p>
                                                    </td>
                                                </tr>
				                            </table>
				                        </div>
				                        <div id="divPeriodicityMonthly" style="padding:5px;display: none;float:none;">
				                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
				                                <tr>
				                                    <td align="center">
				                                        <table cellpadding="0" cellspacing="0" class="periodicity-type" width="0px">
				                                            <tr>
				                                                <th colspan="7" class="alert-dayofmonth-selection-title">
			                                                        <asp:Label runat="server" ID="lblIntroMonthly"></asp:Label>
			                                                    </th>
				                                            </tr>
				                                            <tr>
				                                                <td>
			                                                        <table cellpadding="0" cellspacing="0" border="0" width="100%">
			                                                            <tr>
			                                                                <td><asp:Image runat="server" id="day1" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day2" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day3" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day4" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day5" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day6" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day7" CssClass="imageCursor" /></td>
			                                                            </tr>
			                                                            <tr>
			                                                                <td><asp:Image runat="server" id="day8" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day9" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day10" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day11" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day12" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day13" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day14" CssClass="imageCursor" /></td>
			                                                            </tr>
			                                                            <tr>
			                                                                <td><asp:Image runat="server" id="day15" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day16" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day17" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day18" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day19" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day20" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day21" CssClass="imageCursor" /></td>
			                                                            </tr>
			                                                            <tr>
			                                                                <td><asp:Image runat="server" id="day22" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day23" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day24" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day25" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day26" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day27" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day28" CssClass="imageCursor" /></td>
			                                                            </tr>
			                                                            <tr>
			                                                                <td><asp:Image runat="server" id="day29" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day30" CssClass="imageCursor" /></td>
			                                                                <td><asp:Image runat="server" id="day31" CssClass="imageCursor" /></td>
			                                                                <td colspan="4"></td>
			                                                            </tr>
		                                                            </table>
		                                                        </td>
		                                                     </tr> 
		                                                 </table>  
			                                        </td>
			                                    </tr>
			                                    <tr><td>&nbsp;</td></tr>
			                                    <tr>
			                                        <td>
			                                            <p class="alert-dayofmonth-selection-table-warning">
			                                                <cc1:adexpresstext language="33" id="warningText" runat="server" Code="2616"></cc1:adexpresstext>
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
				                        </div>
                				        
				                        <input type="hidden" runat="server" id="hiddenPeriodicityValue" value="-1" />
				                    </td>
				                </tr>
			                </table>
						
					    <div class="popUpPad2"></div>
                    </div>
					<!-- Footer -->
					<div class="popUpFoot popUpFooterBackground">
                        <div style="padding-top:12px">
						    <cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" runat="server" onclick="validateRollOverWebControl_Click" SkinID="validateButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" runat="server" onclick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
					    </div>
				    </div>
		</form>
	</body>
</html>
