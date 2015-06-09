using System;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class TermRuntimeSettings
	{
		#region private fields
		private bool _enabled;
		private bool _visible;
		private bool _required;
		private bool _hasError;
		private string _setValue;
		private string _errorMessage;
        private bool _migrated = false; //Used for Retro operations

		#endregion

		#region properties
		
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public bool Required
		{
			get { return _required; }
			set { _required = value; }
		}

		public bool HasError
		{
			get { return _hasError; }
			set { _hasError = value; }
		}

		public string SetValue
		{
			get { return _setValue; }
			set { _setValue = value; }
		}

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}

        public bool Migrated
        {
            get { return _migrated; }
            set { _migrated = value; }
        }

		#endregion


		#region public methods

		public void Reset(bool _canEditProfile, bool _termRequired)
		{
			_enabled = _canEditProfile;
			_visible = true;
			_required = _termRequired;
			_setValue = null;
			//NOTE: The HasError property does not get reset.
		}

        public void Clear(bool _canEditProfile, bool _termRequired)
        {
            _enabled = _canEditProfile;
            _visible = true;
            _required = _termRequired;
            _setValue = null;
            _hasError = false;
            _errorMessage = null;
            _migrated = false;
        }

		#endregion


	}
}
