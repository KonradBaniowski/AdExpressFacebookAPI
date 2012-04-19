<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataUpdateRussia.aspx.cs"
    Inherits="AdExpress.Private.Informations.DataUpdateRussia" %>

<%@ Register Assembly="TNS.AdExpress.Web.Controls" Namespace="TNS.AdExpress.Web.Controls.Buttons"
    TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Russia Update Page</title>
    <meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT">
    <meta http-equiv="expires" content="0">
    <meta http-equiv="pragma" content="no-cache">
    <meta content="no-cache" name="Cache-control">
</head>
<body class="popUpBackGround">
    <form id="form1" method="post" runat="server">
    <div>
        <table border="0" align="center" cellpadding="3" cellspacing="0" class="backGroundWhite">
            <tr>
                <td class="txtViolet14Bold">
                    <asp:Label ID="_tableData" runat="server" Text="_tableData" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="935" border="0" cellpadding="1" cellspacing="0" class="backGroundWhite txtViolet11">
                        <!-- HEADER -->
                        <tr align="center" class="txtBlanc11Bold">
                            <td width="70%" class="violetBackGround ColumnHeaderLeft">
                                <asp:Label ID="_columnHeader13" runat="server" Text="_columnHeader13" />
                            </td>
                            <td width="30%" class="violetBackGround ColumnHeaderRight">
                                <asp:Label ID="_columnHeader14" runat="server" Text="_columnHeader14" />
                            </td>
                        </tr>
                        <!-- SEPARATOR -->
                        <tr align="center" class="txtViolet11Bold">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellTopLeft" align="left">
                                <asp:Label ID="_mediaColumn1" runat="server" Text="_mediaColumn1" />
                            </td>
                            <td class="CellTopRight">
                                <asp:Label ID="_dateColumn1" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn2" runat="server" Text="_mediaColumn2" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn2" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn3" runat="server" Text="_mediaColumn3" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn3" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn4" runat="server" Text="_mediaColumn4" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn4" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn5" runat="server" Text="_mediaColumn5" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn5" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn6" runat="server" Text="_mediaColumn6" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn6" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn7" runat="server" Text="_mediaColumn7" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn7" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn8" runat="server" Text="_mediaColumn8" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn8" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn9" runat="server" Text="_mediaColumn9" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn9" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn10" runat="server" Text="_mediaColumn10" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn10" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn11" runat="server" Text="_mediaColumn11" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn11" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn12" runat="server" Text="_mediaColumn12" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn12" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn13" runat="server" Text="_mediaColumn13" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn13" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaColumn14" runat="server" Text="_mediaColumn14" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateColumn14" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellBottomLeft" align="left">
                                <asp:Label ID="_mediaColumn15" runat="server" Text="_mediaColumn15" />
                            </td>
                            <td class="CellBottomRight">
                                <asp:Label ID="_dateColumn15" runat="server" Text="-" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="txtViolet14Bold">
                    <asp:Label ID="_tableShedule" runat="server" Text="_tableShedule" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="935" border="0" cellpadding="1" cellspacing="0" class="backGroundWhite txtViolet11">
                        <!-- HEADER -->
                        <tr align="center" class="txtBlanc11Bold">
                            <td width="15%" class="violetBackGround ColumnHeaderLeft">
                                <asp:Label ID="_columnHeader1" runat="server" Text="Label" />
                            </td>
                            <td width="10%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader2" runat="server" Text="Label" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader3" runat="server" Text="Label" />
                            </td>
                            <td width="10%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader4" runat="server" Text="Label" />
                            </td>
                            <td width="30%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader5" runat="server" Text="Label" />
                            </td>
                            <td width="15%" class="violetBackGround ColumnHeaderRight">
                                <asp:Label ID="_columnHeader6" runat="server" Text="Label" />
                            </td>
                        </tr>
                        <!-- SEPARATOR -->
                        <tr align="center" class="txtViolet11Bold">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr align="center">
                            <td rowspan="12" class="CellSolid">
                                <asp:Label ID="_reportColumn1" runat="server" Text="_reportColumn1" />
                            </td>
                            <td rowspan="2" class="CellTopCenter">
                                <asp:Label ID="_mediumColumn1" runat="server" Text="_mediumColumn1" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_marketColumn1" runat="server" Text="_marketColumn1" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_dailyColumn1" runat="server" Text="_dailyColumn1" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_weeklyColumn1" runat="server" Text="-" />
                            </td>
                            <td class="CellTopRight">
                                <asp:Label ID="_monthlyColumn1" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn2" runat="server" Text="_marketColumn2" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn2" runat="server" Text="_dailyColumn2" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn2" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn2" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn3" runat="server" Text="_mediumColumn3" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn3" runat="server" Text="_marketColumn3" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn3" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn3" runat="server" Text="_weeklyColumn3" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn3" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn4" runat="server" Text="_mediumColumn4" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn4" runat="server" Text="_marketColumn4" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn4" runat="server" Text="_dailyColumn4" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn4" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn4" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td rowspan="2" class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn5" runat="server" Text="_mediumColumn5" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn5" runat="server" Text="_marketColumn5" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn5" runat="server" Text="_dailyColumn5" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn5" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn5" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn6" runat="server" Text="_marketColumn6" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn6" runat="server" Text="_dailyColumn6" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn6" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn6" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn7" runat="server" Text="_mediumColumn7" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn7" runat="server" Text="_marketColumn7" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn7" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn7" runat="server" Text="_weeklyColumn7" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn7" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn8" runat="server" Text="_mediumColumn8" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn8" runat="server" Text="_marketColumn8" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn8" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn8" runat="server" Text="_weeklyColumn8" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn8" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn9" runat="server" Text="_mediumColumn9" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn9" runat="server" Text="_marketColumn9" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn9" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn9" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn9" runat="server" Text="_monthlyColumn9" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn10" runat="server" Text="_mediumColumn10" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn10" runat="server" Text="_marketColumn10" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn10" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn10" runat="server" Text="_weeklyColumn10" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn10" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn11" runat="server" Text="_mediumColumn11" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn11" runat="server" Text="_marketColumn11" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn11" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn11" runat="server" Text="_weeklyColumn11" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn11" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellBottomCenter">
                                <asp:Label ID="_mediumColumn22" runat="server" Text="_mediumColumn22" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_marketColumn22" runat="server" Text="_marketColumn22" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_dailyColumn22" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_weeklyColumn22" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomRight">
                                <asp:Label ID="_monthlyColumn22" runat="server" Text="_monthlyColumn22" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table width="935" border="0" cellpadding="1" cellspacing="0" class="backGroundWhite txtViolet11">
                        <!-- HEADER -->
                        <tr align="center" class="txtBlanc11Bold">
                            <td width="15%" class="violetBackGround ColumnHeaderLeft">
                                <asp:Label ID="_columnHeader7" runat="server" Text="Label" />
                            </td>
                            <td width="10%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader8" runat="server" Text="Label" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader9" runat="server" Text="Label" />
                            </td>
                            <td width="10%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader10" runat="server" Text="Label" />
                            </td>
                            <td width="30%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_columnHeader11" runat="server" Text="Label" />
                            </td>
                            <td width="15%" class="violetBackGround ColumnHeaderRight">
                                <asp:Label ID="_columnHeader12" runat="server" Text="Label" />
                            </td>
                        </tr>
                        <!-- SEPARATOR -->
                        <tr align="center" class="txtViolet11Bold">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr align="center">
                            <td rowspan="10" class="CellSolid">
                                <asp:Label ID="_reportColumn2" runat="server" Text="_reportColumn2" />
                            </td>
                            <td rowspan="2" class="CellTopCenter">
                                <asp:Label ID="_mediumColumn12" runat="server" Text="_mediumColumn12" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_marketColumn12" runat="server" Text="_marketColumn12" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_dailyColumn12" runat="server" Text="_dailyColumn12" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_weeklyColumn12" runat="server" Text="-" />
                            </td>
                            <td class="CellTopRight">
                                <asp:Label ID="_monthlyColumn12" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn13" runat="server" Text="_marketColumn13" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn13" runat="server" Text="_dailyColumn13" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn13" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn13" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn14" runat="server" Text="_mediumColumn14" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn14" runat="server" Text="_marketColumn14" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn14" runat="server" Text="_dailyColumn14" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn14" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn14" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td rowspan="2" class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn15" runat="server" Text="_mediumColumn15" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn15" runat="server" Text="_marketColumn15" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn15" runat="server" Text="_dailyColumn15" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn15" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn15" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn16" runat="server" Text="_marketColumn16" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn16" runat="server" Text="_dailyColumn16" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn16" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn16" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn17" runat="server" Text="_mediumColumn17" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn17" runat="server" Text="_marketColumn17" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn17" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn17" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn17" runat="server" Text="_monthlyColumn17" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn18" runat="server" Text="_mediumColumn18" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn18" runat="server" Text="_marketColumn18" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn18" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn18" runat="server" Text="_weeklyColumn18" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn18" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td rowspan="2" class="CellMiddleCenter">
                                <asp:Label ID="_mediumColumn19" runat="server" Text="_mediumColumn19" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn19" runat="server" Text="_marketColumn19" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn19" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn19" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn19" runat="server" Text="_monthlyColumn19" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketColumn20" runat="server" Text="_marketColumn20" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyColumn20" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyColumn20" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyColumn20" runat="server" Text="_monthlyColumn20" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellBottomCenter">
                                <asp:Label ID="_mediumColumn21" runat="server" Text="_mediumColumn21" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_marketColumn21" runat="server" Text="_marketColumn21" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_dailyColumn21" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_weeklyColumn21" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomRight">
                                <asp:Label ID="_monthlyColumn21" runat="server" Text="_monthlyColumn21" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cc2:ImageButtonRollOverWebControl ID="closeRollOverWebControl" runat="server" OnClick="closeRollOverWebControl_Click"
                        SkinID="fermerButton"></cc2:ImageButtonRollOverWebControl>&nbsp;&nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
