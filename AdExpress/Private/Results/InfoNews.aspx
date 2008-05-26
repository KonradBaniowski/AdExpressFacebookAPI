<%@ Page language="c#" Inherits="AdExpress.Private.Results.InfoNews" CodeFile="InfoNews.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
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
			//-->
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body class="bodyStyle">
		<form id="Form1" method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<!-- marge de gauche-->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image ID="Image153" runat="server" SkinID="logo_cote_gauche" height="100" width="10" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image1" runat="server" SkinID="pixel" height="1" width="1" /></td>
							</tr>
						</table>
					</td>
					<!-- menu -->
					<td vAlign="top" class="imageBackGround">
						<table id="tableMenu" cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" class="whiteBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image8" runat="server" SkinID="logo" height="90" width="185" /></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image9" runat="server" SkinID="plus_sous_logo" height="5" width="185" /></td>
							</tr>
							<tr>
								<td style="HEIGHT: 10px"><asp:Image ID="Image2" runat="server" SkinID="pixel" height="10" width="1" /></td>
							</tr>
							<tr>
								<td><cc1:pagetitlewebcontrol id="PageTitleWebControl1" runat="server" CodeTitle="762" CodeDescription="1324"></cc1:pagetitlewebcontrol></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image3" runat="server" SkinID="pixel" height="10" width="1" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround">
									<table id="Table4" cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td><asp:Image ID="Image4" runat="server" SkinID="pixel" height="3" width="5" /></td>
										</tr>
										<tr>
											<td class="whiteBackGround">
												<A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif';" href="javascript:history.back();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/back_up.gif" border=0 name=bouton></A>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<!--TEST : acces à la page de resultat Plan Media -->
						<!--Fin TEST : acces à la page de resultat Plan Media --></td>
					<!-- Séparateur -->
					<td vAlign="top">
						<table id="Table5" cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><asp:Image ID="Image10" runat="server" SkinID="logo_cote_droit" height="100" width="5" /></td>
							</tr>
							<tr>
								<td class="whiteBackGround"><asp:Image ID="Image5" runat="server" SkinID="pixel" height="1" width="1" /></td>
							</tr>
						</table>
					</td>
					<td class="violetBackGround"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="1" width="1" /></td>
					<td width="10" class="imageBackGround">&nbsp;</td>
					<td width="10" class="whiteBackGround">&nbsp;</td>
					<!-- droite-->
					<td vAlign="top" class="imageBackGround">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- Menu du haut-->
								<td><cc1:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
							</tr>
							<tr>
								<!-- ligne du haut a droite -->
								<td>
									<table id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td vAlign="top" align="left"><asp:Image ID="Image11" runat="server" SkinID="croix" height="5" width="5" /></td>
											<td><asp:Image ID="Image7" runat="server" SkinID="pixel" height="17" width="1" /></td>
											<td vAlign="top" align="right"><asp:Image ID="Image12" runat="server" SkinID="croix" height="5" width="5" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td class="whiteBackGround">
									<br>
									<%=result%>
									<br>
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
					<TD vAlign="top" class="imageBackGround">
						<TABLE id="Table7" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image ID="Image13" runat="server" SkinID="croix" height="5" width="5" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image ID="Image14" runat="server" SkinID="croix" height="5" width="5" /></TD>
							</TR>
						</TABLE>
					</TD>
					<TD></TD>
					<TD class="violetBackGround"></TD>
					<TD class="imageBackGround"></TD>
					<TD></TD>
					<TD id="dellCel" vAlign="top" class="imageBackGround">
						<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD vAlign="top"><asp:Image ID="Image15" runat="server" SkinID="croix" height="5" width="5" /></TD>
								<TD></TD>
								<TD vAlign="top" align="right"><asp:Image ID="Image16" runat="server" SkinID="croix" height="5" width="5" /></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
