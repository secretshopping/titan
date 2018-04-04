using Prem.PTC.Utils.NVP;
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace Prem.PTC.Payments
{
    public class PerfectMoney
    {
        public PerfectMoney()
        {
        }

        #region Base Perfect Money API query methods
        protected Dictionary<string, string> ParsePerfectMoneyResponse(string s)
        {
            if (s == null) return null;

            Regex regEx = new Regex("<input name='(.*)' type='hidden' value='(.*)'>");
            MatchCollection matches = regEx.Matches(s);
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                results.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            return results;
        }

        protected string FetchPage(string url)
        {
            using (WebClient webClient = new MyWebClient())
            {
                string result = null; ;
                try
                {
                    result = webClient.DownloadString(url);
                }
                catch (WebException ex)
                {
                    return null;
                }
                return result;
            }
        }

        protected string FetchPerfectMoneyPageWithQuery(string method, string query)
        {
            return FetchPage(
                String.Format("https://perfectmoney.is/acct/{0}.asp?{1}",
                    method,
                    query));
        }

        protected string FetchPerfectMoneyPage(string method, params string[] args)
        {
            string query = "";

            if (args.Length % 2 != 0) throw new ArgumentException();

            for (int i = 0; i < args.Length; i += 2)
            {
                query = query + "&" + args[i] + "=" + args[i + 1];
            }

            return FetchPerfectMoneyPageWithQuery(method, query.Substring(1));
        }
        #endregion

        /// <summary>
        /// Get a response from Perfect Money and parse it searching for
        /// <input name='...' type='hidden' value='...'>. Result is returned as a
        /// hash.
        /// </summary>
        /// <param name="method">Perfect Money API method</param>
        /// <param name="args">Argument pairs: title, value, title, value ...</param>
        /// <returns>A hash  (Dictionary<string, string>) containing key-value data from Perfect Money response</returns>
        protected Dictionary<string, string> FetchPerfectMoneyPageParameters(string method, params string[] args)
        {
            return ParsePerfectMoneyResponse(FetchPerfectMoneyPage(method, args));
        }

        #region Perfect Money API direct queries
        public Dictionary<string, string> QueryBalance(string accountID, string passPhrase)
        {
            return FetchPerfectMoneyPageParameters("balance",
                    "AccountID", accountID,
                    "PassPhrase", passPhrase);
        }

        public Dictionary<string, string> EvoucherPurchase(string accountID, string passPhrase, string payerAccount, double amount)
        {
            return FetchPerfectMoneyPageParameters("ev_create",
                    "AccountID", accountID,
                    "PassPhrase", passPhrase,
                    "Payer_Account", payerAccount,
                    "Amount", XmlConvert.ToString(amount));
        }

        public Dictionary<string, string> Transfer(string accountID, string passPhrase,
            string payerAccount, string payeeAccount, double amount, int payIn, int paymentId)
        {


            return FetchPerfectMoneyPageParameters("confirm",
                "AccountID", accountID,
                "PassPhrase", passPhrase,
                "Payer_Account", payerAccount,
                "Payee_Account", payeeAccount,
                "Amount", XmlConvert.ToString(amount),
                "PAY_IN", "",
                "PAYMENT_ID", "");
        }
        #endregion

        #region Perfect money data list get methods
        /// <summary>
        /// Parses a table returned by Perfect Money server
        /// </summary>
        /// <param name="s">Full page text</param>
        /// <returns>A list of hashes which are lines of a table returned by Perfect Money</returns>
        protected List<Dictionary<string, string>> ParsePerfectMoneyList(string s)
        {
            string[] lines = s.Split(new char[] { '\r', '\n' });
            if (lines.Length < 2) return null;
            string[] fields = lines[0].Split(new char[] { ',' });

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            Dictionary<string, string> line;
            string[] values;

            for (int y = 1; y < lines.Length; y++)
            {
                values = lines[y].Split(new char[] { ',' });
                if (values.Length != fields.Length) continue;

                line = new Dictionary<string, string>();
                for (int x = 1; x < fields.Length; x++)
                {
                    line.Add(fields[x], values[x]);
                }
                result.Add(line);
            }

            return result;
        }

        /// <summary>
        /// Get a response from Perfect Money, parse it and check if it is a
        /// CSV table.
        /// </summary>
        /// <param name="method">Perfect Money API method</param>
        /// <param name="args">Argument pairs: title, value, title, value ...</param>
        /// <returns>A list of hashes which are table lines</returns>
        protected List<Dictionary<string, string>> FetchPerfectMoneyPageList(string method, params string[] args)
        {
            return ParsePerfectMoneyList(FetchPerfectMoneyPage(method, args));
        }
        #endregion

        public List<Dictionary<string, string>> GetEvouchersCreatedListing(string accountId, string passPhrase)
        {
            return FetchPerfectMoneyPageList("evcsv",
                "AccountID", accountId,
                "PassPhrase", passPhrase);
        }

        public List<Dictionary<string, string>> GetAccountHistory(string accountId, string passPhrase, DateTime start, DateTime end)
        {
            return FetchPerfectMoneyPageList("historycsv",
                "startday", start.Day.ToString(),
                "startmonth", start.Month.ToString(),
                "startyear", start.Year.ToString(),
                "endday", end.Day.ToString(),
                "endmonth", end.Month.ToString(),
                "endyear", end.Year.ToString(),
                "AccountID", accountId,
                "PassPhrase", passPhrase);
        }
    }

    public class PerfectMoneyTransactionResponse : TransactionResponse
    {
        internal PerfectMoneyTransactionResponse(PerfectMoneyAccount usedAccount, Dictionary<string, string> response)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;
            Note = string.Empty;


            if (!response.ContainsKey("ERROR"))
            {
                IsSuccess = true;
                ReferenceNumber = response.ContainsKey("PAYMENT_BATCH_NUM") ? response["PAYMENT_BATCH_NUM"] : "";
            }
            else
            {
                IsSuccess = false;
                Note = response["ERROR"];
            }
        }
    }
}