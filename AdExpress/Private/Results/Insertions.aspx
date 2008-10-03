<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Insertions.aspx.cs" Inherits="Private.Results.Insertions" %>

<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Results.Creatives" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>AdExpress</title>
		<script src="/scripts/prototype.js" type="text/javascript"></script>
		<script src="/scripts/rico.js" type="text/javascript"></script>
		<script src="/scripts/dom-drag.js" type="text/javascript"></script>
</head>
<body class="popUpBackGround" onload="javascript:activateActiveX();">
    <form id="Form1" runat="server">
    <div>
        <cc1:InsertionsWebControl ID="InsertionsWebControl1" runat="server" />
        &nbsp;</div>
    </form>
    <%=divClose%>
</body>
</html>
