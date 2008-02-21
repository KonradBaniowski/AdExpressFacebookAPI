<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.MediaInsertionsCreationsResults" codePage="28591" CodeFile="MediaInsertionsCreationsResults.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<link href="/Css/GenericUI.css" type="text/css" rel="stylesheet"/>
		<link media="all" href="/Css/rico.css" type="text/css" rel="Stylesheet"/>
		<script src="/scripts/prototype.js" type="text/javascript"></script>
		<script src="/scripts/rico.js" type="text/javascript"></script>
		<script src="/scripts/dom-drag.js" type="text/javascript"></script>
	</HEAD>
	<body style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/backgroundPopUp.gif); BACKGROUND-REPEAT: repeat"
		bottomMargin="25" bgColor="#ffffff" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<asp:HiddenField ID="periodNavigation" runat="server" Value="" />
			<cc2:menuwebcontrol id="MenuWebControl2" runat="server"></cc2:menuwebcontrol>
			<table cellSpacing="0" cellPadding="0" width="500" align="center" bgColor="#ffffff" border="0">
                
					<tr bgColor="#ded8e5">
						<td><cc2:informationwebcontrol id="InformationWebControl1" runat="server" InLeftMenu="False"></cc2:informationwebcontrol></td>
					</tr>
					<tr>
						<td><cc2:vehicleheaderwebcontrol language="33" id="VehicleHeaderWebControl1" runat="server"></cc2:vehicleheaderwebcontrol></td>						
					</tr>
					<tr>
						<td><cc2:DetailInsertionPeriodNavigationWebControl id="DetailInsertionPeriodNavigationWebControl1" runat="server"></cc2:DetailInsertionPeriodNavigationWebControl></td></tr>
					<tr>
						<td><cc2:genericdetailselectionwebcontrol id="GenericDetailSelectionWebControl2" runat="server"></cc2:genericdetailselectionwebcontrol></td>
					</tr>
					<tr bgColor="#ffffff">
						<td align="center">
							<cc3:MediaInsertionsCreationsResultsWebControl id="MediaInsertionsCreationsResultsWebControl1" runat="server"></cc3:MediaInsertionsCreationsResultsWebControl>
							<!--result-->
						</td>
					</tr>
				
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
