using System;

namespace GloryKidd.WebCore.Helpers {
  [AttributeUsage(AttributeTargets.Enum)]
  public class CustomEnumAttribute : Attribute {
    public bool IsCustomEnum { get; set; }
    public CustomEnumAttribute(bool isCustomEnum) : this() { IsCustomEnum = isCustomEnum; }
    private CustomEnumAttribute() { IsCustomEnum = false; }
  }

  [AttributeUsage(AttributeTargets.Field)]
  class TextValueAttribute : CustomEnumAttribute {
    public string Value { get; set; }
    public TextValueAttribute(string textValue) : this() { Value = textValue ?? throw new NullReferenceException("Null not allowed in textValue at TextValue attribute"); }
    private TextValueAttribute() : base(true) => Value = string.Empty;
  }

  [CustomEnum(true)]
  public enum PageNames {
    [TextValue("Login Page")] Login,
    [TextValue("CGBC Directory")] Home,
    [TextValue("Edit Member")] EditMember,
    [TextValue("Reset Password")] ResetPassword,
    [TextValue("Forgot Password")] ForgotPassword,
  }

}