<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.PersonnalizeSession" CodeFile="PersonnalizeSession.aspx.cs" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
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
			
			function insertIdMySession4(idSession,idRepertory){
			
			document.Form2.idMySession.name="CKB_"+idSession+"_"+idRepertory;
			}
			function insertIdMySession5(idSession,idRepertory){
			
			document.Form2.idMySession.name="CKB1_"+idSession+"_"+idRepertory;
			}
			//-->
		</script>
		<%=script%>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body onload="MM_preloadImages('images/valider_down.gif')">
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
									<td bgColor="#ffffff"><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<!--Mon Adexpress-->
								<tr>
									<td bgColor="#ffffff">
										<table cellSpacing="0" cellPadding="0" border="0">
											<tr bgColor="#644883">
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="159"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td vAlign="top"><IMG height="12" src="/images/Common/block_fleche.gif" width="12"></td>
												<td background="/images/Common/block_dupli.gif"><IMG height="1" src="/images/Common/pixel.gif" width="13"></td>
												<td class="txtNoir11Bold">
													<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="829"></cc1:adexpresstext></p>
												</td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
										</table>
								<tr>
									<td bgColor="#ffffff" height="5"></td>
								</tr>
								<tr>
									<td class="txtGris11Bold" bgColor="#ffffff"><cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="827"></cc1:adexpresstext></td>
								</tr>
								<tr>
									<td height="5"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff" height="5"><cc3:imagebuttonrolloverwebcontrol id="ImageButtonRollOverWebControl1" runat="server" onclick="ImageButtonRollOverWebControl1_Click"></cc3:imagebuttonrolloverwebcontrol></td>
								</tr>
								<tr>
									<td bgColor="#ffffff" height="5"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff" height="5"><IMG src="/Images/<%=_siteLanguage%>/button/personnaliser_gris.gif" ></td>
								</tr>
								<tr>
									<td height="10"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<!--Mes Univers-->
								<tr>
									<td bgColor="#ffffff">
										<table cellSpacing="0" cellPadding="0" border="0">
											<tr bgColor="#644883">
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="159"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td vAlign="top"><IMG height="12" src="/images/Common/block_fleche.gif" width="12"></td>
												<td background="/images/Common/block_dupli.gif"><IMG height="1" src="/images/Common/pixel.gif" width="13"></td>
												<td class="txtNoir11Bold">
													<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="875"></cc1:adexpresstext></p>
												</td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td height="5" bgColor="#ffffff"></td>
								</tr>
								<tr>
									<td class="txtGris11Bold" bgColor="#ffffff">
										<cc1:AdExpressText language="33" id="AdExpressText4" runat="server" Code="903"></cc1:AdExpressText></td>
								</tr>
								<tr>
									<td height="5"></td>
								</tr>
								<tr>
									<td class="txtGris10" bgColor="#ffffff" height="5">
										<cc3:ImageButtonRollOverWebControl id="universOpenImageButtonRollOverWebControl" runat="server" onclick="universOpenImageButtonRollOverWebControl_Click"></cc3:ImageButtonRollOverWebControl></td>
								</tr>
								<tr>
									<td height="10"></td>
								</tr>
								<tr>
									<td bgColor="#ffffff"><IMG height="5" src="/images/Common/pixel.gif" width="1"></td>
								</tr>
								<!--Mes PDF-->
								<tr>
									<td bgColor="#ffffff">
										<table cellSpacing="0" cellPadding="0" border="0">
											<tr bgColor="#644883">
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="159"></td>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td vAlign="top"><IMG height="12" src="/images/Common/block_fleche.gif" width="12"></td>
												<td background="/images/Common/block_dupli.gif"><IMG height="1" src="/images/Common/pixel.gif" width="13"></td>
												<td class="txtNoir11Bold">
													<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext15" runat="server" Code="1778"></cc1:adexpresstext></p>
												</td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
											<tr>
												<td><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
												<td bgColor="#644883"><IMG height="1" src="/images/Common/pixel.gif" width="1"></td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td bgColor="#ffffff" height="5"></td>
								</tr>
								<tr>
									<td class="txtGris11Bold" bgColor="#ffffff"><cc1:adexpresstext language="33" id="Adexpresstext16" runat="server" Code="1753"></cc1:adexpresstext></td>
								</tr>
								<tr>
									<td height="5"></td>
								</tr>
								<tr>
									<td class="txtGris10" bgColor="#ffffff" height="5"><cc3:imagebuttonrolloverwebcontrol id="pdfOpenImageButtonRollOverWebControl" runat="server" onclick="pdfOpenImageButtonRollOverWebControl_Click"></cc3:imagebuttonrolloverwebcontrol></td>
								</tr>
								<!-- test!!!!--></table>
							<P>
								<!--TEST : acces à la page de resultat Plan Media -->
								<!--Fin TEST : acces à la page de resultat Plan Media --></P>
							<P>&nbsp;</P>
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
									<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic" ActiveMenu="2"></cc2:headerwebcontrol></td>
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
									<td>
										<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="727" border="0">
											<tr>
												<td colspan="3" class="txtGris11Bold" bgColor="#ffffff">
													<cc1:AdExpressText language="33" id="mainDescAdexpresstext" runat="server" Code="904"></cc1:AdExpressText>
												</td>
											</tr>
											<tr>
												<td colspan="3"><IMG height="10" src="/images/Common/pixel.gif"></td>
											</tr>
											<!--Créer un répertoire-->
											<TR bgColor="#644883" height="14">
												<td colSpan="1"><IMG src="/Images/Common/fleche_1.gif"></td>
												<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="696"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<td></td>
												<TD class="txtViolet11" bgColor="#ffffff">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText6" runat="server" Code="700"></cc1:adexpresstext></TD>
												<TD bgColor="#ffffff"><asp:textbox id="createRepertoryTextBox" runat="server" CssClass="txtViolet11" Width="300px"></asp:textbox></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="createRepertoryImageButtonRollOverWebControl" runat="server" onclick="createRepertoryImageButtonRollOverWebControl_Click"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Supprimer un répertoire-->
											<TR bgColor="#644883" height="14">
												<td colSpan="1"><IMG src="/Images/Common/fleche_1.gif"></td>
												<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText7" runat="server" Code="697"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 17px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 17px" bgColor="#ffffff">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText8" runat="server" Code="701"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 17px" bgColor="#ffffff"><asp:dropdownlist id="directoryDropDownList" runat="server" CssClass="txtViolet11" Width="300px"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="deleteImageButtonRollOverWebControl" runat="server" onclick="deleteImageButtonRollOverWebControl_Click"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Renommer un répertoire-->
											<TR bgColor="#644883" height="14">
												<td colSpan="1"><IMG src="/Images/Common/fleche_1.gif"></td>
												<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText9" runat="server" Code="698"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 15px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 15px" bgColor="#ffffff">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText10" runat="server" Code="702"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 15px" bgColor="#ffffff"><asp:dropdownlist id="renameDirectoryDropDownList" runat="server" CssClass="txtViolet11" Width="300px"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" bgColor="#ffffff">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText11" runat="server" Code="703"></cc1:adexpresstext></TD>
												<TD bgColor="#ffffff"><asp:textbox id="renameDirectoryTextBox" runat="server" CssClass="txtViolet11" Width="300px"></asp:textbox></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="renameImageButtonRollOverWebControl" runat="server" onclick="renameImageButtonRollOverWebControl_Click"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Déplacer une session-->
											<TR bgColor="#644883" height="14">
												<td colSpan="1"><IMG src="/Images/Common/fleche_1.gif"></td>
												<TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText12" runat="server" Code="909"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" bgColor="#ffffff" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="910"></cc1:adexpresstext></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" bgColor="#ffffff"></TD>
												<TD bgColor="#ffffff">
													<!--Liste des répertoires-->
													<%=listRepertories%>
													<input id="idMySession" type="hidden" name="nameMySession">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 13px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 13px" bgColor="#ffffff">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="911"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 13px" bgColor="#ffffff"><asp:dropdownlist id="moveDirectoryDropDownList" runat="server" CssClass="txtViolet11" Width="300px"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD bgColor="#ffffff" colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="moveImageButtonRollOverWebControl" runat="server" onclick="moveImageButtonRollOverWebControl_Click"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Début Renommer un résultat -->
                                            <TR bgColor="#644883" height="14">
	                                            <td colSpan="1"><IMG src="/Images/Common/fleche_1.gif"></td>
	                                            <TD class="txtBlanc11Bold" background="/Images/Common/bandeau_titre.gif" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="2257"></cc1:adexpresstext>
	                                            </TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD bgColor="#ffffff" colSpan="2" height="5"></TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD class="txtViolet11" bgColor="#ffffff" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext18" runat="server" Code="2258"></cc1:adexpresstext></TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD bgColor="#ffffff" colSpan="2" height="5"></TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD class="txtViolet11" bgColor="#ffffff"></TD>
	                                            <TD bgColor="#ffffff">
		                                            <!--Liste des résultats sauvegardés-->
		                                            <%=listSesssionsToRename%>
		                                            <input id="idMySession5" type="hidden" name="nameMySession">
	                                            </TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD bgColor="#ffffff" colSpan="2" height="5"></TD>
                                            </TR>
                                            <TR>
	                                            <TD style="HEIGHT: 13px"></TD>
	                                            <TD class="txtViolet11" style="HEIGHT: 13px" bgColor="#ffffff">
		                                            &nbsp;<cc1:adexpresstext language="33" id="Adexpresstext19" runat="server" Code="2259"></cc1:adexpresstext></TD>
	                                            <TD style="HEIGHT: 13px" bgColor="#ffffff">
		                                            <asp:TextBox id="renameSessionsTextBox" runat="server" Width="296px"></asp:TextBox></TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD bgColor="#ffffff" colSpan="2" height="10">
	                                            </TD>
                                            </TR>
                                            <TR>
	                                            <TD></TD>
	                                            <TD></TD>
	                                            <TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="renameSessionsImagebutton" runat="server" onclick="renameSessionsImagebutton_Click"></cc3:imagebuttonrolloverwebcontrol></TD>
                                            </TR>
                                            <!--Fin Renommer un résultat -->
										</TABLE>
									</td>
								</tr>
							</table>
						</td>
						<!-- la fin -->
						<td></td>
					</tr>
					<!-- ligne du bas a droite -->
					<TR>
						<TD vAlign="top"></TD>
						<TD vAlign="top" background="/images/Common/dupli_fond.gif"></TD>
						<TD vAlign="top"></TD>
						<TD bgColor="#644883"></TD>
						<TD width="10" background="/images/Common/dupli_fond.gif"></TD>
						<TD width="10" bgColor="#ffffff"></TD>
						<TD vAlign="top" background="/images/Common/dupli_fond.gif"></TD>
						<TD></TD>
					</TR>
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
			&nbsp;
		</form>
		<!--</TR></TBODY></TABLE>-->
	</body>
</HTML>
