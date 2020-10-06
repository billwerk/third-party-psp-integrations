using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Core.Interfaces;

namespace Core.Services
{
    public class Encoder : IEncoder
    {
        private readonly RijndaelManaged _symmetricKey = new RijndaelManaged
        {
            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            Mode = CipherMode.CBC
        };
        
        private readonly byte[] _keyBytes;
        
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged must be exactly 16 ASCII bytes long.
        private readonly byte[] _initVector;

        public Encoder(IGlobalSettings settings)
            : this(Convert.FromBase64String(settings.EncoderKey), Convert.FromBase64String(settings.EncoderIv))
        {

        }

        private Encoder(IEnumerable<byte> key, IEnumerable<byte> iv)
        {
            _keyBytes = key.ToArray();
            _initVector = iv.ToArray();
        }
        
        public byte[] Encrypt(byte[] plainData)
        {
            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key bytes.
            var encryptor = _symmetricKey.CreateEncryptor(_keyBytes, _initVector);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainData, 0, plainData.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key bytes.
            var decryptor = _symmetricKey.CreateDecryptor(_keyBytes, _initVector);

            using var memoryStream = new MemoryStream(encryptedData);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            var plainTextBytes = new byte[encryptedData.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            return SubArray(plainTextBytes, 0, decryptedByteCount);
        }

        private static TElement[] SubArray<TElement>(TElement[] array, int startIndex, int length)
        {
            var result = new TElement[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }
    }
}