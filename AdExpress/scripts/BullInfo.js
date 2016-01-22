var ns4=document.layers?1:0
var ie4=document.all?1:0
var ns6=document.getElementById&&!document.all?1:0

if(navigator.appName.substring(0,3) == "Net")
	document.captureEvents(Event.MOUSEMOVE)
document.onmousemove = get_mouse

var textBull,transp,Opac=0,OpacOmbr=0,bull=0,afBul=0
var effac=setTimeout('infBul(0)',9000)

function get_mouse(e){
	x = (navigator.appName.substring(0,3) == "Net") ? e.pageX : event.x+document.body.scrollLeft
	y = (navigator.appName.substring(0,3) == "Net") ? e.pageY : event.y+document.body.scrollTop
	    PosBullHoriz = x
	    PosBullVerti = y
}

function infBul(arg){
      if (arg != 1){
            argOp = arg
            if (arg != 0 && afBul == 0){
                  setTimeout('afBul=1',20)
                  setTimeout('infBul(argOp)',40)
            }

            if (ie4){
                  if (afBul == 1 && arg != 0 && Opac < 100){
                        Opac += 5
                        transp = 'filter: alpha(opacity='+Opac+')'
                        if (Opac < 10){
                              OpacOmbr = 0
                        }
                        if (OpacOmbr < 20 && Opac > 30){
                              OpacOmbr += 2
                        }
                        setTimeout('infBul(argOp)',1)
                  }
                  if (arg == 0 && Opac > 0){
                        Opac -= 2
                        transp = 'filter: alpha(opacity='+Opac+')'    
                        if (OpacOmbr > 0 && Opac < 80){
                              OpacOmbr -= 2
                        }
                        setTimeout('infBul(argOp)',2)
                  }
                  scrollPag = document.body.scrollTop
            }
            else {
                  if (afBul == 1 && arg != 0 && Opac < 10){
                        Opac ++
                        valOpac = '.'+Opac
                        if (Opac == 10){
                              valOpac = Opac
                        }
                        transp = '-moz-opacity: '+valOpac+''
                        if (Opac < 2){
                              OpacOmbr = 0
                              valOpacOmbr = '.'+OpacOmbr
                        }
                        if (OpacOmbr < 2 && Opac > 4){
                              OpacOmbr ++
                              valOpacOmbr = '.'+OpacOmbr
                        }
                        setTimeout('infBul(argOp)',50)
                  }
                  if (arg == 0 && Opac > 0){
                        Opac --
                        valOpac = '.'+Opac
                        transp = '-moz-opacity: '+valOpac+''
                        if (OpacOmbr > 0 && Opac < 8){
                              OpacOmbr --
                              valOpacOmbr = '.'+OpacOmbr
                        }
                        setTimeout('infBul(argOp)',60)
                  }
                  scrollPag = window.pageYOffset
            }
            larg_ecran = document.body.clientWidth
            haut_ecran = document.body.clientHeight
            if (arg != 0){
                  limitEcran = scrollPag + haut_ecran
                  
                  //PosHoriz = PosBullHoriz + 10
                  //PosVertic = PosBullVerti + 20
                  
                  PosHoriz = 250
                  PosVertic = PosBullVerti

                  textBull = argOp
            }
      }
      else {
            Opac = 0
      }
      
leCadr = '<table style="'+transp+';"><tr><td>'+textBull+'</td></tr></table>'

      if (afBul == 1){
            if (Opac == 0){
                  document.getElementById("leTest").innerHTML = ""
                  document.getElementById("leTest").style.visibility = 'hidden'
                  document.getElementById("leTestO").innerHTML = ""
                  document.getElementById("leTestO").style.visibility = 'hidden'
                  bull = 0
                  afBul = 0
            }
            else {
                  document.getElementById("leTest").innerHTML = leCadr
                  document.getElementById("leTest").style.visibility = 'visible'
                  document.getElementById("leTestO").style.visibility = 'visible'
                  largBull = document.getElementById("leTest").offsetWidth
                  hautBull = document.getElementById("leTest").clientHeight
                  limiteVert = PosBullVerti + hautBull + 26
                  if (((PosBullHoriz + largBull + 4) > larg_ecran)&&(limiteVert > limitEcran)){
                        PosHoriz = larg_ecran - (largBull + PosBullHoriz) + (PosBullHoriz - 4)
                        PosVertic = PosVertic - 50
                  }
                  if (((PosBullHoriz + largBull + 4) > larg_ecran)&&(limiteVert <= limitEcran)){
                        PosHoriz = larg_ecran - (largBull + PosBullHoriz) + (PosBullHoriz - 4)
                  }
                  if (((PosBullHoriz + largBull + 4)<= larg_ecran)&&(limiteVert > limitEcran)){
                        //PosVertic = PosVertic - 30
                        PosVertic = PosVertic - (limiteVert-limitEcran) +10
                  }
                  if (bull == 0){
                        document.getElementById("leTest").style.top = PosVertic
                        document.getElementById("leTest").style.left = PosHoriz
                        document.getElementById("leTest").style.zIndex = 1000
                        document.getElementById("leTestO").style.top = PosVertic + 6
                        document.getElementById("leTestO").style.left = PosHoriz + 7
                        document.getElementById("leTestO").style.zIndex = 999
                        bull = 1
                        clearTimeout(effac)
                        effac=setTimeout('infBul(0)',9000)
                  }
            }
      }
}

function sourisPress(){
      infBul(1)
}

document.onmousedown = sourisPress

setTimeout('infBul(0)',600)

document.write('<div ID="leTest" style=position:absolute></div><div ID="leTestO" style=position:absolute;left:500px; top:200px;></div>')