<%@ Page language="c#" Inherits="AdExpress.Private.Results.PortofolioDetailMediaPopUp" CodeFile="PortofolioDetailMediaPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
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
		<LINK href="/Css/GenericUI.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bgcolor="#ffffff" style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x"
		bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<cc1:MenuWebControl id="MenuWebControl2" runat="server"></cc1:MenuWebControl>
			<table border="0" width="100%" align="center" cellpadding="0" cellspacing="0" bgcolor="#ffffff">
				<tr>
					<td>
						<cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="False"></cc1:InformationWebControl>
					</td>
				</tr>
				<tr>
				    <td>&nbsp;</td>
				</tr>
				<tr>
					<td>
					    <!--<%=result%>-->
					    <cc3:PortofolioDetailMediaResultWebControl id="PortofolioDetailMediaResultWebControl1" runat="server" BackgroudColorL1="#D0C8DA" CssL1="lv2" CssLHeader="h2" HighlightBackgroundColorL1="#FFFFFF" ImgBtnCroiOverPath="/Images/Common/fl_tri_croi3_in.gif" ImgBtnCroiPath="/Images/Common/fl_tri_croi3.gif" ImgBtnDeCroiOverPath="/Images/Common/fl_tri_decroi3_in.gif" ImgBtnDeCroiPath="/Images/Common/fl_tri_decroi3.gif" JavascriptFilePath="/scripts/WebResult.js" AjaxProTimeOut="120" AllowPaging="True" IdTitleText="0" CssDetailSelectionBordelLevel="dsBorderLevel" CssDetailSelectionL1="dsLv1" CssDetailSelectionL2="dsLv2" CssDetailSelectionL3="dsLv3" CssDetailSelectionTitle="dsTitle" CssDetailSelectionTitleData="dsTitleData"></cc3:PortofolioDetailMediaResultWebControl> 
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
