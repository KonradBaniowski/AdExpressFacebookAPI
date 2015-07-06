<%@ Page language="c#" Inherits="AdExpress.Private.Results.DownloadCreationsPopUp" CodeFile="DownloadCreationsPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE html>
<html>
	<HEAD id="HEAD1" runat="server">
		<title>
			<%=title%>
		</title>
        <meta http-equiv="X-UA-Compatible" content="IE=9">    
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script src="/scripts/CookiesJScript.js" type="text/javascript"></script>
		<script src="/scripts/Plugins.js" type="text/javascript"></script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
           
	</HEAD>
	<body class="popupBackGround whiteBackGround" >
		<form id="Form1" method="post" runat="server">
						<%=streamingCreationsResult%>
		</form>
		<%=divClose%>
	</body>
</html>
