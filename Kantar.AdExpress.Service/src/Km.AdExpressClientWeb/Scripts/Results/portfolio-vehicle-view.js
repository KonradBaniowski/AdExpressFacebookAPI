﻿function ShowVehicleCarousel(vhCarouselData, labelNbInsertion, labelTotalInvest) {
    $("#carou-vehicleView").hide();

    var htmlArr = [];


    $(".carousel-inner").empty();
    var colNb = 0;
    $.each(vhCarouselData, function (i, val) {

        colNb++;

        if (colNb == 1) {
            if (i == 0) {

                htmlArr.push("<div class='item active'><div class='row'> ");
            }
            else htmlArr.push("<div class='item'><div class='row'>");
        }

        htmlArr.push(" <div class='col-sm-3'>");

        htmlArr.push("<div class='text-center text-white'><div class='bg-black-only padder-v-mi'>");
        htmlArr.push(val.ParutionDate);
        htmlArr.push("</div></div>");


        //<!--text-center text-white-->
        htmlArr.push(" <div class='row no-gutter'><div class='col-xs-12' href='#modal-vehicleView' data-toggle='modal' ");
        //data-dayN
        htmlArr.push(" data-dayN='");
        htmlArr.push(val.DayN);
        htmlArr.push("'");
        //data-mId
        htmlArr.push(" data-mId='");
        htmlArr.push(val.Id);
        htmlArr.push("'");
        //data-nbInser
        htmlArr.push(" data-nbInser='");
        htmlArr.push(val.NbInser);
        htmlArr.push("'");
        htmlArr.push(" ><img src='");
        htmlArr.push(val.Src);
        htmlArr.push("' class='img-full' alt='Equipe'>");
        htmlArr.push("</div></div>");
        htmlArr.push(" <div class='text-center padder-v-mi'><p>");
        htmlArr.push(labelNbInsertion);
        htmlArr.push("<strong>");
        htmlArr.push(val.NbInser);
        htmlArr.push("</strong></p><p>");
        htmlArr.push(labelTotalInvest);
        htmlArr.push("<strong>");
        htmlArr.push(val.Invest);
        htmlArr.push("</strong></p>");
        htmlArr.push("</div> </div>");
        // <!--/text-center padder-v-mi  et /col-sm-3 -->

        if (colNb == 4) {
            colNb = 0;
            htmlArr.push("</div></div>");
        }
        // <!-- row et item-->

    });

    if (colNb > 0) {
        htmlArr.push("</div></div>");
        // <!--/col-sm-3 et row et item-->
    }
    $(".carousel-inner").html(htmlArr.join(""));
    $("#carou-vehicleView").show();
}

