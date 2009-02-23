<%@ Register TagPrefix="cc1" Namespace="TNS.FrameWork.WebResultUI" Assembly="TNS.FrameWork.WebResultUI" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Results.ProductClassAnalysis" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.ProductClassReport" CodeFile="ProductClassReport.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="application/vnd.ms-excel;"/>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" Content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<cc2:ProductClassReportWebControl id="ResultWebControl1" runat="server" 
				OutputType="excel" SkinID="productClassResultTableXL"></cc2:ProductClassReportWebControl>
		</form>
	</body>
</html>
