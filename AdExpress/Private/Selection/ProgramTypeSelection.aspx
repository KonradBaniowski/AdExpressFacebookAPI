<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.ProgramTypeSelection" CodeFile="ProgramTypeSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
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
			//textBoxAdvertiserChoice
			function openHelp(sessionId){
				var oN=document.all.item('textBoxAdvertiserChoice');
				var oT=oN.value;
				if(oT.length>0){
					window.open("../Informations/ProductLevelsSearch.aspx?idSession="+sessionId+"&wordToSearch="+oN.value, '', "top="+(screen.height-269)/2+", left="+(screen.width-450)/2+",toolbar=0, directories=0, status=0, menubar=0, width=450, height=269, scrollbars=0, location=0, resizable=0");
				}
			}					
			//-->
			
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
	<body onload="<%=saveScript%>javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellspacing="0" cellpadding="0" border="0">
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
												<cc1:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="2058"></cc1:moduletitlewebcontrol>
											</td>
										</tr>
										
										<!-- Info bt droit -->
										<tr>
											<td><asp:Image ID="Image5" runat="server" height="5" width="1" SkinID="pixel" /></td>
										</tr>
										<tr>
											<td>
												<cc1:InformationWebControl id="InformationWebControl1" runat="server" 
                                                    InLeftMenu="True" BackGroundColor=""></cc1:InformationWebControl></td>
										</tr>
										
									</table>
									
								</td>
							</tr>
							<tr>
								<td><asp:Image ID="Image6" runat="server" height="10" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image7" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image8" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image9" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image10" runat="server" height="1" width="2" SkinID="pixel" /></td>
					
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
						  <!-- Menu haut -->
							<tr>
								<td><cc1:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
									
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td colspan="5" style="padding-left:5px;">
												<!--Chargement mes univers-->
												<cc4:loadableuniverswebcontrol id="LoadableUniversWebControl1" runat="server" SelectionPage="False"></cc4:loadableuniverswebcontrol>
											</td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 10px">
											<td style="WIDTH: 478px" class="imageBackGround" colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>
										<tr>
											<td>
											    <table cellspacing="0" cellpadding="0" border="0">
										            <tr>
										                <td class="txtViolet11Bold" style="width:83px;" >&nbsp;&nbsp;
										                    <cc3:adexpresstext language="33" id="AdExpressText4" runat="server" Code="972"></cc3:adexpresstext>&nbsp;:
										                </td>
										                <td ><asp:textbox id="textBoxProgramTypeChoice" runat="server"></asp:textbox></td>
											            <td style="width:200px;" valign="bottom" >&nbsp;<cc2:imagebuttonrolloverwebcontrol id="okButtonRollOverWebControl2" SkinID="okLoupeButton" runat="server" onclick="okButtonRollOverWebControl2_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="initializeImageButtonRollOverWebControl1" runat="server" SkinID="initializeButton" onclick="initializeImageButtonRollOverWebControl1_Click"></cc2:imagebuttonrolloverwebcontrol></td>
										            </tr>
											    </table>
											</td>
										</tr>
								    </table>
								    
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px"><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px"><asp:Image runat="server" height="15" SkinID="pixel" width="1"/> <input id="idHoldingCompanyAccess" type="hidden" name="nameHoldingAccess_"/>
									<input id="idProductAutomatic" type="hidden" name="nameProductAutomatic_"/> <input id="idHoldingCompanyException" type="hidden" name="nameHoldingException_"/>
									<input id="idAdvertiserAccess" type="hidden" name="nameAdvertiserAccess_"/> <input id="idAdvertiserException" type="hidden" name="nameAdvertiserException_"/>
									<input id="idProductAccess" type="hidden" name="nameProductAccess_"/> <input id="idProductException" type="hidden" name="nameProductException_"/>
									<cc4:ProgramTypeWebControl id="AdvertiserSelectionWebControl1" runat="server"></cc4:ProgramTypeWebControl>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 100%">
									<table cellspacing="0" cellpadding="0" border="0" style="WIDTH: 100%">
										<tr>
											<td align="right" width="100%">&nbsp;
												<cc2:imagebuttonrolloverwebcontrol SkinID="saveButton" id="saveUniverseImageButtonRollOverWebControl" runat="server" onclick="saveUniverseImageButtonRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="formSponsoringshipImageButtonRollOverWebControl" runat="server" SkinID="formButton" onclick="formSponsoringshipImageButtonRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click" SkinID="validateButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;</td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:redirectwebcontrol id="RedirectWebControl" runat="server"></cc2:redirectwebcontrol>
						<cc1:menuwebcontrol id="MenuWebControl2" runat="server"></cc1:menuwebcontrol>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</html>
