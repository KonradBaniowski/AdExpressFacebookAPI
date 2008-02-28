<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Page language="c#" Inherits="AdExpress.selectionModule" CodeFile="selectionModule.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body class="bodyStyle">
		<form id="Form1" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" width="600" border="0">
				<tr valign="top">
					<td width="209">
						<!-- Logo --><asp:Image runat="server" border="0" SkinID="LogoAdexpress" />
					</td>
					<td width="100%"><cc1:headerwebcontrol id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Login Information--><cc1:logininformationwebcontrol id="LoginInformationWebControl1" runat="server"></cc1:logininformationwebcontrol></td>
				</tr>
				<tr>
					<td colspan="2" height="10"><!-- Vide --></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Actualités --><cc1:actualitieswebcontrol id="ActualitiesWebControl1" runat="server" Width="800px" DisplayImage="True" RssFileUrlPath="AdExpressFeed.xml"   
							ColumnImageWidth="93px" RssFileName="AdExpressFeed.xml"></cc1:actualitieswebcontrol></td>
				</tr>
				<tr>
					<td colspan="2" height="10"><!-- Vide --></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Modules --><cc2:moduleselection2webcontrol id="ModuleSelection2WebControl1" runat="server" Width="800px" ColumnInformationWidth="40%"
							 ColumnLeftWidth="10px"></cc2:moduleselection2webcontrol></td>
				</tr>
			</table>
		</form>
	</body>
</html>
