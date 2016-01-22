<%@ Page language="c#" Inherits="AdExpress.Private.Results.RawExcel.DynamicResults" CodeFile="DynamicResults.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD  runat="server">
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
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<cc1:ResultWebControl id="ResultWebControl1" runat="server" OutputType="rawExcel" 
				SkinID="PresentAbsentRawExcelResult"></cc1:ResultWebControl>
		</form>
	</body>
</HTML>
