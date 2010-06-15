<%@ Page language="c#" Inherits="BastetWeb.LoginSelection" CodeFile="LoginSelection.aspx.cs" EnableEventValidation="false" EnableSessionState="True"%>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Bastet.WebControls" Assembly="TNS.AdExpress.Bastet" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>Bastet</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
		<script language="JavaScript" src="/TreeIcons/Icons/ob_tree_2037.js" type="text/javascript"></script>
		<script language="JavaScript"> 
		<!--
			/* AFFICHER / MASQUER LES OPTIONS DE RECHERCHE*/
			function displayOptions(val){
				var optionHidden = document.getElementById('selectedOptionSearch');
			
				if(val == "companyLogin"){
					if (optionSearch.style.display == "none") optionSearch.style.display = "block";
					if (validateSelectionButton.style.display == "none") validateSelectionButton.style.display = "block";
					optionAll.style.display = "none";
					optionHidden.value = 1;
				}
				else{
					if (optionAll.style.display == "none") optionAll.style.display = "block";
					optionSearch.style.display = "none";
					validateSelectionButton.style.display = "none";
					optionHidden.value = 2;
				}
			}
			
			/* AJOUTER UN LOGIN */
			function add(){
				var itemData,login,loginId;
				var item;
				var i;
				var chekedItemsString = ob_t2_list_checked();				
				var chekedItems = chekedItemsString.split(',');
				var nbCheckedItems = chekedItems.length-1;
				var contain=0;
				for(i=0 ; i < nbCheckedItems ; i++){
					itemData = chekedItems[i].split('%');
					loginId = itemData[0];
					login = itemData[1];
					for(var j=0 ; j < document.Form1.loginListBox.length && contain==0 ; j++){
						if(document.Form1.loginListBox.options[j].value==loginId) contain = 1;
					}
					if(contain == 0){
						item = document.createElement("OPTION");
						item.text = login;
						item.value = loginId;
						document.Form1.loginListBox.options.add(item,i);
					}
					contain = 0;
				}
				addToHidden(); // Ajoute les loginId au input hidden
			} 
			
			/* SUPPRIMER UN OU PLUSIEURS LOGIN */
			function remove(){
				var nbItems=document.Form1.loginListBox.length;
				for(var j=0 ; j <nbItems && document.Form1.loginListBox.options[j]!=null  ; j++){
					if(document.Form1.loginListBox.options[j].selected){
						document.Form1.loginListBox.options.remove(j);
						j--;
					}
				}
				addToHidden(); // Ajoute les loginId au input hidden
			}
						
			/* AJOUTER LES LOGINS DANS LE INPUT HIDDEN */
			function addToHidden(){
				var listLogin = "";
				for(var k=0 ; k < document.Form1.loginListBox.length ; k++){
					listLogin += document.Form1.loginListBox.options[k].value +"%"+ document.Form1.loginListBox.options[k].text +",";
				}
				var loginsHidden = document.getElementById('selectedLoginItem');
				loginsHidden.value = listLogin;
			}
		-->
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="txtViolet11" height="50%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr bgColor="#644883" height="90">
					<td width="185"><a href="/Index.aspx"><asp:Image ID="Image4" runat="server" SkinID="logoTNShome" /></a></td>
					<td width="100%">
						<cc1:HeaderWebControl id="HeaderWebControl1" runat="server" ActiveMenu="1" SkinID="HeaderWebControl"></cc1:HeaderWebControl></td>
				</tr>
				<tr>
					<td vAlign="top" colSpan="2">
						<!--<p class="navigation">&nbsp;<%=_email_manage%>&gt;<%=_period_manage%>&gt; <font color="#ff0099"><%=_login_manage%>&amp;<%=_validation%></font></p>-->
						<p class="txtViolet12BoldTitle">&nbsp;<%=_login_manage%>&nbsp;&amp;&nbsp;<%=_validation%></p>
						<table cellSpacing="0" cellPadding="5" width="75%" align="center" border="0">
							<tr>
								<td class="txtViolet12Bold" width="50%"><%=_label_search_option%></td>
								<td>&nbsp;</td>
							</tr>
							<tr>
								<td vAlign="top">
									<table class="txtNoir11" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td>
												<!-- RECHERCHE PAR SOCIETE / LOGIN ou TOUT -->
												<TABLE class="txtNoir11" cellSpacing="0" cellPadding="2" width="100%" align="center" border="0">
													<TBODY>
														<TR bgColor="#ded8e5">
															<TD><input onclick="displayOptions(this.value);" type="radio" CHECKED value="companyLogin"
																	name="radiobutton"><%=_label_company_login%>&nbsp;&nbsp; <input onclick="displayOptions(this.value);" type="radio" value="allLogin" name="radiobutton"><%=_label_all%></TD>
														</TR>
														<TR bgColor="#ded8e5">
															<TD>
																<DIV id="optionSearch" style="DISPLAY: block">
																	<TABLE class="txtNoir11" cellSpacing="0" cellPadding="1" width="100%" align="center" border="0">
																		<tr>
																			<td><asp:textbox id="searchLoginTextBox" runat="server" CssClass="input" Width="200px"></asp:textbox><span onkeypress="javascript:addToHidden();" onclick="javascript:addToHidden();"><asp:button id="searchLoginButton" runat="server" CssClass="input" Width="80px" onclick="searchLoginButton_Click"></asp:button></span></td>
																			<td align="right">
																				<!-- AJOUTER A LA LISTE --><input class="input" id="btnSend" onclick="add();" type="button" value="<%=_bt_add%>">
																			</td>
																		</tr>
																		<tr>
																			<td colSpan="2">
																				<!-- AFFICHAGE DE L'ARBRE -->
																				<p><asp:label id="displayTreeViewLabel" runat="server"></asp:label></p>
																			</td>
																		</tr>
																	</TABLE>
																</DIV>
																<DIV id="optionAll" style="DISPLAY: none">
																	<p align="center"><span onclick="javascript:addToHidden();"><asp:button id="validateButton" runat="server" CssClass="input" onclick="validateButton_Click"></asp:button></span></p>
																</DIV>
															</TD>
														</TR>
													</TBODY>
												</TABLE>
											</td>
										</tr>
										<tr>
											<td>&nbsp;</td>
										</tr>
										<tr>
											<td>
												<!-- LISTE DES TYPES DE CLIENT -->
												<TABLE class="txtNoir11" cellSpacing="0" cellPadding="2" width="100%" align="center" border="0">
													<TBODY>
														<TR bgColor="#ded8e5">
															<TD width="50%"><asp:checkboxlist id="CustomerTypeCheckBoxList" runat="server" CssClass="txtNoir11"></asp:checkboxlist></TD>
															<td align="right"><asp:button id="searchByTypeButton" runat="server" CssClass="input" onclick="searchByTypeButton_Click"></asp:button></td>
														</TR>
													</TBODY>
												</TABLE>
											</td>
										</tr>
									</table>
								</td>
								<td vAlign="top" align="center">
									<!-- SUPPRIMER DE LA LISTE --><input class="input" id="btnRemove" onclick="remove();" type="button" value="<%=_bt_suppress%>"><br>
									<!-- LISTE DES LOGINS --><asp:listbox id="loginListBox" runat="server" Width="210px" EnableViewState="False" Height="450px"
										Font-Size="XX-Small" SelectionMode="Multiple"></asp:listbox><br>
									<DIV id="validateSelectionButton" style="DISPLAY: block">
										<!-- VALIDER VOTRE DEMANDE pour sélection de société/login -->
										<span onkeypress="javascript:addToHidden();" onclick="javascript:addToHidden();">
											<asp:button id="validateButton2" runat="server" CssClass="input" onclick="validateButton_Click"></asp:button></span>
									</DIV>
								</td>
							</tr>
							<tr>
								<td align="center" colSpan="2">
									<!-- INPUT HIDDEN --><input id="selectedLoginItem" type="hidden" name="selectedLoginItem">
									<input id="selectedOptionSearch" type="hidden" value="1" name="selectedOptionSearch">
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
