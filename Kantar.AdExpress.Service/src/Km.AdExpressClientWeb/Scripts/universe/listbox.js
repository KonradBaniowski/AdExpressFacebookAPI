function GetHtmlSelectableFromObject(datas, id, className){

		var sublist = '';
		
		if(datas.length>0){
			var grp = $('<ul/>');
			grp.attr('id',id);
			if(className!=undefined)
				grp.addClass(className);
			for(var i=0; i<datas.length; i++){
				var item = $('<li/>');
				item.addClass('ui-widget-content');
				item.val(datas[i].Id);
				item.text(datas[i].Text);
				
				item.appendTo(grp);
			}
			return grp;
		}
		else{
			return undefined;
		}
	  }
	  
	  jQuery.fn.extend({
		fillSelectable:function(datas, id, className) {
			var html = GetHtmlSelectableFromObject(datas, id, className);
			if(html!=undefined)
				html.appendTo(this);
			else
				this.html('');
		},
		fillGroupSelectable:function(titleText, datas, classNameTitle, classNameDivSelection, idSelection, classNameSelection, nbElemMax, nbElemMaxText) {

			var divHtmlList = $('<div/>');
			if(classNameDivSelection!=undefined)
				divHtmlList.addClass(classNameDivSelection);
			
			var nbElemHtml = $('<span/>');

			var listElem = undefined;
			var warningHtml = undefined;
			if(datas.length>nbElemMax)
			{
				listElem = GetHtmlSelectableFromObject(datas.slice(0, nbElemMax), idSelection, classNameSelection);
				nbElemHtml.text(nbElemMax);
				if(nbElemMaxText!=undefined){
					warningHtml = $('<small/>');
					warningHtml.text(nbElemMaxText.replace('{NB_ELEM_MAX}', nbElemMax).replace('{NB_ELEM}', datas.length));
				}
			}	
			else{
				nbElemHtml.text(datas.length);
				listElem = GetHtmlSelectableFromObject(datas, idSelection, classNameSelection);
			}	
			
			titleText = titleText.replace('{NB_ELEM}', $("<div>").append(nbElemHtml.clone()).html());
			
			var divTitleHtml = $('<div/>');
			if(classNameTitle!=undefined)
				divTitleHtml.addClass(classNameTitle);
			divTitleHtml.append(titleText);
			
			if(warningHtml!=undefined)
				divTitleHtml.append(warningHtml);
			divTitleHtml.appendTo(this);

			if(listElem!=undefined)
			{
				listElem.appendTo(divHtmlList);
			}
			
			divHtmlList.appendTo(this);
		},
		getSelectableSelectedItems:function(){
			var result = [];
			$( ".ui-selected", this ).each(function() {
					var val = $(this).val();
					var text = $(this).text();
					result.push({Value:$(this).val() , Text:$(this).text()})
				});
			return result;
		
		},
		getSelectableSelectedItems2:function(){
			var result = [];
			var items = this[0].getElementsByTagName("li");

			for (var i = items.length; i--;) {
				if((' ' + items[i].className + ' ').indexOf(' ui-selected ') > -1){
					result.push({Value:items[i].value , Text:items[i].innerHTML})
				}
			}
			return result;
		}
	  });