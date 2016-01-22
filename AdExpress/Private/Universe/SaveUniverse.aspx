<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Universe.SaveUniverse" CodeFile="SaveUniverse.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<base target="_self"/>
		<script language="javascript">
		function cancel(){
			window.returnValue=true;
			window.close();
		}
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
        <style type="text/css" media="screen">
            html {
            height:100%; max-height:100%; padding:0; margin:0; border:0; background:#fff; 
            /* hide overflow:hidden from IE5/Mac */ 
            /* \*/ 
            overflow: hidden; 
            /* */ 
            }
            body {height:100%; max-height:100%; overflow:hidden; padding:0; margin:0; border:0;}
        </style>
	</head>
	<body class="popUpbody" onload="javascript:activateActiveX();">
		<form id="Form1" method="post" runat="server">
		        <!-- Header -->
		        <div class="popUpHead popUpHeaderBackground popUpTextHeader">
			        &nbsp;<cc2:adexpresstext language="33" id="AdExpressText4" runat="server" Code="769"></cc2:adexpresstext>
		        </div>
		        <!-- Content -->
		        <div class="popUpContent">
		            <div class="popUpPad2"></div>
			            <table id="SaveTable" cellspacing="0" cellpadding="0" width="100%" border="0">
				            <tr>
					            <td class="txtViolet11Bold" height="20"><cc2:adexpresstext language="33" id="AdExpressText1" runat="server" Code="917"></cc2:adexpresstext></td>
				            </tr>
				            <tr height="40">
					            <td valign="top" ><asp:DropDownList id="directoryDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></td>
				            </tr>
				            <!--Debut liste des univers sauvegardés-------------->
                            <tr>
	                            <td class="txtViolet11Bold" height="20"><cc2:adexpresstext language="33" id="AdExpressText3" runat="server" Code="2261"></cc2:adexpresstext></td>
                            </tr>
                            <tr height="40">
	                            <td vAlign="top"><asp:DropDownList id="universeDropDownList" runat="server" Width="168px" CssClass="txtNoir11"></asp:DropDownList></td>
                            </tr>
                            <!--Fin listes univers sauvegardés------------------->
				            <tr>
					            <td class="txtViolet11Bold"><cc2:adexpresstext language="33" id="AdExpressText2" runat="server" Code="2268"></cc2:adexpresstext>&nbsp;</td>
				            </tr>
				            <tr height="40">
					            <td vAlign="top"><asp:TextBox id="universeTextBox" runat="server" CssClass="txtNoir11" Width="200px"></asp:TextBox></td>
				            </tr>
			            </table>
			        <div class="popUpPad2"></div>
                </div>
		        <!-- Footer -->
		        <div class="popUpFoot popUpFooterBackground">
                    <div style="padding-top:12px">
			            <cc1:ImageButtonRollOverWebControl id="okButton" runat="server" onclick="okButton_Click" SkinID="validateButton"></cc1:ImageButtonRollOverWebControl>&nbsp;<a href="javascript:cancel();" onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif';"><img src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/annuler_up.gif" border="0" name="bouton"></a>&nbsp;&nbsp;</td>
		            </div>
				</div>
		</form>
	</body>
</html>
