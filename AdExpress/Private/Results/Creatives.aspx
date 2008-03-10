<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Creatives.aspx.cs" Inherits="Private.Results.Creatives" %>

<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Results.Creatives"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AdExpress</title>
    <script type="text/javascript" src="/scripts/dom-drag.js"></script>
</head>
<body class="popUpBackGround">
    <form id="form1" runat="server">
    <div>
        <cc1:CreativesWebControl ID="CreativesWebControl1" runat="server" />
        &nbsp;</div>
    </form>
    <%=divClose%>
</body>
</html>
