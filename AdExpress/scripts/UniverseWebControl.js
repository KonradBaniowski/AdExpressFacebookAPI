String.prototype.trim = function()
{
    return this.replace(/(?:^\s+|\s+$)/g, "");
}


 function InitializeAllItems(list_selected_id){   
    customDeleteAllChild(list_selected_id);
 }
 
 function customDeleteAllChild(list_selected_id)
 {  
    // alert('list_selected_id'+list_selected_id);
    var strKEY = 'TreeLevelSelectedIds'; 
    var inputId='';
    var inputOject;
    if(typeof list_selected_id != "undefined")
    {
        var selected_id_Array = list_selected_id.split(',');
        for(var i=0; i<selected_id_Array.length;i++){           
            deleteAllChild(selected_id_Array[i]);
            inputId =  strKEY + selected_id_Array[i].substring(1,selected_id_Array[i].length);
           
            inputOject = document.getElementById(inputId); 
            if( inputOject != null)inputOject.value='';
        }
    }
 }
 
  function trapEnter(currentBranch,event){    
    if((event.which && event.which == 13)||(event.keyCode && event.keyCode == 13)){         
        SearchClassificationItems(document.getElementsByName('searchItemsKeyWord')[0].value,currentBranch);
        return  false;
    }
    else return true;
 }
 
 function AddNewGroup(id,sender){
        try{
      document.getElementById(id).style.display='';
       sender.style.display='none';
      }catch(e){}
 }
// function for removing all child nodes
function deleteAllChild(selected_id)
{    
    if(typeof selected_id != "undefined")
    {
        //ob_t2_Remove(selected_id);
		var node = document.getElementById(selected_id);
		
		var childContainer = node.parentNode.parentNode.parentNode.nextSibling;
		if (childContainer != null ){
		    var parentContainer = childContainer.parentNode;
		    if (parentContainer){
		        // removing all child
		        parentContainer.removeChild(childContainer);
		        // fixing the -/+ image:
		        var nextNode = ob_getNextSiblingOfNode( node );
		        var image = node.parentNode.firstChild.firstChild;
		        //alert(image.src);
		        
		        image.src = image.src.toLowerCase();
		        if ( nextNode == null ){
		            if ( image.src.indexOf("minus_r") >= 0){
		                image.src = image.src.replace("minus_r","hr_L");
		            }else if ( image.src.indexOf("minus_l") >= 0){
		                image.src = image.src.replace("minus_l","hr_L");
		            }else if ( image.src.indexOf("minus") >= 0){
		                image.src = image.src.replace("minus","hr_L");
		            }else if ( image.src.indexOf("plusik_l") >= 0){
		                image.src = image.src.replace("plusik_l","hr_L");
		            }else if ( image.src.indexOf("plusik") >= 0){
		                image.src = image.src.replace("plusik","hr_L");
		            }
		        }else{
		            if ( image.src.indexOf("minus_r") >= 0){
		                image.src = image.src.replace("minus_r","hr");
		            }else if ( image.src.indexOf("minus_l") >= 0){
		                image.src = image.src.replace("minus_l","hr");
		            }else if ( image.src.indexOf("minus") >= 0){
		                image.src = image.src.replace("minus","hr");
		            }else if ( image.src.indexOf("plusik_l") >= 0){
		                image.src = image.src.replace("plusik_l","hr");
		            }else if ( image.src.indexOf("plusik") >= 0){
		                image.src = image.src.replace("plusik","hr");
		            }
		        }
		        
		    }
		}
    }
    else
    {
        alert("Please select a parent node to delete all child nodes!");
    }
}

