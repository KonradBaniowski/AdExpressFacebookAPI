<%@ Page language="c#" Inherits="AdExpress.Private.MyAdExpress.APPMExcelSavePopUp" CodeFile="APPMExcelSavePopUp.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
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
			        &nbsp;<cc1:adexpresstext language="33" id="saveTitle" runat="server" Code="1970"></cc1:adexpresstext>
		        </div>
		        <!-- Content -->
		        <div class="popUpContent">
		            <div class="popUpPad2"></div>
			            <table id="SaveData" cellspacing="2" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="FileNameLabel" runat="server" Code="1746"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxFileName" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr>
								<td class="txtViolet11Bold" width="150"><cc1:adexpresstext language="33" id="MailLabel" runat="server" Code="1136"></cc1:adexpresstext></td>
								<td><asp:textbox id="tbxMail" runat="server" Width="300px"></asp:textbox></td>
							</tr>
							<tr>
								<td colSpan="2">&nbsp;</td>
							</tr>
							<tr>
								<td class="txtViolet11Bold" colspan="2"><asp:CheckBox id="cbxRegisterMail" runat="server" Visible="False"></asp:CheckBox></td>
							</tr>
						</table>
					<div class="popUpPad2"></div>
                </div>
		        <!-- Footer -->
		        <div class="popUpFoot popUpFooterBackground">
                    <div style="padding-top:12px">
			            <cc2:imagebuttonrolloverwebcontrol id="validateRollOverWebControl" runat="server" onclick="validateRollOverWebControl_Click" SkinID="validateButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;<cc2:imagebuttonrolloverwebcontrol id="closeRollOverWebControl" runat="server" onclick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
		            </div>
				</div>
		</form>
	</body>
</html>
