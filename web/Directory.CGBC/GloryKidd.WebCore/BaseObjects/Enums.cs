using GloryKidd.WebCore.Helpers;

namespace Directory.CGBC.Objects {

  //[CustomEnum(true)]
  //public enum MaritalStatus {
  //  [TextValue("Unknown")] Unknown = 1,
  //  [TextValue("Single")] Single = 2,
  //  [TextValue("Separated")] Separated = 3,
  //  [TextValue("Married")] Married = 4,
  //  [TextValue("Divorced")] Divorced = 5,
  //  [TextValue("Widowed")] Widowed = 6,
  //}

  [CustomEnum(true)]
  public enum Gender {
    [TextValue("Male")] Male = 0,
    [TextValue("Female")] Female = 1,
  }
}