using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Contests;
using ExtensionMethods;

namespace Titan
{
    public class OfferwallHandler
    {
        public static void ProcessRequest(HttpContext context)
        {
            try
            {
                string RequestIP = IP.Current;

                string LogMessage = context.Request.Params.ToRawString();
                ErrorLogger.Log(LogMessage, LogType.OfferWalls);

                //Force refresh
                AppSettings.Offerwalls.Reload();
                AppSettings.Points.Reload();

                //Get proper Offerwall 
                string Hash = OfferwallFileManager.GetHashFromClientHandlerHit(context);             

                Offerwall Wall = TableHelper.SelectRows<Offerwall>(TableHelper.MakeDictionary("Hash", Hash))[0];

                //All OK
                string Username = GetFromRequest(context, Wall.VariableNameOfUsername);
                string _Balance = GetFromRequest(context, Wall.VariableNameOfBalance);
                string TrackingInfo = GetFromRequest(context, Wall.VariableNameOfTrackingInfo); //Can be null
                string _CreditVal = GetFromRequest(context, Wall.VariableNameOfType); //Can be null
                string Signature = GetFromRequest(context, Wall.VariableNameOfSignature); //Can be null
                Money Balance = Money.Parse(_Balance);             

                Member member = null;

                //Check Status and IP restrictions
                bool ActiveRestriction = Wall.Status == OfferwallStatus.Active;
                bool IPRestriction = !Wall.HasRestrictedIPs || (Wall.HasRestrictedIPs && IpRangeHelper.isOK(Wall.RestrictedIPs , RequestIP));

                if (ActiveRestriction && IPRestriction)
                {
                    //Check condition
                    if (Signature == null || Signature.Trim() == OfferwallParser.ParseSignatureCondition(Wall.SIgnatureCondition, context).Trim())
                    {
                        //All OK, Verified
                        OfferwallsLogStatus Status = OfferwallsLogStatus.Null;
                        Money Calculated = new Money(0);

                        //Check if member exists
                        try
                        {
                            member = new Member(Username);

                            if ((_CreditVal != null && _CreditVal == Wall.VariableValueOfTypeReversed) ||
                                (_CreditVal == null && Balance < Money.Zero))
                            {
                                //Reverse
                                OfferwallsLog oldLog = PointsLockingHelper.FindSimilarLog(member.Name, Wall.DisplayName, TrackingInfo);

                                if (oldLog == null)
                                {
                                    OfferwallCrediter Crediter = new OfferwallCrediter(member, Wall);
                                    Calculated = Crediter.ReverseCredit(Balance, Wall.CreditAs, Wall.DisplayName, Wall.RequiresConversion);

                                    History.AddOfferwalRevereseCompleted(Username, Wall.DisplayName, Calculated, Wall.CreditAs);
                                    Status = OfferwallsLogStatus.ReversedByOfferwall;
                                }
                                //change old log status 
                                else
                                {
                                    Status = OfferwallsLogStatus.ReversedByOfferwall;
                                    oldLog.Status = Status;
                                }
                            }

                            if ((_CreditVal != null && _CreditVal == Wall.VariableValueOfTypeCredited) ||
                                (_CreditVal == null && Balance >= Money.Zero))
                            {
                                //Credit
                                if (PointsLockingManager.OfferwallShouldBeLocked(Balance, member, Wall))
                                {
                                    Calculated = OfferwallCrediter.CalculatedAndConversion(Balance, member, Wall);
                                    History.AddOfferLocked(Username, Wall.DisplayName, null, Calculated, Wall.CreditAs);

                                    Status = OfferwallsLogStatus.CreditedAndPointsLocked;
                                }
                                else
                                {
                                    OfferwallCrediter Crediter = new OfferwallCrediter(member, Wall);                               
                                    Calculated = Crediter.CreditMember(Balance, Wall);
                                    History.AddOfferwalCompleted(Username, Wall.DisplayName, Calculated, Wall.CreditAs);
                                    Status = OfferwallsLogStatus.CreditedByOfferwall;
                                }
                            }

                            if ((_CreditVal != null && (_CreditVal != Wall.VariableValueOfTypeCredited && _CreditVal != Wall.VariableValueOfTypeReversed)))
                            {
                                Status = OfferwallsLogStatus.WrongCreditVariable;
                            }

                            //Add OfferwallsLog
                            OfferwallsLog.Create(Wall, member.Name, Balance, Calculated, TrackingInfo, Status);

                            //If All was handled OK, than succResponse
                            context.Response.Write(Wall.ValueOfSuccessfulResponse);

                        }
                        catch (MsgException ex)
                        {
                            Status = OfferwallsLogStatus.MemberNotFoundByUsername;
                            OfferwallsLog.Create(Wall, Username, Balance, Calculated, TrackingInfo, Status);
                        }
                    }
                    else
                        OfferwallsLog.Create(Wall, Username, Balance, null, TrackingInfo, OfferwallsLogStatus.WrongSignature);
                }
                else if (IPRestriction)
                    OfferwallsLog.Create(Wall, Username, Balance, null, TrackingInfo, OfferwallsLogStatus.OfferwallInactive);
                else if (ActiveRestriction)
                    OfferwallsLog.Create(Wall, Username, Balance, null, TrackingInfo, OfferwallsLogStatus.SentFromUnallowedIP);
            }
            catch (Exception ex) { Prem.PTC.ErrorLogger.Log(ex); }
        }

        private static string GetFromRequest(HttpContext context, string name)
        {
            if (!string.IsNullOrEmpty(name))
                return context.Request[name];
            return null;
        }
    }
}