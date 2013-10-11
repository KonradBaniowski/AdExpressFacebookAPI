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
    <script src="/js/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="/js/jquery.validate.js" type="text/javascript"></script>
    <script src="/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="/js/jquery.ui.datepicker-fr.js" type="text/javascript"></script>
    <script src="/js/jquery.bxslider.min.js" type="text/javascript"></script>







    <script type="text/javascript">

        var fValidator;

        $(document).ready(function () {
            //debugger;

            $('#loaderdiv').show();
            // Getting URL var by its form id
            var promotionId = $.getUrlVar('promotionId');

            //Get codification form                       
            $.ajax({
                type: "POST",
                async: false,
                data: "{idDataPromotion:" + promotionId + "}",
                url: "Edit.aspx/GetCodification",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    //debugger;
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

                    if (msg.d.Advert.PromotionBrand)
                        $("#pbrand").val(msg.d.Advert.PromotionBrand);

                    if (msg.d.Advert.PromotionContent)
                        $("#pcontent").val(msg.d.Advert.PromotionContent);

                    if (msg.d.Advert.ConditionText)
                        $("#conditiontext").val(msg.d.Advert.ConditionText);

                    if (msg.d.Advert.Script)
                        $("#script_").val(msg.d.Advert.Script);

                    if (msg.d.Advert.DateEndNum && msg.d.Advert.DateEndNum > 0) {
                        var endDate = msg.d.Advert.DateEndNum.toString();
                        endDate = endDate.substr(6, 2) + '/' + endDate.substr(4, 2) + '/' + endDate.substr(0, 4);
                        $("#enddatepicker").val(endDate);
                    }

                    if (msg.d.Advert.DateBeginNum && msg.d.Advert.DateBeginNum > 0) {
                        var datebegin = msg.d.Advert.DateBeginNum.toString();
                        datebegin = datebegin.substr(6, 2) + '/' + datebegin.substr(4, 2) + '/' + datebegin.substr(0, 4);
                        $("#startdatepicker").val(datebegin);
                    }

                    if (msg.d.Advert.ExcluWeb == 1)
                        $("#excluweb_ok").prop('checked', true);
                    else
                        $("#excluweb_ok").prop('checked', false);

                    if (msg.d.Advert.ExcluWeb == 0)
                        $("#excluweb_ko").prop('checked', true);
                    else
                        $("#excluweb_ko").prop('checked', false);

                    if (msg.d.Advert.National == 1)
                        $("#national_ok").prop('checked', true);
                    else
                        $("#national_ok").prop('checked', false);

                    if (msg.d.Advert.National == 0)
                        $("#national_ko").prop('checked', true);
                    else
                        $("#national_ko").prop('checked', false);

                    //Set Visuals
                    LoadVisuals(msg);

                },
                error: function () {
                    alert("Impossible de charger les données de la promotion");

                }


            });
            //Set Data picker
            $("#startdatepicker").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                numberOfMonths: 1,
                regional: "fr",
                showOn: "button",
                buttonImage: "/App_Themes/PromoPSAFr/Images/calendar.gif",
                buttonImageOnly: true,
                beforeShow: function () {
                    $(".ui-datepicker").css('font-size', 11);
                },
                onclose: function (selectedDate) {
                    $("#enddatepicker").datepicker("option", "minDate", selectedDate);
                }
            });


            $("#enddatepicker").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                numberOfMonths: 1,
                regional: "fr",
                showOn: "button",
                buttonImage: "/App_Themes/PromoPSAFr/Images/calendar.gif",
                buttonImageOnly: true,
                beforeShow: function () {
                    $(".ui-datepicker").css('font-size', 11);
                }
                ,
                onclose: function (selectedDate) {
                    $("#startdatepicker").datepicker("option", "maxDate", selectedDate);
                }

            });



            // validate the comment form when it is submitted            

            CodificationEvent("#codifify", 90, 0);

            CodificationEvent("#codifyAndDuplicate", 90, 1);

            RejectFormEvent("#rejectcodif", 60);

            CancelForm("#cancelcodif");

            PendingFormEvent("#pendingcodif",70);

            $(function () {
                $("#dialog").dialog({
                    autoOpen: false,
                    dialogClass: "alert",
                    modal: true,
                    show: {
                        effect: "highlight",
                        duration: 1000
                    },
                    hide: {
                        effect: "fade",
                        duration: 1000
                    }
                });
            });



            $('#loaderdiv').hide();




        });

        $.validator.addMethod("dateComparison", function (value, element) {
            
            var result = false;
            var sp = $("#startdatepicker").val();
            var ep = $("#enddatepicker").val();
            
            if (!IsNullOrEmpty(sp) && !IsNullOrEmpty(ep)) {
                var dateArray = sp.split("/");
                var startDateObj = dateArray[2] + '' + dateArray[1] + '' + dateArray[0];
                var startDateVal = parseInt(startDateObj);

                var endDateArray = ep.split("/");
                var endDateObj = endDateArray[2] + '' + endDateArray[1] + '' + endDateArray[0];
                var endDateVal = parseInt(endDateObj);

                if (endDateVal >= startDateVal) {
                    result = true;
                }
            }
              

            return result;

        }, " La date de début doit être inférieure ou égale à la date de fin");

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
        
        function IsNullOrEmpty(value) {
            return (value == null || value === "");
        }

        function CancelForm(selectorId) {
            $(selectorId).off("click");
            $(selectorId).on("click", function () {
               
                $('#loaderdiv').show();
                var loginId = $.getUrlVar('loginId');

                if (fValidator) {
                    fValidator.resetForm();

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: "Edit.aspx/ReleaseUser",
                        data: "{loginId:" + loginId + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            document.location.href = "/Private/Home.aspx";
                        },
                        error: function () {
                            $('#loaderdiv').hide();
                            alert("Erreur! Impossible de retourner à la page de sélection des fiches.");
                        }
                    });

                } else {
                    $("#commentForm").validate({
                        rules: {
                            startdatepicker: {
                                required: false
                            },
                            enddatepicker: {
                                required: false
                            },
                            segment: {
                                required: false
                            },
                            product: {
                                required: false
                            },
                            brand: {
                                required: false
                            }
                        },
                        submitHandler: function () {

                           


                            $.ajax({
                                type: "POST",
                                async: false,
                                url: "Edit.aspx/ReleaseUser",
                                data: "{loginId:" + loginId + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    document.location.href = "/Private/Home.aspx";
                                },
                                error: function () {
                                    $('#loaderdiv').hide();
                                    alert("Erreur! Impossible de retourner à la page de sélection des fiches.");
                                }
                            });
                            //      //form.submit();
                        }
                    });


                }






            });
        }

        function GetProducts(segmentId) {
            if (segmentId.length > 0) {
                $("#product").get(0).options.length = 0;
                $("#product").get(0).options[0] = new Option("Chargement des Types de pièce", "");
                $('#loaderdiv').show();
                $.ajax({
                    type: "POST",
                    async: true,
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
                        $('#loaderdiv').hide();
                    },
                    error: function () {
                        $("#product").get(0).options.length = 0;
                        $('#loaderdiv').hide();
                        alert("Echec du chargement des types de pièce");
                    }
                });
            }
            else {
                $("#product").get(0).options.length = 0;
            }
        }

        function PendingFormEvent(selectorId, activationCode) {
            $(selectorId).off("click");
            $(selectorId).on("click", function () {

                $('#loaderdiv').show();
                var promotionId = $.getUrlVar('promotionId');
                var loginId = $.getUrlVar('loginId');

                if (fValidator) {

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: "Edit.aspx/PendingForm",
                        data: "{promotionId:" + promotionId + ",loginId:" + loginId + ",activationCode:" + activationCode + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            if (msg.d > 0) {
                                document.location.href = "/Private/Edit.aspx?promotionId=" + msg.d + '&loginId=' + loginId;
                            } else {
                                document.location.href = "/Private/Home.aspx";
                            }

                        },
                        error: function () {
                            $('#loaderdiv').hide();
                            alert("Erreur lors de la mise en litige de la fiche");
                        }
                    });

                } else {
                    $("#commentForm").validate({
                        rules: {
                            startdatepicker: {
                                required: false
                            },
                            enddatepicker: {
                                required: false
                            },
                            segment: {
                                required: false
                            },
                            product: {
                                required: false
                            },
                            brand: {
                                required: false
                            }
                        },
                        submitHandler: function (form) {



                            $.ajax({
                                type: "POST",
                                async: false,
                                url: "Edit.aspx/PendingForm",
                                data: "{promotionId:" + promotionId + ",loginId:" + loginId + ",activationCode:" + activationCode + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    if (msg.d > 0) {
                                        document.location.href = "/Private/Edit.aspx?promotionId=" + msg.d + '&loginId=' + loginId;
                                    } else {
                                        document.location.href = "/Private/Home.aspx";
                                    }

                                },
                                error: function () {
                                    $('#loaderdiv').hide();
                                    alert("Erreur lors de la mise en litige de la fiche");
                                }
                            });

                        }
                    });

                }



            });
        }
        function RejectFormEvent(selectorId, activationCode) {
            $(selectorId).off("click");
            $(selectorId).on("click", function () {
                
                $('#loaderdiv').show();
                var promotionId = $.getUrlVar('promotionId');
                var loginId = $.getUrlVar('loginId');

                if (fValidator) {

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: "Edit.aspx/RejectForm",
                        data: "{promotionId:" + promotionId + ",loginId:" + loginId + ",activationCode:" + activationCode + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            if (msg.d > 0) {
                                document.location.href = "/Private/Edit.aspx?promotionId=" + msg.d + '&loginId=' + loginId;
                            } else {
                                document.location.href = "/Private/Home.aspx";
                            }

                        },
                        error: function () {
                            $('#loaderdiv').hide();
                            alert("Erreur lors du rejet de la fiche");
                        }
                    });

                } else {
                    $("#commentForm").validate({
                        rules: {
                            startdatepicker: {
                                required: false
                            },
                            enddatepicker: {
                                required: false
                            },
                            segment: {
                                required: false
                            },
                            product: {
                                required: false
                            },
                            brand: {
                                required: false
                            }
                        },
                        submitHandler: function (form) {

                          

                            $.ajax({
                                type: "POST",
                                async: false,
                                url: "Edit.aspx/RejectForm",
                                data: "{promotionId:" + promotionId + ",loginId:" + loginId + ",activationCode:" + activationCode + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    if (msg.d > 0) {
                                        document.location.href = "/Private/Edit.aspx?promotionId=" + msg.d + '&loginId=' + loginId;
                                    } else {
                                        document.location.href = "/Private/Home.aspx";
                                    }

                                },
                                error: function () {
                                    $('#loaderdiv').hide();
                                    alert("Erreur lors du rejet de la fiche");
                                }
                            });
                         
                        }
                    });

                }



            });
        }

        function CodificationEvent(selectorId, activationCode, multiPromo) {

            $(selectorId).off("click");
            $(selectorId).on("click", function () {
               

                //Hide errors essage
                fValidator = $("#commentForm").validate({
                    rules: {
                        startdatepicker: {
                            required: true,
                            //date: true,
                            dateComparison: true
                        },
                        enddatepicker: {
                            required: true,
                           // date: true,
                            dateComparison: true
                        }
                    },

                    errorContainer: $('#errorContainer'),
                    errorLabelContainer: $('#errorContainer ul'),
                    wrapper: 'li',


                    messages: {
                        startdatepicker: {
                            required: 'Sélectionnez une date de début',
                        },
                        enddatepicker: {
                            required: 'Sélectionnez une date de fin!',
                        },
                        segment: {
                            required: 'Sélectionnez un segment',
                        },
                        product: {
                            required: 'Sélectionnez un type de pièce',
                        },
                        brand: {
                            required: 'Sélectionnez une enseigne',
                        },
                        pbrand: {
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
                        //debugger;
                        var promotionId = $.getUrlVar('promotionId');
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
                        var national = $("input[name=national]:checked").val();
                        var promotionBrand = $("#pbrand").val();
                        var advertData = {
                            'advert': {
                                'IdDataPromotion': promotionId,
                                'IdForm': formId,
                                'Activation': activationCode,
                                'IdProduct': idProduct,
                                'IdBrand': idBrand,
                                'DateBeginNum': dateBeginNum,
                                'DateEndNum': dateEndNum,
                                'IdSegment': idSegment,
                                'PromotionBrand': promotionBrand,
                                'PromotionContent': promotionContent,
                                'ConditionText': conditionText,
                                'Script': script_,
                                'ExcluWeb': excluWeb,
                                'National': national
                            },
                            'loginId': loginId,
                            'multiPromo': multiPromo
                        };
                        var mokdata = JSON.stringify(advertData);
                        
                        $('#loaderdiv').show();
                        $.ajax({
                            type: "POST",
                            async: true,
                            url: "Edit.aspx/SaveCodification",
                            data: mokdata,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {
                                if (msg.d > 0) {
                                    document.location.href = "/Private/Edit.aspx?promotionId=" + msg.d + '&loginId=' + loginId;
                                } else {
                                    document.location.href = "/Private/Home.aspx";
                                }

                            },
                            error: function () {
                                $("#product").get(0).options.length = 0;
                                $('#loaderdiv').hide();
                                alert("Erreur lors sauvegarde la fiche");
                            }
                        });
                        
                    }
                });




            });
        }


        //If validation errors are occured in any field, it will display field name with link, clicking on link it will set focus of perticular field.
        function setFocus(ele) {
            $(ele).focus();
        }

        function LoadVideo(videofile, year,isZoom) {
            var filePath = '/VideoPsa/' + year + '/' + videofile;
            var vids = ' <object type=\"video/mpeg\" data=\"' + filePath + '\" width=\"100%\" ';
            vids += (isZoom) ? 'height=\"800\">' : 'height=\"500\">';
            vids += '  <param name=\"src\" value=\"' + filePath + '\" />';
            vids += ' <param name=\"autoplay\" value=\"false\" />';
            vids += ' <param name=\"autoStart\" value=\"0\" />';
            vids += '<embed src=\"' + filePath + '\"  width=\"100%\"  ';
            vids += (isZoom) ? 'height=\"800\" ' : 'height=\"500\" ';
            vids += ' autoplay=\"false\" controller=\"true\" ';
            vids += ' pluginspage=\"http://www.apple.com/quicktime/download/\">';
            vids += '</object>';//alt :
            if (!isZoom)
                vids += ' <br/><img class=\"lvid\" src=\"/App_Themes/PromoPSAFr/Images/zoom.png\" onclick=\"LoadVideo(\'' + videofile + '\',' + year + ',true)\">';

            if (isZoom) {
                $('.modalBody').html(vids);
                revealModal('modalPage', videofile, 3);
            } else {
                $('.slider').hide();
                $('.containingBlock').html(vids);

            }
         
           
        }

        function LoadBanner(bannerFile,isZoom) {
            var filePath = '/BannersPsa';
            filePath += '/' + bannerFile.substr(0, 1) + '/' + bannerFile.substr(1, 3);
            filePath += '/' + bannerFile;

            // Flash banner
            var banners = ' <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"100%\" height=\"800\">';//height=\"100%\"
          
            banners += ' <PARAM name=\"movie\" value=\"' + filePath + '\">';
            banners += '  <PARAM name=\"play\" value=\"true\">';
            banners += ' <PARAM name=\"quality\" value=\"high\">';
            banners += ' <EMBED src=\"' + filePath + '\" play=\"true\" swliveconnect=\"true\" quality=\"high\" height=\"800\" width=\"100%\" scale=\"default\">';//height=\"100%\" 
            banners += ' <param name=\"allowfullscreen\" value=\"true\" />';
            banners += '<param name=\"scale\" value=\"default\" />';
            banners += ' </OBJECT>';
            if (!isZoom)
                banners += ' <br/><img class=\"lvid\" src=\"/App_Themes/PromoPSAFr/Images/zoom.png\" onclick=\"LoadBanner(\'' + bannerFile + '\',true)\">';
            
        
            if (isZoom) {
                $('.modalBody').html(banners);
                revealModal('modalPage', bannerFile, 3);
            } else {
                $('.slider').hide();
                $('.containingBlock').html(banners);

            }
        }

        function LoadFlv(flvFile, isZoom) {
            var filePath = '/BannersPsa';
            filePath += '/' + flvFile.substr(0, 1) + '/' + flvFile.substr(1, 3);
            filePath += '/' + flvFile;

            var flv = '<object id=\"video_"promo\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" ';
            flv += (isZoom) ? ' width=\"100%\"  height=\"800\">' : ' width=\"400\" height=\"315\">';// height=\"100%\"
            flv += '<param name=\"movie\" value=\"/Player/playerflv.swf\" />';
            flv += '<param name=\"allowfullscreen\" value=\"true\" />';
            flv += '<param name=\"allowscriptaccess\" value=\"always\" />';
            flv += '<param name=\"flashvars\" value=\"file=' + filePath + '\" />';
            flv += '<embed type=\"application/x-shockwave-flash\"';
            flv += 'src=\"/Player/playerflv.swf\" ';
            flv += (isZoom) ? ' width=\"100%\"  height=\"800\" ' : ' width=\"400\" height=\"315\" ';//height=\"100%\"
            flv += 'allowscriptaccess=\"always\" ';
            flv += 'allowfullscreen=\"false\"';
            flv += 'flashvars=\"file=' + filePath + '\" ';
            flv += '/>';
            flv += '</object>';
            if (!isZoom)
                flv += ' <br/><img class=\"lvid\" src=\"/App_Themes/PromoPSAFr/Images/zoom.png\" onclick=\"LoadFlv(\'' + flvFile + '\',true)\">';


            if (isZoom) {
                $('.modalBody').html(flv);
                revealModal('modalPage', flvFile, 3);
            } else {
                $('.slider').hide();
                $('.containingBlock').html(flv);

            }
        }

        function LoadVisuals(msg) {



            if (msg.d.Advert && msg.d.Advert.PromotionVisual) {


                //Set IMAGES
                if (msg.d.Advert.IdVehicle == 1 || msg.d.Advert.IdVehicle == 8
                    || msg.d.Advert.IdVehicle == 10
                    || (msg.d.Advert.IdVehicle == 7 && msg.d.Advert.PromotionVisual.indexOf("SWF") < 0
                        && msg.d.Advert.PromotionVisual.indexOf("FLV") < 0)) {


                    var pVisuals = msg.d.Advert.PromotionVisual.split(',');
                    var pLi = '';                  
                    for (var i = 0; i < pVisuals.length; i++) {
                        pLi += '<li> ';
                        pLi +=  (msg.d.Advert.IdVehicle == 7) ? '<img src=\"/BannersPsa' : ' <img src=\"/imagesPsaPress';
                        pLi += '/' + pVisuals[i].substr(0, 1) + '/' + pVisuals[i].substr(1, 3);
                        pLi += '/' + pVisuals[i];
                        pLi += '\"  width=\"100%\" ';
                        pLi +=  ' onclick=\"revealModal(\'modalPage\',\'' + pVisuals[i] + '\','+msg.d.Advert.IdVehicle+')\" /></li>';                    
                        

                    }
                    $('.containingBlock').hide();
                    $('.bxslider').html(pLi);
                    $('.bxslider').bxSlider({
                        infiniteLoop: false,
                        hideControlOnEnd: true,
                        adaptiveHeight: false,
                        mode: 'fade',
                        //slideWidth: 800,
                        captions: true
                    });
                }
                    //Set vidéo
                else if (msg.d.Advert.IdVehicle == 3)
                    LoadVideo(msg.d.Advert.PromotionVisual.replace("mp4", "mpg").replace("MP4", "MPG"),
                        msg.d.Advert.LoadDate.toString().substr(0, 4),false);

                    //Set banner
                else if (msg.d.Advert.IdVehicle == 7 && msg.d.Advert.PromotionVisual.indexOf("SWF") > 0)
                    LoadBanner(msg.d.Advert.PromotionVisual, false);
                    //Set FLV
                else if (msg.d.Advert.IdVehicle == 7 && msg.d.Advert.PromotionVisual.indexOf("FLV") > 0)
                    LoadFlv(msg.d.Advert.PromotionVisual);


            } else {
                $('.containingBlock').html("<table  style =\"width: 400px; height: 400px; border: 1px solid #525252;text-align: center; font-size: 16px; font-weight: bold;\"><tr> <td>Aucune création</td></tr></table>");
            }



        }

        function revealModal(divID, visual, idVehicle) {
          
            if (idVehicle == 7 || idVehicle == 1 || idVehicle == 8 || idVehicle == 10) {
                var img = (idVehicle == 7) ? '<img src=\"/BannersPsa' : ' <img src=\"/ImagesPsaHR';
                img += '/' + visual.substr(0, 1) + '/' + visual.substr(1, 3);
                img += '/' + visual;
                img += '\"  width=\"100%\" height=\"100%\" />';
                $('.modalBody').html(img);
            }
         
         
            window.onscroll = function() {
                 document.getElementById(divID).style.top = document.body.scrollTop;
            };
            document.getElementById(divID).style.display = "block";
            document.getElementById(divID).style.top = document.body.scrollTop;
            $('.containingBlock').hide();
            $('.slider').hide();
        }

        function hideModal(divID) {
            document.getElementById(divID).style.display = "none";
            $('.containingBlock').show();
            $('.slider').show();
        }

    </script>

    <style type="text/css">
        #containingBlock
        {
            width: 100%;
            height: 100%;
        }

        .videoWrapper
        {
            position: relative;
            padding-bottom: 56.25%;
            padding-top: 25px;
            height: 0;
        }

            .videoWrapper object,
            .videoWrapper embed
            {
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
            }

        #errorContainer
        {
            display: none;
            overflow: auto;
            background-color: #FFDDDD;
            border: 1px solid #FF2323;
            padding-top: 0;
            font-size: 11px;
        }

            #errorContainer label
            {
                float: none;
                width: auto;
            }

        #loaderdiv
        {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 1000;
            background-color: grey;
            opacity: .8;
        }

        .ajax-loader
        {
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -32px; /* -1 * image width / 2 */
            margin-top: -32px; /* -1 * image height / 2 */
            display: block;
        }

        .bxslider
        {
            padding: 0;
        }

        .nocreation
        {
            width: 400px;
            height: 400px;
            border: 1px solid #525252;
            text-align: center;
            font-size: 16px;
            font-weight: bold;
        }

        .cofs
        {
            margin: 0;
            border-spacing: 0px;
        }

        .lvid
        {
            font-size: 12px;
            font-weight: bold;
            color: #525252;
        }
        /* This is the CSS for the normal div */
        .normal
        {
            background-color: lightblue;
            width: 900px;
            min-height: 200px;
            padding: 20px;
        }

        /* This is the CSS for the modal dialog window */
        #modalPage
        {
            display: none;
            position: absolute;
            width: 100%;
            height: 100%;
            top: 0px;
            left: 0px;
        }

        .modalBackground
        {
            filter: Alpha(Opacity=60);
            -moz-opacity: 0.6;
            opacity: 0.6;
            width: 100%;
            height: 100%;
            background-color: #999999;
            position: absolute;
            z-index: 500;
            top: 0px;
            left: 0px;
        }

        .modalContainer
        {
            position: absolute;
            width: 800px;
             /*height: 800px;*/                   
            left: 15%;
            top: 20%;
            z-index: 1750;
        }

        .modal
        {
            background-color: white;
            border: solid 1px  #525252;
            position: relative;
            top: -150px;
            left: -150px;
            z-index: 2000;
            width: 800px;
            /*height: 600px;*/
            /*width: 100%;*/
            height: 70%;
            padding: 0px;
        }

        .modalTop
        {
            width: 792px;
            background-color:  #525252;
            padding: 4px;
            color: #ffffff;
            text-align: right;
        }

            .modalTop a, .modalTop a:visited
            {
                color: #ffffff;
            }

        .modalBody
        {
            padding: 10px;
           
        }
    </style>
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
        <div id="errorContainer">
            <p><b>Corrigez les erreurs suivantes s'il vous plait, puis réessayez :</b></p>
            <ul />
        </div>
        <table class="tadc">
            <tr>
                <td style="vertical-align: top; width: 50%;">

                    <fieldset class="cofs">

                        <!-- Debut table global -->
                        <legend>Codification</legend>

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
                                        <label for="pbrand">Marque</label>
                                    </td>
                                    <td class="field">
                                        <textarea id="pbrand" name="pbrand" rows="5" cols="50"></textarea>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label">
                                        <label for="pcontent">Contenu de la promotion </label>
                                    </td>
                                    <td class="field">
                                        <textarea id="pcontent" name="pcontent" rows="8" cols="50"></textarea>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label">
                                        <label for="startdatepicker">Date de début *</label>
                                    </td>
                                    <td class="field">
                                        <input type="text" id="startdatepicker" name="startdatepicker" readonly="readonly"/>



                                    </td>
                                </tr>
                                <tr>
                                    <td class="label">
                                        <label for="enddatepicker">Date de fin *</label>
                                    </td>
                                    <td class="field">
                                        <input type="text" id="enddatepicker" name="enddatepicker" readonly="readonly"/>

                                    </td>
                                </tr>

                                <tr>
                                    <td class="label">
                                        <label for="conditiontext">Condition texte</label>
                                    </td>
                                    <td class="field">
                                        <textarea id="conditiontext" name="conditiontext" rows="8" cols="50"></textarea>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="label">
                                        <label for="script_">Script </label>
                                    </td>
                                    <td class="field">
                                        <textarea id="script_" name="script_" rows="12" cols="50"></textarea>
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
                                    <td class="label">
                                        <label for="nat">National *</label>
                                    </td>
                                    <td class="field">
                                        <label for="national_ok">
                                            <input type="radio" id="national_ok" value="1" name="national" checked="checked"/>
                                            Oui
                                        </label>
                                        <label for="national_ko">
                                            <input type="radio" id="national_ko" value="0" name="national"/>
                                            Non		
                                        </label>
                                    </td>
                                </tr>

                                <tr>
                                    <td></td>
                                    <td>
                                        <div class="vButton">

                                            <input class="validateButton" type="submit" id="codifify" name="codifify" value="Codifier" style="width: 80px;" tabindex="14" />
                                            <span></span>
                                            <input class="validateButton" type="submit" id="rejectcodif" name="rejectcodif" value="Rejeter" style="width: 80px;" tabindex="15" />
                                            <span></span>
                                            <input class="validateButton" type="submit" id="cancelcodif" name="cancelcodif" value="Annuler" style="width: 80px;" tabindex="16" />
                                            <span></span>
                                            <input class="validateButton" type="submit" id="pendingcodif" name="pendingcodif" value="Litige" style="width: 80px;" tabindex="17" />
                                            <span></span>
                                            <input class="duplicateButton" type="submit" id="codifyAndDuplicate" name="codifyAndDuplicate" value="Codifier et dupliquer" style="width: 160px;" tabindex="18" />
                                        </div>

                                    </td>
                                </tr>

                            </table>

                        </div>
                 
                    </fieldset>
                </td>
                <!-- fin Td de la saisie-->
                <td style="vertical-align: top; width: 50%; height: 100%;">
                    <fieldset class="cofs">



                        <legend id="legend2">Création</legend>
                        <div class="slider">
                            <ul class="bxslider">
                            </ul>
                        </div>
                        <!---DEFINITION vidéo-->
                      
                        <div class="containingBlock">
                        </div>

                        <!---Fin définition vidéo-->

                    </fieldset>

                </td>
                <!-- Fin Td images-->
                <!-- Fin table global -->
            </tr>
        </table>
        <!--test pop up modal-->

        <div id="modalPage">
            <div class="modalBackground" onclick="javascript:hideModal('modalPage');">
            </div>
            <div class="modalContainer">
                <div class="modal">
                    <div class="modalTop"><a href="javascript:hideModal('modalPage')">[X]</a></div>
                    <div class="modalBody">                       
                      
                    </div>
                </div>
            </div>
        </div>


        <!-- fin test pop up-->
        <!-- Debut Td images-->
        <div id="loaderdiv">
            <img src="/App_Themes/PromoPSAFr/Images/bx_loader.gif" class="ajax-loader" />
        </div>
        
       
    </form>



</body>
</html>
