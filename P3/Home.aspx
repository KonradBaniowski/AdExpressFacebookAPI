<%@ Page Language="C#" MasterPageFile="~/P3MP.master" AutoEventWireup="true" CodeFile="Home.aspx.cs"
    Inherits="Home" Title="Acceuil" %>

<%@ Register TagPrefix="HeadersCtrl" Namespace="KMI.P3.Web.Controls.Headers" Assembly="KMI.P3.Web.Controls" %>
<%@ Register TagPrefix="TranslationCtrl" Namespace="KMI.P3.Web.Controls.Translation" Assembly="KMI.P3.Web.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" width="100%" class="homePage">
  
        <tr>
            <!--Menu gauche Liste des projets - Logins-->
            <td class="leftMenu">
                <HeadersCtrl:LoginLinksWebControl id="LoginLinksWebControl1" runat="server" />
            </td>
            <!--Contenu HTML Marketing-->
            <td style="width:100%;height:100%; background-color:#ffffff;">
                    <div class="contentmarketing" >
                    <%Response.WriteFile(_url);%>
                    </div>
            </td>
        </tr>
    </table>
    <input type =hidden name ="__loginChoiceEVENTTARGET" value =""/>
    <input type =hidden name ="__loginChoiceEVENTARGUMENT" value =""/>


</asp:Content>
