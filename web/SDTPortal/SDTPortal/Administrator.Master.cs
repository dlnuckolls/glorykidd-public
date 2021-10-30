using System;
using System.Reflection;
namespace SDTPortal {
  public partial class Administrator :BaseMasterPage {
    protected void Page_Load(object sender, EventArgs e) { BuildNumber.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
  }
}