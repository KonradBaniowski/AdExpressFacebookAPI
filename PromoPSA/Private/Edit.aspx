<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Private_Edit" %>

<%@ Register TagPrefix="cc1" Namespace="KMI.PromoPSA.Web.Controls.Header" Assembly="KMI.PromoPSA.Web.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta content="no-cache" name="Cache-control" />
    <script src="/js/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="/js/jquery.validate.js" type="text/javascript"></script>
    <script src="/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="/js/jquery.ui.datepicker-fr.js" type="text/javascript"></script>
    <script type="text/javascript">

        //$.validator.setDefaults({


        //    submitHandler: function(form) {


        //        debugger;
        //        form.submit();
        //    }


        //});


        $(document).ready(function () {
            debugger;
            // Getting URL var by its form id
            var formId = $.getUrlVar('formId');




            //Get codification form                       
            $.ajax({
                type: "POST",
                async: false,
                data: "{idForm:" + formId + "}",
                url: "Edit.aspx/GetCodification",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    debugger;
                    $("#fileNumber").text(msg.d.Advert.IdForm);

                    $("#segment").get(0).options.length = 0;
                    $("#segment").get(0).options[0] = new Option("Sélectionnez un segment", "");

                    $.each(msg.d.Segments, function (index, item) {
                        $("#segment").get(0).options[$("#segment").get(0).options.length] = new Option(item.Label, item.Id);
                        if (msg.d.CurrentSegment > 0 && item.Id == msg.d.CurrentSegment) {
                            $("#segment").get(0).options[$("#segment").get(0).options.length - 1].selected = true;
                        }
                    });
                    $("#segment").bind("change", function () {
                        GetProducts($(this).val());
                    });

                    if (msg.d.CurrentSegment > 0) {
                        $("#product").get(0).options.length = 0;
                        $("#product").get(0).options[0] = new Option("Sélectionnez un type de pièce", "");

                        $.each(msg.d.CurrentProducts, function (index, item) {
                            $("#product").get(0).options[$("#product").get(0).options.length] = new Option(item.Label, item.Id);
                            if (msg.d.CurrentProduct > 0 && item.Id == msg.d.CurrentProduct) {
                                $("#product").get(0).options[$("#product").get(0).options.length - 1].selected = true;
                            }
                        });
                    }

                    $("#brand").get(0).options.length = 0;
                    $("#brand").get(0).options[0] = new Option("Sélectionnez une enseigne", "");

                    $.each(msg.d.Brands, function (index, item) {
                        $("#brand").get(0).options[$("#brand").get(0).options.length] = new Option(item.Label, item.Id);
                        if (msg.d.CurrentBrand > 0 && item.Id == msg.d.CurrentBrand) {
                            $("#brand").get(0).options[$("#brand").get(0).options.length - 1].selected = true;
                        }
                    });
                },
                error: function () {
                    alert("Impossible de charger les données de la promotion");
                }
            });
            //Set Data picker
            $("#startdatepicker").datepicker({
                regional: "fr",
                showOn: "button",
                buttonImage: "/App_Themes/PromoPSAFr/Images/calendar.gif",
                buttonImageOnly: true,
                beforeShow: function () {
                    $(".ui-datepicker").css('font-size', 11);
                }
            });

            $("#datepicker").datepicker("option", $.datepicker.regional["fr"]);

            $("#enddatepicker").datepicker({
                regional: "fr",
                showOn: "button",
                buttonImage: "/App_Themes/PromoPSAFr/Images/calendar.gif",
                buttonImageOnly: true,
                beforeShow: function () {
                    $(".ui-datepicker").css('font-size', 11);
                }

            });

            $("#enddatepicker").datepicker("option", $.datepicker.regional["fr"]);

            // validate the comment form when it is submitted
            //$("#commentForm").validate();


            CodificationEvent("#codifify",20);

            CodificationEvent("#rejectcodif", 30);

        });


        $.extend({
            getUrlVars: function () {
                var vars = [], hash;
                var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
                for (var i = 0; i < hashes.length; i++) {
                    hash = hashes[i].split('=');
                    vars.push(hash[0]);
                    vars[hash[0]] = hash[1];
                }
                return vars;
            },
            getUrlVar: function (name) {
                return $.getUrlVars()[name];
            }
        });
    
        function GetProducts(segmentId) {
            if (segmentId.length > 0) {
                $("#product").get(0).options.length = 0;
                $("#product").get(0).options[0] = new Option("Chargement des Types de pièce", "");

                $.ajax({
                    type: "POST",
                    async: false,
                    url: "Edit.aspx/GetProductsBySegment",
                    data: "{segmentId:" + segmentId + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        $("#product").get(0).options.length = 0;
                        $("#product").get(0).options[0] = new Option("Sélectionnez un type de pièce", "");

                        $.each(msg.d, function (index, item) {
                            $("#product").get(0).options[$("#product").get(0).options.length] = new Option(item.Label, item.Id);
                        });
                    },
                    error: function () {
                        $("#product").get(0).options.length = 0;
                        alert("Echec du chargement des types de pièce");
                    }
                });
            }
            else {
                $("#product").get(0).options.length = 0;
            }
        }

        function CodificationEvent(selectorId, activationCode) {
          
            $(selectorId).off("click");
            $(selectorId).on("click", function () {
                debugger;
                //Hide errors essage
                $("#commentForm").validate({
                    messages: {
                        startdatepicker: {
                            required: '',
                        },
                        enddatepicker: {
                            required: '',
                        },
                        segment: {
                            required: '',
                        },
                        product: {
                            required: '',
                        },
                        brand: {
                            required: '',
                        },
                        pcontent: {
                            required: '',
                        },
                        conditiontext: {
                            required: '',
                        },
                        script_: {
                            required: '',
                        },
                    },
                    invalidHandler: function (event, validator) {
                        // 'this' refers to the form    
                        var errors = validator.numberOfInvalids();
                        if (errors) {
                            var message = errors == 1 ?
                                'Vous avez oublié un 1 champ. Il a été surligné' : 'Vous avez oublié ' + errors + ' champs. Ils ont été surlignés';
                            $("div.error span").html(message); $("div.error").show();
                        } else { $("div.error").hide(); }
                    },
                    submitHandler: function () {
                        var action;
                        debugger;
                        var formId = $.getUrlVar('formId');
                        var loginId = $.getUrlVar('loginId');
                        var idProduct = $("#product option:selected").val();
                        var idBrand = $("#brand option:selected").val();
                        var idSegment = $("#segment option:selected").val();
                        var startdates = $("#startdatepicker").val().split('/');
                        var dateBeginNum = startdates[2] + startdates[1] + startdates[0];
                        var enddates = $("#enddatepicker").val().split('/');
                        var dateEndNum = enddates[2] + enddates[1] + enddates[0];
                        var promotionContent = $("#pcontent").val();
                        var conditionText = $("#conditiontext").val();
                        var script_ = $("#script_").val();
                        var excluWeb = $("input[name=excluweb]:checked").val();
                        var advertData = {
                            'advert': {
                                'IdForm': formId,
                                'Activation': activationCode,
                                'IdProduct': idProduct,
                                'IdBrand': idBrand,
                                'DateBeginNum': dateBeginNum,
                                'DateEndNum': dateEndNum,
                                'IdSegment': idSegment,
                                'PromotionContent': promotionContent,
                                'ConditionText': conditionText,
                                'Script': script_,
                                'ExcluWeb': excluWeb
                            },
                            'loginId': loginId
                        };
                        var mokdata = JSON.stringify(advertData);
                        debugger;
                        $.ajax({
                            type: "POST",
                            async: false,
                            url: "Edit.aspx/SaveCodification",
                            data: mokdata,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {
                                document.location.href = "/Private/Edit.aspx?formId=4230429";
                            },
                            error: function () {
                                $("#product").get(0).options.length = 0;
                                alert("Erreur lors sauvegarde la fiche");
                            }
                        });
                        //      //form.submit();
                    }
                });

            });
        }
        
      

    </script>


