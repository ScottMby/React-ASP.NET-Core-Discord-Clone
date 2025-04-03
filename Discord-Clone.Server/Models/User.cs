using Microsoft.AspNetCore.Identity;

namespace Discord_Clone.Server.Models
{
    public class User : IdentityUser
    {
        public string? DisplayName { get; set; }

        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }


        public string? PhotoURL { get; set; }

        public string? AboutMe { get; set; }

        public List<UserFreinds> UserFreinds = new();

    }
}
