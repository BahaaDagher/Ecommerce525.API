namespace Ecommerce525.API.Models
{
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool IsValid { get; set; }   
        public DateTime ValidTo { get; set; }   

        public ApplicationUserOTP()
        {

        }
        public ApplicationUserOTP(string OTP , string ApplicationUserId)
        {
            this.OTP = OTP;
            this.ApplicationUserId = ApplicationUserId;
            Id = Guid.NewGuid().ToString();
            CreatedAt =DateTime.UtcNow;
            IsValid = true;
            ValidTo = DateTime.UtcNow.AddMinutes(30);
        }

    }
}
