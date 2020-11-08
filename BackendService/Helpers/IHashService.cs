namespace BackendService.Helpers
{
    public interface IHashService
    {
        string EncryptString(string plainText);

        string DecryptString(string cipherText);

        public string GenerateCode();
    }
}
