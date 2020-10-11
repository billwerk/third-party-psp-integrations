namespace Business.Interfaces
{
    public interface IRecurringTokenEncoder<T>
    {
        public string Encrypt(T token);

        public T Decrypt(string base64String);
    }
}