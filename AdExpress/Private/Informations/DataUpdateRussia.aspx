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
<body class="popUpBackGround" onload="javascript:activateActiveX();">
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
                                <asp:Label ID="_updateColumnHeader1" runat="server" Text="Тип медиа" />
                            </td>
                            <td width="30%" class="violetBackGround ColumnHeaderRight">
                                <asp:Label ID="_updateColumnHeader2" runat="server" Text="Дата поставки" />
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
                                <asp:Label ID="_mediaTvNationalColumn" runat="server" Text="ТВ Национальная реклама и Москва" />
                            </td>
                            <td class="CellTopRight">
                                <asp:Label ID="_dateTvNationalColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaTvRegionColumn" runat="server" Text="ТВ Регионы" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateTvRegionColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaTvSponsorshipColumn" runat="server" Text="ТВ Спонсорство" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateTvSponsorshipColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaTvNicheChannelsColumn" runat="server" Text="ТВ Нишевые каналы" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateTvNicheChannelsColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaPressColumn" runat="server" Text="Пресса" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_datePressColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaEditorialColumn" runat="server" Text="Редакционная поддержка" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateEditorialColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaRadioMoscowColumn" runat="server" Text="Радио Москва" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateRadioMoscowColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaRadioRegionColumn" runat="server" Text="Радио Регионы" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateRadioRegionColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaRadioSponsorshipColumn" runat="server" Text="Радио Спонсорство" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateRadioSponsorshipColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaRadioMusicColumn" runat="server" Text="Радио Музыка" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateRadioMusicColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediaOutdoorColumn" runat="server" Text="Наружная реклама" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_dateOutdoorColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellBottomLeft" align="left">
                                <asp:Label ID="_mediaInternetColumn" runat="server" Text="Интернет" />
                            </td>
                            <td class="CellBottomRight">
                                <asp:Label ID="_dateInternetColumn" runat="server" Text="-" />
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
                            <td width="20%" class="violetBackGround ColumnHeaderLeft">
                                <asp:Label ID="_scheduleColumnHeader1" runat="server" Text="Тип медиа" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_scheduleColumnHeader2" runat="server" Text="География" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_scheduleColumnHeader3" runat="server" Text="Ежедневно" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeader">
                                <asp:Label ID="_scheduleColumnHeader4" runat="server" Text="Еженедельно" />
                            </td>
                            <td width="20%" class="violetBackGround ColumnHeaderRight">
                                <asp:Label ID="_scheduleColumnHeader5" runat="server" Text="Ежемесячно" />
                            </td>
                        </tr>
                        <!-- SEPARATOR -->
                        <tr align="center" class="txtViolet11Bold">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <!--ТВ-->
                        <tr align="center">
                            <td rowspan="2" class="CellTopLeft" align="left">
                                <asp:Label ID="_mediumTvColumn" runat="server" Text="ТВ" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_marketTvNationalColumn" runat="server" Text="Национальная реклама+Москва" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_dailyTvNationalColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellTopCenter">
                                <asp:Label ID="_weeklyTvNationalColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellTopRight">
                                <asp:Label ID="_monthlyTvNationalColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketTvRegionColumn" runat="server" Text="Регионы" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyTvRegionColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyTvRegionColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyTvRegionColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--ТВ Спонсорство-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumTvSponsorshipColumn" runat="server" Text="ТВ Спонсорство" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketTvSponsorshipColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyTvSponsorshipColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyTvSponsorshipColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyTvSponsorshipColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--ТВ Нишевые каналы-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumTvNicheChannelsColumn" runat="server" Text="ТВ Нишевые каналы" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketTvNicheChannelsColumn" runat="server" Text="Национальная реклама" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyTvNicheChannelsColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyTvNicheChannelsColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyTvNicheChannelsColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Пресса-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumPressColumn" runat="server" Text="Пресса" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketPressColumn" runat="server" Text="Национальная и локальная реклама" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyPressColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyPressColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyPressColumn" runat="server" Text="-" />
                            </td>
                        </tr>                        
                        <!--Редакционная поддержка-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumEditorialColumn" runat="server" Text="Редакционная поддержка" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketEditorialColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_daylyEditorialColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyEditorialColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyEditorialColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Радио-->
                        <tr align="center">
                            <td rowspan="2" class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumRadioColumn" runat="server" Text="Радио" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketRadioMoscowColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyRadioMoscowColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyRadioMoscowColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyRadioMoscowColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketRadioRegionColumn" runat="server" Text="Регионы" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyRadioRegionColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyRadioRegionColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyRadioRegionColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Радио Спонсорство-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumRadioSponsorshipColumn" runat="server" Text="Радио Спонсорство" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketRadioSponsorshipColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyRadioSponsorshipColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyRadioSponsorshipColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyRadioSponsorshipColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Радио Музыка-->
                        <tr align="center">
                            <td class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumRadioMusicColumn" runat="server" Text="Радио Музыка" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketRadioMusicColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyRadioMusicColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyRadioMusicColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyRadioMusicColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Наружная реклама-->
                        <tr align="center">
                            <td rowspan="2" class="CellMiddleLeft" align="left">
                                <asp:Label ID="_mediumOutdoorColumn" runat="server" Text="Наружная реклама" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketOutdoorMoscowColumn" runat="server" Text="Москва" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyOutdoorMoscowColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyOutdoorMoscowColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyOutdoorMoscowColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr align="center">
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_marketOutdoorRegionColumn" runat="server" Text="Регионы" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_dailyOutdoorRegionColumn082" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleCenter">
                                <asp:Label ID="_weeklyOutdoorRegionColumn082" runat="server" Text="-" />
                            </td>
                            <td class="CellMiddleRight">
                                <asp:Label ID="_monthlyOutdoorRegionColumn" runat="server" Text="-" />
                            </td>
                        </tr>
                        <!--Интернет-->
                        <tr align="center">
                            <td class="CellBottomLeft" align="left">
                                <asp:Label ID="_mediumInternetColumn" runat="server" Text="Интернет" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_marketInternetColumn" runat="server" Text="Национальная и локальная реклама" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_dailyInternetColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomCenter">
                                <asp:Label ID="_weeklyInternetColumn" runat="server" Text="-" />
                            </td>
                            <td class="CellBottomRight">
                                <asp:Label ID="_monthlyInternetColumn" runat="server" Text="-" />
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
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cc2:ImageButtonRollOverWebControl ID="ImageButtonRollOverWebControl2" runat="server" OnClick="closeRollOverWebControl_Click" SkinID="fermerButton"></cc2:ImageButtonRollOverWebControl>
                    &nbsp;&nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
