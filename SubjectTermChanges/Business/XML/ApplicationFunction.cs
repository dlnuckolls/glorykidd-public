using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class ApplicationFunction
	{
		private string _name;
		private List<string> _allowedRoles;


		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public List<string> AllowedRoles
		{
			get { return _allowedRoles; }
			set { _allowedRoles = value; }
		}

		public ApplicationFunction()
		{
			_allowedRoles = new List<string>();
		}



	}
}
