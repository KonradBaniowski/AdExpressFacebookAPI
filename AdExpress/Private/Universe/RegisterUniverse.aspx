﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegisterUniverse.aspx.cs" Inherits="Private_Universe_RegisterUniverse" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<base target="_self"/>
		<script language="javascript">
        function GetSelectedItemsLevel(){
        var keyWordTree="TreeLevelSelectedIds";
        var subStrIds; 
        var tab_level='';
        var j=0;
        var tabOfLevels;
            var tab_input =  window.opener.document.getElementsByTagName('input');
            if(tab_input != null){
                for(var i=0; i<tab_input.length; i++){
                    if(tab_input[i].id.indexOf("TreeLevelSelectedIds")!=-1  && tab_input[i].value != null && tab_input[i].value.length>0){
                        subStrIds = tab_input[i].id.substring(keyWordTree.length,tab_input[i].id.length);
                        tab_level += subStrIds + ':' + tab_input[i].value + '|';                       
                        j++;
                    }
                }
                if(tab_level != null && typeof tab_level != "undefined" && tab_level.length>0)
                tab_level = tab_level.substring(0,tab_level.length-1); 
                 
               tabOfLevels = document.getElementById('LevelsIdsHiddenField');
               if(tabOfLevels != null && typeof tabOfLevels != "undefined" && tab_level.length>0){
                    tabOfLevels.value =  tab_level; 
               }              
            }
        }
		
		function cancel(){
			window.returnValue=true;
			window.close();
		}
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta name="Cache-control" content="no-cache">
	</HEAD>
	<body class="popUpbody" onload="GetSelectedItemsLevel();javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server" >
			<table cellSpacing="0" cellPadding="0" width="100%" height="100%" border="0">
				<!-- Header -->
				<tr>
					<td class="popUpHeaderBackground popUpTextHeader">&nbsp;<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="769"></cc2:adexpresstext>
					</td>
				</tr>

				<!-- Content -->
				<tr>
					<td style="height:100%;background-color:#FFF;padding:10;" valign="top">
					
					    <TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite" height="20">
						            <cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="917"></cc2:adexpresstext></TD>
				            </TR>
				            <TR height="40">
					            <TD vAlign="top" class="backGroundWhite">
						            <asp:DropDownList id="directoryDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></TD>
				            </TR>
				            <TR height="10">
					            <TD></TD>
				            </TR>
				            <!--Debut liste des univers sauvegardés-------------->
                            <TR>
	                            <TD class="txtViolet11Bold backGroundWhite" height="20">
		                            <cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2261"></cc2:adexpresstext></TD>
                            </TR>
                            <TR height="40">
	                            <TD vAlign="top" class="backGroundWhite">
		                            <asp:DropDownList id="universeDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></TD>
                            </TR>
                            <TR height="10">
	                            <TD></TD>
                            </TR>
                            <!--Fin listes univers sauvegardés------------------->
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite">
						            <cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="2268"></cc2:adexpresstext>&nbsp;</TD>
				            </TR>
				            <TR height="40">
					            <TD vAlign="top" class="backGroundWhite">
						            <asp:TextBox id="universeTextBox" runat="server" CssClass="txtNoir11" Width="200px"></asp:TextBox></TD>
				            </TR>
			            </TABLE>
			
					</td>
				</tr>
				
				<!-- Footer -->
				<tr>
					<td class="popUpFooterBackground" align="right"><cc1:ImageButtonRollOverWebControl id="okButton" runat="server" onclick="okButton_Click" SkinID="validateButton"></cc1:ImageButtonRollOverWebControl>&nbsp;<A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif';" href="javascript:cancel();" ><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif" border=0 name=bouton></A>&nbsp;&nbsp;</td>
				</tr>
			</table>
			<input type="hidden" id="LevelsIdsHiddenField" name="LevelsIdsHiddenField" />
		</form>
	</body>
</HTML>

