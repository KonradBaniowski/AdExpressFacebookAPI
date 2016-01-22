<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.CreativeMediaPlanResults" CodeFile="CreativeMediaPlanResults.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/CreativeMediaSchedule.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body>
		<form id="Form2" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<td>&nbsp;</td>
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td height="20">&nbsp;</td>
							</tr>
							<tr>
								<td><cc4:CreativeSelectionWebControl id="creativeSelectionWebControl" runat="server"></cc4:CreativeSelectionWebControl></td>
							</tr>
							<tr>
								<td height="20">&nbsp;</td>
							</tr>
							<tr>
								<!-- Tableau de Résultat -->
								<td bgColor="#ffffff"><%=result%></td>
							</tr>
						</table>
					</td>
					<!-- la fin -->
				</tr>
			</table>
		</form>
		<!--=divClose-->
	</body>
</HTML>