/*Fonction personalisées D'ajout d'éléments*/
function ob_t2_Custom_Add(parentId, currentLevelId,treeviewId,oSelect,hiddenFieldId,nodeCss,expanded, image, subTreeURL)
{		
   
	var pNode = document.getElementById(parentId);
	var hiddenFieldIElement = document.getElementById(hiddenFieldId);
	var childId;
	var tempSelectedIds='';
	var  bt_delete_Image= new Image();	
	bt_delete_Image.src = '/Images/Common/button/bt_delete_item.gif';
	
	if (!pNode) 
	{	   
	    alert("Parent does not exist.");
		return;		
	}	
	if(pNode)	
	{
		if (pNode.className.toLowerCase() != 'ob_t2' && pNode.className.toLowerCase() != 'ob_t3')
		{
			alert("Parent node is not a valid tree node.");
			return;
		}
	}
		   	
	dParent = pNode.parentNode.parentNode.parentNode.parentNode;
	     
	for( i=0; i<oSelect.options.length; i++){//Accolade Boucle debut Ajout des éléments d'ans un niveau
	        
	        childId =  treeviewId + '_' + currentLevelId + '_' + oSelect.options[i].value ; 
         
	    if (oSelect.options[i].selected && currentLevelId != null && currentLevelId.length>0 && document.getElementById(childId) == null) { //Debut Si  un élément sélectionné	  
	     if (!ob_hasChildren(pNode))
        {
	        var e = dParent.firstChild.firstChild.firstChild.firstChild.firstChild;
	        e.src=ob_style+"/minus" + (ob_getLastChildOfNode(ob_getParentOfNode(pNode)) == pNode ? "_l.gif" : ".gif");
	        e.onclick=function(){ob_t21(this,'')};
    		
	        e = dParent.appendChild(document.createElement("TABLE"));		    
		    e.className = "ob_t2g";
    		
		    if (document.all)
		    {			
			    e.cellSpacing = "0";
		    }
		    else
		    {			
			    e.setAttribute("cellspacing", "0");
		    }
    		
	        e.appendChild(document.createElement("tbody"));
	        var e2=e.firstChild.appendChild(document.createElement("TR"));
	        e=e2.appendChild(document.createElement("TD"));
	        if(dParent.parentNode.lastChild!=dParent)
		        e.style.backgroundImage="url("+ob_style+"/vertical.gif)";
	        e.innerHTML="<div class=ob_d5></div>";
	        e=e2.appendChild(document.createElement("TD"));
	        e.className="ob_t5";	    
	        dParent.className = 'ob_d2b';
        }
        else if(i<2)
        {
		    prevS = ob_getLastChildOfNode(pNode, true);
		    if(prevS == null) {      
			    if(pNode.parentNode.firstChild.firstChild.src.indexOf("plusik_l.gif")) {
				    ob_t2_OnDynamicLoad = function() {ob_t2_Add(parentId, childId,  textOrHTML, expanded, image, subTreeURL);}    
			    }
			    return;
	       }
	        var oPrevSImg = prevS.parentNode.parentNode.parentNode.parentNode.firstChild.firstChild.firstChild.firstChild.firstChild;
    	    
		    if (!ob_hasChildren(prevS)) oPrevSImg.src = ob_style + "/hr.gif";
		    else 
		    {				
			    if (!ob_isExpanded(prevS))
			    {					
				    oPrevSImg.src = ob_style + "/plusik.gif";										
			    }
			    else oPrevSImg.src = ob_style + "/minus.gif";
			    prevS.parentNode.parentNode.parentNode.parentNode.firstChild.nextSibling.firstChild.firstChild.firstChild.style.backgroundImage = "url(" + ob_style + "/vertical.gif)";				
		    }		    			
    		
	        if(dParent.parentNode.lastChild!=dParent)
		        prevS.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.firstChild.style.backgroundImage = "url(" + ob_style + "/vertical.gif)";
    		    
		    oPrevSImg.parentNode.className = "ob_t6v";
        }
    	        
    	 /* Debut Sauvegarde  des identifiants enfant*/
        if(hiddenFieldIElement!=null){       
            if(hiddenFieldIElement.value !=null &&  hiddenFieldIElement.value.length>0){
            if(!(hiddenFieldIElement.value.split(',').length<1000)){
            //alert(' Vous ne pouvez sélectionner au maximum que 1000 éléments par niveau.');
             alert(capacityExceptionMessage);
                break;
            }
             hiddenFieldIElement.value =  hiddenFieldIElement.value +',' + oSelect.options[i].value  
            }else {
                hiddenFieldIElement.value = oSelect.options[i].value 
            }        
        }
	   /* Debut Sauvegarde  des identifiants enfant*/
	   
        /*Debut ajout d'un noeud enfant*/                	                           
       textOrHTML = '<div class='+nodeCss+' nowrap>'+ oSelect.options[i].text + '</div>'; 
       
        div = document.createElement('div');
        div.className = 'ob_d2c';
         	
        sInnerHtml = '<table cellspacing="0" class="ob_t2g"><tr><td class="ob_t6"><img ' + ((subTreeURL != null) ? 'src="' + ob_style + '/plusik_l.gif" onclick="ob_t21(this, \'' + subTreeURL + '\')"' : 'src="' + ob_style + '/hr_l.gif"') + '></td>';
        sInnerHtml += '<td class="ob_t4"' + (ob_t2_showicons == false ? ' style="display:none;"' : '') + '>' + (ob_t2_showicons == true ? '<div class="ob_d4">' : '') + ((image != null && typeof(ob_icons) != 'undefined' && ob_t2_showicons == true) ? '<img src="' + ob_icons + '/' + image + '">' : '') + (ob_t2_showicons == true ? '</div>' : '') + '</td>';
        sInnerHtml += '<td id=' + childId + ' onclick="ob_t22(this, event)" class="ob_t2">' + textOrHTML + '</td><td nowrap>&nbsp;<a href=# onclick="removeItem(\''+hiddenFieldId+'\', \''+oSelect.options[i].value+'\');ob_t2_Remove(\''+childId+'\');"><img src=\"'+bt_delete_Image.src+'\" border=0></a></td></tr></table>';	///Images/Common/button/bt_delete_item.gif
    	     	
        div.innerHTML = sInnerHtml;
        node = div.firstChild.firstChild.firstChild.firstChild.nextSibling.nextSibling;
        node.className = 'ob_t2';
        node.nowrap = "true";
        dParent.firstChild.nextSibling.firstChild.firstChild.firstChild.nextSibling.appendChild(div);	    		
        
	    ob_OnAddNode(parentId, childId, textOrHTML, expanded, image, subTreeURL);    
	    /* Fin ajout d'un noeud enfant*/ 
	    
	    
	   	   
	}//Fin Si  un élément sélectionné
  } //Accolade Boucle debut Ajout des éléments d'ans un niveau   
}


