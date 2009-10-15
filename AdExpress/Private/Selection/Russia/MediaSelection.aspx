﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MediaSelection.aspx.cs" Inherits="Private_Selection_Russia_MediaSelection" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections.Russia" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="JavaScript" type="text/JavaScript">
		<!--
			function MM_preloadImages() { //v3.0
			var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
				var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
				if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
			}

			function MM_swapImgRestore() { //v3.0
			var i,x,a=document.MM_sr; for(i=0;a&&i<a.length&&(x=a[i])&&x.oSrc;i++) x.src=x.oSrc;
			}

			function MM_findObj(n, d) { //v4.01
			var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
				d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
			if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
			for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
			if(!x && d.getElementById) x=d.getElementById(n); return x;
			}

			function MM_swapImage() { //v3.0
			var i,j=0,x,a=MM_swapImage.arguments; document.MM_sr=new Array; for(i=0;i<(a.length-2);i+=3)
			if ((x=MM_findObj(a[i]))!=null){document.MM_sr[j++]=x; if(!x.oSrc) x.oSrc=x.src; x.src=a[i+2];}
			}
								
			//-->
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body onload="<%=saveScript%>javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2"  method="post" runat="server">
			<table style="height:600px" cellspacing="0" cellpadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td valign="top">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image ID="Image1" runat="server" height="100" width="10" SkinID="logo_cote_gauche" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image2" runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" class="imageBackGround">
						<table cellspacing="0" cellpadding="0" width="10" border="0">
							<tr valign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image3" runat="server" height="90" width="185" SkinID="logo" /></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image4" runat="server" height="5" width="185" SkinID="plus_sous_logo" /></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr valign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl2" runat="server" CodeDescription="1456"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Navigation Interne -->											
							
				            <tr>
								<td><asp:Image ID="Image20" runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
				            <tr class="whiteBackGround">
					            <td><cc4:imagebuttonrolloverwebcontrol id="okDetailMediaImageButton" runat="server" SkinID="okButton"></cc4:imagebuttonrolloverwebcontrol></td>
				            </tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image ID="Image7" runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td>
									<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td valign="top">
						<table id="Table5" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Image ID="Image8" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image9" runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image ID="Image10" runat="server" height="1" width="1" SkinID="pixel" /></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td style="WIDTH: 731px" valign="top" class="imageBackGround">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td style="WIDTH: 772px"><cc2:headerwebcontrol language="33" id="HeaderWebControl2" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td id="lineAVIrer" style="WIDTH: 773px" valign="top" class="imageBackGround"
									height="1%">
									<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td valign="top" align="left" height="1%"><asp:Image ID="Image11" runat="server" height="5" width="5" SkinID="croix" /></td>
											<td height="1%"><asp:Image ID="Image12" runat="server" height="17" width="1" SkinID="pixel" /></td>
											<td valign="top" align="right" height="1%"><asp:Image ID="Image13" runat="server" height="5" width="5" SkinID="croix" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td style="WIDTH: 773px" class="whiteBackGround">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="HEIGHT: 5px">
											<td style="WIDTH: 478px" colspan="5"></td>
										</tr>										
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 10px">
											<td style="WIDTH: 478px" width="478" class="imageBackGround" colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>
										<tr>
										    <td colspan="6">
										        <table cellspacing="0" cellpadding="0" width="400px" border="0">
										            <tr>
											            <td class="txtViolet11Bold">&nbsp;&nbsp;
												            <cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="972"></cc1:adexpresstext>&nbsp;:
											            </td>
											            <td valign="middle" align="left"><asp:textbox id="keyWordTextBox" runat="server"></asp:textbox>&nbsp;
												            <cc4:imagebuttonrolloverwebcontrol id="OkImageButtonRollOverWebControl" runat="server" SkinID="okButton" onclick="OkImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol></td>
											            <td valign="middle" align="left">
											                <cc4:imagebuttonrolloverwebcontrol id="initializeButton" runat="server" SkinID="initializeButton"></cc4:imagebuttonrolloverwebcontrol>&nbsp;<cc4:imagebuttonrolloverwebcontrol id="initializeAllButton" runat="server" SkinID="initializeAllButton"></cc4:imagebuttonrolloverwebcontrol></td>
													    </tr>
											    </table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px" class="whiteBackGround"><asp:Image ID="Image14" runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px; HEIGHT: 27px">&nbsp;&nbsp; <a id="A1" onmouseover="MM_swapImage('ok','','/images/Common/button/ok_down.gif',1)"
										onclick="javascript:createLists();" onmouseout="MM_swapImgRestore()" href="javascript:__doPostBack('ok','')" runat="server">
									</a>
								</td>
							</tr>
							
							<tr class="whiteBackGround">
								<td style="WIDTH: 772px; HEIGHT: 18px"><cc3:genericdetailvehicleselectionwebcontrol id="GenericDetailVehicleSelectionWebControl1" runat="server"></cc3:genericdetailvehicleselectionwebcontrol></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td align="right" width="649">
											    <cc4:imagebuttonrolloverwebcontrol id="nextImageButtonRollOverWebControl" runat="server" Visible="False" onclick="nextImageButtonRollOverWebControl_Click" SkinID="nextButton"></cc4:imagebuttonrolloverwebcontrol>&nbsp;
											    <cc4:imagebuttonrolloverwebcontrol id="validImageButtonRollOverWebControl" runat="server" Visible="False" onclick="validImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;&nbsp;
											</td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl>
					</td>
					<!--</td>--><!--2005MI-->
					</tr>
				<!-- ligne du bas a droite -->
				<tr style="height:5px">
					<td></td>
					<td valign="top" class="imageBackGround">
						<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><asp:Image ID="Image15" runat="server" height="5" width="5" SkinID="croix" /></td>
								<td></td>
								<td valign="bottom" align="right"><asp:Image ID="Image16" runat="server" height="5" width="5" SkinID="croix" /></td>
							</tr>
						</table>
					</td>
					<td></td>
					<td class="violetBackGround"></td>
					<td class="imageBackGround"></td>
					<td></td>
					<td id="dellCel" style="WIDTH: 732px" valign="top" class="imageBackGround">
						<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><asp:Image ID="Image17" runat="server" height="5" width="5" SkinID="croix" /></td>
								<td></td>
								<td valign="bottom" align="right"><asp:Image ID="Image18" runat="server" height="5" width="5" SkinID="croix" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</html>
