using System;

namespace BackendService.Domain.Entity
{
    public class User : AuditableBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }
        public string EmailVerificationCode { get; set; }
        public DateTime? VerificationEndTime { get; set; }
        public string PasswordResetCode { get; set; }
        public DateTime? ResetCodeEndTime { get; set; }
        public string SecretEmail { get; set; }
        public int AccessFailedCount { get; set; }
        public string Token { get; set; }
    }
}
