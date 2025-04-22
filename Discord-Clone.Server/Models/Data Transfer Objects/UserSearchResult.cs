namespace Discord_Clone.Server.Models.Data_Transfer_Objects
{
    public class UserSearchResult
    {
        public required string Id { get; set; }

        public required string DisplayName { get; set; }

        public required string PhotoURL { get; set; }

        public required float Rank { get; set; }

    }
}
