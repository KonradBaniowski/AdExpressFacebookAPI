<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.RefineMediaUniverseSelection" CodeFile="RefineMediaUniverseSelection.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
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
	<body onload="<%=saveScript%>javascript:activateActiveX();" class="bodyStyle">
		<form id="Form2"  method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_gauche" width="10"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" class="imageBackGround">
						<table cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height=90 SkinID="logo" width="185" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="plus_sous_logo" width="185"/></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl2" runat="server" CodeDescription="1456"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Navigation Interne -->
							<tr>
								<td><asp:Image runat="server" height="10" SkinID="pixel" width="1"/></td>
							</tr>
							<tr vAlign="top">
								<td id="selectionRegie"><cc2:mediasellerwebcontrol id="MediaSellerWebControl2" runat="server" CodeDescription="1456" MediaDetailOption="True"></cc2:mediasellerwebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image runat="server" height="5" SkinID="pixel" width="1"/></td>
							</tr>
							<tr>
								<td>
									<cc2:InformationWebControl id="InformationWebControl1" runat="server"></cc2:InformationWebControl></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" SkinID="logo_cote_droit" width="5"/></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image runat="server" height="1" SkinID="pixel" width="1"/></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td style="WIDTH: 731px" vAlign="top" class="imageBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td style="WIDTH: 772px"><cc2:headerwebcontrol language="33" id="HeaderWebControl2" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td id="lineAVIrer" style="WIDTH: 773px" vAlign="top" class="imageBackGround"
									height="1%">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
											<td height="1%"><asp:Image runat="server" height="17" SkinID="pixel" width="1"/></td>
											<td vAlign="top" align="right" height="1%"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
										</tr>
									</TABLE>
								</td>
							<!-- Centre -->
							<tr>
								<td style="WIDTH: 773px" class="whiteBackGround">
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr style="HEIGHT: 5px">
											<td style="WIDTH: 478px" colSpan="5"></td>
										</tr>
										<tr>
											<td colSpan="5">
												<!--Chargement mes univers-->
												<cc3:loadableuniverswebcontrol id="LoadableUniversWebControl1" runat="server"></cc3:loadableuniverswebcontrol></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colSpan="6"></td>
										</tr>
										<tr style="HEIGHT: 10px">
											<td style="WIDTH: 478px" width="478" class="imageBackGround" colSpan="6"></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colSpan="6"></td>
										</tr>
										<tr>
											<td class="txtViolet11Bold" width="15%">&nbsp;&nbsp;
												<cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="972"></cc1:adexpresstext>&nbsp;:
											</td>
											<td width="10%"><asp:textbox id="keyWordTextBox" runat="server"></asp:textbox></td>
											<td vAlign="middle" align="left" width="10%">&nbsp;
												<cc4:imagebuttonrolloverwebcontrol id="OkImageButtonRollOverWebControl" runat="server" SkinID="okButton"></cc4:imagebuttonrolloverwebcontrol></td>
											<td align="center" width="10%"><cc4:imagebuttonrolloverwebcontrol id="initializeButton" runat="server" SkinID="initializeButton"></cc4:imagebuttonrolloverwebcontrol></td>
											<td width="10%"><cc4:imagebuttonrolloverwebcontrol id="initializeAllButton" runat="server" SkinID="initializeAllButton" ></cc4:imagebuttonrolloverwebcontrol></td>
											<td width="55%">&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px" class="whiteBackGround"><asp:Image runat="server" height="5" src="images/pixel.gif" width="1"/></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px; HEIGHT: 27px">&nbsp;&nbsp; <A id="A1" onmouseover="MM_swapImage('ok','','/images/Common/button/ok_down.gif',1)"
										onclick="javascript:createLists();" onmouseout="MM_swapImgRestore()" href="javascript:__doPostBack('ok','')" runat="server">
									</A>
								</td>
							</tr>
							<tr class="whiteBackGround">
								<td style="WIDTH: 772px; HEIGHT: 18px"><cc3:detailvehicleselectionwebcontrol id="AdvertiserSelectionWebControl1" runat="server"></cc3:detailvehicleselectionwebcontrol></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td align="right" width="649"><cc4:imagebuttonrolloverwebcontrol id="saveImageButtonRollOverWebControl" SkinID="saveButton" runat="server" Visible="False" onclick="saveImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol>&nbsp;<cc4:imagebuttonrolloverwebcontrol id="validImageButtonRollOverWebControl" SkinID="validateButton"  runat="server" Visible="False" onclick="validImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;&nbsp;</td>
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
				<tr height="5">
					<td></td>
					<td vAlign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td></td>
								<td vAlign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</TABLE>
					</td>
					<td></td>
					<td class="violetBackGround"></td>
					<td class="imageBackGround"></td>
					<td></td>
					<td id="dellCel" style="WIDTH: 732px" vAlign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td vAlign="bottom"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
								<td></td>
								<td vAlign="bottom" align="right"><asp:Image runat="server" height="5" SkinID="croix" width="5"/></td>
							</tr>
						</TABLE>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</html>
