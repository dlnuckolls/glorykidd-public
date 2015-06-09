var _isChanged = false;
var _focusControlId = '';
var _activeTermGroupId = null;
var _activeTermGroupName = null;

//This function will hide any "SELECT" tags whose current "top" value is less than "minTop" AND current "left" value is at least "minLeft"
//This is so that the dropdown menus (DIV's, SPAN's, and A's) are not obstructed by windowed controls (SELECT tags) which ignore z-indexing rules in IE.
function _kh_ShowWindowedControls(menu)
{
	var o = menu;
	var minLeft = 0;
	var minTop =  o.offsetHeight;
	while (o && o.tagName != "BODY")
	{
		minTop += o.offsetTop - o.offsetParent.scrollTop;
		minLeft += o.offsetLeft -  o.offsetParent.scrollLeft;
		o = o.offsetParent;
	}
	var elements = document.forms[0].elements;
	for (i = 0; i<elements.length; i++)
	{
		var elem = elements[i];
		if (elem.tagName.toUpperCase() == 'SELECT')
		{
			var e = elem;
			var eTop = 0;
			var eLeft = e.offsetWidth;
			while (e && e.tagName != "BODY")
			{
				eTop += e.offsetTop - e.offsetParent.scrollTop;
				eLeft += e.offsetLeft -  e.offsetParent.scrollLeft;
				e = e.offsetParent;
			}
			if (eTop < 30)
				elem.style.visibility = 'visible';
			else
				if ((eTop < minTop) && (eLeft >= minLeft))
					elem.style.visibility = 'hidden';
				else
					elem.style.visibility = 'visible';
		}
	}
}


function FillDataControl(ctlName, value)
{
	if (value == null || value == "&nbsp;" || value == "undefined")
		value = "";
	else
		value = _kh_trim(value);

	var o =document.getElementById(ctlName);
	if (o)
	{
		o.value = value;
	}
	
}


function _kh_areItemsSelected(grdId, chkId) {
	var rtn = false;
	var ec = document.getElementsByTagName('input');
	for (var i = 0; i < ec.length; i++) {
		var e = ec[i];
		if (e.type.toLowerCase() == 'checkbox' && e.checked && e.id.indexOf(grdId) != -1 && e.id.indexOf(chkId) != -1) {
			rtn = true;
			break;
		} /* if */
	} /* for */
	return rtn;
} /* function */


function _kh_deselectListItems(lst)
{
	for (i = 0; i < lst.options.length; i++)
		lst.options[i].selected = false;
}


function _kh_deselectItems(grdId, chkId) {
	var ec = document.getElementsByTagName('input');
	for (var i = 0; i < ec.length; i++) {
		var e = ec[i];
		if (e.type.toLowerCase() == 'checkbox' && e.checked && e.id.indexOf(grdId) != -1 && e.id.indexOf(chkId) != -1) {
			e.checked = false;
		} /* if */
	} /* for */
	return true;
} /* function */

function _kh_getSelectedItemIds(grdId, chkId, lblId) {
	var rtn = new Array();
	var ec = document.getElementsByTagName('input');
	for (var i = 0; i < ec.length; i++) {
		var e = ec[i];
		if (e.type.toLowerCase() == 'checkbox' && e.checked && e.id.indexOf(grdId) != -1 && e.id.indexOf(chkId) != -1) {
			rtn.push(document.getElementById(e.id.substring(0, e.id.length - chkId.length) + lblId).innerText);
		} /* if */
	} /* for */
	return rtn.join('|');
} /* function */

function _kh_limitText(ctlName, length) {
	var ctl = document.getElementById(ctlName);
	if (ctl) {
		if (ctl.value.length > length) {
			ctl.value = ctl.value.substring(0, length);
		} /* if */
	} /* if */
} /* function */

function _kh_onKeyDown(acceptCtlName, cancelCtlName) {
	var rtn = true;
	if (!window.event.altKey && !window.event.ctrlKey && !window.event.shiftKey) {
		if (window.event.keyCode == '13') {
			var acceptCtl = document.getElementById(acceptCtlName);
			if (acceptCtl) {
				acceptCtl.click();
				rtn = false;
			} /* if */
		} /* if */
		else if (window.event.keyCode == '27') {
			var cancelCtl = document.getElementById(cancelCtlName);
			if (cancelCtl) {
				cancelCtl.click();
				rtn = false;
			} /* if */
		} /* else..if */
	} /* if */
	return rtn;
} /* function */



