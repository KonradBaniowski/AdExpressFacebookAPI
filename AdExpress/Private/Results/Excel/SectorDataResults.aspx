<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.SectorDataResults" CodeFile="SectorDataResults.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results.Appm" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<%Response.ContentType = "application/vnd.ms-excel";%>
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
	<body>
		<form id="Form1" method="post" runat="server">
			<cc1:SectorDataContainerWebControl id="SectorDataContainerWebControl1" runat="server" CssL1="exlv2" CssL2="exlv3" CssLHeader="exh2"
				CssTitle="synthesisTitleEx" CssDetailSelectionBordelLevel="dsBorderLevel" CssDetailSelectionL1="dsLv1Excel" CssDetailSelectionTitle="dsTitle"
				CssDetailSelectionTitleData="dsTitleData" CssDetailSelectionTitleGlobal="dsTitleGlobal" OutputType="excel" CssLAffinities="exlv2"
				CssLTotal="exlv0" CssDetailSelectionL2="dsLv2Excel"></cc1:SectorDataContainerWebControl>
		</form>
	</body>
</HTML>
