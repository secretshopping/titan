using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Contests;
using Prem.PTC.Offers;
using Prem.PTC.Achievements;
using Prem.PTC.Advertising;
using ExtensionMethods;

namespace Titan
{
    public class CPAHandler
    {
        public static void ProcessRequest(HttpContext context)
        {
            try
            {
                string RequestIP = IP.Current;

                //Log
                string LogMessage = context.Request.Params.ToRawString();
                ErrorLogger.Log(LogMessage, LogType.CPAGPTNetworks);

                AppSettings.Points.Reload();

                //Get proper Network 
                string Hash = CPAFileManager.GetHashFromClientHandlerHit(context);              

                AffiliateNetwork Network = TableHelper.SelectRows<AffiliateNetwork>(TableHelper.MakeDictionary("Hash", Hash))[0];

                //All OK
                string Username = GetFromRequest(context, Network.VariableNameOfUsername);
                //string _Balance = GetFromRequest(context, Network.VariableNameOfBalance);
                string OfferID = GetFromRequest(context, Network.VariableNameOfOfferId);
                string _WebsiteId = GetFromRequest(context, Network.VariableNameOfWebsiteId); //Can be null

                string OfferTitle = GetFromRequest(context, Network.VariableNameOfOfferName); //Can be null

                string _CreditVal = GetFromRequest(context, Network.VariableNameOfType); //Can be null
                string Signature = GetFromRequest(context, Network.VariableNameOfSignature); //Can be null
                string UserIP = GetFromRequest(context, Network.VariableNameOfMemberIP); //Can be null

                string ShortOfferTitle = OfferTitle != null ? ": " + ShortenString(OfferTitle, 8) + "..." : "";
                string TrackingInfo = OfferID;

                bool isLocked = false;  //Points Locking feature

                Money Balance = new Money(0);
                //try
                //{
                //    Balance = Money.Parse(_Balance);
                //}
                //catch (Exception ex) { }

                //Check Status and IP restrictions
                bool ActiveRestriction = Network.Status == NetworkStatus.Active;
                bool IPRestriction = !Network.HasRestrictedIPs || (Network.HasRestrictedIPs && Network.RestrictedIPs.Contains(RequestIP));

                if (ActiveRestriction && IPRestriction)
                {
                    //Check condition
                    if (Signature == null || Signature.Trim() == CPAParser.ParseSignatureCondition(Network.SIgnatureCondition, context).Trim())
                    {
                        //All OK, Verified
                        CPAPostBackLogStatus Status = CPAPostBackLogStatus.Null;
                        Money Calculated = new Money(0);

                        //Let's get our offer
                        CPAOffer OurOffer = null;
                        try
                        {
                            var where = TableHelper.MakeDictionary("NetworkOfferIdInt", OfferID);
                            where.Add("AdvertiserUsername", Network.Name);
                            where.Add("Status", (int)AdvertStatus.Active);

                            OurOffer = TableHelper.SelectRows<CPAOffer>(where)[0];
                            Balance = OurOffer.BaseValue;
                        }
                        catch (Exception ex)
                        { ErrorLogger.Log(ex); }


                        if (OurOffer != null)
                        {
                            //Lets find submission
                            try
                            {
                                Member User = new Member(Username);
                                CreditAs As = GetCreditAs(OurOffer, Network);
                                PostbackActionType ActionType = CPAHandler.GetActionType(Network, Balance, _CreditVal);

                                var conditions = TableHelper.MakeDictionary("Username", Username);
                                conditions.Add("OfferId", OurOffer.Id);
                                conditions.Add("OfferStatus", (int)OfferStatus.Pending);

                                int IsSubmissionPresent = TableHelper.CountOf<OfferRegisterEntry>(conditions);

                                //Add submission automatically if credit and not found
                                if (IsSubmissionPresent == 0 && ActionType == PostbackActionType.Credit)
                                {
                                    OfferRegisterEntry.CheckDuplicateAndStatus(User.Name, OurOffer, true);
                                    OfferRegisterEntry.AddNew(OurOffer, User.Name, OfferStatus.Pending, String.Empty, String.Empty);

                                    IsSubmissionPresent = TableHelper.CountOf<OfferRegisterEntry>(conditions);
                                }

                                if (IsSubmissionPresent > 0)
                                {
                                    OfferRegisterEntry ThisEntry = TableHelper.SelectRows<OfferRegisterEntry>(conditions)[0];
                                   
                                    if (ActionType == PostbackActionType.Reverse)
                                    {
                                        CPAPostbackLog oldLog = PointsLockingHelper.FindSimilarCpaLog(User.Name, Network.Name, TrackingInfo);

                                        if (oldLog == null)
                                        {
                                            Calculated = CPAManager.DenyEntryFromPostback(ThisEntry, Balance, As, OurOffer.Id,
                                            Network.Name, OfferTitle, Network.RequiresConversion);
                                            Status = CPAPostBackLogStatus.ReversedByNetwork;
                                        }
                                        else
                                        {
                                            Status = CPAPostBackLogStatus.ReversedByNetwork;
                                            oldLog.Status = Status;
                                            CPAManager.DenyEntry(ThisEntry, User, OfferTitle);
                                        }

                                    }

                                    if (ActionType == PostbackActionType.Credit)
                                    {

                                        if (PointsLockingManager.CPAGPTShouldBeLocked(Balance, As, User, Network.RequiresConversion))
                                        {
                                            ThisEntry.HasBeenLocked = true;
                                            ThisEntry.Save();

                                            Calculated = CPAGPTCrediter.CalculatePostback(Balance, Network.RequiresConversion, User, As);
                                            Status = CPAPostBackLogStatus.CreditedAndPointsLocked;
                                            History.AddOfferLocked(User.Name, Network.Name, OfferTitle, Calculated, As);
                                        }
                                        else
                                        {
                                            var where = TableHelper.MakeDictionary("Id", ThisEntry._OfferId);
                                            var CpaOffer_entryId = TableHelper.SelectRows<CPAOffer>(where);
                                            Calculated = CPAManager.AcceptEntryFromPostback(ThisEntry, Balance, As, OurOffer.Id,
                                            Network.Name, CpaOffer_entryId[0].Title, Network.RequiresConversion, out isLocked);
                                            Status = CPAPostBackLogStatus.CreditedByNetwork;

                                        }
                                    }

                                    //Add CPAPostbackLog
                                    CPAPostbackLog.Create(Network, Username, Balance, Calculated, TrackingInfo, Status);

                                    //If All was handled OK, than succResponse
                                    context.Response.Write(Network.ValueOfSuccessfulResponse);
                                }
                                else
                                {
                                    CPAPostbackLog.Create(Network, Username, Balance, Calculated, TrackingInfo, CPAPostBackLogStatus.SubmissionNotFound);
                                }
                            }
                            catch (MsgException ex)
                            {
                                //Offer has been already submited and it's not daily
                                //Offer is daily, but was submited today
                                //Offer is daly and can be submited X times a day, but was submited >= X today
                                CPAPostbackLog.Create(Network, Username, Balance, Calculated, TrackingInfo, CPAPostBackLogStatus.ExceededSubmissionLimitForThisOffer);
                                context.Response.Write(Network.ValueOfSuccessfulResponse);
                            }
                            catch (Exception ex)
                            {
                                ErrorLogger.Log(ex);
                                CPAPostbackLog.Create(Network, Username, Balance, Calculated, TrackingInfo, CPAPostBackLogStatus.Null);
                            }
                        }
                        else
                            CPAPostbackLog.Create(Network, Username, Balance, null, TrackingInfo, CPAPostBackLogStatus.OfferNotFoundByOfferId);
                    }
                    else
                        CPAPostbackLog.Create(Network, Username, Balance, null, TrackingInfo, CPAPostBackLogStatus.WrongSignature);
                }
                else if (IPRestriction)
                    CPAPostbackLog.Create(Network, Username, Balance, null, TrackingInfo, CPAPostBackLogStatus.NetworkInactive);
                else if (ActiveRestriction)
                    CPAPostbackLog.Create(Network, Username, Balance, null, TrackingInfo, CPAPostBackLogStatus.SentFromUnallowedIP);
            }
            catch (Exception ex) { Prem.PTC.ErrorLogger.Log(ex); }
        }

