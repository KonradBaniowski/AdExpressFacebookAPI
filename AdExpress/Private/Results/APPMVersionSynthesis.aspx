<%@ Page language="c#" Inherits="AdExpress.Private.Results.APPMVersionSynthesis" CodeFile="APPMVersionSynthesis.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
		<meta http-equiv="expires" content="0" />
		<meta http-equiv="pragma" content="no-cache" />
		<meta name="Cache-control" content="no-cache" />
	</head>
	<body class="darkBackGround" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<cc1:MenuWebControl id="MenuWebControl2" style="Z-INDEX: 101; LEFT: -8px; POSITION: absolute; TOP: 0px"
				runat="server"></cc1:MenuWebControl>
			<table align="center">
				<tr>
					<td>
						<%=result%>
						<table id="retour" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="2" SkinID="pixel" width="5" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</html>