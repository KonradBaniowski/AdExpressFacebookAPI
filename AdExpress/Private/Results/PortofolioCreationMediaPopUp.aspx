<%@ Page language="c#" Inherits="AdExpress.Private.Results.PortofolioCreationMediaPopUp" CodeFile="PortofolioCreationMediaPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>

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
		<script language="javascript">

		</script>
		<BODY class="darkBackGround whiteBackGround" onload="javascript:goToAnchorImage('<%=pageAnchor%>');">
		<form id="Form1" method="post" runat="server">
		<%=result%>
		<cc1:displaymediaPageswebcontrol id="displaymediaPageswebcontrol1" runat="server"></cc1:displaymediaPageswebcontrol>
		</form>
	</BODY>
</HTML>
