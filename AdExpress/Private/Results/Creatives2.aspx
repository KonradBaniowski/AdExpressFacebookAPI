<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Creatives2.aspx.cs" Inherits="Private.Results.Creatives2" %>

<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Results.Creatives"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>AdExpress</title>
    <script type="text/javascript" src="/scripts/dom-drag.js"></script>
</head>
<body class="popUpBackGround" onload="javascript:activateActiveX();">
    <form id="form1" runat="server">
    <asp:HiddenField ID="zoomParam" runat="server" EnableViewState="true"/>
    <asp:HiddenField ID="vehicleParam" runat="server" EnableViewState="true"/>

    <div>
        <cc1:InsertionsWebControl ID="CreativesWebControl1" runat="server" SkinID="creativeControl" PageSize="10" PageSizeOptions="10,15,20"/>
        &nbsp;</div>
    </form>
    <%=divClose%>
</body>
</html>
