namespace Core.Interfaces
{
    public interface IEncoder
    {
        public byte[] Encrypt(byte[] plainData);
        public byte[] Decrypt(byte[] encryptedData);
    }
}