<%@ Page language="c#" Inherits="AdExpress.Private.Alerts.ShowAlerts" CodeFile="ShowAlerts.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta http-equiv="Content-Type" content="text/html;" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
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
	</head>
	<body class="bodyStyle" onload="javascript:activateActiveX();">
		<form id="Form2" method="post" runat="server">
			<table height="600" cellspacing="0" cellpadding="0" border="0">
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
							<tr vAlign="top" class="whiteBackGround logoCoteDroitBackGround">
								<!-- Logo -->
								<td><asp:Image ID="Image2" runat="server" border="0" SkinID="logo" /></td>
							</tr>
							<tr>
								<td class="txtBlanc11Bold" width="185" height="9">
									<table id="Table6" cellspacing="0" cellpadding="0" width="10" border="0">
										<tr>
											<td><asp:Image ID="Image3" runat="server" height="5" SkinID="plus_sous_logo" /></td>
										</tr>
										<tr>
											<td><asp:Image ID="Image4" runat="server" height="10" width="1" SkinID="pixel" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<!--Mon Adexpress-->
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0" width="185">
										<tr>
											<td valign="top"><asp:Image ID="Image5" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image6" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="AdExpressText1" runat="server" Code="829"></cc1:adexpresstext></p>
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
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="AdExpressText2" runat="server" Code="827"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td height="5"><cc3:imagebuttonrolloverwebcontrol id="ImageButtonRollOverWebControl2" runat="server" onclick="ImageButtonRollOverWebControl1_Click" SkinID="ouvrirButton" ></cc3:imagebuttonrolloverwebcontrol></td>
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
							
							<!--Mes Univers-->
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0" width="185">
										<tr>
											<td valign="top"><asp:Image ID="Image9" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image10" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext3" runat="server" Code="875"></cc1:adexpresstext></p>
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
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="AdExpressText4" runat="server" Code="903"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris10 whiteBackGround" height="5"><cc3:imagebuttonrolloverwebcontrol id="universOpenImageButtonRollOverWebControl" runat="server" onclick="universOpenImageButtonRollOverWebControl_Click" SkinID="personnaliserButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image12" runat="server" SkinID="pixel" height="5" width="1" /></td>
							</tr>
							
							<!--Mes PDF-->
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0" width="185">
										<tr>
											<td valign="top"><asp:Image ID="Image13" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image14" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext7" runat="server" Code="1778"></cc1:adexpresstext></p>
											</td>
											<td><asp:Image ID="Image15" runat="server" SkinID="pixel" height="1" width="1" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="Adexpresstext8" runat="server" Code="1753"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris10" height="5"><cc3:imagebuttonrolloverwebcontrol id="pdfOpenImageButtonRollOverWebControl" runat="server" onclick="pdfOpenImageButtonRollOverWebControl_Click" SkinID="ouvrirButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td><asp:Image ID="Image16" runat="server" SkinID="pixel" height="5" width="1" /></td>
							</tr>
							
							<!-- Mes Alertes -->
							<tr runat="server" id="blockAlerts">
								<td>
									<table cellspacing="0" cellpadding="0" border="0" width="185">
										<tr>
											<td valign="top"><asp:Image ID="Image17" runat="server" SkinID="block_fleche" height="12" width="12" /></td>
											<td class="blockdupliBackGround"><asp:Image ID="Image18" runat="server" SkinID="pixel" height="1" width="13" /></td>
											<td class="txtNoir11Bold backGroundModuleTitle" width="100%">
												<p style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px"><cc1:adexpresstext language="33" id="Adexpresstext9" runat="server" Code="2585"></cc1:adexpresstext></p>
											</td>
											<td><asp:Image ID="Image19" runat="server" SkinID="pixel" height="1" width="1" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris11Bold"><cc1:adexpresstext language="33" id="Adexpresstext10" runat="server" Code="2586"></cc1:adexpresstext></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td class="txtGris10" height="5"><asp:Image ID="Image20" runat="server" SkinID="ouvrir_gris" /></td>
							</tr>	
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td height="5"><cc3:imagebuttonrolloverwebcontrol id="Imagebuttonrolloverwebcontrol1" runat="server" onclick="personalizeAlertesImagebuttonrolloverwebcontrol_Click" SkinID="personnaliserButton"></cc3:imagebuttonrolloverwebcontrol></td>
							</tr>
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
									<table id="Table1" cellspacing="0" cellpadding="0" width="727" border="0">
										<tr class="violetBackGround" height="14">
											<td width="14"><asp:Image ID="Image25" runat="server" SkinID="fleche_1" /></td>
											<td class="txtBlanc11Bold bandeauTitreBackGround" colSpan="2">&nbsp;
												<cc1:adexpresstext language="33" id="AdExpressText5" runat="server" Code="2584"></cc1:adexpresstext></td>
										</tr>
										<tr>
											<td></td>
											<td class="txtViolet11" valign="middle" colSpan="2" height="20">&nbsp;
												<cc1:adexpresstext language="33" id="AdExpressText6" runat="server" Code="907"></cc1:adexpresstext></td>
										</tr>
										<tr>
											<td></td>
											<td colSpan="2" height="10"></td>
										</tr>
										<tr>
											<td></td>
											<td align="right">
												<!--Liste des alertes-->
												<asp:Repeater ID="repeaterAlerts" runat="server" OnItemDataBound="alertsItemBinding">
												    <ItemTemplate>
												        <div class="block-valid-alert">
												            <div style="height: 20px">
												                <asp:Image ID="flagStatus" runat="server" SkinID="alertFlagPending" ImageAlign="Left"/>
												                <asp:Image ID="pictureArrowContent" runat="server" SkinID="bt_arrow_down" height="15" width="15" ImageAlign="Right"/>
												                <asp:Image ID="detailSelection" runat="server" ImageAlign="Right" />
											                    <h2 id="headerAlert" runat="server" style="text-align:left;">
											                        <%# DataBinder.Eval(Container.DataItem, "Title") %>
											                    </h2>
											                </div>
											                <div id="content" class="alert-content" runat="server" style="display: none">
											                    <ul runat="server" id="listOccurrences">
											                        <asp:Repeater Visible="false" ID="repeaterAlertOccurrences" runat="server" OnItemDataBound="alertOccurrenceItemDataBound">
											                            <ItemTemplate>
											                                <li>
											                                    <asp:HyperLink runat="server" ID="lnkOccurrence">
											                                    </asp:HyperLink>
											                                </li>
											                            </ItemTemplate>
												                    </asp:Repeater>
												                </ul>
												                <div class="block-alert-details">
												                    <div runat="server" id="alertDetails">
												                    </div>
												                </div>
												                <br style="clear: both">
												                <asp:Label runat="server" ID="lblNoOccurrence" Visible="false"></asp:Label>
												            </div>
												        </div>
												    </ItemTemplate>
												</asp:Repeater>
										        <div class="block-valid-alert-last"></div>
												<input id="idMySession" type="hidden" name="nameMySession" /> <input id="idPopup" type="hidden" name="namePopup" />
											</td>
											<td width="4px"></td>
										</tr>
										<tr>
											<td></td>
											<tdcolSpan="2" height="10"></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
