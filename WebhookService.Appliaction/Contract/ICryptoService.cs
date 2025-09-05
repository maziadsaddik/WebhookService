namespace WebhookService.Appliaction.Contract;

public interface ICryptoService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string GenerateSecret();
    string ComputeSignature(string secret, string payload);
}
