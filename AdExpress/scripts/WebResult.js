/*
 *
 * WebResult Version 1.0
 * Copyright (C) 2004 - 2006 TNS AdExpress (TNS Media Intelligence)
 * Author: G. Facon
 * Last Release: 29/11/2006
 *
 */

function OpenMediaPlanAlert(idSession,id,Level){
	window.open('/Private/Results/MediaSchedulePopUp.aspx?idSession='+idSession+'&id='+id+'&Level='+Level, '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenGenericMediaSchedule(idSession,id,Level){
	window.open('/Private/Results/MediaSchedulePopUp.aspx?idSession='+idSession+'&id='+id+'&Level='+Level, '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenGad(idSession, advertiser, idAddress){
	window.open('/Private/Results/Gad.aspx?idSession='+idSession+'&advertiser='+advertiser+'&idAddress='+idAddress, '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+",toolbar=0, directories=0, status=0, menubar=0, width=500, height=300, scrollbars=0, location=0, resizable=0");
}

function OpenCreationCompetitorAlert(idSession,ids,zoomDate){
	window.open('/Private/Results/CompetitorAlertCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate, '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenInsertions(idSession,ids,zoomDate){
	window.open('/Private/Results/CompetitorAlertCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate, '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenCreation(idSession,ids,zoomDate){
	window.open('/Private/Results/MediaInsertionsCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&param='+Math.random(), '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenProof(idSession,idProduct,idMedia,dateFacial,dateParution,page){
	window.open('/Private/Results/Proof.aspx?idSession='+idSession+'&idProduct='+idProduct+'&dateFacial='+dateFacial+'&dateParution='+dateParution+'&page='+page+'&idMedia='+idMedia, 'AdExpress', "top="+(screen.height-700)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=1, menubar=0, width=1024, height=700, scrollbars=1, location=0, resizable=1");
}

function openPressCreation(file){
    window.open('/Private/Results/ZoomCreationPopUp.aspx?creation='+file,'',"left="+((screen.width-530)/2)+", top="+((screen.height-700)/2)+",toolbar=0, directories=0, status=0, menubar=0,width=530 , height=700, scrollbars=1, location=0, resizable=1");
}

function openDownload(file,idSession, idVehicle){
    window.open("/Private/Results/DownloadCreationsPopUp.aspx?idSession="+idSession+"&idVehicle="+idVehicle+"&creation="+file, '', "top="+(screen.height-420)/2+", left="+(screen.width-830)/2+",toolbar=0, directories=0, status=0, menubar=0, width=830, height=420, scrollbars=0, location=0, resizable=0");
}

function OpenCreatives(idSession,ids,zoomDate, idUnivers,moduleId){
	window.open('/Private/Results/Creatives2.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&idUnivers='+idUnivers+'&moduleId='+moduleId+'&param='+Math.random(), '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}

function OpenInsertion(idSession,ids,zoomDate, idUnivers,moduleId){
	window.open('/Private/Results/Insertions.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&idUnivers='+idUnivers+'&moduleId='+moduleId+'&param='+Math.random(), '', "top="+(screen.height-600)/2+", left="+(screen.width-1024)/2+", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1");
}
