//<!--
//             Copyright obout inc      http://www.obout.com
//     
//             obout_ASP_TreeView_2

// string containing previous node content
var prevNodeContent;

var tree_edit_id="";

var e_O= false; e_o= false; document.onkeydown= function (e_Y){e_y(e_Y);} ; if (document.layers)try {document.registerEvents(Event.KEYDOWN);}catch (e_Y){} ; function ob_afterNodeEditing(name){

// EVENT. After node editing
// alert(prevNodeContent + " -> " + name + ";" + tree_edit_id);

}function e_y(e_Y){if (!e_Y)e_Y=window.event; if (e_o)return; if (typeof(tree_selected_id)!="undefined")e_I=document.getElementById(tree_selected_id); if (typeof(e_I)!="undefined" && e_I!=null){if (!e_O){e_i=document.createElement("input"); e_I.appendChild(e_i); try {e_i.focus(); }catch (e_Y){}e_I.removeChild(e_i); e_O= true; e_Y.cancelBubble= true; e_Y.returnValue= false; if (e_Y.stopPropagation)e_Y.stopPropagation(); }if (typeof(ob_tree_keynav_enable)!="undefined" && ob_tree_keynav_enable){var blockEvent= false; if (e_Y.keyCode==046){tree_node_exp_col= true; e_A=ob_getNodeUp(e_I, false); if (e_A!=null)ob_t22(e_A); tree_node_exp_col= false; blockEvent= true; }else if (e_Y.keyCode==050){tree_node_exp_col= true; e_a=ob_getNodeDown(e_I, false); if (e_a!=null)ob_t22(e_a); tree_node_exp_col= false; blockEvent= true; }else if (e_Y.keyCode==045){tree_node_exp_col= true; if (ob_hasChildren(e_I) && ob_isExpanded(e_I)){e_I.parentNode.firstChild.firstChild.onclick(); }else {parentNode=ob_getParentOfNode(e_I); if (parentNode!=null)ob_t22(parentNode); }tree_node_exp_col= false; blockEvent= true; }else if (e_Y.keyCode==047){tree_node_exp_col= true; if (ob_hasChildren(e_I)){if (!ob_isExpanded(e_I)){e_I.parentNode.firstChild.firstChild.onclick(); }else {firstChild=ob_getFirstChildOfNode(e_I); if (firstChild!=null)ob_t22(firstChild); }}tree_node_exp_col= false; blockEvent= true; }if (blockEvent){if (document.all){e_Y.cancelBubble= true; e_Y.returnValue= false; if (e_Y.stopPropagation)e_Y.stopPropagation(); }}}if (typeof(ob_tree_editnode_enable)!="undefined" && ob_tree_editnode_enable){if (e_Y.keyCode==015 || e_Y.keyCode==0161){ob_t22(e_I); }}}}function e_E(e_Y){if (!e_Y)e_Y=window.event; e_I=document.getElementById(tree_selected_id); if (e_I!=null){if (e_Y.keyCode==015){if (e_I.childNodes.length>0){e_e=e_I; try {if (ob_hasChildren(e_I) && e_I.childNodes[0].nodeName.toLowerCase()!="input")e_e=e_I.childNodes[0]; }catch (e_Y){}if (e_e.childNodes[0]!=null && e_e.childNodes[0].nodeName.toLowerCase()=="input"){var name=e_e.childNodes[0].value; if (name.length==0 || name.indexOf(":")!=-1 || name.indexOf("|")!=-1 || name.indexOf(",")!=-1 || name.indexOf("<")!=-1 || name.indexOf(">")!=-1){e_I.childNodes[0].onblur=null;

alert("The node name cannot be empty\nand\nIt cannot contain the following characters : | , \x3c >");

e_e.childNodes[0].value=prevNodeContent; try {e_e.childNodes[0].focus(); }catch (e_Y){}try {e_U=e_e.childNodes[0].ownerDocument.selection.createRange().duplicate(); e_U.moveStart("textedit",-1);e_U.moveEnd("textedit"); e_U.select(); }catch (e_Y){}e_e.childNodes[0].onblur= function (){e_u( true);} ; e_o= true; }else {e_e.removeChild(e_e.childNodes[0]); e_e.innerHTML=name; e_e.className="ob_t3"; e_o= false; if (name!=prevNodeContent){tree_edit_id+=tree_selected_id+":"+name+"|"; ob_afterNodeEditing(name); }}}}e_Y.cancelBubble= true; e_Y.returnValue= false; if (e_Y.stopPropagation)e_Y.stopPropagation(); }if (e_Y.keyCode==033){e_u( false); }else {}}}function e_u(e_Z){e_I=document.getElementById(tree_selected_id); if (e_I!=null){if (e_I.childNodes.length>0){e_e=e_I; try {if (ob_hasChildren(e_I) && e_I.childNodes[0].nodeName.toLowerCase()!="input")e_e=e_I.childNodes[0]; }catch (e_Y){}if (e_e.childNodes[0]!=null && e_e.childNodes[0].nodeName.toLowerCase()=="input"){var name=e_e.childNodes[0].value; e_e.removeChild(e_e.childNodes[0]); e_e.innerHTML=e_Z?name:prevNodeContent; e_e.className="ob_t3"; e_o= false; if (e_Z && (name!=prevNodeContent)){tree_edit_id+=tree_selected_id+":"+name+"|"; ob_afterNodeEditing(name); }}}}}function ob_attemptStartEditing(e_z){if ((typeof(tree_node_exp_col)=="undefined" || (typeof(tree_node_exp_col!="undefined") && !tree_node_exp_col)) && typeof(e_o)!="undefined" && typeof(ob_tree_editnode_enable)!="undefined" && ob_tree_editnode_enable){if (e_z.id==tree_selected_id){if (typeof(ob_noedit)!="undefined" && ob_noedit!=""){var a=new Array; a=ob_noedit.replace(" ","").split(","); if (a.length>0){for (i=0; i<a.length; i++){if (a[i]==e_z.id){alert("Can't edit. The node is marked as not editable."); return; }}}}if (e_z.childNodes.length>0){if (e_I==null)e_I=document.getElementById(tree_selected_id); e_e=e_I; try {if (ob_hasChildren(e_I) && e_I.childNodes[0].nodeName.toLowerCase()!="#text")e_e=e_I.childNodes[0]; }catch (e_Y){}if (e_e.childNodes[0]!=null && e_e.childNodes[0].nodeName.toLowerCase()=="#text"){prevNodeContent=e_e.childNodes[0].nodeValue; var e_X=document.createElement("input"); e_X.setAttribute("type","text"); e_X.setAttribute("value",prevNodeContent); e_X.id=e_z.id+"_txtBox"; e_X.style.borderWidth=0; e_X.style.width=e_z.offsetWidth+036; e_X.style.backgroundColor="transparent"; e_X.className=e_z.className; while (e_e.childNodes.length>0)e_e.removeChild(e_e.childNodes[0]); e_e.appendChild(e_X); e_X.onkeydown= function (e_Y){e_E(e_Y);} ; e_X.onblur= function (){e_u( true);} ; try {e_X.focus(); }catch (e_Y){}try {e_U=e_X.ownerDocument.selection.createRange().duplicate(); e_U.moveStart("textedit",-1);e_U.moveEnd("textedit"); if (e_U.htmlText.toLowerCase().indexOf("body")==-1)e_U.select(); else {try {e_X.focus(); }catch (e_Y){}}}catch (e_Y){}e_o= true; }}}}}function ob_attemptEndEditing(e_z){e_I=e_z; if (typeof(e_I)!="undefined" && typeof(e_o)!="undefined" && typeof(ob_tree_editnode_enable)!="undefined" && ob_tree_editnode_enable){e_e=e_I; try {if (ob_hasChildren(e_I) && e_I.childNodes[0].nodeName.toLowerCase()!="input")e_e=e_I.childNodes[0]; }catch (e_Y){}if (e_e.childNodes.length>0){if (e_e.childNodes[0]!=null && e_e.childNodes[0].nodeName.toLowerCase()=="input"){if (e_e.id!=tree_selected_id){var name=e_e.childNodes[0].value; if (name.length==0 || name.indexOf(":")!=-1 || name.indexOf("|")!=-1 || name.indexOf(",")!=-1 || name.indexOf("<")!=-1 || name.indexOf(">")!=-1){alert("The node name cannot be empty\nand\nIt cannot contain the following characters : | , \x3c >"); e_e.childNodes[0].value=prevNodeContent; try {e_e.childNodes[0].focus(); }catch (e_Y){}try {e_U=e_e.childNodes[0].ownerDocument.selection.createRange().duplicate(); e_U.moveStart("textedit",-1);e_U.moveEnd("textedit"); e_U.select(); }catch (e_Y){}e_o= true; return; }e_e.removeChild(e_e.childNodes[0]); e_e.innerHTML=name; e_o= false; if (name!=prevNodeContent){tree_edit_id+=tree_selected_id+":"+name+"|"; ob_afterNodeEditing(name); }}}}}}
//-->