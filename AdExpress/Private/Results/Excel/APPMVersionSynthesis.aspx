<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.APPMVersionSynthesis" CodeFile="APPMVersionSynthesis.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>AdExpress</title>
      <%Response.ContentType = "application/vnd.ms-excel";%>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">

  </head>
  <body>
	
    <form id="Form1" method="post" runat="server">
			<%=result%>
     </form>
	
  </body>
</html>
