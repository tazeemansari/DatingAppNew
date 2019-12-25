using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.Dtos
{

   public class UserForLoginDto
   {
       [Required]
       public string UserName { get; set; }
       [Required]
       [StringLength(8,MinimumLength=4,ErrorMessage="You must provide password between 4 to 8 charachter long")]
       public string Password { get; set; }
      
   }

}
