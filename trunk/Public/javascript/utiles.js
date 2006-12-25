function loadCalendar(formId, outputName)
{
	var cal = new calendar1(document.forms[formId].elements[outputName]);
	cal.year_scroll = false;
	cal.time_comp = true;
	cal.popup();
}

/*function loadCalendars(formId, startId, endId)
{
   var calStartDate = new calendar1(document.forms[formId].elements[startId]);
   calStartDate.year_scroll = false;
   calStartDate.time_comp = true;
   var calEndDate = new calendar1(document.forms[formId].elements[endId]);
   calEndDate.year_scroll = false;
   calEndDate.time_comp = true;
}*/

function tinyRepaint() {
    var textarea_num = document.getElementsByTagName("textarea").length
    var textarea;
    for (var i = 0; i <= textarea_num; i++)    {       
        textarea = document.getElementsByTagName("textarea")[i];
        if (textarea != null)    {
				inst = tinyMCE.getInstanceById(textarea.id);
				if (!inst) 
            	//tinyMCE.execCommand("mceRemoveControl", false , textarea.id);
            	tinyMCE.execCommand("mceAddControl", false , textarea.id);
				else
		      	tinyMCE.execCommand('mceResetDesignMode', false, textarea.id);

        }
		 
    }
}

function updateValues() {
// manually save the tinyMCE content for each textarea
	var tas = document.getElementsByTagName('textarea');
	for(var i = 0; i < tas.length; i++) {
	    // snag the textarea
  	  var ta = tas[i];
  	  // put focus in the editor
     inst = tinyMCE.getInstanceById(ta.id);
     if (inst)
     { 
  	     tinyMCE.execInstanceCommand(ta.id, 'mceFocus');
  	     // manually save the content
  	     ta.value = tinyMCE.getContent();
        tinyMCE.execCommand("mceRemoveControl", false , ta.id);
	     ta.style.visible = "hidden";
     }
	}

}



function changeValue(id, value)
{
	element = document.getElementById(id);
	element.value = value;
}

function getAllSelect (select, stringName)
{
	selectToString(this, select, stringName);
	//form.submit();
}

function getSelected ()
{
	selectToString(this, 'in', 'selectedPerm');
	form.submit();
}

function reloadContent(div, controler)
{
	new Ajax.Updater(div, controler, {asynchronous:true, evalScripts:true, parameters:'void=0'}); 
}

function reloadMenu(id, div, controler)
{
	new Ajax.Updater(div, controler, {asynchronous:true, evalScripts:true, parameters:'parent='+id}); 
}

function reloadSubmenu(parentId, div, controler, siteRoot)
{
	imgsrc = "img"+ parentId;
	openimg = document.getElementById(imgsrc);
	if (openimg.src.indexOf('plus') != -1)
	{
		openimg.src = siteRoot+"/Public/images/minus.gif";
		new Ajax.Updater(div, controler, {asynchronous:true, evalScripts:true, parameters:'parent='+parentId}); 
	}
	else
	{
		div = document.getElementById(div);
		div.innerHTML = "";
		openimg.src = siteRoot+"/Public/images/plus.gif";
	}
	return false;
}

function reloadGenericSubmenu(parentId, div, controler, siteRoot, div2update)
{
	imgsrc = "img"+ parentId;
	openimg = document.getElementById(imgsrc);
	if (openimg.src.indexOf('plus') != -1)
	{
		openimg.src = siteRoot+"/Public/images/minus.gif";
		new Ajax.Updater(div, controler, {onLoading:function(request) { waitPointer('content')}, onComplete:function(request) { javascript:defaultPointer('content')}, asynchronous:true, evalScripts:true, parameters:'parent='+parentId}); 
	}
	else
	{
		div = document.getElementById(div);
		div.innerHTML = "";
		openimg.src = siteRoot+"/Public/images/plus.gif";
	}
	return false;
}

function reloadTree(id, div, controler, siteRoot)
{
	imgsrc = "img"+ id;
	openimg = document.getElementById(imgsrc);
	if (openimg.src.indexOf('plus') != -1)
	{
		openimg.src = siteRoot+"/Public/images/minus.gif";
		new Ajax.Updater(div, controler, {asynchronous:true, evalScripts:true, parameters:'id='+id+'&layout=false'}); 
	}
	else
	{
		div = document.getElementById(div);
		div.innerHTML = "";
		openimg.src = siteRoot+"/Public/images/plus.gif";
	}
	return false;
}

function addSelectItem(idFrom, idTo)
{
	from = document.getElementById(idFrom);
	to = document.getElementById(idTo);
	options = from.getElementsByTagName("option");
	j = 0;
	aux = options.length;
	for (i=0; i < aux; i++)
	{
		if (options[j].selected == true)
		{
			newOption = document.createElement("option");
			newOption.value = options[j].value;
			newOption.text = options[j].text;
                        newOption.selected = true;
			to.appendChild(newOption);
		}
		j++;
	}
}

function removeSelectItem(idFrom)
{
	from = document.getElementById(idFrom);
	options = from.getElementsByTagName("option");
	j = 0;
	aux = options.length;
	for (i=0; i < aux; i++)
	{
		if (options[j].selected == true)
		{
			from.removeChild(options[j]);
		}
		else
			j++;
	}
}

