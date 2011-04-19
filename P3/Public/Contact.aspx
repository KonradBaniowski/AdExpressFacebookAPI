<%@ Page Language="C#" MasterPageFile="~/P3MP.master" AutoEventWireup="true" CodeFile="Contact.aspx.cs"  Inherits="Contact" Title="Contact" %>
<%@ Register TagPrefix="cc1" Namespace="KMI.P3.Web.Controls.Translation" Assembly="KMI.P3.Web.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <div class="contact">
            <table border="0" cellpadding="0" cellspacing="0" align="center"><tr><td>
                <ul>
                    <li><cc1:TextWebControl  id="titleTextId" runat="server" Code="2"></cc1:TextWebControl></li>
                    <li>
                        <ul>
                            <li><cc1:TextWebControl  id="informationTextId" runat="server" Code="4"></cc1:TextWebControl></li>
                            <li>
                                <ul>
                                    <li><cc1:TextWebControl  id="ContactNameTextId1" runat="server" Code="5"></cc1:TextWebControl>
                                        <ul>
                                            <li><cc1:TextWebControl  id="ContactFunctionTextId1" runat="server" Code="6"></cc1:TextWebControl></li>
                                            <li><cc1:TextWebControl  id="ContactTelTextId1" runat="server" Code="7"></cc1:TextWebControl></li>
                                            <li><a href="#" id="link1" runat="server"><cc1:TextWebControl  id="ContactEmailTextId1" runat="server" Code="8"></cc1:TextWebControl></a></li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                            <li>
                                <ul>
                                    <li><cc1:TextWebControl  id="ContactNameTextId2" runat="server" Code="9"></cc1:TextWebControl>
                                        <ul>
                                            <li><cc1:TextWebControl  id="ContactFunctionTextId2" runat="server" Code="10"></cc1:TextWebControl></li>
                                            <li><cc1:TextWebControl  id="ContactTelTextId2" runat="server" Code="11"></cc1:TextWebControl></li>
                                            <li><a href="#" id="link2" runat="server"><cc1:TextWebControl  id="ContactEmailTextId2" runat="server" Code="12"></cc1:TextWebControl></a></li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                            <li>
                                <ul>
                                    <li><cc1:TextWebControl  id="ContactNameTextId3" runat="server" Code="13"></cc1:TextWebControl>
                                        <ul>
                                            <li><cc1:TextWebControl  id="ContactFunctionTextId3" runat="server" Code="14"></cc1:TextWebControl></li>
                                            <li><cc1:TextWebControl  id="ContactTelTextId3" runat="server" Code="15"></cc1:TextWebControl></li>
                                            <li><a href="#" id="link3" runat="server"><cc1:TextWebControl  id="ContactEmailTextId3" runat="server" Code="16"></cc1:TextWebControl></a></li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                         </ul>
                     </li>
                </ul>
            </td></tr>
        </table>
    </div>
</asp:Content>

