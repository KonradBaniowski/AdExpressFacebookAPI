<%@ Page language="c#" Inherits="AdExpress.Private.Results.PortofolioDetailMediaPopUp" CodeFile="PortofolioDetailMediaPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body class="darkBackGround" bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<cc1:MenuWebControl id="MenuWebControl2" runat="server"></cc1:MenuWebControl>
			<table border="0" width="100%" align="center" cellpadding="0" cellspacing="0" class="whiteBackGround">
				<tr>
					<td>
						<cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="False"></cc1:InformationWebControl>
					</td>
				</tr>
				<tr>
				    <td>&nbsp;</td>
				</tr>
				<tr>
					<td>
					    <!--<%=result%>-->
					    <cc3:PortofolioDetailMediaResultWebControl id="PortofolioDetailMediaResultWebControl1" runat="server" JavascriptFilePath="/scripts/WebResult.js" AjaxProTimeOut="120" AllowPaging="True" IdTitleText="0" SkinID="PortofolioPopUpResult"></cc3:PortofolioDetailMediaResultWebControl> 
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
