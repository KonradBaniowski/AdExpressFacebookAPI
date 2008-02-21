<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.MediaPlanResults" CodeFile="MediaPlanResults.aspx.cs" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<%Response.ContentType = "application/vnd.ms-excel";%><!--codePage="28591"-->
		<meta http-equiv="Content-Type" content="application/vnd.ms-excel; charset=iso-8859-1, windows-1252">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/MediaSchedule.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body>
		<form id="Form2" method="post" runat="server">
			<%=result%>
		</form>
	</body>
</HTML>
