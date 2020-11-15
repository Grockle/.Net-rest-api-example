﻿namespace BackendService.Common
{
    public interface ICommonHelper
    {
        string GenerateJwtToken(string id);
        string GenerateCode();
        string SetSecretEmail(string email);
    }
}