<%@ Page language="c#" Inherits="AdExpress.Private.Results.CompetitorAlertCreationsResults" CodeFile="CompetitorAlertCreationsResults.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
        <script type="text/javascript" src="/scripts/dom-drag.js"></script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	
	<body class="creationDarkBackGround" onload="javascript:activateActiveX();">
	
		<form id="Form1" method="post" runat="server">
	
		    <asp:HiddenField id="ZoomParam" runat="server" EnableViewState="true"/>
		    <script language="javascript" type="text/javascript">
		        var periodContainer = document.getElementById("ZoomParam").value;
		        function RefreshData(){
		            document.getElementById("ZoomParam").value = periodContainer;
		            __doPostBack('','');
		        }
		    </script>
			<cc1:MenuWebControl id="MenuWebControl2" runat="server"></cc1:MenuWebControl>
			    <table class="whiteBackGround" cellpadding="0" cellspacing="0" border="0">
				    <tr>
					    <td>
						    <cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="False"></cc1:InformationWebControl></td>
				    </tr>
                    <tr>
                        <td>
			                <table  class="creationWhiteBorder" width="500" cellpadding="0" cellspacing="0">
				                <tr height="5">
					                <td></td>
				                </tr>
				                <tr>
					                <td><cc1:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" JavascriptRefresh="RefreshData" PeriodContainerName="periodContainer" /></td>
				                </tr>
                                <tr height="5">
                                    <td>
                                    </td>
                                </tr>
                                <tr>
					                <td><%=result%></td>
				                </tr>
			                </table>
			                </td>
			            </tr>
			    </table>
			
		</form>
		<%=divClose%>
	</body>
</html>
