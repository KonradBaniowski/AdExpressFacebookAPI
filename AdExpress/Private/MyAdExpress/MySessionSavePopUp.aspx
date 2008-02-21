<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.MySessionSavePopUp" CodeFile="MySessionSavePopUp.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
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
	<body background="/images/Common/dupli_fond.gif">
		<form id="Form1" method="post" runat="server">
			<TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR bgColor="#644883" height="14">
					<TD width="14"><IMG src="/Images/Common/fleche_1.gif"></TD>
					<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif">&nbsp;
						<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="908"></cc2:adexpresstext></TD>
				</TR>
				<TR>
					<TD></TD>
					<TD bgColor="#ffffff"></TD>
				</TR>
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff" height="20">&gt;
						<cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="702"></cc2:adexpresstext></TD>
				</TR>
				<TR height="40">
					<TD></TD>
					<TD vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:dropdownlist id="directoryDropDownList" runat="server" Width="200px" CssClass="txtNoir11">												
						</asp:dropdownlist></TD>
				</TR>
				<TR height="10">
					<TD></TD>
					<TD></TD>
				</TR>
				<!---Debut liste des résultats-->
				<TR>
	                <TD></TD>
	                <TD class="txtViolet11Bold" bgColor="#ffffff" height="20">&gt;
		                <cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2261"></cc2:adexpresstext></TD>
                </TR>
                <TR height="40">
	                <TD></TD>
	                <TD vAlign="top" bgColor="#ffffff">&nbsp;
		                <asp:dropdownlist id="sessionDropDownList" runat="server" Width="200px" CssClass="txtNoir11">		                
		                </asp:dropdownlist></TD>
                </TR>
                <TR height="10">
	                <TD></TD>
	                <TD></TD>
                </TR>
				<!---Fin liste des résultats-->
				<TR>
					<TD></TD>
					<TD class="txtViolet11Bold" bgColor="#ffffff">&gt;
						<cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="2263"></cc2:adexpresstext></TD>
				</TR>
				<TR height="40">
					<TD></TD>
					<TD vAlign="top" bgColor="#ffffff">&nbsp;
						<asp:textbox id="mySessionTextBox" runat="server" Width="200px" CssClass="txtNoir11"></asp:textbox></TD>
				</TR>
				<TR height="20">
					<TD></TD>
					<TD vAlign="top" align="right"><cc1:imagebuttonrolloverwebcontrol id="oKImageButtonRollOverWebControl" runat="server" ImageUrl="/images/Common/button/ok_out_up.gif"
							RollOverImageUrl="/images/Common/button/ok_out_down.gif" onclick="oKImageButtonRollOverWebControl_Click"></cc1:imagebuttonrolloverwebcontrol>&nbsp;<cc1:imagebuttonrolloverwebcontrol id="cancelImageButtonRollOverWebControl" runat="server" onclick="cancelImageButtonRollOverWebControl_Click"></cc1:imagebuttonrolloverwebcontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
