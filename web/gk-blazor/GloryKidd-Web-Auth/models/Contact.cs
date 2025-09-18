using System;
using System.ComponentModel.DataAnnotations;
using BlazorDynamicForm.Attributes;
//using 


public class ContactForm
{
  [Required, Display(Name = "Name")]
  public required string Name { get; set; }

  [EmailAddress]
  public required string Email { get; set; }

  [Phone, Display(Name = "Phone Number")]
  public required string PhoneNumber { get; set; }

  [TextArea]
  public required string Message { get; set; }
}
