<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Translation" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Page language="c#" Inherits="AdExpress.Private.Universe.UniverseDetailPopUp" CodeFile="UniverseDetailPopUp.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
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
		<meta name="Cache-control" content="no-cache">
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
        <script language="javascript" type="text/javascript">
            function resizeWin() {
                if (document.getElementById('contentDiv').scrollHeight > document.getElementById('popUpBody').offsetHeight) {
                    document.getElementById('header').style.right = "16px";
                    document.getElementById('headerTitle').style.paddingLeft = "16px";
                    document.getElementById('footer').style.right = "16px";
                }
            }
        </script>
	</HEAD>
	<body id="popUpBody" class="popUpbody" onload="javascript:activateActiveX();resizeWin();">
		<form id="Form1" method="post" runat="server">
				<!-- Header -->
				<div id="header" class="popUpHead popUpHeaderBackground popUpTextHeader">
                    <div id="headerTitle">
					    &nbsp;<cc1:adexpresstext language="33" id="detailUniversAdExpressText" runat="server" Code="960"></cc1:adexpresstext>&nbsp;
						<asp:label id="universeLabel" runat="server"></asp:label>
					</div>
				</div>
				<!-- Content -->
				<div id="contentDiv" class="popUpContent">
		            <div class="popUpPad2"></div>
					    <TABLE id="SaveTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
				            <!--Annonceurs / Références ...-->
				            <TR>
					            <TD class="txtViolet11Bold backGroundWhite" style="HEIGHT: 18px">&nbsp;
						            <cc1:adexpresstext language="33" id="advertiserAdexpressText" runat="server" Code="0"></cc1:adexpresstext></TD>
				            </TR>
				            <TR height="20">
					            <TD vAlign="top" class="backGroundWhite"><%=advertiserText%></TD>
				            </TR>
				            <TR>
					            <TD>&nbsp;</TD>
				            </TR>
			            </TABLE>
					<div class="popUpPad2"></div>
                </div>
				<!-- Footer -->
				<div id="footer" class="popUpFoot popUpFooterBackground">
                    <div style="padding-top:12px">
					    <cc2:imagebuttonrolloverwebcontrol id="closeImageButtonRollOverWebControl" runat="server" onclick="closeImageButtonRollOverWebControl_Click" SkinID="fermerButton" ></cc2:imagebuttonrolloverwebcontrol>&nbsp;&nbsp;
				</div>
				</div>
		</form>
	</body>
</HTML>
