<%@ Page language="c#" Inherits="BastetWeb.Indicators.IndicatorsResult" CodeFile="IndicatorsResult.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Bastet</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" cellSpacing="0" cellPadding="0" width="100%" border="0" height="50%">
				<tr height="90" bgcolor="#644883">
					<td width="185"><a href="/Index.aspx"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%">
						<cc1:HeaderWebControl id="HeaderWebControl1" runat="server" ActiveMenu="2" SkinID="HeaderWebControl"></cc1:HeaderWebControl>
					</td>
				</tr>
				<tr>
					<td colspan="2" valign="top">
						<p class="txtViolet12BoldTitle">&nbsp;<cc1:BastetText language="33" id="BastetText2" runat="server" Code="55"></cc1:BastetText></p>
						<p>
							&nbsp;<a href="/DateSelection.aspx?moduleId=2" class="roll05"><asp:Image ID="Image1" runat="server" align="absmiddle" border="0" title="Sélection de la période" SkinID="iconCalendar" />&nbsp;<cc1:BastetText language="33" id="BastetText1" runat="server" Code="51"></cc1:BastetText></a>&nbsp;<a href="/Indicators/IndicatorsExcelResult.aspx" target="_blank" class="roll05"><asp:Image ID="Image2" runat="server" align="absmiddle" border="0" title="Export Excel" SkinID="iconExcel" />&nbsp;<cc1:BastetText language="33" id="AdExpressText1" runat="server" Code="52"></cc1:BastetText></a><br>
							<%=_result%>
						</p>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
