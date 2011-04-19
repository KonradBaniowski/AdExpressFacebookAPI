<%@ Page Language="C#" MasterPageFile="~/P3MP.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>
<%@ Register TagPrefix="HeadersCtrl" Namespace="KMI.P3.Web.Controls.Headers" Assembly="KMI.P3.Web.Controls" %>
<%@ Register TagPrefix="TranslationCtrl" Namespace="KMI.P3.Web.Controls.Translation" Assembly="KMI.P3.Web.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" class="homePage">
        <tr>
            <td valign="top" style="width:100%; height:100%">
                <table cellpadding="0" cellspacing="0" border="0" style="width:100%; height:100%">
                    <tr>
                        <td style="width:260px; height:100%; vertical-align:top">
                            <table cellpadding="0" cellspacing="0" border="0" >
                                <tr><td><asp:image ID="Image2" runat="server" SkinID="pixel" Height="20px"/></td></tr>
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" border="0" width="100%" class="authentication">
                                            <tr><td class="authenticationText"><TranslationCtrl:TextWebControl id="TextWebControl1" runat="server" Code="1" /> :</td></tr>
                                            <tr><td class="authenticationValue"><HeadersCtrl:AuthenticationWebControl id="AuthenticationWebControl1" runat="server" /></td></tr>
                                            <tr><td><asp:image ID="Image1" runat="server" SkinID="pixel" Height="20px"/></td></tr>
                                         </table>
                                     </td>
                                </tr>
                                <tr><td><asp:image ID="Image3" runat="server" SkinID="pixel" Height="100%"/></td></tr>
                            </table>
                        </td>
                        <td style="margin:0px; padding:0px"><asp:image ID="Image4" runat="server" SkinID="pixel" />gdfgdgdg</td>
                    </tr>
                 </table>
            </td>
           <%-- <td><HeadersCtrl:FlashWebControl ID="flashWebControl1" runat="server" Width="733" Height="484"/></td>--%>
        </tr>
    </table>
</asp:Content>
