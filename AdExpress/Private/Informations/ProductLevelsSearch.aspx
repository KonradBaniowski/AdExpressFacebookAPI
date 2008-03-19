<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Results" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Informations.ProductLevelsSearch" CodeFile="ProductLevelsSearch.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
		<meta http-equiv="expires" content="0">
		<meta http-equiv="pragma" content="no-cache">
		<meta content="no-cache" name="Cache-control">
		<script language="javascript">
			function bloqueTouche(){ 
			if(event.ctrlKey) //Touche Ctrl enfoncée 
				if((event.keyCode == 78) || (event.keyCode == 104)) //touche n ou N enfoncée 
				event.returnValue = false; 
			if (event.keyCode == 122){ //touche F11 enfoncée 
				event.keyCode=0;return false;} 
			} 
			document.onkeydown =bloqueTouche; 
		</script>
	</HEAD>
	<body class="imageBackGround whiteBackGround" style="MARGIN: 0px;">
		<form id="Form1" method="post" runat="server">
			<table height="269" cellSpacing="0" cellPadding="0" width="400" border="0">
				<tr>
					<td vAlign="top"><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Others/helpBanner.jpg" ></td>
					<td>
						<table height="269" cellSpacing="0" cellPadding="0" width="400" border="0">
							<tr vAlign="top">
								<td><cc2:countitemsinclassificationwebcontrol id="CountItemsInClassificationWebControl1" runat="server" TextCss="txtViolet12"><classificationdescription labelTextId="1847" LevelType="0" />
										<classificationdescription labelTextId="1848" LevelType="1" />
										<classificationdescription labelTextId="1849" LevelType="2" />
										<classificationdescription labelTextId="1948" LevelType="6" />
										<classificationdescription labelTextId="1146" LevelType="5" />
										<classificationdescription labelTextId="1149" LevelType="11" />
										<classificationdescription labelTextId="1164" LevelType="4" />
									</cc2:countitemsinclassificationwebcontrol></td>
							</tr>
							<tr vAlign="bottom">
								<td align="right" width="400"><A onmouseover="bouton.src='/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_down.gif';" onmouseout="bouton.src = '/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif';" href="javascript:window.close();" ><IMG src="/App_Themes/<%= this.Theme %>/Images/Culture/Button/fermer_up.gif" border=0 name=bouton ></A>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
		<%=divClose%>
	</body>
</HTML>
