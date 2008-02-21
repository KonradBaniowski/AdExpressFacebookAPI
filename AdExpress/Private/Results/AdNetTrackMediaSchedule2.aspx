<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.AdNetTrackMediaSchedule2" CodeFile="AdNetTrackMediaSchedule2.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdNetTrackMediaSchedule2</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<LINK href="/Css/MediaSchedule.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<%=_result%>
			<P><cc1:optionlayerwebcontrol id="OptionLayerWebControl1" runat="server" CssDefaultListLabel="txtViolet11Bold"
					Width="200px" GenericDetailLevelType="adnettrack" GenericDetailLevelComponentProfile="adnettrack" BackGroundColor="#E1E0DA"
					DisplayInformationComponent="True"></cc1:optionlayerwebcontrol></P>
			<P>
				<cc1:MenuWebControl id="MenuWebControl1" runat="server"></cc1:MenuWebControl></P>
		</form>
	</body>
</HTML>
