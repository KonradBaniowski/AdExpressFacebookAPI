<%@ Page Language="C#" MasterPageFile="~/P3MP.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
    Inherits="_Default" Title="TEST" %>

<%@ Register TagPrefix="HeadersCtrl" Namespace="KMI.P3.Web.Controls.Headers" Assembly="KMI.P3.Web.Controls" %>
<%@ Register TagPrefix="TranslationCtrl" Namespace="KMI.P3.Web.Controls.Translation"
    Assembly="KMI.P3.Web.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" width="100%" class="homePage">
  
        <tr>
            <!--Menu gauche Liste des projets - Logins-->
            <td class="leftMenu">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td> <!--class="authenticationValue"-->
                          
                            <div id="listLoginAdexpress" class="menulogins">
                                <ul>
                                    <li><a href="javascript:loginChoiceDoPostBack('buttonadexpress','1-1084')" class="adexpresslink">
                                         AdExpress [login 1]</a></li>
                                </ul>
                            </div>
                            <div id="listLoginAdscope" class="menulogins">
                                <ul>
                                    <li><a href="#"> AdScope [login 1]</a></li>
                                    <li><a href="#"> AdScope [login 2]</a></li>
                                    <li><a href="#"> AdScope [login 3]</a></li>
                                    <li><a href="#"> AdScope [login 4]</a></li>
                                    <li><a href="#"> AdScope [login 5]</a></li>
                                    <li><a href="#"> AdScope [login 6]</a></li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <!--Contenu HTML Marketing-->
            <td  width="1000" height="484" style="background-color:#ffffff;"><div class="contentmarketing"></div></td>
        </tr>
    </table>
    <input type =hidden name ="__loginChoiceEVENTTARGET" value =""/>
    <input type =hidden name ="__loginChoiceEVENTARGUMENT" value =""/>


</asp:Content>

