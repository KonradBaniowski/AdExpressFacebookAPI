<%@ Page language="c#" Inherits="AdExpress.Private.Results.ZoomCreationPopUp" CodeFile="ZoomCreationPopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<script language="javascript">
			function setWindow(){
				if(Form1.fileNb.value>1){
					window.resizeTo(1010,700);
					window.moveTo((screen.width-document.body.clientWidth)/2,(screen.height-document.body.clientHeight)/2);
				}
			}
		</script>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta name="Cache-control" content="no-cache"/>
	</HEAD>
	<body onload="javascript:setWindow();javascript:activateActiveX();" class="backGroundWhite darkBackGround">
		<form id="Form1" method="post" runat="server">
			<%=result%>
			&nbsp;
		</form>
	</body>
</HTML>
