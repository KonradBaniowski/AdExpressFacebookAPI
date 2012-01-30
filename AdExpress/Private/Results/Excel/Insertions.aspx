<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Insertions.aspx.cs" Inherits="Private.Results.Excel.Insertions" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results.Creatives" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns:x=""urn:schemas-microsoft-com:office:excel"">
<head runat="server">
    <title>AdExpress</title>
</head>
<body>
    <form id="Form1" runat="server">
        <cc1:InsertionsWebControl id="ResultWebControl1" runat="server" OutputType="rawExcel" SkinID="excelInsertion"/>
    </form>
</body>
</html>
