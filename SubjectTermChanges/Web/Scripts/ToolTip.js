function ShowItatToolTip(ev,pnlName,msg)
{
	var div=document.getElementById(pnlName);
	if (div)
	{
		div.innerText=msg;
		div.style.position="absolute";
		div.style.visibility="visible";
		div.style.display="inline";
		div.style.left=(ev.clientX + 12) + "px";
		div.style.top=(ev.clientY + 15) + "px";
	}
}

function MoveItatToolTip(ev, pnlName)
{
	var div=document.getElementById(pnlName);
	if (div)
	{
		if (div.style.visibility == "visible")
		{
			div.style.left=(ev.clientX + 12) + "px";
			div.style.top=(ev.clientY + 15) + "px";
		}
	}
}
      
function HideItatToolTip(pnlName)
{
	var div=document.getElementById(pnlName);
	if (div)
	{
		div.style.visibility="hidden";
		div.style.display="none";
	}
}
