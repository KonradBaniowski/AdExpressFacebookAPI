<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="dcwc" Namespace="TNS.AdExpress.Web.UI.Results.APPM" Assembly="TNS.AdExpress.Web" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc6" Namespace="TNS.AdExpress.Web.Controls.Results.Appm" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc7" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Results.APPMResults" CodeFile="APPMResults.aspx.cs" EnableEventValidation="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
		<meta http-equiv="expires" content="0" />
		<meta http-equiv="pragma" content="no-cache" />
		<meta name="Cache-control" content="no-cache" />
		<script language="javascript" type="text/javascript" src="/scripts/WebResult4.js"></script>
		<script type="text/javascript" src="/scripts/dom-drag.js"></script>
	</head>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
		<asp:HiddenField ID="zoomParam" runat="server" EnableViewState="true"/>
			<table style="height:600px" cellspacing="0" cellpadding="0" width="800" border="0">
			    <!-- Gradient -->
				<tr>
					<td colspan="7" height="3" class="gradientBar"></td>
				</tr>
			    <tr>
				    <!-- marge de gauche-->
					<td vAlign="top" class="whiteBackGround">
						<asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" />
					</td>
				    <!-- menu -->
					<td vAlign="top" class="whiteBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" class="backGroundBlack">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									
									<table id="Table6" cellspacing="0" cellpadding="0" width="185" border="0">
										<tr>
											<td class="backGroundBlack"><asp:Image ID="Image4" runat="server" height="19" width="1" SkinID="pixel" /></td>
										</tr>
										
										<!-- éléments variables du menus de gauche -->
										<tr valign="top">
											<td id="pageTitle">
												<cc2:moduletitlewebcontrol id="Moduletitlewebcontrol2" runat="server" CodeDescription="900"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Options -->
										<tr valign="top">
											<td><asp:Image ID="Image5" runat="server" height="10" SkinID="pixel" width="1" /></td>
										</tr>
										<tr valign="top">
											<td valign="top"><cc2:resultsoptionswebcontrol id="ResultsOptionsWebControl1" 
                                                    runat="server" UnitOptionAppm="False" ProductDetailOption="False"
													InsertOption="False" MediaDetailOption="False" ResultOption="True" AutoPostBackOption="False" Percentage="False" UnitOption="False"
													ShowPictures="False" InializeSlogansOption="True" ></cc2:resultsoptionswebcontrol></td>
										</tr>
										<tr class="backGroundOptions">
											<td style="padding:0px 5px 5px 0px;" align="right"><cc1:imagebuttonrolloverwebcontrol id="okImageButton" runat="server" SkinID="validButton"></cc1:imagebuttonrolloverwebcontrol></td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image7" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc2:InformationWebControl id="InformationWebControl1" runat="server" 
                                                    BackGroundColor=""></cc2:InformationWebControl></td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image8" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image9" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image11" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image12" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
	                <!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
			                <!-- Centre -->
			                <tr>
				                <td>
					                <table cellspacing="0" cellpadding="0" width="100%" border="0">
						                <%=zoomButton%>										
                                        <tr>
                                            <td><cc2:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" />
                                        </td>
                                        </tr>
						                <tr>
							                <td style="HEIGHT: 5px" ></td>
						                </tr>
                                        <tr>
                                            <td>
                                                <cc4:DetailPeriodWebControl ID="DetailPeriodWebControl1" runat="server" SkinID="detailPeriodWebControl"></cc4:DetailPeriodWebControl>
                                            </td>
                                        </tr>
						                <tr>
							                <td align="center" >
								                <cc6:AppmContainerWebControl id="AppmContainerWebControl1" runat="server" SkinID="container"></cc6:AppmContainerWebControl>
                                                <cc7:GenericMediaScheduleWebControl id="GenericMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240" ShowVersion="False"/>
                                            </td>
						                </tr>
						                <tr>
							                <td height="5"><cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></td>
						                </tr>
					                </table>
				                </td>
			                </tr>
		                </table>
	                </td>
	            </tr>
		    </table>
		</form>
	</body>
</html>
