using System;

namespace Kindred.Knect.ITAT.Business
{
    public class Retro
    {
        public enum RetroModel
        {
            [Utility.EnumDescription("Off")]
            Off = 0,
            [Utility.EnumDescription("With Edit Language")]
            OnWithEditLanguage,
            [Utility.EnumDescription("Without Edit Language")]
            OnWithoutEditLanguage
        }

        public enum AuditType
        {
            None = 0,       //Indicates that it is not actually used
            Created,
            Saved,
            RollBack,
            Promoted,
            Orphaned,
            RetroWithEditLanguage,
            RetroWithoutEditLanguage
        }

		public struct RetroData
		{
            public DateTime? TemplateRetroDate;
            public Retro.AuditType AuditType;
            public DateTime AuditDate;
            public Guid StateID;
			public string TemplateDef;
		}

    }
}
