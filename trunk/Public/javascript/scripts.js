;

/* *****************************************************
   Javascript for datecalendar
*/

function cal3(name)
{
    Calendar.setup({
        inputField     :    "f_date_c" + name,     // id of the input field
        ifFormat       :    "%d/%m/%Y",      // format of the input field
        showsTime      :    false,            // will display a time selector
        button         :    "f_trigger_c" + name,  // trigger for the calendar (button ID)
        align          :    "Tl",           // alignment (defaults to "Bl")
        singleClick    :    true,
        firstDay       :    1
    });
}

/* *****************************************************
   Javascript for chat
*/

function resetChatEnter()
{
   input2 = document.getElementById("chat2");
   input = document.getElementById("chat");
   input2.value = input.value;
   input.value = "";

}

function reloadMessage(updateDiv, method, idchat, idmsg)
{
   updateDiv.scrollTop = updateDiv.scrollHeight;
   params = 'idchat=' + idchat + '&idmessage=' + idmsg;
   try {
      new Ajax.Updater(updateDiv, method,
         { asynchronous:true, evalScripts:true, parameters:params});
   }
   catch (e)
   {
      alert('Please select an user first:' + e);
   }
}
      
/* *****************************************************
	Javascript for scheduled
*/
function callCal(obj)
{
   var cal = new calendar1(obj);
   cal.year_scroll = false;
   cal.time_comp = true;
	cal.popup()
}

function createDateTime()
{
	createStartDateTime();
	createEndDateTime();
}

function createStartDateTime()
{
	var event_StartDate = document.getElementsByName("Event.StartDate")[0];
	var date =  event_StartDate.value;
	var hour = document.getElementsByName("hourStartDate")[0].value;
	var minute = document.getElementsByName("minuteStartDate")[0].value;

	var dateTime = "" + date + " " + hour + ":" + minute;
	event_StartDate.value = dateTime;
}

function createEndDateTime()
{
	var event_EndDate = document.getElementsByName("Event.EndDate")[0];
	var date =  event_EndDate.value;
	var hour = document.getElementsByName("hourEndDate")[0].value;
	var minute = document.getElementsByName("minuteEndDate")[0].value;

	var dateTime = "" + date + " " + hour + ":" + minute;
	event_EndDate.value = dateTime;
}

function splitDateTime()
{
	var event_StartDate = document.getElementsByName("Event.StartDate")[0];
	var datetime = event_StartDate.value;
	if (datetime)
	{
		var temp = datetime.split(" ");
		var time = temp[1].split(":");
		var hourStartDate = document.getElementsByName("hourStartDate")[0];
		var minuteStartDate = document.getElementsByName("minuteStartDate")[0];
		event_StartDate.value = temp[0];
		hourStartDate.value = time[0];
		minuteStartDate.value = time[1];
	}

	var event_EndDate = document.getElementsByName("Event.EndDate")[0];
	datetime = event_EndDate.value;
	if (datetime)
	{
		temp = datetime.split(" ");
		time = temp[1].split(":");
		var hourEndDate = document.getElementsByName("hourEndDate")[0];
		var minuteEndDate = document.getElementsByName("minuteEndDate")[0];
		event_EndDate.value = temp[0];
		hourEndDate.value = time[0];
		minuteEndDate.value = time[1];
	}
}

function toggleModify(id)
{
   field = document.getElementById(id)
   if (field)
   {
      if (field.value)
         field.value = "1";
   }
}

