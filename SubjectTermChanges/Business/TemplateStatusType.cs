using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public enum TemplateStatusType
	{
		Inactive = 1,
		SearchOnly = 2,  //"Cannot create new contracts of this template, but it still appears in search/report selection screens"
		Active = 3
	}

}
