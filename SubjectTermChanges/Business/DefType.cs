using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public enum DefType
	{
		NotAssigned,
		ManagedItem,		//Source is ManagedItem:TemplateDef
		Draft,				//Source is Template:DraftTemplateDef
		Final				//Source is Template:FinalTemplateDef
	}
}
