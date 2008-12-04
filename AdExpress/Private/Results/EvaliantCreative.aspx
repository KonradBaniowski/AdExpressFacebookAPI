<%@ Page Language="C#" CodeFile="EvaliantCreative.aspx.cs" Inherits="AdExpress.Private.Results.EvaliantCreative" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head id="HEAD1" runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script type="text/javascript" language="javascript">
			function setWindow(){
			    var width = 530;
			    var height = 530;
			    var change = false;
				if(Form1.width.value > width || Form1.height.value > height){
				    width = parseInt(Form1.width.value) + 100;
				    height = parseInt(Form1.height.value) + 100;
				    change = true;
				}
				if(change == true)
				{
					window.resizeTo(width,height);
					window.moveTo((screen.width-document.body.clientWidth)/2,(screen.height-document.body.clientHeight)/2);
				}
			}			
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</head>
	<body onload="javascript:setWindow();javascript:activateActiveX();" class="whiteBackGround darkBackGround">
		<form id="Form1" method="post" runat="server">
		    <asp:HiddenField id="width" Value="1" runat="server"/>
		    <asp:HiddenField id="height" Value="1" runat="server"/>
		
			<%=result%>
			
			&nbsp;
		</form>
	</body>
</html>
