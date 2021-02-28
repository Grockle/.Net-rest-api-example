namespace BackendService.Application.Models.Requests.General
{
    public  abstract class TokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get;  set; }
        public string SecurityKey { get; set; }
    }
}