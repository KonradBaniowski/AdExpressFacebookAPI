<%@ Import Namespace="TNS.AdExpress.Web.Translation" %>
<%@ Import Namespace="System.Data" %>
<%@ Page language="c#" Inherits="AdExpress.selectionModule" CodeFile="selectionModule.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Selections" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>AdExpress</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
		<meta http-equiv="expires" content="0"/>
		<meta http-equiv="pragma" content="no-cache"/>
		<meta content="no-cache" name="Cache-control"/>
		<link href="/Css/AdExpress.css" type="text/css" rel="stylesheet"/>
		<style type="text/css">BODY { BACKGROUND-IMAGE: none; MARGIN: 0px; BACKGROUND-COLOR: #ffffff }
		</style>
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
	</head>
	<body >
		<form id="Form1" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" width="600" border="0">
				<tr valign="top">
					<td width="209">
						<!-- Logo --><img src="/images/<%=_siteLanguage%>/logo/LogoAdexpress.gif" border=0 >
					</td>
					<td width="100%"><cc1:headerwebcontrol language="33" id="HeaderWebControl1" runat="server" Type_de_page="generic"></cc1:headerwebcontrol></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Login Information--><cc1:logininformationwebcontrol id="LoginInformationWebControl1" runat="server"></cc1:logininformationwebcontrol></td>
				</tr>
				<tr>
					<td colspan="2" height="10"><!-- Vide --></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Actualités --><cc1:actualitieswebcontrol id="ActualitiesWebControl1" runat="server" Width="800px" CssUrlPath="/Css/ModuleSelection.css"
							DisplayImage="True" RssFileUrlPath="AdExpressFeed.xml" ImageUrlPath="/Images/Common/AdExpressVisu.gif" TitleCss="actualities" RssTitleCss="txtViolet11Bold"
							RssDescriptionCss="txtViolet11" ImageRssUrlPath="/Images/Common/iconRss.gif" ColumnImageWidth="93px" RssFileName="AdExpressFeed.xml"></cc1:actualitieswebcontrol></td>
				</tr>
				<tr>
					<td colspan="2" height="10"><!-- Vide --></td>
				</tr>
				<tr>
					<td colspan="2"><!-- Modules --><cc2:moduleselection2webcontrol id="ModuleSelection2WebControl1" runat="server" Width="800px" ColumnInformationWidth="40%"
							 ColumnLeftWidth="10px" ModuleGroupWithBackgroundCss="moduleGroupWithBackgroung"
							CssUrlPath="/Css/ModuleSelection.css" ModuleSubGroupCss="moduleSubGroup" ModuleGroupInformationCss="moduleGroupInformation" ModuleGroupCss="moduleGroup"
							ModuleCss="Tips1" ImageModuleUrlPath="/Images/Common/arrowModule.gif"></cc2:moduleselection2webcontrol></td>
				</tr>
			</table>
		</form>
	</body>
</html>
