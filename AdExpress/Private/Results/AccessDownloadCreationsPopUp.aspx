<%--%><%@ Page language="c#" Inherits="AdExpress.Private.Results.AccessDownloadCreationsPopUp" CodeFile="AccessDownloadCreationsPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>
			<%=title%>
		</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">

		<meta http-equiv="expires" content="0">

		<meta http-equiv="pragma" content="no-cache">

		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bgcolor="#ffffff" style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x"
		bottomMargin="0" topMargin="20">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" cellSpacing="4" cellPadding="1" align="center" bgColor="white" border="0">
				<tr>
					<td bgcolor="#644883" colSpan="2" align="right"><IMG src="../../Images/Common/bandeau400.GIF"></td>
				</tr>
				<TR vAlign="middle">
					<TD align="center" colSpan="2"><asp:radiobuttonlist id="RadioButtonList1" runat="server" AutoPostBack="True" CssClass="txtViolet12Bold"
							RepeatColumns="2"></asp:radiobuttonlist></TD>
				</TR>
				<tr vAlign="middle">
					<td colspan="2" align="center">
						<asp:Label CssClass="txtViolet11Bold" id="Label1" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td height="5" colspan="2"></td>
				</tr>
				<%=creationsLine%>
			</TABLE>
		</form>
	</body>
</HTML>
--%>