/* *****************************************************
	Javascript for Internal Windows

    // Window x co-ordinate (relative to the viewport)
    windowX=48;

    // Window y co-ordinate (relative to the viewport)
    windowY=48;

    // Window width
    windowWidth=512;

    // Window height
    windowHeight=400;

    // Window padding
    windowPadding=48;

    // Window offset
    offset=0;

    // Speed of change in window offset
    offsetSpeed=0;

    // Vertical scrollbar position
    scrollbarY=0;

    // Mouse x co-ordinate (relative to the viewport)
    mouseX=0;

    // Mouse y co-ordinate (relative to the viewport)
    mouseY=0;

    // Whether the window is currently visible
    windowVisible=false;

    // Whether the window is currently selected
    windowSelected=true;

    // Whether the window is currently being moved
    beingMoved=false;

    // Mouse x co-ordinate (relative to left of window) when move began
    moveMouseX=0;

    // Mouse y co-ordinate (relative to top of window) when move began
    moveMouseY=0;

    // Whether the window is currently being resized
    beingResized=false;

    // Mouse x co-ordinate (relative to right of window) when resize began
    resizeMouseX=0;

    // Mouse y co-ordinate (relative to bottom of window) when resize began
    resizeMouseY=0;

    // Whether close has been pressed
    closePressed=false;

    // Begins the move process, by finding the mouse position
    function beginMove(){
      beingMoved=true;
      moveMouseX=mouseX-windowX;
      moveMouseY=mouseY-windowY;
    }

    // Begins the resize process, by finding the mouse position
    function beginResize(){
      beingResized=true;
      resizeMouseX=windowX+windowWidth-mouseX;
      resizeMouseY=windowY+windowHeight-mouseY;
    }

    // Called when the mouse is pressed down on the close button
    function mouseDownClose(){
      closePressed=true;
      document.getElementById('close').style.background=
          'url(\'../Public/images/close_down.png\')';
    }

    // Called when the mouse moves over the close button
    function mouseOverClose(){
      if (closePressed){
        document.getElementById('close').style.background=
            'url(\'../Public/images/close_down.png\')';
      }
    }

    // Called when the mouse moves off the close button
    function mouseOutClose(){
      if (closePressed){
        document.getElementById('close').style.background=
            'url(\'../Public/images/close.gif\')';
      }
    }

    // Called when the mouse is released on the close button
    function mouseUpClose(){
      if (closePressed){
        document.getElementById('internalWindow').style.display='none';
        windowVisible=false;
        closePressed=false;
        document.getElementById('close').style.background=
            'url(\'../Public/images/close.gif\')';
      }
    }

    // Displays the window
    function displayWindow(){
      document.getElementById('internalWindow').style.display='block';
      windowVisible=true;
    }

    // Checks whether the window should be selected or deselected
    function checkSelection(){
      if ((mouseX>=windowX+windowPadding)&&
          (mouseX<windowX+windowWidth+windowPadding)&&
          (mouseY>=windowY+windowPadding)&&
          (mouseY<windowY+windowHeight+windowPadding)){
        if (!windowSelected) selectWindow();
      }else{
        if (windowVisible) deselectWindow();
      }
    }

    // Selects the window
    function selectWindow(){
      windowSelected=true;
      document.getElementById('iWindowTitle').style.background=
          'url(\'../Public/images/title.png\')';
      document.getElementById('top').style.background=
          'url(\'../Public/images/titlebar.gif\')';
      if (!closePressed){
        document.getElementById('close').style.background=
            'url(\'../Public/images/close.gif\')';
      }
    }

    // Deselects the window
    function deselectWindow(){
      windowSelected=false;
      document.getElementById('iWindowTitle').style.background=
          'url(\'../Public/images/title_unselected.png\')';
      document.getElementById('top').style.background=
          'url(\'../Public/images/top_unselected.png\')';
      document.getElementById('close').style.background=
          'url(\'../Public/images/close_unselected.png\')';
    }

    // Deselectes all icons when the mouse button is released
    function deselectAll(){
      beingMoved=false;
      beingResized=false;
      closePressed=false;
    }

    // Updates the variables storing the mouse co-ordinates
    function mouseMoved(pageX,pageY,clientX,clientY){
      if (pageX){
        mouseX=pageX;
        mouseY=pageY-scrollbarY;
      }else if (clientX){
        mouseX=clientX;
        mouseY=clientY;
      }
    }

    // Updates the variable storing the vertical scrollbar co-ordinate
    function updateVerticalScrollbar(){
      if (window.pageYOffset){
        scrollbarY=window.pageYOffset;
      }else if (document.documentElement && document.documentElement.scrollTop){
        scrollbarY=document.documentElement.scrollTop;
      }else if (document.body){
        scrollbarY=document.body.scrollTop;
      }
    }

    // Updates the window offset and moves the window accordingly
    function updWindowOffset(){
      if (offset>scrollbarY){
        if (offset-scrollbarY<-offsetSpeed*(1-offsetSpeed)/2) offsetSpeed++;
        if (offset-scrollbarY>(1-offsetSpeed)*(2-offsetSpeed)/2) offsetSpeed--;
      }
      if (offset<scrollbarY){
        if (scrollbarY-offset<offsetSpeed*(offsetSpeed+1)/2) offsetSpeed--;
        if (scrollbarY-offset>(offsetSpeed+1)*(offsetSpeed+2)/2) offsetSpeed++;
      }
      if (offset==scrollbarY) offsetSpeed=0;
      offset+=offsetSpeed;
      iwindow = document.getElementById('internalWindow');
		if (iwindow)
			iwindow.style.top=windowY+offset+'px';
    }

    // Moves the window
    function updateWindowPosition(){
      windowX=mouseX-moveMouseX;
      windowY=mouseY-moveMouseY;
      document.getElementById('internalWindow').style.left=windowX+'px';
      document.getElementById('internalWindow').style.top=windowY+offset+'px';
    }

    // Resizes the window
    function updateWindowSize(){
      windowWidth=Math.min(668,Math.max(256,mouseX-windowX+resizeMouseX));
      windowHeight=Math.min(450,Math.max(128,mouseY-windowY+resizeMouseY));
      document.getElementById('top').style.width=windowWidth-245+'px';
      document.getElementById('left').style.height=windowHeight-50+'px';
      document.getElementById('imagePane').style.width=windowWidth-12+'px';
      document.getElementById('imagePane').style.height=windowHeight-50+'px';
      document.getElementById('right').style.height=windowHeight-50+'px';
      document.getElementById('bottom').style.width=windowWidth-26+'px';
    }

    // Updates the position and size of the window
    function updateWindow() {
      updateVerticalScrollbar();
      updWindowOffset();
      if (beingMoved) updateWindowPosition();
      if (beingResized) updateWindowSize();
    }

    // Interval to update the window
    window.setInterval('updateWindow();',20);




/* *************************************************** 
*/


