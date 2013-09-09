<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Private_Home" %>
<%@ Register TagPrefix="cc1" Namespace="KMI.PromoPSA.Web.Controls.Header" Assembly="KMI.PromoPSA.Web.Controls" %>
<%@ Register Assembly="Trirand.Web" TagPrefix="trirand" Namespace="Trirand.Web.UI.WebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;"/>
	<meta content="C#" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
	<meta http-equiv="expires" content="0"/>
	<meta http-equiv="pragma" content="no-cache"/>
	<meta content="no-cache" name="Cache-control"/>
    <!-- The jQuery UI theme that will be used by the grid -->
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/redmond/jquery-ui.css" />
    <!-- The jQuery UI theme extension jqGrid needs -->
    <link rel="stylesheet" type="text/css" media="screen" href="/themes/ui.jqgrid.css" />
    <!-- jQuery runtime minified -->
    <script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.8.3.min.js" type="text/javascript"></script>    
    <!-- The localization file we need, English in this case -->
    <script src="/js/trirand/i18n/grid.locale-fr.js" type="text/javascript"></script>
    <!-- The jqGrid client-side javascript -->
    <script src="/js/trirand/jquery.jqGrid.min.js" type="text/javascript"></script>    
    <!-- This jQuery UI library is needed only for the code tabs, not needed by the grid per se -->
</head>
<body class="bodyStyle">
    <form id="form1" runat="server">
        <div class="header" >
            <div style="float:left;">
                <asp:Image ID="Image1" runat="server" SkinID="logo_tns_home" />
            </div>
        </div>
        <div class="promotionsInformationDiv">
            <table cellspacing="0" cellpadding="0" border="0" height="100%">
                <tr>
                    <td><cc1:DisconnectUserWebControl runat="server" id="DisconnectUserWebControl1" SkinID="DisconnectUserWebControl"/></td>
                    <td><cc1:LoginInformationWebControl runat="server" id="LoginInformationWebControl1" SkinID="LoginInformationWebControl"/></td>
                    <td><cc1:PromotionInformationWebControl runat="server" id="PromotionInformationWebControl1" SkinID="PromotionInformationWebControl"/></td>
                </tr>
            </table>
        </div>
        <div style="margin-top:10px; margin-left:10px;">
            <trirand:JQGrid runat="server" ID="LinqAtRuntimeGrid" Width="900" Height="500">            
                <Columns>                
                    <trirand:JQGridColumn DataField="IdForm" />                
                    <trirand:JQGridColumn DataField="IdVehicle" />                
                    <trirand:JQGridColumn DataField="DateMediaNum" />                
                </Columns>        
            </trirand:JQGrid>
        </div>
    </form>
</body>
</html>


