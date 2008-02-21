using System;
using System.Text;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using System.Collections;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpress.Web.Functions
{
	/// <summary>
	/// Fournit les fonctions pour la gestion des détails de séléction 
	/// </summary>
	public class DetailSelectionScript
	{	

		///<summary>
		/// Fonction qui permet de générer le script pour la gestion du Drag and Drop
		///</summary>
		///<param name="nbColumnItemList">Nombre de colonnes dans la liste</param>
		///<param name="nbTrashItemList">Nombre d'éléments dans la poubelle</param>
		///<returns></returns>
		public static string GenericDetailSelectionScript(int nbColumnItemList, int nbTrashItemList){
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<SCRIPT language=\"JavaScript\" type=\"text/JavaScript\">");
			
            
			#region Fonction vider 
			t.Append("\nfunction vider()");
			t.Append("\n{");
			t.Append("\nvar htmlElement1 = new Array();");
			t.Append("\nvar theGUI1 = new Array();");
      		t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+"+1; i++ )");
			t.Append("\nhtmlElement1[i] = dndMgr.dropZones[i].getHTMLElement();");
       		t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+"; i++ )");
			t.Append("\ntheGUI1[i] = dndMgr.draggables[i].getDroppedGUI();");
			t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+"; i++ )");
			t.Append("\n{");
			t.Append("\n{");
			t.Append("\ntheGUI1[i].style.position = \"static\";");
			t.Append("\ntheGUI1[i].style.top = \"\";");
			t.Append("\ntheGUI1[i].style.top = \"\";");
			t.Append("\ntheGUI1[i].style.height=\"15px\";");
			t.Append("\ntheGUI1[i].style.width=\"120px\";");
			t.Append("\n}");
	    	t.Append("\nhtmlElement1["+nbColumnItemList+"].appendChild(theGUI1[i]);");
			t.Append("\ndndMgr.draggables[i].currentpos="+nbColumnItemList+";");
			t.Append("\ndndMgr.dropZones[i].isempty=1;");
			t.Append("\n}");   
			t.Append("\ndndMgr.dropZones["+nbColumnItemList+"].nb_elements=0;");
			t.Append("\nForm1.columnItemSelectedList.value=\"-1\";");
			t.Append("\n}");
			#endregion
			
			#region Fonction initialiser
			t.Append("\nfunction initialiser()");
			t.Append("\n{");
    		t.Append("\nvar htmlElement1 = new Array();");
			t.Append("\nvar theGUI1 = new Array();");
			t.Append("\nvar index = new Array();");
			t.Append("\nvar j=0;");
        	t.Append("\nfor( var i = 0 ; i <"+ nbColumnItemList+"+1; i++ )");
			t.Append("\nhtmlElement1[i] = dndMgr.dropZones[i].getHTMLElement();");
       		t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+" ; i++ )");
			t.Append("\nif(dndMgr.draggables[i].currentpos == "+nbColumnItemList+")");
			t.Append("\n{");
			t.Append("\ntheGUI1[j] = dndMgr.draggables[i].getDroppedGUI();");
			t.Append("\nindex[j]=i;");
			t.Append("\nj++;");
			t.Append("\n}");
	 		t.Append("\nfor( var i = dndMgr.dropZones["+nbColumnItemList+"].nb_elements ; i < "+nbColumnItemList+" ; i++ )");
			t.Append("\n{");
			t.Append("\n{");
			t.Append("\ntheGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements].style.position = \"static\";");
			t.Append("\ntheGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements].style.top = \"\";");
			t.Append("\ntheGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements].style.top = \"\";");
			t.Append("\ntheGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements].style.height=\"25px\";");
			t.Append("\ntheGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements].style.width=\"55px\";");
			t.Append("\n}");
	    	t.Append("\nhtmlElement1[i].appendChild(theGUI1[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements]);");
			t.Append("\ndndMgr.draggables[index[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements]].currentpos = dndMgr.dropZones[i].pos ;");
			t.Append("\ndndMgr.dropZones[i].currentobj = dndMgr.draggables[index[i-dndMgr.dropZones["+nbColumnItemList+"].nb_elements]].pos ;");
			t.Append("\ndndMgr.dropZones[i].isempty=0;");
			t.Append("\n}");
			t.Append("\ndndMgr.dropZones["+nbColumnItemList+"].nb_elements="+nbColumnItemList+";");
			
			#region Traitement liste colonnes sélectionnées
			//Mise à jour de la liste des colonnes sélectionnées
			t.Append("\nForm1.columnItemSelectedList.value=\"\";");
			t.Append("\nvar first=0;");
			t.Append("\nfor( var i=0; i<"+nbColumnItemList+"; i++)");
			t.Append("\n{");
			t.Append("\nif(first==0)");
			t.Append("\n{");
			t.Append("\nForm1.columnItemSelectedList.value+=dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\nfirst=1;");
			t.Append("\n}");
			t.Append("\nelse");
			t.Append("\nForm1.columnItemSelectedList.value+=\",\"+dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\n}");
				
			#endregion

			t.Append("\n}");
			#endregion

			#region La classe CustomDropzoneCorbeille

			t.Append("\n// La Classe qui se charge de définir le conteneur des objets draggable (La corbeille)");
			t.Append("\nvar CustomDropzoneCorbeille = Class.create();");
			t.Append("\nCustomDropzoneCorbeille.prototype = (new Rico.Dropzone()).extend( {");
			t.Append("\ninitialize: function( htmlElement ) {");
			t.Append("\nthis.htmlElement  = $(htmlElement);");
			t.Append("\nthis.absoluteRect = null;");
			t.Append("\nthis.dropZones    = new Array();");
			t.Append("\nthis.nb_elements = "+nbTrashItemList+";");
			t.Append("\n},");
			t.Append("\naccept: function(draggableObjects) {");
			t.Append("\nvar htmlElement = this.getHTMLElement();");
			t.Append("\nvar htmlElement1 = new Array();");
            t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+" ; i++ )");
	        t.Append("\nhtmlElement1[i] = dndMgr.dropZones[i].getHTMLElement();");
            t.Append("\nvar theGUI = draggableObjects[0].getDroppedGUI();");
            t.Append("\nvar theGUI1 = new Array();");
            t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+" ; i++ )");
	 		t.Append("\ntheGUI1[i] = dndMgr.draggables[i].getDroppedGUI();");
	      	t.Append("\nif ( htmlElement == null )");
			t.Append("\nreturn;");
            t.Append("\nif ( RicoUtil.getElementsComputedStyle( theGUI, \"position\" ) == \"absolute\" )");
			t.Append("\n{");
	        t.Append("\ntheGUI.style.position = \"static\";");
			t.Append("\ntheGUI.style.top = \"\";");
			t.Append("\ntheGUI.style.top = \"\";");
			t.Append("\ntheGUI.style.height=\"15px\";");
			t.Append("\ntheGUI.style.width=\"120px\";");
			t.Append("\n}");
			t.Append("\nif (draggableObjects[0].currentpos != "+nbColumnItemList+")");
            t.Append("\n{");
            t.Append("\nvar init = draggableObjects[0].currentpos ;");
            t.Append("\nvar fin = this.nb_elements-1 ;");
          	t.Append("\nfor( var i=init ; i<fin ; i++)");
			t.Append("\n{");
	        t.Append("\nhtmlElement1[i].appendChild(theGUI1[dndMgr.dropZones[i+1].currentobj]);");
		    t.Append("\ndndMgr.draggables[dndMgr.dropZones[i+1].currentobj].currentpos = dndMgr.dropZones[i].pos ;");
		    t.Append("\ndndMgr.dropZones[i].currentobj = dndMgr.draggables[dndMgr.dropZones[i+1].currentobj].pos ;");
	  	    t.Append("\n}");
			t.Append("\ndndMgr.dropZones[fin].isempty=1;");
			t.Append("\nthis.nb_elements = this.nb_elements-1;");
			t.Append("\ndraggableObjects[0].currentpos = "+nbColumnItemList+" ;");
            t.Append("\n}");
            t.Append("\nhtmlElement.appendChild(theGUI);");
			
			#region Traitement liste colonnes sélectionnées
			//Mise à jour de la liste des colonnes sélectionnées
			t.Append("\nForm1.columnItemSelectedList.value=\"\";");
			t.Append("\nvar first=0;");
			t.Append("\nfor( var i=0; i<dndMgr.dropZones["+nbColumnItemList+"].nb_elements; i++)");
			t.Append("\n{");
			t.Append("\nif (dndMgr.dropZones[i].isempty==0)");
			t.Append("\n{");
			t.Append("\nif(first==0)");
			t.Append("\n{");
			t.Append("\nForm1.columnItemSelectedList.value+=dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\nfirst=1;");
			t.Append("\n}");
			t.Append("\nelse");
			t.Append("\nForm1.columnItemSelectedList.value+=\",\"+dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\n}");
			t.Append("\n}");
			t.Append("\nif (this.nb_elements == 0)");
			t.Append("\nForm1.columnItemSelectedList.value+=\"-1\";");
		
			#endregion
			
            t.Append("\n}");
			t.Append("\n} );");
			#endregion

			#region La classe CustomDropzone
            t.Append("\n/////////////////////////////////////////////////////////////////////////////////////////////////");
            t.Append("\n// La Classe qui se charge de dÃ©finir le conteur des objets draggable (Les colonnes)");
            t.Append("\nvar CustomDropzone = Class.create();");
			t.Append("\nCustomDropzone.prototype = (new Rico.Dropzone()).extend( {");
            t.Append("\ninitialize: function( htmlElement, pos, currentobj, isempty ) {");
			t.Append("\nthis.htmlElement  = $(htmlElement);");
			t.Append("\nthis.absoluteRect = null;");
			t.Append("\nthis.dropZones    = new Array();");
			t.Append("\nthis.pos = pos;");
			t.Append("\nthis.currentobj = currentobj;");
			t.Append("\nthis.isempty = isempty;");
			t.Append("\n},");
			t.Append("\naccept: function(draggableObjects) {");
		    t.Append("\nvar htmlElement = this.getHTMLElement();");
            t.Append("\nvar htmlElement1 = new Array();");
            t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+" ; i++ )");
			t.Append("\nhtmlElement1[i] = dndMgr.dropZones[i].getHTMLElement();");
            t.Append("\nvar theGUI = draggableObjects[0].getDroppedGUI();");
            t.Append("\nvar theGUI1 = new Array();");
            t.Append("\nfor( var i = 0 ; i < "+nbColumnItemList+" ; i++ )");
			t.Append("\ntheGUI1[i] = dndMgr.draggables[i].getDroppedGUI();");
            t.Append("\nif ( htmlElement == null )");
            t.Append("\nreturn;");
            t.Append("\nif (draggableObjects[0].currentpos == "+nbColumnItemList+")");
            t.Append("\n{			      ");
            t.Append("\nvar position = dndMgr.dropZones["+nbColumnItemList+"].nb_elements;");
            t.Append("\nvar fin = this.pos;");
            t.Append("\nif ( RicoUtil.getElementsComputedStyle( theGUI, \"position\" ) == \"absolute\" )");
         	t.Append("\n{");
			t.Append("\ntheGUI.style.position = \"static\";");
		    t.Append("\ntheGUI.style.top = \"\";");
		    t.Append("\ntheGUI.style.top =  \"\";");
			t.Append("\ntheGUI.style.height=\"25px\";");
			t.Append("\ntheGUI.style.width=\"55px\";");
		    t.Append("\n}");
            t.Append("\nif (this.isempty == 1)");
            t.Append("\n{");
		    t.Append("\nhtmlElement1[position].appendChild(theGUI);");
			t.Append("\ndraggableObjects[0].currentpos = dndMgr.dropZones[position].pos ;");
		    t.Append("\ndndMgr.dropZones[position].currentobj = draggableObjects[0].pos ;");
			t.Append("\ndndMgr.dropZones["+nbColumnItemList+"].nb_elements = dndMgr.dropZones["+nbColumnItemList+"].nb_elements+1;");
		    t.Append("\ndndMgr.dropZones[position].isempty=0;");
		    t.Append("\n}");
	        t.Append("\nelse");
	        t.Append("\n{");
			t.Append("\nvar init = dndMgr.dropZones["+nbColumnItemList+"].nb_elements ;");
			t.Append("\nfor( var j=init ; j>fin ; j--)");
			t.Append("\n{");
		    t.Append("\nhtmlElement1[j].appendChild(theGUI1[dndMgr.dropZones[j-1].currentobj]);");
			t.Append("\ndndMgr.draggables[dndMgr.dropZones[j-1].currentobj].currentpos = dndMgr.dropZones[j].pos ;");
		    t.Append("\ndndMgr.dropZones[j].currentobj = dndMgr.draggables[dndMgr.dropZones[j-1].currentobj].pos ;");
		    t.Append("\n}");
			t.Append("\nhtmlElement.appendChild(theGUI);");
		    t.Append("\ndraggableObjects[0].currentpos = this.pos ;");
		    t.Append("\nthis.currentobj = draggableObjects[0].pos ;");
			t.Append("\ndndMgr.dropZones["+nbColumnItemList+"].nb_elements = dndMgr.dropZones["+nbColumnItemList+"].nb_elements+1;");
			t.Append("\ndndMgr.dropZones[position].isempty=0;");
		    t.Append("\n}");
            t.Append("\n}");
			t.Append("\nelse");
			t.Append("\n{");
		    t.Append("\nif ( RicoUtil.getElementsComputedStyle( theGUI, \"position\" ) == \"absolute\" )");
		    t.Append("\n{");
		    t.Append("\ntheGUI.style.position = \"static\";");
		    t.Append("\ntheGUI.style.top = \"\";");
		    t.Append("\ntheGUI.style.top = \"\";");
			t.Append("\ntheGUI.style.height=\"25px\";");
			t.Append("\ntheGUI.style.width=\"55px\";");
		    t.Append("\n}");
            t.Append("\nvar verif = dndMgr.dropZones["+nbColumnItemList+"].nb_elements;");
            t.Append("\nif ((verif == "+nbColumnItemList+") || (this.isempty == 0))");
			t.Append("\n{");
			t.Append("\nvar init = draggableObjects[0].currentpos ;");
			t.Append("\nvar fin = this.pos ;");
            t.Append("\nif (init < fin)");
			t.Append("\nfor( var i=init ; i<fin ; i++)");
			t.Append("\n{");
			t.Append("\nhtmlElement1[i].appendChild(theGUI1[dndMgr.dropZones[i+1].currentobj]);");
			t.Append("\ndndMgr.draggables[dndMgr.dropZones[i+1].currentobj].currentpos = dndMgr.dropZones[i].pos ;");
			t.Append("\ndndMgr.dropZones[i].currentobj = dndMgr.draggables[dndMgr.dropZones[i+1].currentobj].pos ;");
			t.Append("\n}");
			t.Append("\nelse");	 
		    t.Append("\nfor( var j=init ; j>fin ; j--)");
			t.Append("\n{");
			t.Append("\nhtmlElement1[j].appendChild(theGUI1[dndMgr.dropZones[j-1].currentobj]);");
			t.Append("\ndndMgr.draggables[dndMgr.dropZones[j-1].currentobj].currentpos = dndMgr.dropZones[j].pos ;");
			t.Append("\ndndMgr.dropZones[j].currentobj = dndMgr.draggables[dndMgr.dropZones[j-1].currentobj].pos ;");
			t.Append("\n}");
			t.Append("\nhtmlElement.appendChild(theGUI);");
			t.Append("\ndraggableObjects[0].currentpos = this.pos ;");
			t.Append("\nthis.currentobj = draggableObjects[0].pos ;");
			t.Append("\n}");
			t.Append("\nelse");
			t.Append("\n{");
			t.Append("\nvar init = draggableObjects[0].currentpos ;");
		    t.Append("\nvar fin = verif-1 ;");
            t.Append("\nif (init < fin)");
			t.Append("\nfor( var i=init ; i<fin ; i++)");
			t.Append("\n{");
			t.Append("\nhtmlElement1[i].appendChild(theGUI1[dndMgr.dropZones[i+1].currentobj]);");
			t.Append("\ndndMgr.draggables[dndMgr.dropZones[i+1].currentobj].currentpos = dndMgr.dropZones[i].pos ;");
			t.Append("\ndndMgr.dropZones[i].currentobj = dndMgr.draggables[dndMgr.dropZones[i+1].currentobj].pos ;");
			t.Append("\n}");
			t.Append("\nelse");
		    t.Append("\n{");
 			t.Append("\nfor( var j=init ; j>fin ; j--)");
			t.Append("\n{");
			t.Append("\nhtmlElement1[j].appendChild(theGUI1[dndMgr.dropZones[j-1].currentobj]);");
			t.Append("\ndndMgr.draggables[dndMgr.dropZones[j-1].currentobj].currentpos = dndMgr.dropZones[j].pos ;");
			t.Append("\ndndMgr.dropZones[j].currentobj = dndMgr.draggables[dndMgr.dropZones[j-1].currentobj].pos ;");
			t.Append("\n}");
			t.Append("\n}");
			t.Append("\nhtmlElement1[verif-1].appendChild(theGUI);");
			t.Append("\ndraggableObjects[0].currentpos = dndMgr.dropZones[verif-1].pos ;");
			t.Append("\ndndMgr.dropZones[verif-1].currentobj = draggableObjects[0].pos ;");
			t.Append("\n}");
            t.Append("\n}");

			#region Traitement liste colonnes sélectionnées
			//Mise à jour de la liste des colonnes sélectionnées
			t.Append("\nForm1.columnItemSelectedList.value=\"\";");
			t.Append("\nvar first=0;");
			t.Append("\nfor( var i=0; i<dndMgr.dropZones["+nbColumnItemList+"].nb_elements; i++)");
			t.Append("\n{");
			t.Append("\nif (dndMgr.dropZones[i].isempty==0)");
			t.Append("\n{");
			t.Append("\nif(first==0)");
			t.Append("\n{");
			t.Append("\nForm1.columnItemSelectedList.value+=dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\nfirst=1;");
			t.Append("\n}");
			t.Append("\nelse");
			t.Append("\nForm1.columnItemSelectedList.value+=\",\"+dndMgr.draggables[dndMgr.dropZones[i].currentobj].id;");
			t.Append("\n}");	
       		t.Append("\n}");
			#endregion

			t.Append("\n}");
			t.Append("\n} );");
			#endregion

			#region La classe CustomDraggable
            t.Append("\n////////////////////////////////////////////////////////////////////////////////////////////////");
            t.Append("\n// la Classe des objets draggable");
            t.Append("\nvar CustomDraggable = Class.create();");
            t.Append("\nCustomDraggable.prototype = (new Rico.Draggable()).extend( {");
            t.Append("\ninitialize: function( type, htmlElement, pos, currentpos, id ) {");
            t.Append("\nthis.type          = type;");
            t.Append("\nthis.htmlElement   = $(htmlElement);");
            t.Append("\nthis.selected      = false;");
            t.Append("\nthis.pos    = pos;");
	        t.Append("\nthis.currentpos = currentpos;");
			t.Append("\nthis.id    = id;");
            t.Append("\n},");
            t.Append("\n/**");
            t.Append("\n*   Returns the HTML element that should have a mouse down event");
            t.Append("\n*   added to it in order to initiate a drag operation");
            t.Append("\n*");
            t.Append("\n**/");
            t.Append("\ngetMouseDownHTMLElement: function() {");
            t.Append("\nreturn this.htmlElement;");
            t.Append("\n},");
            t.Append("\nselect: function() {");
            t.Append("\nthis.selected = true;");
            t.Append("\nif ( this.showingSelected )");
            t.Append("\nreturn;");
            t.Append("\nvar htmlElement = this.getMouseDownHTMLElement();");
            t.Append("\nvar color = Rico.Color.createColorFromBackground(htmlElement);");
            t.Append("\ncolor.isBright() ? color.darken(0.033) : color.brighten(0.033);");
            t.Append("\nthis.saveBackground = RicoUtil.getElementsComputedStyle(htmlElement, \"backgroundColor\", \"background-color\");");
            t.Append("\nhtmlElement.style.backgroundColor = color.asHex();");
            t.Append("\nthis.showingSelected = true;");
            t.Append("\n},");
            t.Append("\ndeselect: function() {");
            t.Append("\nthis.selected = false;");
            t.Append("\nif ( !this.showingSelected )");
            t.Append("\nreturn;");
            t.Append("\nvar htmlElement = this.getMouseDownHTMLElement();");
            t.Append("\nhtmlElement.style.backgroundColor = this.saveBackground;");
            t.Append("\nthis.showingSelected = false;");
            t.Append("\n},");
            t.Append("\nisSelected: function() {");
            t.Append("\nreturn this.selected;");
            t.Append("\n},");
            t.Append("\nstartDrag: function() {");
            t.Append("\n},");
            t.Append("\ncancelDrag: function() {");
            t.Append("\nvar theGUI = this.getDroppedGUI();");
            t.Append("\nvar htmlElement = dndMgr.dropZones[this.currentpos].getHTMLElement();");
            t.Append("\nif ( RicoUtil.getElementsComputedStyle( theGUI, \"position\" ) == \"absolute\" )");
            t.Append("\n{");
            t.Append("\ntheGUI.style.position = \"static\";");
            t.Append("\ntheGUI.style.top = \"\";");
            t.Append("\ntheGUI.style.top = \"\";");
            t.Append("\n }");
            t.Append("\nhtmlElement.appendChild(theGUI);");
            t.Append("\n},");
            t.Append("\nendDrag: function() {");
            t.Append("\n},");
            t.Append("\ngetSingleObjectDragGUI: function() {");
            t.Append("\nreturn this.htmlElement;");
            t.Append("\n},");
            t.Append("\ngetMultiObjectDragGUI: function( draggables ) {");
            t.Append("\nreturn this.htmlElement;");
            t.Append("\n},");
            t.Append("\ngetDroppedGUI: function() {");
            t.Append("\nreturn this.htmlElement;");
            t.Append("\n},");
            t.Append("\ntoString: function() {");
            t.Append("\nreturn this.type + \":\" + this.htmlElement + \":\";");
            t.Append("\n}");
            t.Append("\n} );");
			#endregion

			t.Append("\n</SCRIPT>");
			return t.ToString();

		}
		/// <summary>
		/// Permet de generer le script qui instancie les objets Draggable
		/// </summary>
		/// <param name="columnItemList">Liste des colonnes</param>
		/// <returns></returns>
		public static string DragAndDropScript(ArrayList columnItemList)
		{
		
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
		
			t.Append("\n<SCRIPT language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n/////////////////////////////////////////////////////////////////////////////////////////////////");

			int i=0, j=0;
			foreach(GenericColumnItemInformation Column in columnItemList)
			{
				j=(int)Column.Id;
				t.Append("\ndndMgr.registerDraggable( new CustomDraggable('test-rico-dnd','"+(int)Column.Id+"', "+i+", "+i+", "+j+") );");
				t.Append("\ndndMgr.registerDropZone( new CustomDropzone('zone"+(int)Column.Id+"', "+i+", "+i+", 0) );");
				i++;
			}
			t.Append("\ndndMgr.registerDropZone( new CustomDropzoneCorbeille('droponme') );");
			
			t.Append("\n</SCRIPT>");
			return t.ToString();
		}

		/// <summary>
		/// Permet de generer le script qui instancie les objets Draggable après un postback
		/// </summary>
		/// <returns></returns>
		public static string DragAndDropPostBackScript(ArrayList columnItemSelectedList, ArrayList columnItemTrashdList, int nbColumnItemList)
		{
		
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
		
			t.Append("\n<SCRIPT language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n/////////////////////////////////////////////////////////////////////////////////////////////////");

			int i=0, j=0;
			foreach(GenericColumnItemInformation Column in columnItemSelectedList)
			{
				j=(int)Column.Id;
				t.Append("\ndndMgr.registerDraggable( new CustomDraggable('test-rico-dnd','"+(int)Column.Id+"', "+i+", "+i+", "+j+") );");
				t.Append("\ndndMgr.registerDropZone( new CustomDropzone('zone"+(int)Column.Id+"', "+i+", "+i+", 0) );");
				i++;
			}

			foreach(GenericColumnItemInformation Column in columnItemTrashdList)
			{
				j=(int)Column.Id;
				t.Append("\ndndMgr.registerDraggable( new CustomDraggable('test-rico-dnd','"+(int)Column.Id+"', "+i+", "+nbColumnItemList+", "+j+") );");
				t.Append("\ndndMgr.registerDropZone( new CustomDropzone('zone"+(int)Column.Id+"', "+i+", "+i+", 1) );");
				i++;
			}

			t.Append("\ndndMgr.registerDropZone( new CustomDropzoneCorbeille('droponme') );");
			
			t.Append("\n</SCRIPT>");
			return t.ToString();
		}

	}
}
