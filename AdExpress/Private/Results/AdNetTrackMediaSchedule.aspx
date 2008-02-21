<%@ Reference Page="~/Private/Results/MediaInsertionsCreationsResults.aspx" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.AdNetTrackMediaSchedule" CodeFile="AdNetTrackMediaSchedule.aspx.cs" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">

	<head>
		<title>AdExpress</title>
		
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<link href="/Css/MediaSchedule.css" type="text/css" rel="stylesheet"/>
		
        <script type="text/javascript" src="/scripts/dom-drag.js"></script>
		
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
		
	</head>
	
	<body style="margin-bottom:0; margin-left:0;margin-top:0; background-color:#ffffff;" background="/Images/Common/backgroundPopUp.gif">

		<form id="Form1" method="post" runat="server">
		    
		    <asp:HiddenField EnableViewState="true" id="ZoomParam" runat="server" />
		    <script language="javascript" type="text/javascript">
		        var periodContainer = document.getElementById("ZoomParam").value;
		        function RefreshData(){
		            document.getElementById("ZoomParam").value = periodContainer;
		            __doPostBack('','');
		        }
		    </script>

			<table style="background-color:#ffffff; margin-top: 25px; margin-left: 0px; margin-right: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">

				<tr>
					<td colSpan="3"><img src="/Images/Common/Logo_Adnettrack.jpg" width="200" vspace="10" border="0"/></td>
				</tr>

				<tr>
					<td colspan="3" valign="top" height="30">
					<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0" width="30" height="8" VIEWASTEXT>
						<param name=movie value="/Flash/Common/Arrow_Back.swf">
						<param name=quality value="high">
						<param name=menu value="false">
						<embed src="/Flash/Common/Arrow_Back.swf" width="30" height="8" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" menu="false"></embed> 
					</object>
					<%=_returnLink%></td>
				</tr>
				
                <tr height="5">
	                <td colspan="3"></td>
                </tr>
				<tr>
					<td valign="top" height="100%">
						<cc3:OptionLayerWebControl id="OptionLayerWebControl1" runat="server" CssDefaultListLabel="txtViolet11Bold"
							BackGroundColor="#E1E0DA" GenericDetailLevelComponentProfile="adnettrack" GenericDetailLevelType="adnettrack"
							Width="200px" DisplayInformationComponent="True"></cc3:OptionLayerWebControl></td>
					<td valign="top" width="5">&nbsp;</td>
					<td valign="top" align="left" width="100%">
					    <table cellpadding="0" cellspacing="0" border="0">
					        <tr>
					            <td><cc3:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" JavascriptRefresh="RefreshData" PeriodContainerName="periodContainer" /></td>
					        </tr>
                            <tr height="5">
                                <td></td>
                            </tr>
                            <tr>
					            <td><cc2:alertadnettrackmediaschedulewebcontrol id="AlertAdNetTrackMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240"></cc2:alertadnettrackmediaschedulewebcontrol></td>
					        </tr>
					    </table>
					    
					</td>
				</tr>
			</table>
			<cc3:menuwebcontrol id="MenuWebControl1" runat="server"></cc3:menuwebcontrol>
			
		</form>
			
	</body>
	
</html>
