using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Hash logic
/// </summary>
public static class DiceGameHashLogic
{
    public static int GetNumberOfBets(int id)
    {
        string query = string.Format(
                       "SELECT COUNT(ub.BetSize) from UserBets ub " +
                       "INNER JOIN DiceGameHashes dgh " +
                       "ON ub.UserId = dgh.UserId JOIN Users us " +
                       "ON ub.UserId = us.UserId " +
                       "WHERE us.UserId = {0} " +
                       "AND ub.BetDate > dgh.CreatedDateCurrent", id);
        int numberOfBets;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            numberOfBets = (int)(bridge.Instance.ExecuteRawCommandScalar(query));
        }
        return numberOfBets;
    }
    public static string GenerateSalt(string number, int id)
    {
        int numberOfBets = GetNumberOfBets(id) + 1;
        return string.Format("{0}:{1}", number, numberOfBets);
    }
    public static string GenerateClientSeed()
    {

        Random random = new Random();
        string r = "";
        for (int i = 0; i < 24; i++)
        {
            r += random.Next(0, 9).ToString();
        }
        return r;
    }


    public static string GenerateServerSeed(DateTime date, string userName, string offerwallsHash)
    {
        string query;
        string plainText;
        byte[] plainTextByte;
        HashAlgorithm hash;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            query = bridge.Instance.ExecuteRawCommandScalar("SELECT SUM(Balance4) from Users").ToString();
        }
        plainText = date.ToString() + userName + offerwallsHash + query;
        plainTextByte = Encoding.UTF8.GetBytes(plainText);
        hash = new SHA256Managed();
        byte[] hashBytes = hash.ComputeHash(plainTextByte);

        string hex = BitConverter.ToString(hashBytes);

        string hexToReturn = hex.Replace("-", "");
        return hexToReturn.ToLower();
    }

    public static string GenerateServerHash(string serverSeed)
    {
        byte[] serverSeedByte;
        HashAlgorithm hash;

        serverSeedByte = Encoding.UTF8.GetBytes(serverSeed);
        hash = new SHA256Managed();
        byte[] hashBytes = hash.ComputeHash(serverSeedByte);
        string hex = BitConverter.ToString(hashBytes);
        string hexToReturn = hex.Replace("-", "");
        return hexToReturn.ToLower();
    }

    /// <summary>
    /// Generates hash with salt
    /// </summary>
    /// <param name="clientHash"></param>
    /// <param name="hashAlgorithm"></param>
    /// <param name="serverHash"></param>
    /// <returns></returns>
    public static string ComputeHash(string clientHash,
                                     string serverHash)
    {
        byte[] serverHashBytes = Encoding.UTF8.GetBytes(serverHash);
        // Convert plain text into a byte array.
        byte[] clientHashBytes = Encoding.UTF8.GetBytes(clientHash);

        // Allocate array, which will hold plain text and salt.
        byte[] clientWithServerBytes =
                new byte[clientHashBytes.Length + serverHashBytes.Length];

        // Copy plain text bytes into resulting array.
        for (int i = 0; i < clientHashBytes.Length; i++)
            clientWithServerBytes[i] = clientHashBytes[i];

        // Append salt bytes to the resulting array.
        for (int i = 0; i < serverHashBytes.Length; i++)
            clientWithServerBytes[clientHashBytes.Length + i] = serverHashBytes[i];

        // Because we support multiple hashing algorithms, we must define
        // hash object as a common (abstract) base class. We will specify the
        // actual hashing algorithm class later during object creation.
        HashAlgorithm hash;

       

        // Initialize appropriate hashing algorithm class.
        hash = new SHA512Managed();

        // Compute hash value of our plain text with appended salt.
        byte[] hashBytes = hash.ComputeHash(clientWithServerBytes);

        string hex = BitConverter.ToString(hashBytes);
        string hexToReturn = hex.Replace("-", "");
        return hexToReturn.ToLower();
    }
}