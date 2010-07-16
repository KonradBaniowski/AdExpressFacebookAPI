<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Universe.UniverseDetailPopUp" CodeFile="UniverseDetailPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
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
	</HEAD>
	<body class="imageBackGround" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR class="violetBackGround" height="14">
					<TD width="14"><asp:Image ID="Image153" runat="server" SkinID="fleche_1" /></TD>
					<TD class="txtBlanc11Bold bandeauTitreBackGround">&nbsp;
						<cc1:adexpresstext language="33" id="detailUniversAdExpressText" runat="server" Code="960"></cc1:adexpresstext>&nbsp;
						<asp:label id="universeLabel" runat="server"></asp:label></TD>
				</TR>
				<!--Annonceurs / Références ...-->
				<TR>
					<TD style="HEIGHT: 18px"></TD>
					<TD class="txtViolet11Bold backGroundWhite" style="HEIGHT: 18px">&nbsp;
						<cc1:adexpresstext language="33" id="advertiserAdexpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="center" class="backGroundWhite"><%=advertiserText%></TD>
				</TR>
				<TR height="5">
					<TD></TD>
					<TD class="backGroundWhite"></TD>
				</TR>
				<TR height="7">
					<TD colSpan="2"></TD>
				</TR>
				<!--Fermer-->
				<TR>
					<TD align="right" colSpan="2"><cc2:imagebuttonrolloverwebcontrol id="closeImageButtonRollOverWebControl" runat="server" onclick="closeImageButtonRollOverWebControl_Click" SkinID="fermerButton" ></cc2:imagebuttonrolloverwebcontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
