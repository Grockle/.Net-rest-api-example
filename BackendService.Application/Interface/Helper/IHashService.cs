namespace BackendService.Application.Interface.Helper
{
    public interface IHashService
    {
        string EncryptString(string plainText);

        string DecryptString(string cipherText);

        public string GenerateCode();
    }
}
