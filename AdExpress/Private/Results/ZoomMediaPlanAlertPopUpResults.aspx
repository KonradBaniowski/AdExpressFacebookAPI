<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.ZoomMediaPlanAlertPopUpResults" CodeFile="ZoomMediaPlanAlertPopUpResults.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/MediaSchedule.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body bgcolor="#ffffff" style="BACKGROUND-ATTACHMENT: fixed; BACKGROUND-IMAGE: url(/Images/Common/dark_back.bmp); BACKGROUND-REPEAT: repeat-x">
		<form id="Form1" method="post" runat="server">
			<table>
				<tr>
					<td>
						<%=result%>
						<TABLE id="retour" cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<TD><IMG height="2" src="/Images/Common/pixel.gif" width="5"></TD>
							</TR>
							<TR>
								<td>
									<a href="javascript:history.back(-<%=backPageNb%>)" onmouseover="bouton1.src='/Images/<%=_siteLanguage%>/button/back_down.gif';" onmouseout="bouton1.src = '/Images/<%=_siteLanguage%>/button/back_up.gif';">
										<img src="/Images/<%=_siteLanguage%>/button/back_up.gif" border=0 name=bouton1></a>
								</td>
							</TR>
						</TABLE>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
		<cc1:MenuWebControl id="MenuWebControl2" style="Z-INDEX: 101; LEFT: 56px; POSITION: absolute; TOP: 16px"
			runat="server"></cc1:MenuWebControl>
	</body>
</HTML>
