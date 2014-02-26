<%@ Import Namespace="System.Data" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.VpScheduleResults" CodeFile="VpScheduleResults.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results.VP" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" width="800" border="0">
				<!-- Gradient -->
				<tr>
					<td height="3" class="gradientBar"></td>
				</tr>
				<!-- Header -->
				<tr height="3">

		            <!-- marge de gauche-->
		            <td valign="top">
		                <table cellspacing="0" cellpadding="0" width="100%" border="0" style="font-size:0px;">
		                <tr><td width="1px" valign="top"><asp:Image ID="Image2" runat="server" border="0" SkinID="LogoAdexpress" /></td><td valign="top"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td></tr>
	                    </table>
		            </td>
				</tr>
				<!-- Resultat -->
				<tr>
					<td valign="top">
						<table cellspacing="0" cellpadding="0" width="100%" border="0" class="vpScheduleResults">
						    <tr>
                                <!-- Title -->
                                <td><cc3:AdExpressText language="33" id="_adExpressText1" runat="server"></cc3:AdExpressText></td>
                            </tr>
                            <tr>
                                <td style="border-bottom: 1px dotted #000000;font-size:1px;">&nbsp;</td>
                            </tr>
                            <tr>
                                <td valign="top" align="center">
								    <cc5:VpScheduleContainerWebControl ID="VpScheduleContainerWebControl1" runat="server" SkinID="VpScheduleContainerWebControl"/>
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
		</form>
	</body>
</html>
