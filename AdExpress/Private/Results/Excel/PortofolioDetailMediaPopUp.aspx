<%@ Page language="c#" Inherits="AdExpress.Private.Results.Excel.PortofolioDetailMediaPopUp" CodeFile="PortofolioDetailMediaPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="application/vnd.ms-excel; charset=iso-8859-1, windows-1252"/>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" Content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<!--<%=result%>-->
		    <cc3:PortofolioDetailMediaResultWebControl id="PortofolioDetailMediaResultWebControl1" runat="server" AjaxProTimeOut="120" AllowPaging="False" IdTitleText="0" OutputType="excel" SkinID="PortofolioPopUpExcelResult"></cc3:PortofolioDetailMediaResultWebControl> 
		</form>
	</body>
</HTML>
