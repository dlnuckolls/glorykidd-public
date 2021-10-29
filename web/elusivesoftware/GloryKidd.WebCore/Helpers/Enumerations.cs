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
    [TextValue("Elusive Software")] Home,
    [TextValue("Web Services")] Websites,
    [TextValue("Software Services")] Software,
    [TextValue("Hosting Plans")] Plans,
    [TextValue("Professional Consulting")] Consulting,
    [TextValue("About")] About,
    [TextValue("Contact Us")] Contact,
    [TextValue("Reset Password")] ResetPassword,
    [TextValue("Forgot Password")] ForgotPassword,
    [TextValue("Our Team")] OurTeam,
    [TextValue("Find Events")] Search,
    [TextValue("Questions")] FAQ,
    [TextValue("Blog")] Blog,
    [TextValue("Helpful Links")] Resources,
    [TextValue("Podcasts")] Podcast,
    [TextValue("Technology")] Technology,
    [TextValue("Partners")] Partners,
    [TextValue("Admin Dashboard")] Admin,
  }

}