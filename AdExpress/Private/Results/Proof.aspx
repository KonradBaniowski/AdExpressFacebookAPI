<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.Proof" CodeFile="Proof.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body class="darkBackGround">
		<form id="Form1" method="post" runat="server">
			<table align="center">
				<tr>
					<td>
						<cc5:proofresultwebcontrol id="proofresultwebcontrol1" runat="server"></cc5:proofresultwebcontrol>
						<table id="retour" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="2" SkinID="pixel" width="5" /></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl>
						<table id="proff" cellspacing="0" cellpadding="0" border="0">
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