        private static string GetFromRequest(HttpContext context, string name)
        {
            if (!string.IsNullOrEmpty(name))
                return context.Request[name];
            return null;
        }

        private static string ShortenString(string input, int count)
        {
            if (input.Length > count)
                return input.Substring(0, count - 1);
            return input;
        }

        private static CreditAs GetCreditAs(CPAOffer Offer, AffiliateNetwork Network)
        {
            if (Offer.CreditOfferAs == CreditOfferAs.NetworkDefault)
                return Network.CreditAs;

            if (Offer.CreditOfferAs == CreditOfferAs.NonCash)
                return CreditAs.Points;

            return CreditAs.MainBalance;
        }

        private static PostbackActionType GetActionType(AffiliateNetwork Network, Money Balance, string _CreditVal)
        {
            if ((_CreditVal != null && _CreditVal == Network.VariableValueOfTypeReversed) ||
                (_CreditVal == null && Balance < new Money(0)))
                return PostbackActionType.Reverse;

            if ((_CreditVal != null && _CreditVal == Network.VariableValueOfTypeCredited) ||
                (_CreditVal == null && Balance >= new Money(0)))
                return PostbackActionType.Credit;

            return PostbackActionType.None;
        }
    }

    public enum PostbackActionType
    {
        Credit = 1,
        Reverse = 2,
        None = 3
    }
}