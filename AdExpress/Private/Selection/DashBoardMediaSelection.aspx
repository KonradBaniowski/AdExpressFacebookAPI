<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc4" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Selection.DashBoardMediaSelection" CodeFile="DashBoardMediaSelection.aspx.cs" %>
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
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body onload="<%=saveScript%>">
		<form id="Form2" action="AdvertiserSelection.aspx" method="post" runat="server">
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
								<td id="pageTitle"><cc2:moduletitlewebcontrol id="ModuleTitleWebControl2" runat="server" CodeDescription="1456"></cc2:moduletitlewebcontrol></td>
							</tr>
							<!-- Info bt droit -->
							<tr>
								<td><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
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
								<td style="WIDTH: 772px"><cc2:headerwebcontrol language="33" id="HeaderWebControl2" runat="server" Type_de_page="generic"></cc2:headerwebcontrol></td>
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

											<td colSpan="5"><cc3:loadableuniverswebcontrol id="LoadableUniversWebControl1" runat="server"></cc3:loadableuniverswebcontrol></td>
										</tr>
										<tr style="HEIGHT: 15px">
											<td style="WIDTH: 478px" width="478" background="/images/Common/dupli_fond.gif" colSpan="6"></td>
										</tr>

									</table>
								</td>
							</tr>

							<tr bgColor="#ffffff">
								<td>
									<TABLE id="centerTable" height="100%" cellSpacing="0" cellPadding="0" width="100%" bgColor="#ffffff"
										border="0">
										<cc3:mediaselectionwebcontrol id="MediaSelectionWebControl1" runat="server"></cc3:mediaselectionwebcontrol>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td style="WIDTH: 772px">
									<table cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td align="right" width="649"><cc4:imagebuttonrolloverwebcontrol id="saveImageButtonRollOverWebControl" runat="server" onclick="saveImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;<cc4:imagebuttonrolloverwebcontrol id="validImageButtonRollOverWebControl" runat="server" onclick="validImageButtonRollOverWebControl_Click"></cc4:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;&nbsp;</td>
											<td width="1%"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
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
								<TD>
									<cc2:MenuWebControl id="MenuWebControl2" runat="server"></cc2:MenuWebControl></TD>
								<TD vAlign="bottom" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
		<%=openDiv%>
		<%=divClose%>
	</body>
</HTML>
