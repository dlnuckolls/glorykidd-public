using System;

namespace Kindred.Knect.ITAT.Utility
{
    public sealed class EnumDescriptionAttribute : Attribute
    {

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
        }


        public EnumDescriptionAttribute(string description)
            : base()
        {
            this._description = description;

        }
    }
}
