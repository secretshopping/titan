using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Prem.PTC;

public class Encryption
{
    private static readonly string BANNER_KEY = HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword + "22");
    private static readonly string UNIVERSAL_KEY = HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword + "jde");

    private byte[] key = { };

    /// <summary>
    /// Declare the Local Variable
    /// </summary>
    private byte[] iV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

    /// <summary>
    /// Decrypts the specified string to decrypt.
    /// </summary>
    /// <param name="stringToDecrypt">The string to decrypt.</param>
    /// <param name="decryptionKey">The decryption key.</param>
    /// <returns>Return string</returns>
    public string Decrypt(string stringToDecrypt, string decryptionKey)
    {
        ///// byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
        byte[] inputByteArray;
        try
        {
            stringToDecrypt = stringToDecrypt.Replace(" ", "+");
            stringToDecrypt = stringToDecrypt.Replace("-", "/");
            this.key = System.Text.Encoding.UTF8.GetBytes(this.Left(decryptionKey, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(stringToDecrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(this.key, this.iV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    /// <summary>
    /// Lefts the specified param.
    /// </summary>
    /// <param name="param">The param.</param>
    /// <param name="length">The length.</param>
    /// <returns>Return string</returns>
    public string Left(string param, int length)
    {
        string result = param.Substring(0, length);
        return result;
    }

    /// <summary>
    /// Rights the specified param.
    /// </summary>
    /// <param name="param">The param.</param>
    /// <param name="length">The length.</param>
    /// <returns>Return string</returns>
    public string Right(string param, int length)
    {
        string result = param.Substring(param.Length - length, length);
        return result;
    }

    /// <summary>
    /// Encrypts the specified string to encrypt.
    /// </summary>
    /// <param name="stringToEncrypt">The string to encrypt.</param>
    /// <param name="encryptionKey">The encryption key.</param>
    /// <returns>Return string</returns>
    public string Encrypt(string stringToEncrypt, string encryptionKey)
    {
        try
        {

            this.key = System.Text.Encoding.UTF8.GetBytes(this.Left(encryptionKey, 8));
            DESCryptoServiceProvider encrypt = new DESCryptoServiceProvider();
            byte[] inputByteArrayEncrypt = Encoding.UTF8.GetBytes(stringToEncrypt);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypt.CreateEncryptor(this.key, this.iV), CryptoStreamMode.Write);
            cryptoStream.Write(inputByteArrayEncrypt, 0, inputByteArrayEncrypt.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray()).Replace("/", "-");

        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public string EncryptBannerId(int bannerid)
    {
        return Encrypt(bannerid.ToString(), BANNER_KEY);
    }

    public int DecryptBannerId(string input)
    {
        return Convert.ToInt32(Decrypt(input, BANNER_KEY));
    }

    public static string Encrypt(string input)
    {
        Encryption instance = new Encryption();
        return instance.Encrypt(input, UNIVERSAL_KEY);
    }

    public static string Decrypt(string input)
    {
        Encryption instance = new Encryption();
        return instance.Decrypt(input, UNIVERSAL_KEY);
    }
}