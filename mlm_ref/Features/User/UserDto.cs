
namespace mlm_ref.Features.User
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string OwnerId { get; set; }

        public string UserRef { get; set; }
        public string ActivationCode { get; set; }
        public string SponsorRef { get; set; }
        public string PlacementRef { get; set; }
        public char Position { get; set; }
    }
}