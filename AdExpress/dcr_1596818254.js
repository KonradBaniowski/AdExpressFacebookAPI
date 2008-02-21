// fix for "Click to activate and use this control"
// fix for Flash
function DoIEFlashFix()
{
  if (document.getElementsByTagName) 
  {
      var objects = document.getElementsByTagName("object");
      for (var i = 0; i < objects.length; i++)
      {
          objects[i].outerHTML = objects[i].outerHTML;
     }
   }
}

// fix for Svg
function DoIESvgFix(src_value)
{
    embeds = document.getElementsByTagName("embed");
    for (var i = 0; i < embeds.length; i++)
    {
        if (embeds[i].src == src_value)
        {
            embeds[i].outerHTML = embeds[i].outerHTML;
        }
    }    
}