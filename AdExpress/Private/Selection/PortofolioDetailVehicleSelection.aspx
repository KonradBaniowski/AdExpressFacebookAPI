<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.PortofolioDetailVehicleSelection" CodeFile="PortofolioDetailVehicleSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
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
			
			/*function insertValueInHidden(idSession,idRepertory){
				document.Form2.idMedia.name="CKB_"+idSession+"_"+idRepertory;
			}*/
			function insertValueInHidden(idMedia){
				document.Form2.idMedia.name="CKB_"+idMedia;
			}
								
			//-->
			
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body  class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image runat="server" height="100" width="10" SkinID="logo_cote_gauche" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" class="imageBackGround">
						<table cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image runat="server" height=90 width=185 SkinID="logo" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" width="185" SkinID="plus_sous_logo" /></td>
							</tr>
							<tr>
								<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl2" runat="server" CodeDescription="1457"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Navigation Interne -->
							<TR>
								<TD><asp:Image runat="server" height="10" width="1" SkinID="pixel" /></TD>
							</TR>
							<tr vAlign="top">
								<td id="selectionRegie"><cc2:mediasellerwebcontrol id="MediaSellerWebControl2" runat="server" CodeDescription="1456" MediaDetailOption="True"></cc2:mediasellerwebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
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
								<td><asp:Image runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image runat="server" height="1" width="1" SkinID="pixel" /></td>
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
								<TD id="lineAVIrer" style="WIDTH: 773px" vAlign="top" class="imageBackGround" height="1%">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR>
											<TD vAlign="top" align="left" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
											<TD height="1%"><asp:Image runat="server" height="17" width="1" SkinID="pixel" /></TD>
											<TD vAlign="top" align="right" height="1%"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
										</TR>
									</TABLE>
								</TD>
							<!-- Centre -->
							<tr>
								<td style="WIDTH: 773px" class="whiteBackGround">
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr style="HEIGHT: 5px">
											<td colSpan="6"></td>
										</tr>
										<tr>
											<td class="txtViolet11Bold" width="15%">&nbsp;&nbsp;
												<cc1:adexpresstext language="33" id="keyWord" runat="server" Code="972"></cc1:adexpresstext>&nbsp;:
											</td>
											<td width="10%"><asp:textbox id="keyWordTextBox" runat="server"></asp:textbox></td>
											<td vAlign="middle" align="left" width="10%">&nbsp;
												<cc4:imagebuttonrolloverwebcontrol id="OkImageButtonRollOverWebControl" runat="server" SkinID="okButton"></cc4:imagebuttonrolloverwebcontrol></td>
											<td align="center" width="10%"><cc4:imagebuttonrolloverwebcontrol id="initializeButton" runat="server" SkinID="initializeButton"></cc4:imagebuttonrolloverwebcontrol></td>
											<td width="10%"></td>
											<td width="55%">&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px" class="whiteBackGround"><asp:Image runat="server" height="5" width="1" SkinID="pixel" /></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px; HEIGHT: 18px">&nbsp;&nbsp; <A id="A1" onmouseover="MM_swapImage('ok','','/images/Common/button/ok_down.gif',1)"
										onclick="javascript:createLists();" onmouseout="MM_swapImgRestore()" href="javascript:__doPostBack('ok','')" runat="server">
									</A>
								</td>
							</tr>
							<tr class="whiteBackGround">
								<td style="WIDTH: 772px; HEIGHT: 17px">
									<cc3:PortofolioDetailVehicleSelectionWebControl id="portofolioDetailVehicleSelectionWebControl" runat="server"></cc3:PortofolioDetailVehicleSelectionWebControl>
									<input id="idMedia" type="hidden" name="nameMedia">
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td align="right" width="649">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc4:imagebuttonrolloverwebcontrol id="validImageButtonRollOverWebControl" runat="server" onclick="validImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc4:imagebuttonrolloverwebcontrol></td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl>
					</td>
					<!--</TD>--><!--2005MI-->
				</tr>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD vAlign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD class="violetBackGround"></TD>
					<TD class="imageBackGround"></TD>
					<TD></TD>
					<TD id="dellCel" style="WIDTH: 732px" vAlign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><asp:Image runat="server" height="5" width="5" SkinID="croix" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
