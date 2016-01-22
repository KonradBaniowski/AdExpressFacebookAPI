<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.PortofolioDetailMediaPopUp" CodeFile="PortofolioDetailMediaPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html xmlns:x=""urn:schemas-microsoft-com:office:excel"">
	<head runat="server">
		<title>AdExpress</title>	
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<!--<%=result%>-->
		    <cc3:PortofolioDetailMediaResultWebControl id="PortofolioDetailMediaResultWebControl1" runat="server" AjaxProTimeOut="120" AllowPaging="False" IdTitleText="0" OutputType="excel" SkinID="PortofolioPopUpExcelResult"></cc3:PortofolioDetailMediaResultWebControl> 
		</form>
	</body>
</html>
