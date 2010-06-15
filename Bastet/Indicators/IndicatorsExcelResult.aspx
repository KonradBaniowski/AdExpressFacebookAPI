<%@ Page language="c#" Inherits="BastetWeb.Indicators.IndicatorsExcelResult" CodeFile="IndicatorsExcelResult.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat="server">
    <title>Bastet</title>
	<meta http-equiv="Content-Type" content="application/vnd.ms-excel; charset=iso-8859-1, windows-1252">
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
	<meta http-equiv="expires" content="0">
	<meta http-equiv="pragma" content="no-cache">
	<meta name="Cache-control" content="no-cache">
  </head>
  <body bgcolor="#ffffff" bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0">
    <form id="Form1" method="post" runat="server">
	<%=_result%>
     </form>
  </body>
</html>
