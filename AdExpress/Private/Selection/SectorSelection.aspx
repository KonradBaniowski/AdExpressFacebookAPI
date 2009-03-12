<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.SectorSelection" CodeFile="SectorSelection.aspx.cs" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="/Css/AdExpress.css" type="text/css" rel="stylesheet">
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
	</HEAD>
	<body onload="<%=saveScript%>">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="/images/Common/logo_cote_gauche.gif" width="10"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" background="/images/Common/dupli_fond.gif">
						<table cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<td><IMG height=90 src="/images/<%=_siteLanguage%>/logo/logo.gif" width=185 ></td>
							</tr>
							<tr>
								<td><IMG height="5" src="/images/Common/plus_sous_logo.gif" width="185"></td>
							</tr>
							<tr>
								<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<!-- éléments variables du menus de gauche -->
							<tr vAlign="top">
								<td id="pageTitle"><cc1:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="772"></cc1:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td><cc1:informationwebcontrol id="InformationWebControl1" runat="server"></cc1:informationwebcontrol></td>
							</tr>
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="/images/Common/logo_cote_droit.gif" width="5"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
					<td width="10" background="/images/Common/dupli_fond.gif">&nbsp;</td>
					<td width="10" bgColor="#ffffff">&nbsp;</td>
					<!-- droite-->
					<td style="WIDTH: 731px" vAlign="top" background="/images/Common/dupli_fond.gif">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td style="WIDTH: 772px"><cc1:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<TD id="lineAVIrer" style="WIDTH: 773px" vAlign="top" background="/Images/Common/dupli_fond.gif"
									height="1%">
									<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR>
											<TD vAlign="top" align="left" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
											<TD height="1%"><IMG height="17" src="/Images/Common/pixel.gif" width="1"></TD>
											<TD vAlign="top" align="right" height="1%"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
										</TR>
									</TABLE>
								</TD>
							<!-- Centre -->
							<tr>
								<td style="WIDTH: 773px" bgColor="#ffffff">
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr style="HEIGHT: 5px">
											<td style="WIDTH: 478px" colSpan="5"></td>
										</tr>
										<tr>
											<td colSpan="5">
												<!--Chargement mes univers
												<table style="BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid"
													cellSpacing="0" cellPadding="0" width="200" border="0">
													<tr>
														<td class="txtViolet11Bold">&nbsp;<cc3:adexpresstext language="33" id="AdExpressText8" runat="server" Code="893"></cc3:adexpresstext>&nbsp;</td>
														<td align="right"><IMG style="CURSOR: hand" onclick="showHideContent6('listAdvertiser');" src="/Images/Common/button/bt_arrow_down.gif"
																align="absMiddle">
														</td>
													</tr>
												</table>
												<div id="listAdvertiserContent6" style="BORDER-RIGHT: #644883 1px solid; DISPLAY: none; BORDER-LEFT: #644883 1px solid; WIDTH: 620px; BORDER-BOTTOM: #644883 1px solid">
													<table cellSpacing="0" cellPadding="0" width="100%" align="center" bgColor="#ffffff" border="0">
														<tr>
															<td width="199"><IMG height="1" src="/images/Common/pixel.gif"></td>
															<td style="BORDER-TOP: #644883 1px solid" width="421"><IMG height="1" src="/images/Common/pixel.gif"></td>
														</tr>
														<tr>
															<td class="txtGris11Bold" style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px"
																colSpan="2">&nbsp;</td>
														</tr>
														<tr>
															<td style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px"
																colSpan="2">
																<!--Tableau des univers
																<%=listUniverses%>
																<input id="idMySession" type="hidden" name="nameMySession">
															</td>
														</tr>
														<tr>
															<td style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 5px; PADDING-TOP: 2px; TEXT-ALIGN: right"
																colSpan="2"><cc2:imagebuttonrolloverwebcontrol id="loadImageButtonRollOverWebControl" runat="server"></cc2:imagebuttonrolloverwebcontrol></td>
														</tr>
													</table>
												</div>--><cc4:loadableuniverswebcontrol id="LoadableUniversWebControl1" runat="server" SelectionPage="False"></cc4:loadableuniverswebcontrol></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colSpan="6"></td>
										</tr>
										<tr style="HEIGHT: 10px">
											<td style="WIDTH: 478px" width="478" background="/images/Common/dupli_fond.gif" colSpan="6"></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colSpan="6"></td>
										</tr>
										<tr>
											<td class="txtViolet11Bold" style="WIDTH: 83px">&nbsp;
												<cc3:adexpresstext language="33" id="AdExpressText4" runat="server" Code="972"></cc3:adexpresstext>&nbsp;:
											</td>
											<td style="WIDTH: 397px"><asp:textbox id="textBoxAdvertiserChoice" runat="server"></asp:textbox></td>
											<td vAlign="middle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc2:imagebuttonrolloverwebcontrol id="initializeImageButtonRollOverWebControl1" runat="server" RollOverImageUrl="/Images/Common/button/initialize_down.gif"
													ImageUrl="/Images/Common/button/initialize_up.gif" onclick="initializeImageButtonRollOverWebControl1_Click">Nouvelle Recherche</cc2:imagebuttonrolloverwebcontrol>
											</td>
											<td colSpan="3">&nbsp;</td>
										</tr>
										<tr>
											<td style="WIDTH: 83px"><IMG height="1" src="/images/Common/pixel.gif" width="80"></td>
											<td class="txtGris10" style="WIDTH: 397px" colSpan="5"><cc3:adexpresstext language="33" id="AdExpressText5" runat="server" Code="156"></cc3:adexpresstext></td>
										</tr>
										<tr>
											<td style="WIDTH: 83px"><IMG height="1" src="/images/Common/pixel.gif" width="80"></td>
											<td class="txtGris10" style="WIDTH: 397px" colSpan="5"><cc3:adexpresstext language="33" id="AdExpressText7" runat="server" Code="811"></cc3:adexpresstext></td>
										</tr>
										<tr>
											<td style="HEIGHT: 10px" colSpan="6"></td>
										</tr>
										<tr>
											<td width="5%"></td>
											<td class="txtViolet11" width="25%"><cc3:adexpresstext language="33" id="AdexpressSector" runat="server" Code="962"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="RadiobuttonSector" runat="server" Checked="True" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td width="10%"></td>
											<td class="<%=cssHoldingCompany%>" width="20%" ><cc3:adexpresstext language="33" id="AdExpressHolding" runat="server" Code="801"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="radioButtonHoldingCompany" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td colSpan="3">&nbsp;</td>
										</tr>
										<tr>
											<td width="5%"></td>
											<td class="txtViolet11" width="25%"><cc3:adexpresstext language="33" id="AdexpresstextSubSector" runat="server" Code="963"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="RadiobuttonSubSector" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td width="10%"></td>
											<td class="txtViolet11" width="20%"><cc3:adexpresstext language="33" id="AdExpressAdvertiser" runat="server" Code="802"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="radiobuttonAdvertiser" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td colSpan="3">&nbsp;</td>
										</tr>
										<tr>
											<td width="5%"></td>
											<td class="txtViolet11" width="25%"><cc3:adexpresstext language="33" id="AdexpresstextGroup" runat="server" Code="964"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="RadiobuttonGroup" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td width="10%"></td>
											<td class="txtViolet11" width="20%"><cc3:adexpresstext language="33" id="AdExpressProduct" runat="server" Code="803"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="radiobuttonProduct" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td colSpan="3">&nbsp;</td>
										</tr>
										<tr>
											<td width="5%"></td>
											<td class="<%=cssBrand%>" width="20%"><cc3:adexpresstext language="33" id="brandText" runat="server" Code="1584"></cc3:adexpresstext></td>
											<td width="20%"><asp:radiobutton id="RadiobuttonBrand" runat="server" GroupName="ChoiceAdvertiser"></asp:radiobutton></td>
											<td width="10%"></td>
											<td class="txtViolet11" width="20%">&nbsp;</td>
											<td width="20%">&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px" bgColor="#ffffff"><IMG height="5" src="images/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px; HEIGHT: 18px">&nbsp;&nbsp; <A id="A1" onmouseover="MM_swapImage('ok','','/images/Common/button/ok_down.gif',1)"
										onclick="javascript:createLists();" onmouseout="MM_swapImgRestore()" href="javascript:__doPostBack('ok','')" runat="server">
									</A>
									<cc2:imagebuttonrolloverwebcontrol id="okButtonRollOverWebControl2" runat="server" onclick="okButtonRollOverWebControl2_Click"></cc2:imagebuttonrolloverwebcontrol></td>
							</tr>
							<!--	<tr class="txtGris11Bold">
						<td style="HEIGHT: 57px" width="450" >
						
				<td bgcolor=#ffffff>
						
						<IMG height="20" src="images/pixel.gif" width="1">
							<cc3:adexpresstext language="33" id="AdExpressText6" runat="server" Code="812"></cc3:adexpresstext></td>
					</tr> -->
							<tr>
								<td style="WIDTH: 772px"><IMG height="15" src="images/pixel.gif" width="1"> <input id="idHoldingCompanyAccess" type="hidden" name="nameHoldingAccess_">
									<input id="idProductAutomatic" type="hidden" name="nameProductAutomatic_"> <input id="idHoldingCompanyException" type="hidden" name="nameHoldingException_">
									<input id="idAdvertiserAccess" type="hidden" name="nameAdvertiserAccess_"> <input id="idAdvertiserException" type="hidden" name="nameAdvertiserException_">
									<input id="idProductAccess" type="hidden" name="nameProductAccess_"> <input id="idProductException" type="hidden" name="nameProductException_">
									<cc4:advertiserselectionwebcontrol id="AdvertiserSelectionWebControl1" runat="server"></cc4:advertiserselectionwebcontrol></td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td align="right" width="649">&nbsp;
												<cc2:imagebuttonrolloverwebcontrol id="saveUniverseImageButtonRollOverWebControl" runat="server" onclick="saveUniverseImageButtonRollOverWebControl_Click"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click"></cc2:imagebuttonrolloverwebcontrol></td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:redirectwebcontrol id="RedirectWebControl" runat="server"></cc2:redirectwebcontrol></td>
					<!--</TD>--><!--2005MI-->
					</tr>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD bgColor="#644883"></TD>
					<TD background="/Images/Common/dupli_fond.gif"></TD>
					<TD></TD>
					<TD id="dellCel" style="WIDTH: 732px" vAlign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="bottom"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD><cc1:menuwebcontrol id="MenuWebControl2" runat="server"></cc1:menuwebcontrol></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
