<%@ Page language="c#" Inherits="AdExpress.Private.Results.VpScheduleResults" CodeFile="VpScheduleResults.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
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
			<table height="600" cellspacing="0" cellpadding="0" width="800" border="0">
				<!-- Gradient -->
				<tr>
					<td height="3" class="gradientBar"></td>
				</tr>
				<!-- Header -->
				<tr>
				    <td valign="top">
				        <table cellspacing="0" cellpadding="0" width="100%" border="0">
				            <tr>
					            <!-- marge de gauche-->
					            <td valign="top">
						            <asp:Image ID="Image1" runat="server" border="0" SkinID="logo_cote_gauche_bis" />
					            </td>
					            <td valign="top">
					                <asp:Image ID="Image2" runat="server" border="0" SkinID="LogoAdexpress" />
					            </td>

					            <!-- Right column -->
					            <td valign="top"><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
					        </tr>
					    </table>
					</td>
				</tr>
				<!-- Resultat -->
				<tr>
					<td>
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
						    <tr>
                                <!-- Title -->
                                <td></td>
                            </tr>
                            <tr>
                                <!-- Selection -->
                                <td></td>
                            </tr>
                            <tr>
								<!-- Tableau de R�sultat -->
								<td></td>
							</tr>
						</table>
					</td>
				</tr>
            </table>
		</form>
	</body>
</html>
