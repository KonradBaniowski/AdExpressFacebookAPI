<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Informations.DataUpdate" CodeFile="DataUpdate.aspx.cs" %>
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
	<body>
		<form id="Form1" method="post" runat="server">
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
						<table id="tableMenu" cellSpacing="0" cellPadding="0" width="10" border="0">
							<tr vAlign="top" bgColor="#ffffff">
								<!-- Logo -->
								<td><IMG height=90 src="/images/<%=_siteLanguage%>/logo/logo.gif" width=185 ></td>
							</tr>
							<tr>
								<td><IMG height="5" src="/images/Common/plus_sous_logo.gif" width="185"></td>
							</tr>
							<tr>
								<td style="HEIGHT: 10px"><IMG height="10" src="/images/Common/pixel.gif" width="1"></td>
							</tr>
							<tr>
								<td><cc1:pagetitlewebcontrol id="PageTitleWebControl1" runat="server" CodeDescription="1323" CodeTitle="1253"></cc1:pagetitlewebcontrol></td>
							</tr>
							<tr>
								<td><IMG height="10" src="/Images/Common/pixel.gif" width="1"></td>
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
					<td vAlign="top" background="/images/Common/dupli_fond.gif">
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
											<td vAlign="top" align="left"><IMG height="5" src="/images/Common/croix.gif" width="5"></td>
											<td><IMG height="17" src="/images/Common/pixel.gif" width="1"></td>
											<td vAlign="top" align="right"><IMG height="5" src="/images/Common/croix.gif" width="5"></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- Centre -->
							<tr>
								<td bgColor="#ffffff"><br>
									<table class="txtViolet11" height="10" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<!--TITRE : MODULES TENDANCES ET TABLEAUX DE BORDS-->
										<tr bgColor="#644883">
											<td class="txtBlanc14Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext163" runat="server" Code="1831"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Texte Explicatif & lien-->
										<tr>
											<td>&nbsp;<%=_link%></td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!--TITRE : MODULES D'ALERTE ET D'ANALYSE-->
										<tr bgColor="#644883">
											<td class="txtBlanc14Bold">&nbsp;
												<cc2:adexpresstext language="33" id="AdExpressText56" runat="server" Code="1276"></cc2:adexpresstext></td>
										</tr>
										<!-- ********** MEDIA PRESSE **********-->
										<tr bgColor="#ded8e5">
											<td class="txtViolet12Bold">&nbsp;
												<cc2:adexpresstext language="33" id="AdExpressText6" runat="server" Code="1255"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext164" runat="server" Code="1255"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText28" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td colSpan="2">&nbsp;</td>
														<td class="txtBlanc11Bold" align="center"><cc2:adexpresstext language="33" id="AdExpressText29" runat="server" Code="1258"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" width="15%" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="654"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText7" runat="server" Code="1259"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText17" runat="server" Code="1263"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText8" runat="server" Code="1260"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText18" runat="server" Code="1264"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="655"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText9" runat="server" Code="1261"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText23" runat="server" Code="1265"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText10" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText19" runat="server" Code="1264"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="656"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText11" runat="server" Code="1261"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText24" runat="server" Code="1265"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText12" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText20" runat="server" Code="1264"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="657"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText13" runat="server" Code="1261"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText25" runat="server" Code="1265"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText14" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText21" runat="server" Code="1264"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText5" runat="server" Code="658"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText15" runat="server" Code="1261"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText26" runat="server" Code="1265"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText16" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText22" runat="server" Code="1264"></cc2:adexpresstext></td>
													</tr>
													<tr>
														<td align="left" bgColor="#ffffff" colSpan="3">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText27" runat="server" Code="1266"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext165" runat="server" Code="1255"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText30" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td align="center" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText31" runat="server" Code="1267"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!-- ********** MEDIA TV **********-->
										<tr bgColor="#ded8e5">
											<td class="txtViolet12Bold">&nbsp;
												<cc2:adexpresstext language="33" id="AdExpressText35" runat="server" Code="1838"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte hors chaine thématiques-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext166" runat="server" Code="1268"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText33" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td colSpan="2">&nbsp;</td>
														<td class="txtBlanc11Bold" align="center"><cc2:adexpresstext language="33" id="AdExpressText36" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" width="15%" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText37" runat="server" Code="654"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText42" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText47" runat="server" Code="1270"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText38" runat="server" Code="655"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText43" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText48" runat="server" Code="1271"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText39" runat="server" Code="656"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText44" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText49" runat="server" Code="1272"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText40" runat="server" Code="657"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText45" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText50" runat="server" Code="1273"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" align="left" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText41" runat="server" Code="658"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText46" runat="server" Code="1262"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText51" runat="server" Code="1274"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse hors chaine thématiques-->
										<tr align="center">
											<td class="txtViolet11Bold" style="HEIGHT: 13px">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext167" runat="server" Code="1268"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText34" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td align="center" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText32" runat="server" Code="1267"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte chaine thématiques-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext178" runat="server" Code="1839"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext168" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext169" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext170" runat="server" Code="657"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext171" runat="server" Code="1840"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext172" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse chaine thématiques-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext179" runat="server" Code="1839"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext173" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext174" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext175" runat="server" Code="658"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext176" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext177" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module parrainage TV-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext213" runat="server" Code="2161"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext214" runat="server" Code="2160"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext215" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext216" runat="server" Code="657"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext217" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext218" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!-- ********** MEDIA RADIO **********-->
										<tr bgColor="#ded8e5">
											<td class="txtViolet12Bold">&nbsp;
												<cc2:adexpresstext language="33" id="AdExpressText52" runat="server" Code="1841"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte hors station IDF-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext180" runat="server" Code="1275"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText53" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="3">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText74" runat="server" Code="1280"></cc2:adexpresstext></td>
														<td align="center">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText58" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText59" runat="server" Code="654"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText64" runat="server" Code="1278"></cc2:adexpresstext></td>
														<td width="35%" bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText81" runat="server" Code="1283"></cc2:adexpresstext><br>
															<cc2:adexpresstext language="33" id="AdExpressText86" runat="server" Code="1284"></cc2:adexpresstext><br>
															<cc2:adexpresstext language="33" id="AdExpressText87" runat="server" Code="1285"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;</td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText65" runat="server" Code="1279"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText88" runat="server" Code="1286"></cc2:adexpresstext><br>
															<cc2:adexpresstext language="33" id="AdExpressText89" runat="server" Code="1287"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText79" runat="server" Code="1281"></cc2:adexpresstext>&nbsp;</td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText60" runat="server" Code="655"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText66" runat="server" Code="1278"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText82" runat="server" Code="1283"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															</td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText67" runat="server" Code="1279"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText90" runat="server" Code="2018"></cc2:adexpresstext><br>
															<cc2:adexpresstext language="33" id="AdExpressText91" runat="server" Code="1288"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText80" runat="server" Code="1282"></cc2:adexpresstext><br />
														<cc2:adexpresstext language="33" id="AdExpressText78" runat="server" Code="1271"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText61" runat="server" Code="656"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText68" runat="server" Code="1278"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText83" runat="server" Code="1283"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff">&nbsp;</td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText69" runat="server" Code="1279"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText92" runat="server" Code="1289"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText77" runat="server" Code="1272"></cc2:adexpresstext>&nbsp;</td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText62" runat="server" Code="657"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText70" runat="server" Code="1278"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText84" runat="server" Code="1283"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff">&nbsp;</td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText71" runat="server" Code="1279"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText93" runat="server" Code="1290"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText76" runat="server" Code="1273"></cc2:adexpresstext>&nbsp;</td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" bgColor="#644883" rowSpan="2">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText63" runat="server" Code="658"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText72" runat="server" Code="1278"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText85" runat="server" Code="1283"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff">&nbsp;</td>
													</tr>
													<tr align="center">
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText73" runat="server" Code="1279"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText94" runat="server" Code="1291"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText75" runat="server" Code="1274"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse hors station IDF-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext181" runat="server" Code="1275"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="AdExpressText54" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td align="center" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText55" runat="server" Code="1267"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte Stations IDF-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext182" runat="server" Code="1842"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext192" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext183" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext184" runat="server" Code="657"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext185" runat="server" Code="1600"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext186" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse Station IDF-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext187" runat="server" Code="1842"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext193" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext188" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext189" runat="server" Code="658"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext190" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext191" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!-- ********** MEDIA PUBLICITE EXTERIEURE **********-->
										<tr bgColor="#ded8e5">
											<td class="txtViolet12Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext145" runat="server" Code="1825"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext194" runat="server" Code="1825"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext146" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext147" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext148" runat="server" Code="654"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext149" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext150" runat="server" Code="1827"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext195" runat="server" Code="1825"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext151" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext152" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext153" runat="server" Code="655"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext154" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext155" runat="server" Code="1827"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!-- ********** MEDIA TV PAN EUROPEENE **********-->
										<tr bgColor="#ded8e5">
											<td class="txtViolet12Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext137" runat="server" Code="1599"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte (chaines classiques)-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext199" runat="server" Code="1843"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext138" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext142" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext141" runat="server" Code="655"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext143" runat="server" Code="1600"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext144" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse (chaines classiques)-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext200" runat="server" Code="1843"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext139" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext140" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext196" runat="server" Code="656"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext197" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext198" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'alerte (chaines Parrainage)-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext201" runat="server" Code="1844"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext202" runat="server" Code="1256"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext203" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext204" runat="server" Code="657"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext205" runat="server" Code="1845"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext206" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Module d'analyse (chaines Parrainage)-->
										<tr align="center">
											<td class="txtViolet11Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext207" runat="server" Code="1844"></cc2:adexpresstext>&nbsp;:&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext208" runat="server" Code="1257"></cc2:adexpresstext></td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center" colSpan="2">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext209" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="15%" bgColor="#644883">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext210" runat="server" Code="658"></cc2:adexpresstext></td>
														<td width="15%" bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext211" runat="server" Code="1826"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext212" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!--TITRE : MODULES D'ALERTE ET D'ANALYSE-->
										<tr bgColor="#644883">
											<td class="txtBlanc14Bold">&nbsp;
												<cc2:adexpresstext language="33" id="AdExpressText57" runat="server" Code="1277"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold" align="center">
														<td rowSpan="2"><cc2:adexpresstext language="33" id="AdExpressText95" runat="server" Code="1292"></cc2:adexpresstext>&nbsp;</td>
														<td colSpan="4">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText96" runat="server" Code="1293"></cc2:adexpresstext></td>
													</tr>
													<tr class="txtBlanc11Bold" align="center">
														<td><cc2:adexpresstext language="33" id="AdExpressText97" runat="server" Code="1294"></cc2:adexpresstext>&nbsp;</td>
														<td><cc2:adexpresstext language="33" id="AdExpressText98" runat="server" Code="1295"></cc2:adexpresstext>&nbsp;</td>
														<td>&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText99" runat="server" Code="1296"></cc2:adexpresstext></td>
														<td><cc2:adexpresstext language="33" id="AdExpressText100" runat="server" Code="1297"></cc2:adexpresstext>&nbsp;</td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="AdExpressText101" runat="server" Code="1298"></cc2:adexpresstext>&nbsp;</td>
														<td width="20%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText107" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td width="20%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText119" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td width="20%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText120" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td width="20%" bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText131" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText102" runat="server" Code="1299"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText109" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText118" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText122" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText130" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="AdExpressText103" runat="server" Code="1300"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText110" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText117" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText124" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText129" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="AdExpressText104" runat="server" Code="1301"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText111" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText116" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText125" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText128" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="AdExpressText105" runat="server" Code="1302"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText112" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText115" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText123" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText127" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="AdExpressText106" runat="server" Code="1303"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText113" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText114" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText121" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="AdExpressText126" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtBlanc11Bold" width="20%" bgColor="#644883"><cc2:adexpresstext language="33" id="Adexpresstext132" runat="server" Code="1304"></cc2:adexpresstext>&nbsp;</td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="Adexpresstext133" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="Adexpresstext134" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="Adexpresstext135" runat="server" Code="1305"></cc2:adexpresstext></td>
														<td bgColor="#ffffff"><cc2:adexpresstext language="33" id="Adexpresstext136" runat="server" Code="1305"></cc2:adexpresstext></td>
													</tr>
													<tr align="left">
														<td bgColor="#ffffff" colSpan="5">&nbsp;
															<cc2:adexpresstext language="33" id="AdExpressText108" runat="server" Code="1306"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
										<!--TITRE : MODULES TENDANCES ET TABLEAUX DE BORDS-->
										<tr bgColor="#644883">
											<td class="txtBlanc14Bold">&nbsp;
												<cc2:adexpresstext language="33" id="Adexpresstext156" runat="server" Code="1828"></cc2:adexpresstext></td>
										</tr>
										<!--Vide-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr class="txtBlanc11Bold">
														<td align="center">&nbsp;</td>
														<td align="center">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext157" runat="server" Code="1269"></cc2:adexpresstext></td>
													</tr>
													<tr align="center">
														<td class="txtViolet11Bold" align="center" width="30%" bgColor="#ffffff">&nbsp;
															<cc2:adexpresstext language="33" id="Adexpresstext160" runat="server" Code="1558"></cc2:adexpresstext>&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext159" runat="server" Code="1826"></cc2:adexpresstext><br>
															<cc2:adexpresstext language="33" id="Adexpresstext162" runat="server" Code="1830"></cc2:adexpresstext></td>
														<td bgColor="#ffffff">&nbsp;<cc2:adexpresstext language="33" id="Adexpresstext161" runat="server" Code="1282"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Asterix Tendances-->
										<tr>
											<td>&nbsp;</td>
										</tr>
										<!--Tableau-->
										<tr>
											<td>
												<table class="txtViolet11" borderColor="#ffffff" cellSpacing="1" cellPadding="0" width="99%"
													align="center" bgColor="#644883" border="1">
													<tr>
														<td align="center" bgColor="#ffffff"><cc2:adexpresstext language="33" id="Adexpresstext158" runat="server" Code="1829"></cc2:adexpresstext></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--Vide 40-->
										<tr>
											<td height="40">&nbsp;</td>
										</tr>
									</table>
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
					<TD vAlign="top" background="/Images/Common/dupli_fond.gif">
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
					<TD id="dellCel" vAlign="top" background="/Images/Common/dupli_fond.gif">
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