function _kh_onPropertyChange() {
	if (!_isChanged) {
		var e = event.srcElement;
		
		var hasSuppressSet = false;
		try
		{
			var attr = e.attributes.getNamedItem("suppressnotification");
			if (attr)
			    if (attr.value == "true") {
			        hasSuppressSet = true;
			    }
		}
		catch(e)
		{
			hasSuppressSet = false;
		}
		if (hasSuppressSet)
			return;
			
		var et = e.type.toLowerCase();
		if (et == "hidden")
			return;
		var etn = e.tagName.toLowerCase();
		if (et == "file" || et == "text" || etn == "textarea") {
			if (e.style.display != 'none' && escape(e.defaultValue) != escape(e.value)) {
			    _isChanged = true;
} /* if */
		} /* if */
		else if (et == "checkbox" || et == "radio") {
			if (e.defaultChecked != e.checked) {
				_isChanged = true;
} /* if */
		} /* else..if */
		else if (etn == "select") {
			var o = e.options;
			var olen = o.length;
			for (var j = 0; j < olen; j++) {
				if (o[j].defaultSelected != o[j].selected) {
					_isChanged = true;
	            } /* if */
			} /* for */
		} /* else..if */
		if (_isChanged) {
			if (_kh_onChange) {
				 //alert('_kh_onChange called');
                _kh_onChange();
			} /* if */
		} /* if */
	} /* if */
} /* function */

function _kh_setDefaultValue(e) {
	if (e) {
		var etn = e.tagName.toLowerCase();
//		if (etn == "fieldset")
//			return;		
		var et = e.type.toLowerCase();
		if (et == "file" || et == "text" || etn == "textarea") {
			e.defaultValue = e.value;
		} /* if */
		else if (et == "checkbox" || et == "radio") {
			e.defaultChecked = e.checked;
		} /* else..if */
		else if (etn == "select") {
			var o = e.options;
			var olen = o.length;
			for (var j = 0; j < olen; j++) {
				o[j].defaultSelected = o[j].selected;
			} /* for */
		} /* else..if */
		e.onpropertychange = _kh_onPropertyChange;
	} /* if */
	return true;
} /* function */

function _kh_setDefaultValues(frm) {
	if (frm) {
		var elen = frm.elements.length;
		for (var i = 0; i < elen; i++) {
			var e = frm.elements[i];
			var etn = e.tagName.toLowerCase();
			if (etn == "fieldset")
		    {
	            var inputs = e.getElementsByTagName("input");
                for (var j=0;j<inputs.length;j++) {
			        _kh_setDefaultValue(inputs[j]);
			    }
			}
			else
		    {
    			_kh_setDefaultValue(e);
		    }
		} /* for */
	} /* if */
	return true;
} /* function */

function _kh_setFocus(ctlName) {
	var ctl = document.getElementById(ctlName);
	if (ctl) {
		try {
			ctl.focus();
		} /* try */
		catch (ex) {
		} /* catch */
	} /* if */
} /* function */

function _kh_showModalDialog(url, args, w, h) {
	return window.showModalDialog(url, args, 'dialogWidth:' + w.toString() + 'px;dialogHeight:' + h.toString() + 'px;help:no;resizable:yes;scroll:yes;status:yes;');
} /* function */

function _kh_trim(s) {
	return _kh_trimLeft(_kh_trimRight(s));
} /* function */

function _kh_trimLeft(s) 
{
	var rtn = s;
	while (rtn.length > 0 && (rtn.charCodeAt(0) <= 32 || rtn.charCodeAt(0) == 160))
	{
			if (rtn.length > 1)
				rtn = rtn.substr(1);
			else
				rtn = "";
	}
	return rtn;
}


function _kh_trimRight(s) 
{
	var rtn = s;
	while (rtn.length > 0 && (rtn.charCodeAt(rtn.length - 1) <= 32 || rtn.charCodeAt(rtn.length - 1) == 160))
	{
			if (rtn.length > 1)
				rtn = rtn.substr(1);
			else
				rtn = "";
	}
	return rtn;
}


		
function FixFileUploadText(fakeTextBoxID, inputControlID)
{
	var g = document.getElementById(fakeTextBoxID);
	if (!g)
		return;		
	var b = document.getElementById(inputControlID);
	if (!b)
		return;		
	g.value = b.value;
}

function ResizeFileUpload(containerID, fakeTextBoxID, fakeButtonID)
{
	var c = document.getElementById(containerID);
	var b = document.getElementById(fakeButtonID);
	var g = document.getElementById(fakeTextBoxID);
	
	if (g && b && c)
	{
		var w = c.offsetWidth - b.offsetWidth - 210;
		if (w > 0)
			g.style.width = w;
		else
			g.style.width = 1;
	}
}


