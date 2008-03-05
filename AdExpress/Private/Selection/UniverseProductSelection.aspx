<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UniverseProductSelection.aspx.cs" Inherits="Private_Selection_UniverseProductSelection" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<LINK href="/Css/Universe.css" type="text/css" rel="stylesheet">
	    <script language="javascript" src="/scripts/UniverseWebcontrol.js"></script>
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
			//textBoxAdvertiserChoice
			function openHelp(sessionId){
				var oN=document.all.item('textBoxAdvertiserChoice');
				var oT=oN.value;
				if(oT.length>0){
					window.open("../Informations/ProductLevelsSearch.aspx?idSession="+sessionId+"&wordToSearch="+oN.value, '', "top="+(screen.height-269)/2+", left="+(screen.width-450)/2+",toolbar=0, directories=0, status=0, menubar=0, width=450, height=269, scrollbars=0, location=0, resizable=0");
				}
			}					
			//-->
			
			function ouvre_popup_univers(sessionId) {
                window.open("/Private/Universe/RegisterUniverse.aspx?idSession="+sessionId+"&brancheType=2&idUniverseClientDescription=16","nom_popup","menubar=no, status=no, scrollbars=no, menubar=no, width=450px, height=300px");
            }
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
	</head>
<body onload="initjsDOMenu();<%=saveScript%>">
		<form id="Form2" action="UniverseProductSelection.aspx" method="post" runat="server">
			<table height="600" cellspacing="0" cellpadding="0" border="0">
				<tr>	
					<td valign="top"><!-- marge de gauche-->
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="100" src="/images/Common/logo_cote_gauche.gif" width="10"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><img height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td valign="top" background="/images/Common/dupli_fond.gif">
						<table cellspacing="0" cellpadding="0" width="10" border="0">
							<tr valign="top" bgColor="#ffffff">
								<!-- Logo -->
								<td><img height="90" src="/images/<%=_siteLanguage%>/logo/logo.gif" width="185" ></td>
							</tr>
							<tr>
								<td><img height="5" src="/images/Common/plus_sous_logo.gif" width="185"></td>
							</tr>
							<tr>
								<td><img height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<!-- �l�ments variables du menus de gauche -->
							<tr valign="top">
								<td id="pageTitle"><cc1:moduletitlewebcontrol id="ModuleTitleWebControl1" runat="server" CodeDescription="772"></cc1:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><img height="5" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td><cc1:InformationWebControl id="InformationWebControl1" runat="server" InLeftMenu="True"></cc1:InformationWebControl></td>
							</tr>
						</table>
					</td>
					<!-- S�parateur -->
					<td valign="top">
						<table id="Table5" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="100" src="/images/Common/logo_cote_droit.gif" width="5"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><img height="1" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<td bgColor="#644883"><img height="1" src="/images/Common/pixel.gif" width="1"></td>
					<td width="10" background="/images/Common/dupli_fond.gif">&nbsp;</td>
					<td width="10" bgcolor="#ffffff">&nbsp;</td>
					<!-- droite-->
					<td style="WIDTH: 731px" valign="top" background="/images/Common/dupli_fond.gif">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td style="WIDTH: 772px"><cc1:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td id="lineAVIrer" style="WIDTH: 773px" valign="top" background="/Images/Common/dupli_fond.gif"
									height="1%">
									<table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
										<TR>
											<td valign="top" align="left" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
											<td height="1%"><img height="17" src="/Images/Common/pixel.gif" width="1"></td>
											<td valign="top" align="right" height="1%"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
										</TR>
									</table>
								</td>
							<!-- Centre -->
							<tr>
								<td style="WIDTH: 773px" bgColor="#ffffff">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="HEIGHT: 5px">
											<td style="WIDTH: 478px" colspan="5"></td>
										</tr>
										<!--emplacement du chargement des univers-->
										<tr>
											<td colspan="5">
												<!--Chargement mes univers-->
												<cc4:loadableuniverswebcontrol id="LoadableUniversWebControl1" runat="server"></cc4:loadableuniverswebcontrol>											
												</td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 10px">
											<td style="WIDTH: 478px" width="478" background="/images/Common/dupli_fond.gif" colspan="6"></td>
										</tr>
										<tr style="HEIGHT: 5px">
											<td colspan="6"></td>
										</tr>																				
									</table>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px" bgColor="#ffffff"><img height="5" src="/Images/Common/pixel.gif" width="1"></td>
							</tr>														
							<tr>
								<td   bgColor="#ffffff"><!--<img height="15" src="/Images/Common/pixel.gif" width="1"> -->
									<cc4:SelectItemsInClassificationWebControl id="SelectItemsInClassificationWebControl1" runat="server" ChildNodeExcludeCss="txtChildNodeExcludeCss" ChildNodeIncludeCss="txtChildNodeIncludeCss" CustomSelectCss="customSelectCss" DefaultBranchId="1" LabelNbCurrentElementCss="labelNbelements" ListBoxBodyCss="listBoxBodyCss" ListBoxCss="listBoxCss" ListBoxHeaderCss="listBoxHeaderCss" NbMaxIncludeTree="2" ParentNodeChildExcludeCss="txtParentNodeChildExcludeCss" ParentNodeChildIncludeCss="txtParentNodeChildIncludeCss" SelectOptionItemDefaultCss="optionItemDefault" SelectOptionItemHierarchyCss="optionItemHierarchy" TrashNodeCss="trashNodeCss" TreeExcludeFrameBodyCss="treeExcludeFrameBodyCss" TreeExcludeFrameCss="treeExcludeFrameCss" TreeExcludeFrameHeaderCss="treeExcludeFrameHeaderCss" TreeIncludeFrameBodyCss="treeIncludeFrameBodyCss" TreeIncludeFrameCss="treeIncludeFrameCss" TreeIncludeFrameHeaderCss="treeIncludeFrameHeaderCss" TreeViewIcons="/Styles/TreeView/Icons" TreeViewScripts="/Styles/TreeView/Scripts" TreeViewStyles="/Styles/TreeView/Css" DBSchema="adexpr03"></cc4:SelectItemsInClassificationWebControl>
									</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td align="right" width="649">&nbsp;
												<A style="cursor:hand;" onmouseover="bouton.src='/Images/<%=_siteLanguage%>/button/enregistrer_univers_down.gif';" onmouseout="bouton.src = '/Images/<%=_siteLanguage%>/button/enregistrer_univers_up.gif';" onclick="ouvre_popup_univers('<%=sessionId%>');" >
													<img src="/Images/<%=_siteLanguage%>/button/enregistrer_univers_up.gif" border="0" name="bouton"></A>&nbsp;
												&nbsp;<cc2:imagebuttonrolloverwebcontrol id="validateButton" runat="server" onclick="validateButton_Click"></cc2:imagebuttonrolloverwebcontrol></td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<cc2:redirectwebcontrol id="RedirectWebControl" runat="server"></cc2:redirectwebcontrol><cc1:menuwebcontrol id="MenuWebControl2" runat="server"></cc1:menuwebcontrol>
						</td>
					<!--</td>--></tr>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<td></td>
					<td valign="top" background="/Images/Common/dupli_fond.gif">
						<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
							<TR>
								<td valign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5">&nbsp;</td>
								<td></td>
								<td valign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
							</TR>
						</table>
					</td>
					<td></td>
					<td bgcolor="#644883"></td>
					<td background="/Images/Common/dupli_fond.gif"></td>
					<td></td>
					<td id="dellCel" style="WIDTH: 732px" valign="top" background="/Images/Common/dupli_fond.gif">
						<table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="bottom"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
								<td>&nbsp;</td>
								<td valign="bottom" align="right"><img height="5" src="/Images/Common/croix.gif" width="5"></td>
							</TR>
						</table>
					</td>
				</TR>
			</table>
		</form>
		<%=divClose%>
	</body>
</html>

