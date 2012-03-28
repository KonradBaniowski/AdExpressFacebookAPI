﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PortofolioMediaSelection.aspx.cs" Inherits="Private_Selection_Russia_PortofolioMediaSelection" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections.Russia" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc5" Namespace="TNS.AdExpress.Web.Controls.Headers.Russia" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="JavaScript" type="text/JavaScript">
		<!--
		    function MM_preloadImages() { //v3.0
		        var d = document; if (d.images) {
		            if (!d.MM_p) d.MM_p = new Array();
		            var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
		                if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; } 
		        }
		    }

		    function MM_swapImgRestore() { //v3.0
		        var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
		    }

		    function MM_findObj(n, d) { //v4.01
		        var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
		            d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
		        }
		        if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
		        for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
		        if (!x && d.getElementById) x = d.getElementById(n); return x;
		    }

		    function MM_swapImage() { //v3.0
		        var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
		            if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
		    }

		    /*function insertValueInHidden(idSession,idRepertory){
		    document.Form2.idMedia.name="CKB_"+idSession+"_"+idRepertory;
		    }*/
		    function insertValueInHidden(idMedia) {
		        document.Form2.idMedia.name = "CKB_" + idMedia;
		    }
								
			//-->
			
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body  class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2"  method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
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
												<cc2:moduletitlewebcontrol id="ModuleTitleWebControl2" runat="server" CodeDescription="1457"></cc2:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Navigation Interne -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>

<cc5:GenericMediaDetailSelectionWebControl id="_genericMediaDetailSelectionWebControl" runat="server" NbDetailLevelItemList="3"							            
							            GenericDetailLevelComponentProfile="media" GenericDetailLevelType="dynamicCompetitorPotential"   BackGroundColor="" CssDefaultListLabel="txtBlanc11Bold" Width="194px"></cc5:genericmediadetailselectionwebcontrol>
</td>
										</tr>
										<tr class="backGroundOptions">
											<td style="padding:0px 5px 5px 0px;" align="right"><cc4:imagebuttonrolloverwebcontrol id="okDetailMediaImageButton" runat="server" SkinID="validButton"></cc4:imagebuttonrolloverwebcontrol></td>
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
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl2" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>

							<!-- Centre -->
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td class="txtViolet11Bold" style="width:83px;" >&nbsp;&nbsp;
												<cc1:adexpresstext language="33" id="keyWord" runat="server" Code="972"></cc1:adexpresstext>&nbsp;:
											</td>
											<td ><asp:textbox id="keyWordTextBox" runat="server"></asp:textbox></td>
											<td style="width:200px;" valign="bottom" >&nbsp;<cc4:imagebuttonrolloverwebcontrol id="OkImageButtonRollOverWebControl" runat="server" SkinID="okLoupeButton"></cc4:imagebuttonrolloverwebcontrol>&nbsp;<cc4:imagebuttonrolloverwebcontrol id="initializeButton" runat="server" SkinID="initializeButton"></cc4:imagebuttonrolloverwebcontrol></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr><td>&nbsp;</td></tr>
							<tr>
								<td>
									<cc3:PortofolioGenericDetailVehicleSelectionWebControl id="GenericDetailVehicleSelectionWebControl1" runat="server"></cc3:PortofolioGenericDetailVehicleSelectionWebControl>
									<input id="idMedia" type="hidden" name="nameMedia">
								</td>
							</tr>
							<tr>
								<td>
									<table cellSpacing="0" cellPadding="0" border="0" width="100%">
										<tr>
											<td align="right">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc4:imagebuttonrolloverwebcontrol id="validImageButtonRollOverWebControl" runat="server" onclick="validImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol>&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>