//This is designed to be put in a clientclick event of a server control (typically a Button).  
//It will change the cursor to an hourglass and "disable" the control to prevent accidentally clicking it again.
//(It actually changes the forecolor of the control to Silver, and superimposes a transparent <div> in front of the control to make it appear disabled.)
function ShowWait(ctl)
{
	document.body.style.cursor='wait';
	if (ctl)
	{
		//ctl.style.visibility=  'hidden';
		ctl.style.color = "Silver";
		var top=0;
		var left=0;
		var p = ctl;
		while (p && p.tagName !="BODY")
		{
			top += (p.offsetTop - p.offsetParent.scrollTop);
			left += (p.offsetLeft - p.offsetParent.scrollLeft);
			p=p.offsetParent;
		}
		var height = ctl.offsetHeight;
		var width = ctl.offsetWidth;
		var div = document.createElement('<div style="position:absolute; z-index:10000; background-color:Silver; filter:alpha(opacity:0); top:' + top + '; left:' + left + '; width:' + width + '; height:' + height + ';" />');
		document.body.appendChild(div);
	}
	return true;
}



var _forcePostBack = false;

//this function is designed to be called on ManagedItemProfile.aspx
function ControlOnFocus()	
{
	_inControlOnFocus = true;
	var ctlGettingFocus = window.event.srcElement;
	if (ctlGettingFocus)
	{
		var gettingFocusTermId = TermID(ctlGettingFocus);
		var ctlLosingFocus = document.getElementById(_focusControlId);
		//determine if a postback will be needed WHEN THE TermID CHANGES
		if (ctlLosingFocus)			
		{		
			var losingFocusTermId = TermID(ctlLosingFocus);
			if (losingFocusTermId)			
				if (IsTermDepSourceControl(losingFocusTermId))		
					if (ValueChanged(ctlLosingFocus))		
						_forcePostBack = true;
		}
		//record the control that just received focus
		if (ctlGettingFocus.id.length > 0)
			_focusControlId = ctlGettingFocus.id;
		_kh_SaveFocus(ctlGettingFocus);

		//determine if a postback is required NOW
		if (_forcePostBack)		
		{
			if (losingFocusTermId != gettingFocusTermId)
			{		
				_kh_SaveScrollPosition();		
				_kh_ForcePostback();
			}
		}
	}
}	


//var _TDSourceTermIDs --  array created by ClientScript in ManagedItemProfile.aspx.cs
function IsTermDepSourceControl(termID)
{
	for (var index = 0; index < _TDSourceTermIDs.length; index++)
		if (_TDSourceTermIDs[index] == termID)
			return true;
	return false;
}


function TermID(ctl)
{
	var c = ctl;
	while (c && c.tagName != 'BODY')
	{
		var termId = c.getAttribute('TermID');
		if (termId != null)
			return termId;
		c=c.parentElement;
	}
	return null;
}


function ValueChanged(ctl)
{
	try
	{
		var et = ctl.type.toLowerCase();
		if (et == "hidden")
			return false;
		var etn = ctl.tagName.toLowerCase();
		
		if (et == "file" || et == "text" || etn == "textarea") 
			return (escape(ctl.defaultValue) != escape(ctl.value));
		
		if (et == "checkbox" || et == "radio") 
			return (ctl.defaultChecked != ctl.checked);

		if (etn == "select") 
		{
			var o = ctl.options;
			var olen = o.length;
			for (var j = 0; j < olen; j++)
				if (o[j].defaultSelected != o[j].selected) 
					return true;
			return false;
		}
	}
	catch(e)
	{
		return false;
	}
}

function _kh_RestoreScrollPosition(hdnField, panel)
{
    var newValue = 0;
	var hf = document.getElementById(hdnField);
	if (hf)
		if (!isNaN(hf.value))
			newValue = hf.value;
	document.getElementById(panel).scrollTop = newValue;
}

function _kh_SaveScrollPosition(hdnField, panel)
{
	var hf = document.getElementById(hdnField);
	if (hf)
		hf.value = document.getElementById(panel).scrollTop;
}