function GetAllAddedChildsItems(parentId,hiddenFieldId){
     var hiddenFieldIElement = document.getElementById(hiddenFieldId);
     if(hiddenFieldIElement!=null){
        hiddenFieldIElement.value =  GetAllAddedChildsItems(parentId);
     }
}

function GetAllAddedChildsItems(parentId){
	       var addedIds='';
           var parentNode = document.getElementById(parentId);
            if(parentNode != null){
              var list = ob_getChildrenList(parentNode);
              if(list != null && list.length>0) {
                for(var i=0; i<list.length; i++){
                    addedIds +=  list[i].id +",";                   
                }
                if(addedIds.length>0)addedIds = addedIds.substring(0,addedIds.length-1);                 
              }
            }
          return addedIds;
}

function removeItem(hiddenFieldId, itemToRemove) {
	var hiddenFieldIElement = document.getElementById(hiddenFieldId);
	 var selected_id_Array;
	 var final_id_list="";
	
	if(hiddenFieldIElement != null && hiddenFieldIElement.value !=null &&  hiddenFieldIElement.value.length>0){
	     selected_id_Array = hiddenFieldIElement.value.split(',');
	   
        for(var i=0; i<selected_id_Array.length;i++){ 
             if(selected_id_Array[i] !=null && typeof(selected_id_Array[i]) != "undefined" && selected_id_Array[i].length>0 && !(parseInt(selected_id_Array[i]) == parseInt(itemToRemove))){
                final_id_list += selected_id_Array[i] +',';
                  
            }
        }     
        
         if(final_id_list != null && final_id_list.length>0)
           final_id_list = final_id_list.substring(0,final_id_list.length-1);                         
        hiddenFieldIElement.value = (final_id_list!=null && final_id_list.length>0) ? final_id_list : "";        
	}
}