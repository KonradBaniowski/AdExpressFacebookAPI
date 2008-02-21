<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<script language="javascript">
		
        var tab_input =  window.opener.document.getElementsByTagName('input');
        var keyWordTree="TreeLevelSelectedIds";
        var subStrIds; 
        var tab_level = new Array();
        var j=0;
        if(tab_input != null){
            for(var i=0; i<tab_input.length; i++){
                if(tab_input[i].id.indexOf("TreeLevelSelectedIds")!=-1  && tab_input[i].value != null && tab_input[i].value.length>0){
                    //alert(' tab_input[i] .value : ' + tab_input[i].value.substring(keyWordTree,tab_input[i].id.length));
                    subStrIds =  tab_input[i].id.substring(keyWordTree.length,tab_input[i].id.length);
                     //alert(' subStrIds : ' + subStrIds);
                    tab_level[j] = subStrIds + ':' + tab_input[i] .value;
                   
                    //alert(' tab_input[i] .value : ' + tab_input[i].id.substring(keyWordTree,tab_input[i].id.length));
                  
                    //alert('  tab_level[j]: ' +  tab_level[j]);
                    j++;
                }
            }
            //alert(' tab_level.LENGTH : ' + tab_level.length );
        }
		
		</script>
    <title>Page sans titre</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
