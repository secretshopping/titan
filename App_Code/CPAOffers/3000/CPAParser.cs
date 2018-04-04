using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace Titan
{
    public class CPAParser
    {
        public static string ParseHTMLCode(string Input, string Username)
        {
            //1. [USERNAME] = Username
            return Input.Replace("[USERNAME]", Username);
        }

        public static string ParseSignatureCondition(string Input, HttpContext Context)
        {
            //1. [sample] = ANY GET request named 'sample'
            //2. MD5(x) = MD5 of 'x'
            //3. MD5L(x) = lower-case MD5 of 'x'

            Input = ReplaceGETs(Input, Context);
            Input = ReplaceMD5(Input);
            Input = ReplaceMD5U(Input);

            return Input;
        }

        /// <summary>
        /// Returns always TO.UPPER MD5
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string MD5(string Input)
        {
            // Calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // Convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToUpper();
        }

        public static string ToViewableHTML(string Input)
        {
            return Input.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// [sample] -> value
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static string ReplaceGETs(string Input, HttpContext Context)
        {
            List<string> ValuesToReplace = new List<string>();

            bool AwaitingStart = true;
            string Temp = "";

            for (int i = 0; i < Input.Length; ++i)
            {
                if (Input[i] == '[' && AwaitingStart)
                {
                    //This is start
                    AwaitingStart = false;
                    continue;
                }
                if (!AwaitingStart)
                {
                    if (Input[i] == ']')
                    {
                        //This is end
                        AwaitingStart = true;
                        ValuesToReplace.Add(Temp);
                        Temp = "";
                    }
                    else
                        Temp = Temp + Input[i];
                }
            }

            try
            {
                foreach (var val in ValuesToReplace)
                    Input = Input.Replace('[' + val + ']', Context.Request[val].ToString());
            }
            catch (Exception ex) { }

            return Input;
        }

        public static string ReplaceMD5(string Input)
        {
            int WhereToStart = Input.IndexOf("MD5(");
            if (WhereToStart > -1)
            {
                int StartIndex = WhereToStart + 4;
                string InsideBrackets = "";
                while (Input[StartIndex] != ')')
                {
                    InsideBrackets += Input[StartIndex];
                    StartIndex++;
                }

                Input = Input.Replace("MD5(" + InsideBrackets + ")", MD5(InsideBrackets).ToLower());
            }

            return Input;
        }

        public static string ReplaceMD5U(string Input)
        {
            int WhereToStart = Input.IndexOf("MD5U(");
            if (WhereToStart > -1)
            {
                int StartIndex = WhereToStart + 5;
                string InsideBrackets = "";
                while (Input[StartIndex] != ')')
                {
                    InsideBrackets += Input[StartIndex];
                    StartIndex++;
                }

                Input = Input.Replace("MD5U(" + InsideBrackets + ")", MD5(InsideBrackets));
            }

            return Input;
        }
    }
}