function _kh_RestoreFocus(TermGroupHFID, ControlIdHFID)
{
    if (TermGroupHFID)
    {
        var hfTermGroup = document.getElementById(TermGroupHFID);
        if (hfTermGroup)
        {
            if (hfTermGroup.value.length > 0)
            {
                var termGroupSegments = hfTermGroup.value.split('|');
                DisplayTermGroup(termGroupSegments[0], termGroupSegments[1]);
            }
        }
    }

    var hfControl = document.getElementById(ControlIdHFID);
    if (hfControl)
	{
	    if (hfControl.value.length > 0)
		{
		    var ctl = document.getElementById(hfControl.value);
			if (ctl)
			{
				if (!ctl.disabled)
				{
					try {ctl.focus();}
					catch (ex) {}
				}
			}
		}
	}
}


String.prototype.endsWith = function(str)
{
    return (this.match(str + "$") == str);
}


function _kh_SaveFocus(ctl)
{
    var hfTermGroup = document.getElementById('_kh_hf_FocusTermGroup');
    if (hfTermGroup)
        hfTermGroup.value = _activeTermGroupId + '|' + _activeTermGroupName;
    
	var value = '';
	if (ctl)
	{
	    if (ctl.id.length > 0)
	    {
	        value = ctl.id;
	    }
	}
	//make sure that the control "rolls up" to tblTerms
	var c = ctl;
	while (c && c.tagName != 'BODY')
	{
		if (c.id.endsWith('tblTerms'))
		{
			var hf = document.getElementById('_kh_hf_FocusControl');
			if (hf)
			{
			    hf.value = value;
			} 
			return;
		}
		c=c.parentElement;
	}
}

function _kh_ForcePostback()
{
    if (_focusControlId.length > 0)
        if (document.getElementById(_focusControlId))
            __doPostBack(_focusControlId,'TD');
}


function SelectAllCheckboxes(control, truefalse)
{
	if (control)
	{
		if (control.tagName == 'INPUT')
			if (control.getAttribute('type') == 'checkbox')
				if (control.checked != truefalse)    //				control.checked = truefalse;
					control.click();
		var children = control.children;
		if (children)
			for (var i=0; i<children.length; i++)
				SelectAllCheckboxes(children[i], truefalse);
	}
}


function SetMaxHeaderHeight(id, h)
{
    var o = document.getElementById(id);
    if (o)
    {
        if (o.offsetHeight > h)
        {
            o.style.height = h;
        }
    }
}


function DisplayTermGroup(tgIdToShow, tgName)
{
    var pnlTerms = document.getElementById('pnlTerms');
    if (!pnlTerms)
        return;
    var divs = pnlTerms.getElementsByTagName('div');
    if (!divs)
        return;
    if (divs.length == 0)
        return;
    for (var i = 0; i < divs.length; i++)
    {
        var tgId = divs[i].TermGroupId;
        if (tgId)
        {
            if (tgId == tgIdToShow)
            {
                divs[i].style.display = "block";
                SetBanner(tgName);
            }
            else
            {
                if ((divs[i].TermGroupId == _activeTermGroupId))
                {
                    divs[i].style.display = "none";
                }
            }
        }
    }
    _activeTermGroupId = tgIdToShow;
    _activeTermGroupName = tgName;
}


function radOnClientLoad(edt)
{
  edt.AttachEventHandler("RADEVENT_KEYUP", _kh_onChange);
}

//Here is a list of all the undocumented Rad events, extracted from the RadEditor.Net2.dll:
//"RADEVENT_CALLBACK_STARTED",
//"RADEVENT-MODE_CHANGED",
//"RADEVENT-CONTEXTMENU",
//"RADEVENT_SEL-CHANGED",
//"RADEVENT_SIZE-CHANGED",
//"RADEVENT-BEFORE-EDIT-FOCUS",
//"RADEVENT-KEYDOWN",
//"RADEVENT_KEYUP",
//"RADEVENT_MOUSEDOWN"
//"RADEVENT_MOUSEUP",
//"RADEVENT_CUT",
//"RADEVENT_COPY",
//"RADEVENT_PASTE",
//"RADEVENT_RESIZE_START",
//"RADEVENT_RESIZE_END",
//"RADEVENT_DRAG_START",
//"RADEVENT_DRAG_END",
//"RADEVENT_DROP",

//LRR for Print of search results
function CallPrint(strId)
{
var prtContent = document.getElementById(strId);
var WinPrint = window.open('','','left=10000px,top=10000px,width=3px,height=3px,toolbar=0,scrollbars=0,status=0');
WinPrint.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Global/CSS/AppBody.css"><link rel="stylesheet" type="text/css" href="itat.css"></head><body>');
WinPrint.document.write(prtContent.innerHTML);
WinPrint.document.write('</body></html>');
WinPrint.document.close();
WinPrint.focus();
WinPrint.print();
WinPrint.close();
}
