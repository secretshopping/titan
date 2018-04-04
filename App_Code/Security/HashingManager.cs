using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public static class HashingManager
{
    /// <summary>
    /// Generates a SHA256 from a specified string
    /// </summary>
    /// <param name="bytesToHash"></param>
    /// <returns></returns>
    [Obsolete]
    public static string GenerateSHA256(string input)
    {
        return SHA256(input);
    }

    public static string SHA256(string input)
    {
        byte[] bytesToHash = Encoding.UTF8.GetBytes(input);
        HashAlgorithm hash = new SHA256Managed();
        byte[] hashBytes = hash.ComputeHash(bytesToHash);
        string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
        return hashString;
    }

    public static string SHA512HMAC(string key, string message)
    {
        var keyByte = Encoding.UTF8.GetBytes(key);
        using (var hmacsha512 = new HMACSHA512(keyByte))
        {
            hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(message));

            return ByteToString(hmacsha512.Hash);
        }
    }

    static string ByteToString(byte[] buff)
    {
        string sbinary = "";
        for (int i = 0; i < buff.Length; i++)
            sbinary += buff[i].ToString("X2"); /* hex format */
        return sbinary;
    }

    /// <summary>
    /// Generates MD5 from a specified string
    /// </summary>
    /// <param name="Input"></param>
    /// <returns></returns>
    public static string GenerateMD5(string input)
    {
        // Calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // Convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static string GenerateBase64(string input)
    {
        var sha1Provider = HashAlgorithm.Create("SHA512");
        var binHash = sha1Provider.ComputeHash(System.Text.Encoding.Unicode.GetBytes(input));
        var base64HashOutput = Convert.ToBase64String(binHash);
        return base64HashOutput;
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}