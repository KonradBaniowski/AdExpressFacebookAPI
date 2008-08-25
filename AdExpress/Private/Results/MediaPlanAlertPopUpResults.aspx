<%--<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.MediaPlanAlertPopUpResults" CodeFile="MediaPlanAlertPopUpResults.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/MediaSchedule.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bgcolor="#ffffff" style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x">
		<form id="Form1" method="post" runat="server">
			<table bgColor="#ffffff" style="MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px"
				cellPadding="0" cellSpacing="3" align="center" border="0">
				<tr>
					<td colspan="2">
						<cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="False"></cc1:InformationWebControl></td>
				</tr>
				<tr>
					<td colspan="2" align="left"><%=header%></td>
				</tr>
				<tr>
					<td width="300">
						<cc1:GenericMediaLevelDetailSelectionWebControl id="GenericMediaLevelDetailSelectionWebControl1" runat="server" Width="220px" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx"
							SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"></cc1:GenericMediaLevelDetailSelectionWebControl></td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2" width="300">
						<cc2:ImageButtonRollOverWebControl id="okImageButton" runat="server" ImageUrl="/Images/Common/Button/ok_up.gif" RollOverImageUrl="/Images/Common/Button/ok_down.gif"></cc2:ImageButtonRollOverWebControl>
					</td>
				<tr>
					<td colspan="2"><%=result%>
						<cc3:AlertMediaPlanResultWebControl id="AlertMediaPlanResultWebControl1" runat="server"></cc3:AlertMediaPlanResultWebControl>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
		<cc1:MenuWebControl id="MenuWebControl2" style="Z-INDEX: 101; LEFT: 184px; POSITION: absolute; TOP: 272px"
			runat="server"></cc1:MenuWebControl>
	</body>
</HTML>
--%>
