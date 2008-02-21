<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.index" CodeFile="index.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="/favicon.ico" rel="SHORTCUT ICON"/>
		<link href="Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<script language="javascript" type="text/javascript" src="/scripts/CookiesJScript.js"></script>
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
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>

		<meta http-equiv="expires" content="0"/>

		<meta http-equiv="pragma" content="no-cache"/>

		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body onload="MM_preloadImages('images/valider_down.gif')">
		<form id="Form2" method="post" runat="server">
			<script language="javascript" type="text/javascript">
				var language = <%=_siteLanguage%>;
				<%=_setLanguage%>
				var cook = GetCookie('language');
				if (cook != null){
					if (language != cook){
						document.location="/index.aspx?sitelanguage="+cook;
					}
				}
			</script>
			<table cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="images/Common/logo_cote_gauche.gif" width="10"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" background="images/Common/dupli_fond.gif">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<td><IMG height="90" src="images/Common/logoTNShome.gif" width="185"></td>
							</tr>
							<tr>
								<td><IMG height="5" src="images/Common/plus_sous_logo.gif" width="185"></td>
							</tr>
							<tr>
								<td><IMG height="40" src="images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td class="txtNoir11Bold" width="185" bgColor="#ffffff" style="HEIGHT: 9px">
									<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; PADDING-TOP: 5px">
										<cc1:AdExpressText language="33" id="AdExpressText3" runat="server" Code="821"></cc1:AdExpressText></p>
								</td>
							</tr>
							<tr>
								<td><img height="10" src="images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff">
									<table id="Table4" cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td colspan="2"><img height="5" src="images/Common/pixel.gif" width="6"></td>
										</tr>
										<tr>
											<td><img height="1" src="/Images/Common/pixel.gif" width="5"></td>
											<td><asp:textbox id="loginTextbox" runat="server" CssClass="txtNoir11Bold"></asp:textbox></td>
										</tr>
										<tr>
											<td colspan="2"><img height="5" src="images/Common/pixel.gif" width="6"></td>
										</tr>
										<tr>
											<td><img height="1" src="/Images/Common/pixel.gif" width="5"></td>
											<td><asp:textbox id="passwordTextbox" runat="server" CssClass="txtNoir11Bold" TextMode="Password"></asp:textbox></td>
										</tr>
										<tr>
											<td colspan="2"><img height="10" src="images/Common/pixel.gif" width="6"></td>
										</tr>
										<tr>
											<td><img height="1" src="/Images/Common/pixel.gif" width="5"></td>
											<!-- <td><A onmouseover="MM_swapImage('valider1','','images/valider_down.gif',1)" onmouseout="MM_swapImgRestore()"
													href="#"><IMG id="valider1" height="13" alt="valider" src="images/valider_up.gif" width="93" border="0"
														name="valider1"></A></td> -->
											<td><cc2:imagebuttonrolloverwebcontrol id="ImageButtonRollOverWebControl1" runat="server" onclick="ImageButtonRollOverWebControl1_Click"></cc2:imagebuttonrolloverwebcontrol></td>
										</tr>
										<tr>
											<td colspan="2"><img height="5" src="images/Common/pixel.gif" width="6"></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Logo TNSMI 
							<tr>
								<td><IMG height="211" src="images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr bgcolor="#ffffff">
								<TD colspan="2" align="left" valign="bottom"><IMG height="1" src="images/Common/pixel.gif" width="5"><img src="/Images/Common/logo_Tns.gif"></TD>
							</tr>
							-->
						</table>
					</td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG height="100" src="images/Common/logo_cote_droit.gif" width="5"></td>
							</tr>
							<tr>
								<td bgColor="#ffffff"><IMG height="1" src="images/Common/pixel.gif" width="1"></td>
							</tr>
						</table>
					</td>
					<td bgColor="#644883"><IMG height="1" src="images/Common/pixel.gif" width="1"></td>
					<td width="10" background="images/Common/dupli_fond.gif">&nbsp;</td>
					<td  width="10" bgColor="#ffffff">&nbsp;</td>
					<!-- droite-->
					<td>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td>
									<cc3:HeaderWebControl language="33" id="HeaderWebControl1" runat="server"></cc3:HeaderWebControl></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td background="images/Common/dupli_3.gif">
									<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left"><IMG height="5" src="images/Common/croix.gif" width="5"></td>
											<td><IMG height="17" src="images/Common/pixel.gif" width="1"></td>
											<td vAlign="top" align="right"><IMG height="5" src="images/Common/croix.gif" width="5"></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td>
									<script  type="text/javascript" language="javascript">
										if(hasRightFlashVersion){
											document.writeln('<OBJECT id="Object1" codeBase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"');
											document.writeln('height="484" width="733" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" VIEWASTEXT>');
											document.writeln('<PARAM NAME="_cx" VALUE="19394">');
											document.writeln('<PARAM NAME="_cy" VALUE="12806">');
											document.writeln('<PARAM NAME="FlashVars" VALUE="">');										
											document.writeln('<PARAM NAME="Movie" VALUE="/Flash/'+<%=_siteLanguage%>+'/anime6.swf">');
											document.writeln('<PARAM NAME="Src" VALUE="/Flash/'+<%=_siteLanguage%>+'/anime6.swf">');
											document.writeln('<PARAM NAME="WMode" VALUE="Window">');
											document.writeln('<PARAM NAME="Play" VALUE="-1">');
											document.writeln('<PARAM NAME="Loop" VALUE="-1">');
											document.writeln('<PARAM NAME="Quality" VALUE="High">');
											document.writeln('<PARAM NAME="SAlign" VALUE="">');
											document.writeln('<PARAM NAME="Menu" VALUE="0">');
											document.writeln('<PARAM NAME="Base" VALUE="">');
											document.writeln('<PARAM NAME="AllowScriptAccess" VALUE="always">');
											document.writeln('<PARAM NAME="Scale" VALUE="ShowAll">');
											document.writeln('<PARAM NAME="DeviceFont" VALUE="0">');
											document.writeln('<PARAM NAME="EmbedMovie" VALUE="0">');
											document.writeln('<PARAM NAME="BGColor" VALUE="">');
											document.writeln('<PARAM NAME="SWRemote" VALUE="">');
											document.writeln('<PARAM NAME="MovieData" VALUE="">');
											document.writeln('<PARAM NAME="SeamlessTabbing" VALUE="1">');
											document.writeln('<embed src="/Flash/'+<%=_siteLanguage%>+'/anime6.swf" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer"');
											document.writeln('type="application/x-shockwave-flash" width="733" height="484"> </embed>');
											document.writeln('</OBJECT>');
										}
										else{
											document.writeln('<img src="/Images/'+<%=_siteLanguage%>+'/FlashReplacement/HomeAnime.gif" width="733" height="484">');
										}
									</script>
								</td>
							</tr>
						</table>
					</td>
					<!-- la fin -->
					<td></td>
				</tr>
				<!-- ligne du bas a droite -->
				<TR height="5">
					<TD></TD>
					<TD valign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD bgColor="#644883"></TD>
					<TD background="/Images/Common/dupli_fond.gif"></TD>
					<TD></TD>
					<TD id="dellCel" valign="top" background="/Images/Common/dupli_fond.gif">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><IMG height="5" src="/Images/Common/croix.gif" width="5"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
