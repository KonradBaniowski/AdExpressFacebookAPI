<%@ Page language="c#" Inherits="AdExpress.Private.Results.APPMProof" CodeFile="APPMProof.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
	</HEAD>
	<body bgcolor="#ffffff" style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x">
		<form id="Form1" method="post" runat="server">
			<table align="center">
				<tr>
					<td>
						<%=result%>
						<TABLE id="retour" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG height="2" src="/Images/Common/pixel.gif" width="5"></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
