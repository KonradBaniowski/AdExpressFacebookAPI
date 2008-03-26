<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MediaSchedulePopUp.aspx.cs" Inherits="AdExpress.Private.Results.MediaSchedulePopUp" %>
<%@ Register TagPrefix="cc1" Namespace="TNS.AdExpress.Web.Controls.Headers" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc2" Namespace="TNS.AdExpress.Web.Controls.Buttons" Assembly="TNS.AdExpress.Web.Controls" %>
<%@ Register TagPrefix="cc3" Namespace="TNS.AdExpress.Web.Controls.Results.MediaPlan" Assembly="TNS.AdExpress.Web.Controls" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta name="Cache-control" content="no-cache" />
    <script language="javascript" type="text/javascript" src="/scripts/WebResult.js"></script>
	<script type="text/javascript" src="/scripts/dom-drag.js"></script>
    <title>AdExpress</title>
    
</head>
<body style="margin:0px; background-attachment: fixed; background-repeat: repeat; " class="popupBackGround whiteBackGround">

    <form id="form1" runat="server">
        <div style="text-align:center;margin:0">
            <asp:HiddenField ID="zoomParam" runat="server" EnableViewState="true"/>

            <table height="300" width="100%" style="margin-top: 25px; margin-left: 0px; margin-right: 25px;" class="whiteBackGround" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td colspan="2">
                        <cc1:informationwebcontrol id="InformationWebControl1" runat="server" inleftmenu="False" />
                    </td>
                </tr>
                <tr>
                    <td class="whiteBackGround" height="15"></td>
                </tr>
			    <%=zoomButton%>
                <tr class="whiteBackGround">
                    <td height="100%">
                        <cc1:OptionLayerWebControl id="OptionLayerWebControl1" runat="server" GenericDetailLevelComponentProfile="media" GenericDetailLevelType="mediaSchedule"
							Width="200px" DisplayInformationComponent="False" RemoveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelRemove.aspx" SaveASPXFilePath="/Private/MyAdExpress/GenericDetailLevelSave.aspx"></cc1:OptionLayerWebControl>
                    </td>
                    <td valign="top">
                        <table>
                            <tr valign="top"><td valign="top"><cc1:SubPeriodSelectionWebControl id="SubPeriodSelectionWebControl1" runat="server" /></td></tr>
                            <tr>
                                <td class="whiteBackGround" height="15">
                                </td>
                            </tr>
                            <tr valign="middle"><td valign="middle"><cc3:GenericMediaScheduleWebControl id="GenericMediaScheduleWebControl1" runat="server" AjaxProTimeOut="240" ShowVersion="True" /></td></tr>
                        </table>
                    </td>
                </tr>

            </table>
        </div>
        <cc1:MenuWebControl id="MenuWebControl1" runat="server"/>
    </form>
    
    <%=divClose%>

</body>
</html>
