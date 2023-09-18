using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Directory.CGBC.Objects {
  public class Member {
    public int Id;
    public string PreFix;
    public string FirstName;
    public string MiddleName;
    public string LastName;
    public string Suffix;
    public MaritalStatus MaritalStatus;
    public DateTime MarriageDate;
    public DateTime DateOfBirth;
    public DateTime Modified;
    public DateTime Created;

    public Member() { }

  }
}