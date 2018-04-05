using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Reflection;
using Prem.PTC.Utils.NVP;
using Prem.PTC.Payments;
using Prem.PTC.Members;
using Prem.PTC;
using ExtensionMethods;

namespace Prem.PTC.Payments
{

    public class PerfectMoneyHandler
    {

        private const string IPList = "77.109.141.170, 91.205.41.208, 94.242.216.60, 78.41.203.75, 173.245.53.120, 173.245.53.195";

        public static void ProcessRequest(HttpContext context)
        {
            ErrorLogger.Log(context.Request.Params.ToRawString(), LogType.PerfectMoney);

            try
            {
                string RequestIP = IP.Current;

                if (IPList.Contains(RequestIP))
                {
                    var request = context.Request;
                    ProcessPayment(request);
                    context.Response.Write("OK");
                }
            }
            catch (Exception ex) { Prem.PTC.ErrorLogger.Log(ex); }
        }

        private static void ProcessPayment(HttpRequest request)
        {
            if (!PerfectMoneyAccountDetails.Exists(request["PAYEE_ACCOUNT"]))
                throw new MsgException("PerfectMoney account not exists: " + request["PAYEE_ACCOUNT"]);
            if (!CheckHash(request))
                throw new MsgException("Hashes are different");
            if (!CheckIfNotDoneYet(request))
                throw new MsgException("Prevention against many activities");
            
            string commandName = request["ITEM_COMMAND"];

            Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
            var type = assembly.GetType(commandName, true, true);
            IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;
            command.HandlePerfectMoney(request);                       
        }

        private static bool CheckHash(HttpRequest request)
        {
            string sentHash = request["V2_HASH"];
            List<PerfectMoneyAccountDetails> gateWays = TableHelper.SelectRows<PerfectMoneyAccountDetails>(TableHelper.MakeDictionary("IsActive", true));
            bool isOk = false;

            foreach (var gateway in gateWays)
            {
                string AlternateMerchantPassphraseHash = gateway.AlternatePassphrase;
                AlternateMerchantPassphraseHash = Titan.OfferwallParser.MD5(AlternateMerchantPassphraseHash).Trim();

                string calculatedHash = request["PAYMENT_ID"] + ":" + request["PAYEE_ACCOUNT"] + ":" + request["PAYMENT_AMOUNT"] + ":" +
                                        request["PAYMENT_UNITS"] + ":" + request["PAYMENT_BATCH_NUM"] + ":" + request["PAYER_ACCOUNT"] + ":"
                                        + AlternateMerchantPassphraseHash + ":" + request["TIMESTAMPGMT"];

                if (sentHash.Trim() == Titan.OfferwallParser.MD5(calculatedHash).Trim())
                    isOk = true;
            }

            return isOk;
        }

        private static bool CheckIfNotDoneYet(HttpRequest request)
        {
            string sentId = request["PAYMENT_BATCH_NUM"];
            List<CompletedPaymentLog> AllTransations = TableHelper.SelectRows<CompletedPaymentLog>(TableHelper.MakeDictionary("PaymentProcessor", (int)PaymentProcessor.PerfectMoney));
            bool isOk = true;

            foreach (var trans in AllTransations)
            {
                if (trans.TransactionId.Trim() == sentId.Trim())
                    isOk = false;
            }

            return isOk;
        }
    }

}