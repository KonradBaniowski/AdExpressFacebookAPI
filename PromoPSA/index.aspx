<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;"/>
	<meta content="C#" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
	<meta http-equiv="expires" content="0"/>
	<meta http-equiv="pragma" content="no-cache"/>
	<meta content="no-cache" name="Cache-control"/>
</head>
<body class="bodyStyle">
    <form id="form1" runat="server">
    <div style="display:none;"><asp:TextBox runat="server" ID="__CUSTOMIZECLASSIFICATION"/></div>
    <div class="header"><asp:Image ID="Image1" runat="server" SkinID="logo_tns_home" /></div>
    <div id="global">
	    <div id="content">
		    <table border="0" width="300" cellpadding="3" cellspacing="0">
			    <tr>
			        <td align="right" width="50%"><asp:Label ID="loginLabel" runat="server" CssClass="txtGreyBold12"></asp:Label></td>
			        <td align="left"><asp:TextBox ID="loginTextBox" runat="server" CssClass="textBox"></asp:TextBox></td>
			    </tr>
			    <tr>
			        <td align="right"><asp:Label ID="passwordLabel" runat="server" CssClass="txtGreyBold12"></asp:Label></td>
			        <td align="left"><asp:TextBox ID="passwordTextBox" runat="server" TextMode="Password" CssClass="textBox"></asp:TextBox></td>
			    </tr>
			    <tr>
			        <td>&nbsp;</td>
			        <td align="left"><asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Valider" CssClass="validateButton" /></td>
			    </tr>
		    </table>
        </div>
    </div>
    </form>
</body>
</html>