/*
 element is a div id
*/
function waitPointer(element) {
	try {
		body = document.getElementById("body");
		body.style.cursor = 'wait';
		e = document.getElementById(element);
		if (e)
			recursiveSet(e, 'wait');
	}
	catch (e)
	{
		//alert("body id not found,  <body id='body'> dont exists"); Calladito estas mejor  
	}
}
function defaultPointer(element) {
	try {
		body = document.getElementById("body");
		body.style.cursor = 'default';
		e = document.getElementById(element);
		if (e)
			recursiveSet(e, 'default');
	}
	catch (e)
	{
		//alert("body id not found,  <body id='body'> dont exists"); Callate hombre 
	}
}

/*
DANGER!!! - Too slow code when first node is document.documentElement or body.
 Use it only for little elements with a few elements inside 
*/
function recursiveSet(node, mode){
	max_nodes = 2;	// do not explore if node has more than max_nodes childs
	if (node)
	{
		if (node.style)
			node.style.cursor = mode;
		if ((node.hasChildNodes()) && (node.childNodes.length < max_nodes))
			for (i = 0; i < node.childNodes.length; i++)
				if (node.childNodes.item(i))
					recursiveSet(node.childNodes.item(i), mode);
	}
}

/*
 This is for groupsedit.vm. The CreateGroup div may exist and be hidden or may not 
 exist. If not exist is because ModifyGroup exist, so controller must reload view
 thru AJAX
*/
function showCreateGroup(updateDiv, createDiv, method, params)
{
   divCreate = document.getElementById(createDiv);
	if (!divCreate)
	{
		new Ajax.Updater(updateDiv, method, 
			{onComplete:function(request) 
               { 
                  javascript:showCreateGroup(updateDiv, createDiv, method)   
               } , 
			asynchronous:true, evalScripts:true, parameters: params}); 
	}
	else	
     divCreate.style.visibility = 'visible';
}

function addUser(updateDiv, method, id, gid, gindex, uindex, element)
{
	if (gid)
	{
		userSelect = document.getElementById(id);	
		groupSelect = document.getElementById(gid); 
		try {
		   user  = userSelect.options[userSelect.selectedIndex].value; // FIXME: catch exception
		   group = groupSelect.options[groupSelect.selectedIndex].value;
		   params = "gid=" + group + "&uid=" + user + "&gindex=" + gindex + "&uindex=" + uindex;
			new Ajax.Updater(updateDiv, method, 
              {onLoading:function(request) { 
                    javascript:waitPointer(element) 
              } , 
              onComplete:function(request) { 
                    javascript:defaultPointer(element) 
              } , asynchronous:true, evalScripts:true, parameters:params});
      }
		catch (e)
		{
			alert('Primero seleccione un usuario del sistema:' + e);
		}
	}
	else
		alert('Por favor, selecciona un grupo primero: ' +e);
}

function delUser(updateDiv, method, id, gid, gindex, uindex, element)
{
	if (gid)
	{
		userSelect = document.getElementById(id);	
		groupSelect = document.getElementById(gid); 
		try {
		   user  = userSelect.options[userSelect.selectedIndex].value; // FIXME: catch exception
		   group = groupSelect.options[groupSelect.selectedIndex].value;
		   params = "gid=" + group + "&uid=" + user + "&gindex=" + gindex + "&uindex=" + uindex;
			new Ajax.Updater(updateDiv, method, 
              {onLoading:function(request) { 
                    javascript:waitPointer(element) 
              } , 
              onComplete:function(request) { 
                    javascript:defaultPointer(element) 
              } , asynchronous:true, evalScripts:true, parameters:params});
      }
		catch (e)
		{
			alert('Primero seleccione un usuario del grupo');
		}
	}
	else
		alert('Por favor, selecciona un grupo primero');
}

// creo que estas dos no se usan, borrarlas y ver nota siguiente
function Show(hiddenField, eNoEditor, eEditor)
{
	document.getElementById(eEditor).style.display	= '' ;
	document.getElementById(eNoEditor).style.display	= 'none' ;
}

function Hide(hiddenField, eNoEditor, eEditor)
{
	document.getElementById(eEditor).style.display	= 'none' ;
	document.getElementById(eNoEditor).style.display	= '' ;
}

// las otras dos se podrían unificar como esta
function ShowHide(show, hide)
{
	document.getElementById(show).style.display	= '' ;
	document.getElementById(hide).style.display	= 'none' ;
}

function AlternateShowHide(id)
{
	var elem = document.getElementById(id);
	if (elem.style.display == 'none')
		elem.style.display = '';
	else
		elem.style.display = 'none';
}


