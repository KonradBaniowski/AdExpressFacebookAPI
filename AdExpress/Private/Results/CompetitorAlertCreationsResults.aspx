<%@ Page language="c#" Inherits="AdExpress.Private.Results.CompetitorAlertCreationsResults" CodeFile="CompetitorAlertCreationsResults.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress 3.0</title>

		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>

		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
        <link href="/Css/GenericUI.css" type="text/css" rel="stylesheet"/>
        <script type="text/javascript" src="/scripts/dom-drag.js"></script>

		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
		
	</head>
	
	<body style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x; background-color:#FFFFFF; margin:25 0 25 0; text-align:center;">
	
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
			    <table style="background-color:#ffffff" cellpadding="0" cellspacing="0" border="0">
				    <tr>
					    <td>
						    <cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="False"></cc1:InformationWebControl></td>
				    </tr>
                    <tr>
                        <td>
			                <table  style="border:solid 10px #ffffff" style="background-color:#ffffff" width="500" cellpadding="0" cellspacing="0">
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