function removeSelectItemCascade(idFrom, idFrom2, idFrom3)
{
	from = document.getElementById(idFrom);
	from2 = document.getElementById(idFrom2);
	from3 = document.getElementById(idFrom3);
	options = from.getElementsByTagName("option");
	options2 = from2.getElementsByTagName("option");
	options3 = from3.getElementsByTagName("option");
	j = 0;
	aux = options.length;
	for (i=0; i < aux; i++)
	{
		if (options[j].selected == true)
		{
			h = 0;
			aux2 = options2.length;
			for (k=0; k < aux2; k++)
				if (options2[h].value == options[j].value)
					from2.removeChild(options2[h]);
				else
					h++
			h = 0;
			aux3 = options3.length;
			for (k=0; k < aux3; k++)
				if (options3[h].value == options[j].value)
					from3.removeChild(options3[h]);
				else
					h++
			from.removeChild(options[j]);
		}
		else
			j++;
	}
}

function changeSelectItem(idFrom, idTo)
{
	from = document.getElementById(idFrom);
	to = document.getElementById(idTo);
	options = from.getElementsByTagName("option");
	j = 0;
	aux = options.length;
	for (i=0; i < aux; i++)
	{
		if (options[j].selected == true)
		{
			newOption = document.createElement("option");
			newOption.value = options[j].value;
			newOption.text = options[j].text;
			to.appendChild(newOption);
			from.removeChild(options[j]);
		}
		else
			j++;
	}
}

function selectToString (form, select, string)
{
	options = document.getElementById(select);
	str = document.getElementById(string);
	for (i = 0; i < options.length; i++)
		str.value = str.value+options[i].value+":";	
}

function addRow(id)
{
	var tbody = document.getElementById(id).getElementsByTagName("TBODY")[0];

	var row = document.createElement("TR");
	var td1 = document.createElement("TD");
	
	var input01 = document.createElement("SELECT");
	input01.name="header";
	input01.onChange = "alert('DEBUG')";

	var option01 = document.createElement("OPTION");
	option01.value="FILA 01";
	option01.text ="FILA 01";
	var option02 = document.createElement("OPTION");
	option02.value="FILA 02";
	option02.text ="FILA 02";

	input01.appendChild(option01);
	input01.appendChild(option02);
	td1.appendChild(input01);

	row.appendChild(td1);
	tbody.appendChild(row);

}

function show (id)
{
	element = document.getElementById(id);
	element.style.visible = 'visible';
}

function hide (id)
{
	element = document.getElementById(id);
	element.style.visible = 'hidden';
}

function addOption(id_select, text, value)
{
	var select = document.getElementById(id_select);
	var new_option = document.createElement("OPTION");
	new_option.text = text + value;
	new_option.value = value;
	select.appendChild(new_option);
}

function resetSelect(id_select)
{
	var select = document.getElementById(id_select);

	while (select.hasChildNodes())
		select.removeChild(select.childNodes[0]);
}

function checkAll (form, id)
{
	var objForm = form;
	for(i=0;i < objForm.num_users.value;i++)
	{
		aux = document.getElementById (id+i);
		aux.checked = !(aux.checked);
	}
}

function getDate ()
{
	var mydate=new Date();
	var year=mydate.getYear();
	if (year < 1000)
		year+=1900;
	
	var day=mydate.getDay();
	var month=mydate.getMonth();
	var daym=mydate.getDate();
	if (daym<10)
		daym="0"+daym;

	var dayarray=new Array("Domingo","Lunes","Martes","Miercoles","Jueves","Viernes","Sabado");
	var montharray=new Array("Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre");
	return (dayarray[day]+" "+daym+" de "+montharray[month]+" de "+year);
}

/*
Avoid content with a greater height than the container
intern - the internal content
cont   - the childContent (monorail)
maincontainer - the document body or main div
*/
function pushDown(intern, cont, maincontainer, offset) 
{
	//alert("intern=" + intern);
	var margin = 20;
	var footer = 100;
	var internal = document.getElementById(intern);
	var container = document.getElementById(cont);
	var mainc = document.getElementById(maincontainer);
	try
	{
		if (internal.offsetHeight > mainc.offsetHeight)
		{
			mainc.style.height = internal.offsetHeight + offset + footer + 'px';
		}
		else if (internal.offsetHeight + margin > container.offsetHeight)
		{
			mainc.style.height = mainc.offsetHeight + 
				(internal.offsetHeight + margin - container.offsetHeight) + 'px';
		}
	}
	catch (e)
	{
		//alert("error!:" +e);
	}
}

function setGreaterHeight(div1, div2) 
{
	var div1 = document.getElementById(div1);
	var div2 = document.getElementById(div2);
	//alert("set greater=" + div1.offsetHeight +","+ div2.offsetHeight);
	try
	{
		if (div1.offsetHeight > div2.offsetHeight)
			div2.style.height = div1.offsetHeight + 20 + 'px';
		else
		{
			div1.style.height = div2.offsetHeight + 20 + 'px';	
			//alert ("new height = " + div1.offsetHeight);
		}
	}
	catch (e)
	{
		//alert("error!:" +e);
	}
}

