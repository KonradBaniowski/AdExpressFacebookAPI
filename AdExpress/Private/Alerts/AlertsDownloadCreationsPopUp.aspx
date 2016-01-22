<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>

<%@ Page Language="c#" Inherits="AdExpress.Private.Alerts.AlertsDownloadCreationsPopUp" CodeFile="AlertsDownloadCreationsPopUp.aspx.cs" %>

<!DOCTYPE html>
<html>
<head>
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
    <link href="../../App_Themes/KMAE-Fr/Css/AdExpress.css" type="text/css" rel="stylesheet">
     
  
    <style>
        body
        {
            background-image: url("../../App_Themes/KMAE-Fr/Images/Common/backgroundPopUp.gif");
            background-color: #ffffff;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <cc1:AlertsDownloadCreationsWebControl ID="alertsdownloadcreationswebControl1" runat="server"></cc1:AlertsDownloadCreationsWebControl>
            <%=divClose%>
    </form>

</body>
</html>
