<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Universe.PersonnalizeUniverse" CodeFile="PersonnalizeUniverse.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
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
			
			
			function insertIdMySession3(idSession,idRepertory){			
			document.Form2.idMySession3.name="CKB1_"+idSession+"_"+idRepertory;
			}
			function insertIdMySession4(idSession,idRepertory){			
			document.Form2.idMySession4.name="CKB_"+idSession+"_"+idRepertory;
			}
			function insertIdMySession5(idSession,idRepertory){			
			document.Form2.idMySession5.name="CKB2_"+idSession+"_"+idRepertory;
			}	
			function insertIdMySession6(idSession,idRepertory){			
			document.Form2.idMySession6.name="CKB1_"+idSession+"_"+idRepertory;
			}	
			
			function insertIdMySession7(idSession,idRepertory){			
			document.Form2.idMySession7.name="CKB3_"+idSession+"_"+idRepertory;
			}		
			
			//-->
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellSpacing="0" cellPadding="0" border="0">
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
						<table cellSpacing="0" cellPadding="0" border="0" width="10">
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
									</table>
								</td>
							</tr>
							<tr><td>&nbsp;</td></tr>
							<!--Mon Adexpress-->
							<tr>
								<td class="whiteBackGround">
									<table cellSpacing="0" cellPadding="0" border="0" width="185">
										<tr>
											<td vAlign="top"><asp:Image ID="Image5" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="829"></cc1:adexpresstext></p>
											</td>
											<td><asp:Image ID="Image7" runat="server" SkinID="pixel" height="1" width="1" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="Adexpresstext4" runat="server" Code="827"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td height="18" style="HEIGHT: 18px"><cc3:imagebuttonrolloverwebcontrol id="openImageButtonRollOverWebControl" runat="server" onclick="openImageButtonRollOverWebControl_Click" SkinID="ouvrirButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td height="5"><cc3:imagebuttonrolloverwebcontrol id="personalizeImagebuttonrolloverwebcontrol" runat="server" onclick="personalizeImagebuttonrolloverwebcontrol_Click" SkinID="personnaliserButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image8" runat="server" SkinID="pixel" height="5" width="1" /></td>
							</tr>
							
							<!--Mes univers-->
							<tr>
								<td class="whiteBackGround">
									<table cellSpacing="0" cellPadding="0" border="0" width="185">
										<tr>
											<td vAlign="top"><asp:Image ID="Image9" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="875"></cc1:adexpresstext></p>
											</td>
											<td><asp:Image ID="Image11" runat="server" SkinID="pixel" height="1" width="1" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="903"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td height="5"><asp:Image ID="Image12" runat="server" SkinID="personnaliser" /></td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image13" runat="server" SkinID="pixel" height="5" width="1" /></td>
							</tr>
							
							<!--Mes PDF-->
							<tr>
								<td>
									<table cellSpacing="0" cellPadding="0" border="0" width="185">
										<tr>
											<td vAlign="top"><asp:Image ID="Image14" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image15" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px">
													<cc1:adexpresstext language="33" id="Adexpresstext22" runat="server" Code="1778"></cc1:adexpresstext></p>
											</td>
											<td><asp:Image ID="Image16" runat="server" SkinID="pixel" height="1" width="1" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="Adexpresstext23" runat="server" Code="1753"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris10" height="5"><cc3:imagebuttonrolloverwebcontrol id="pdfOpenImageButtonRollOverWebControl" runat="server" onclick="pdfOpenImageButtonRollOverWebControl_Click" SkinID="ouvrirButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
							
							<!-- Mes Alertes -->
							<% if (IsAlertsActivated) { %>
						    <tr>
							    <td height="10"></td>
						    </tr>
						    <tr>
							    <td><asp:Image ID="Image17" runat="server" SkinID="pixel" height="5" width="1" /></td>
						    </tr>								
						    <tr runat="server" id="blockAlerts">
							    <td>
								    <table cellSpacing="0" cellPadding="0" border="0" width="185">
									    <tr>
										    <td vAlign="top"><asp:Image ID="Image18" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
										    <td class="blockdupliBackGround"><asp:Image ID="Image19" runat="server" SkinID="pixel" height="1" width="13" /></td>
										    <td class="txtNoir11Bold backGroundModuleTitle" width="100%">
											    <p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext24" runat="server" Code="2585"></cc1:adexpresstext></p>
										    </td>
										    <td><asp:Image ID="Image20" runat="server" SkinID="pixel" height="1" width="1" /></td>
									    </tr>
								    </table>
							    </td>
						    </tr>
						    <tr>
							    <td height="5"></td>
						    </tr>
						    <tr>
							    <td class="txtGris11Bold"><cc1:adexpresstext language="33" id="Adexpresstext25" runat="server" Code="2586"></cc1:adexpresstext></td>
						    </tr>
						    <tr>
							    <td height="5"></td>
						    </tr>
						    <tr>
							    <td class="txtGris10" height="5"><cc3:imagebuttonrolloverwebcontrol id="Imagebuttonrolloverwebcontrol2" runat="server" onclick="alertOpenImageButtonRollOver_Click" SkinID="ouvrirButton"></cc3:imagebuttonrolloverwebcontrol></td>
						    </tr>								
						     <tr>
							    <td height="5"></td>
						    </tr>
						    <tr>
							    <td height="5"><cc3:imagebuttonrolloverwebcontrol id="Imagebuttonrolloverwebcontrol3" runat="server" onclick="personalizeAlertesImagebuttonrolloverwebcontrol_Click" SkinID="personnaliserButton"></cc3:imagebuttonrolloverwebcontrol></td>
						    </tr>	
						    <% } %>
						    <tr>
							    <td height="20"></td>
						    </tr>
						</table>
					</td>
	
					<!-- New separator -->
					<td vAlign="top" class="whiteBackGround"><asp:Image ID="Image21" runat="server" height="100" width="5" SkinID="logo_cote_droit" /></td>
					<td class="lineVerticalBackGround2px"><asp:Image ID="Image22" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround1px"><asp:Image ID="Image23" runat="server" height="1" width="2" SkinID="pixel" /></td>
					<td class="lineVerticalBackGround"><asp:Image ID="Image24" runat="server" height="1" width="2" SkinID="pixel" /></td>
						
					<!-- Right column -->
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" border="0" >
						  <!-- Menu haut -->
							<tr>
								<td><cc2:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic" ActiveMenu="2"></cc2:headerwebcontrol></td>
							</tr>
							<!-- Centre -->
							<tr>
								<td style="padding-left:5px;">
									<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="727" border="0">
										<TBODY>
											<tr>
												<td class="txtGris11Bold" colSpan="3"><cc1:adexpresstext language="33" id="mainDescAdexpresstext" runat="server" Code="905"></cc1:adexpresstext></td>
											</tr>
											<tr>
												<td colSpan="3"><asp:Image ID="Image25" runat="server" SkinID="pixel" height="10" /></td>
											</tr>
											<!--Cr�er un Groupe d'univers-->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image26" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="912"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<td></td>
												<TD class="txtViolet11">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText6" runat="server" Code="913"></cc1:adexpresstext></TD>
												<TD><asp:textbox id="createRepertoryTextBox" runat="server" Width="300px" CssClass="txtViolet11"></asp:textbox></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="createRepertoryImageButtonRollOverWebControl" runat="server" onclick="createRepertoryImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Supprimer un Groupe d'univers-->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image27" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText7" runat="server" Code="914"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 17px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 17px">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText8" runat="server" Code="915"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 17px"><asp:dropdownlist id="directoryDropDownList" runat="server" Width="300px" CssClass="txtViolet11"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="deleteImageButtonRollOverWebControl" runat="server" onclick="deleteImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Renommer un Groupe d'univers-->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image28" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText9" runat="server" Code="916"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 15px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 15px" >&nbsp;<cc1:adexpresstext language="33" id="AdExpressText10" runat="server" Code="917"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 15px" ><asp:dropdownlist id="renameDirectoryDropDownList" runat="server" Width="300px" CssClass="txtViolet11"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 5px"></TD>
												<TD colSpan="2" height="5" style="HEIGHT: 5px"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 22px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 22px">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText11" runat="server" Code="918"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 22px"><asp:textbox id="renameDirectoryTextBox" runat="server" Width="300px" CssClass="txtViolet11"></asp:textbox></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="renameImageButtonRollOverWebControl" runat="server" onclick="renameImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--D�but Renommer un univers -->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image29" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext19" runat="server" Code="1096"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext20" runat="server" Code="1097"></cc1:adexpresstext></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11"></TD>
												<TD>
													<!--Liste des groupes d'univers-->
													<%=listUniversToRename%>
													<input id="idMySession7" type="hidden" name="nameMySession">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 13px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 13px">
													&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext21" runat="server" Code="1100"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 13px">
													<asp:TextBox id="renameUniverseTextBox" runat="server" Width="296px"></asp:TextBox></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="renameUniverseImagebutton" runat="server" onclick="renameUniverseImagebutton_Click" SkinID="validateButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--Fin Renommer un univers -->
											<!--D�placer un univers-->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image30" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText12" runat="server" Code="883"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="AdExpressText13" runat="server" Code="881"></cc1:adexpresstext></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11"></TD>
												<TD >
													<!--Liste des groupes d'univers-->
													<%=listRepertories%>
													<input id="idMySession4" type="hidden" name="nameMySession">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 13px"></TD>
												<TD class="txtViolet11" style="HEIGHT: 13px">
													&nbsp;<cc1:adexpresstext language="33" id="AdExpressText14" runat="server" Code="882"></cc1:adexpresstext></TD>
												<TD style="HEIGHT: 13px" ><asp:dropdownlist id="moveDirectoryDropDownList" runat="server" Width="300px" CssClass="txtViolet11"></asp:dropdownlist></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="moveImageButtonRollOverWebControl" runat="server" onclick="moveImageButtonRollOverWebControl_Click" SkinID="validateButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!-- Supprimer un univers-->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image31" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext15" runat="server" Code="932"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext16" runat="server" Code="936"></cc1:adexpresstext>&nbsp;
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11"></TD>
												<TD >
													<!--Liste des r�pertoires-->
													<%=listUniversToDelete%>
													<input id="idMySession6" type="hidden" name="nameMySession">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="deleteUniversImageButtonRollOverWebControl" runat="server" onclick="deleteUniversImageButtonRollOverWebControl_Click" SkinID="supprimerButton"></cc3:imagebuttonrolloverwebcontrol></TD>
											</TR>
											<!--D�tail d'un Univers -->
											<TR class="violetBackGround" height="14">
												<td colSpan="1"><asp:Image ID="Image32" runat="server" SkinID="fleche_1" /></td>
												<TD class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext17" runat="server" Code="960"></cc1:adexpresstext>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11" colSpan="2">&nbsp;<cc1:adexpresstext language="33" id="Adexpresstext18" runat="server" Code="961"></cc1:adexpresstext></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD class="txtViolet11"></TD>
												<TD>
													<!--Liste des groupes d'univers-->
													<%=listDetailGroupUnivers%>
													<input id="idMySession5" type="hidden" name="nameMySession">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="5"></TD>
											</TR>
											<TR>
												<TD></TD>
												<TD colSpan="2" height="10">
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD vAlign="top" height="40"><cc3:imagebuttonrolloverwebcontrol id="DetailUniversbuttonrolloverwebcontrol" runat="server" onclick="DetailUniversbuttonrolloverwebcontrol_Click" SkinID="detailButton"></cc3:imagebuttonrolloverwebcontrol>
												</TD>
											</TR>
										</TBODY>
									</TABLE>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
