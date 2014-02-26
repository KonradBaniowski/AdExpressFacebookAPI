<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>

<%@ Page Language="c#" Inherits="AdExpress.Private.MyAdExpress.PdfSavePopUp" CodeFile="PdfSavePopUp.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AdExpress</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <style type="text/css" media="screen">
            html {
            height:100%; max-height:100%; padding:0; margin:0; border:0; background:#fff; 
            /* hide overflow:hidden from IE5/Mac */ 
            /* \*/ 
            overflow: hidden; 
            /* */ 
            }
            body {height:100%; max-height:100%; overflow:hidden; padding:0; margin:0; border:0;}
    </style>
</head>
<body class="popUpbody" onload="javascript:activateActiveX();">
    <form id="Form2" method="post" runat="server">
        <!-- Header -->
        <div class="popUpHead popUpHeaderBackground popUpTextHeader">
            &nbsp;<cc1:AdExpressText Language="33" ID="saveTitle" runat="server" Code="1747"></cc1:AdExpressText>
        </div>
        <!-- Content -->
        <div class="popUpContent">
		    <div class="popUpPad2"></div>
                <cc3:AskRemoteExportWebControl ID="askremoteexportwebControl1" runat="server"></cc3:AskRemoteExportWebControl>
            <div class="popUpPad2"></div>
        </div>
        <!-- Footer -->
        <div class="popUpFoot popUpFooterBackground">
            <div style="padding-top:12px">
                <cc2:ImageButtonRollOverWebControl ID="closeRollOverWebControl" runat="server" OnClick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:ImageButtonRollOverWebControl>&nbsp;
            </div>
		</div>
    </form>
</body>
</html>
