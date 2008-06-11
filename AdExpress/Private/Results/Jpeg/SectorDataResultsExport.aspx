<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Results.Appm" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.Jpeg.SectorDataResultsExport" CodeFile="SectorDataResultsExport.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body onload="javascript:setWindow();" class="darkBackGround"
		bottomMargin="25" leftMargin="0" topMargin="25" rightMargin="0">
		<form id="Form1" method="post" runat="server">
			<div align="center">
				<cc2:SectorDataContainerWebControl id="sectorDataContainerWebControl1" runat="server" SkinID="SectorData"></cc2:SectorDataContainerWebControl>
			</div>
		</form>
		<%=divClose%>
	</body>
</html>
