<%@ Page language="c#" Inherits="AdExpress.Private.Results.APPMInsertion" CodeFile="APPMInsertion.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body class="backGroundWhite darkBackGround" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<cc1:MenuWebControl id="MenuWebControl2" style="Z-INDEX: 101; LEFT: -8px; POSITION: absolute; TOP: 0px"
				runat="server"></cc1:MenuWebControl>
			<table align="center">
				<tr>
					<td>
						<%=result%>
						<TABLE id="retour" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><asp:Image ID="Image1" runat="server" height="2" SkinID="pixel" width="5" /></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
