<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;"/>
	<meta content="C#" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
	<meta http-equiv="expires" content="0"/>
	<meta http-equiv="pragma" content="no-cache"/>
	<meta content="no-cache" name="Cache-control"/>
    <script src="/js/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="/js/json2.js" type="text/javascript" ></script>
</head>
<script type="text/javascript" language="javascript">

    $(document).ready(function () {
        $("#passwordTextBox").keypress(function (event) {
            if (event.keyCode == 13) {
                $("#sbtButton").click();
            }
        });
    });

    function VerifUser() {
        var loginObj = document.getElementById("loginTextBox");
        var passwordObj = document.getElementById("passwordTextBox");

        $.ajax({
            type: "POST",
            url: 'index.aspx/verifUser',
            async: false,
            data: JSON.stringify({ login: loginObj.value, password: passwordObj.value }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, st) {
                var res = JSON.parse(msg.d);
                if (res == true)
                    VerifDialog();
                else {
                    document.getElementById('Button1').click();
                }
            },
            error: function () {
                alert("Erreur lors de la connexion");
            }
        });
    }

    function VerifDialog() {

        var res = confirm("Ce login est deja en cours d'utilisation, voulez vous déconnecter la personne utilisant ce login ?");

        if (res == true) {
            document.getElementById('Button1').click();
        }
        else {
            return;
        }
    }
</script>
<body class="bodyStyle">
    <form id="form1" runat="server">
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
                    <td><input type="button" id="sbtButton" name="sbtButton" onclick="VerifUser()" value="Valider" class="validateButton"/></td>
			        <td align="left"><asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Valider" CssClass="validateButton" style="display:none;"/></td>
			    </tr>
		    </table>
        </div>
    </div>
    </form>
</body>
</html>