function OpenOneVehicleModalCarousel(vehicleItemsData, labelNbPages, labelNext, labelPrev) {
    $("#modal-vehicleView > .modal-dialog > .modal-content").empty();

    $("#modal-vehicleView").on('shown.bs.modal', function (event) {



        var htmlArr = [];
        var colNb = 0;
        $.each(vehicleItemsData, function (i, val) {

            if (i == 0) {
                //<!--modal-header -->
                htmlArr.push("<div class='modal-header panel-heading header-popup-vehicleView'>");
                htmlArr.push(" <button class='close' aria-hidden'true' type='button' data-dismiss='modal'><span aria-hidden='true'>&times;</span></button>");
                htmlArr.push(" <h4 class='modal-title font-normal m-t-none m-b-md text-primary-lt' id='myModalLabel'>");
                htmlArr.push("<i class='fa fa-file-text blue'></i>  ");
                htmlArr.push(val.Title);
                htmlArr.push(" - ");
                htmlArr.push(val.ParutionDate);
                htmlArr.push("</h4><p class='pull-right text-white'>");
                htmlArr.push(labelNbPages);
                htmlArr.push(val.NbPage);
                htmlArr.push(" </p>  </div>");
                //<!--/modal-header -->

                //<!--modal-body -->
                htmlArr.push(" <div class='modal-body'>");

                //<!--carou-vehicleView-OneVehicle-->
                htmlArr.push("  <div id='carou-vehicleView-OneVehicle' class='carousel'>");

                //<!--carousel-inner-->
                htmlArr.push("<div class='carousel-inner'>");
            }

            colNb++;

            // <!--item et row-->
            if (colNb == 1) {
                if (i == 0) {

                    htmlArr.push("<div class='item active'><div class='row'> ");
                }
                else htmlArr.push("<div class='item'><div class='row'>");
            }

            //  <!--col-md-4-->
            htmlArr.push("<div class='col-md-4 no-padder' href='#modal-vehicleView-Img' data-toggle='modal' ");
            htmlArr.push(" data-src-zoom='");
            htmlArr.push(val.SrcZoom);
            htmlArr.push("' data-parution-date='");
            htmlArr.push(val.ParutionDate);
            htmlArr.push("' data-title='");
            htmlArr.push(val.Title);
            htmlArr.push("'> <img class='img-full' alt='image 01' src='");
            htmlArr.push(val.Src);
            htmlArr.push("'> </div>");
            //  <!/--col-md-4-->

            if (colNb == 3) {
                colNb = 0;
                htmlArr.push("</div></div>");
                // <!--/item et row-->
            }
        });

        if (vehicleItemsData.length > 0) {

            if (colNb > 0) {
                htmlArr.push("</div></div>");
                // <!--/item et row-->
            }

            htmlArr.push("  </div>"); //<!--/carousel-inner-->

            //<!--left carousel-control-->
            htmlArr.push(" <a class='left carousel-control black' href='#carou-vehicleView-OneVehicle' role='button' data-slide='prev'>");
            htmlArr.push("   <span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span> <span class='sr-only'>");
            htmlArr.push(labelPrev);
            htmlArr.push("</span>  </a>");
            //<!--/left carousel-control-->

            //<!--right carousel-control-->
            htmlArr.push(" <a class='right carousel-control black' href='#carou-vehicleView-OneVehicle' role='button' data-slide='next'>");
            htmlArr.push("   <span class='glyphicon glyphicon-chevron-right' aria-hidden='true'></span> <span class='sr-only'>");
            htmlArr.push(labelNext);
            htmlArr.push("</span></a>");
            //<!--/right carousel-control-->

            htmlArr.push("  </div>"); //<!--/carou-vehicleView-OneVehicle-->
            htmlArr.push("  </div>");   //<!--/modal-body -->
        }

        $("#modal-vehicleView > .modal-dialog > .modal-content").html(htmlArr.join(""));

    });

    $("#modal-vehicleView").on('hide.bs.modal', function () {
        $("#modal-vehicleView > .modal-dialog > .modal-content").empty();
    });

}

function OpenZoomImageModal() {
    $("#modal-vehicleView-Img > .modal-dialog > .modal-content > .modal-body > .img-full").attr('src', '');

    $("#modal-vehicleView-Img").on('shown.bs.modal', function (event) {
        var button = $(event.relatedTarget);// Button that triggered the modal
        var srcZoom = button.data('src-zoom').toString(); // Extract info from data-* attributes
        var pDate = button.data('parution-date').toString(); // Extract info from data-* attributes
        var title = button.data('title').toString(); // Extract info from data-* attributes
        $("#modal-vehicleView-Img > .modal-dialog > .modal-content > .modal-body > .img-full").attr('src', srcZoom);
        $("#modal-vehicleView-Img h4 ").html("<i class='fa fa-file-text blue'></i>" + title + " - " + pDate);
    });

    $("#modal-vehicleView-Img").on('hide.bs.modal', function () {
        $("#modal-vehicleView-Img > .modal-dialog > .modal-content > .modal-body > .img-full").attr('src', '');
    });

}