</head>
<body class="bodyStyle">
    <form id="commentForm" class="cmxform" runat="server" method="post" action="">
        <div class="header">
            <div style="float: left;">
                <asp:Image ID="Image1" runat="server" SkinID="logo_tns_home" />
            </div>
        </div>
        <div class="promotionsInformationDiv">
            <table cellspacing="0" cellpadding="0" border="0" height="100%">
                <tr>
                    <td>
                        <cc1:DisconnectUserWebControl runat="server" ID="DisconnectUserWebControl1" SkinID="DisconnectUserWebControl" />
                    </td>
                    <td>
                        <cc1:LoginInformationWebControl runat="server" ID="LoginInformationWebControl1" SkinID="LoginInformationWebControl" />
                    </td>
                    <td>
                        <cc1:PromotionInformationWebControl runat="server" ID="PromotionInformationWebControl1" SkinID="PromotionInformationWebControl" />
                    </td>
                </tr>
            </table>
        </div>


        <p>
        </p>
        <br />
        <fieldset>
            <legend>Codification</legend>
            <div class="error" style="display: none;">
                <img src="/App_Themes/PromoPSAFr/Images/warning.gif" alt="Warning!" width="16" height="16" style="float: left; margin: -5px 10px 0px 0px;" />

                <span></span>.
            </div>
            <div>
                <table class="tadc">

                    <tr>

                        <td class="label">
                            <label id="fileLabel">Numéro de fiche</label>
                        </td>
                        <td class="label">
                            <label id="fileNumber">0 </label>
                        </td>
                    </tr>
                    <tr>

                        <td class="label">
                            <label for="segment">Segment * </label>
                        </td>
                        <td class="field">
                            <select id="segment" class="required" name="segment" title="Sélectionnez un segment!">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label for="product">Type de pièce *</label>
                        </td>
                        <td class="field">
                            <select id="product" class="required" name="product" title="Sélectionnez Type de pièce!">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label for="brand">Enseigne *</label>
                        </td>
                        <td class="field">
                            <select id="brand" class="required" name="brand" title="Sélectionnez une enseigne!">
                            </select>
                        </td>
                    </tr>

                    <tr>
                        <td class="label">
                            <label for="pcontent">Contenu de la promotion *</label>
                        </td>
                        <td class="field">
                            <textarea class="required" id="pcontent" name="pcontent" title="Ce champ est obligatoire!"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label for="startdatepicker">Date de début *</label>
                        </td>
                        <td class="field">
                            <input type="text" class="required" id="startdatepicker" name="startdatepicker" />



                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label for="enddatepicker">Date de fin *</label>
                        </td>
                        <td class="field">
                            <input type="text" class="required" id="enddatepicker" name="enddatepicker" />



                        </td>
                    </tr>

                    <tr>
                        <td class="label">
                            <label for="conditiontext">Condition texte *</label>
                        </td>
                        <td class="field">
                            <textarea class="required" id="conditiontext" name="conditiontext" title="Ce champ est obligatoire!"></textarea>
                        </td>
                    </tr>

                    <tr>
                        <td class="label">
                            <label for="script_">Script *</label>
                        </td>
                        <td class="field">
                            <textarea class="required" id="script_" name="script_" title="Ce champ est obligatoire!"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label for="exclu">Exclu Web *</label>
                        </td>
                        <td class="field">
                            <label for="excluweb_ok">
                                <input type="radio" id="excluweb_ok" value="1" name="excluweb" />
                                Oui
		
                            </label>
                            <label for="excluweb_ko">
                                <input type="radio" id="excluweb_ko" value="0" name="excluweb" checked="checked" />
                                Non		
                            </label>
                        </td>
                    </tr>

                    <tr>
                        <td></td>
                        <td>
                            <div>
                                <span></span>
                                <input class="validateButton" type="submit" id="codifify" name="codifify" value="Codifier" style="width: 100px" tabindex="14" />
                                <span></span>
                                <input class="validateButton" type="submit" id="rejectcodif" name="rejectcodif" value="Rejeter" style="width: 100px" tabindex="15" />
                                <%-- <span></span>
                                   <input class="validateButton" type="submit" id="cancelcodif" name ="cancelcodif" value="Annuler" style="width: 100px" tabindex="16" />--%>
                            </div>

                        </td>
                    </tr>

                </table>


            </div>
        </fieldset>
    </form>



</body>
</html>
