using System;
using System.Collections.Generic;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Security;
using Resources;
using System.Web.UI.WebControls;
using Titan.Leadership;

public class TitanRegisterService
{
    public static void Register(string InUsername, string InEmail, int InPIN, DateTime InBirthYear, string InPassword, string InReferer, Gender InGender, Panel CustomFields,
        string FirstName, string SecondName, string Address, string City, string StateProvince, string ZipCode, bool isEarner, bool isAdvertiser, bool isPublisher, string FacebookId = null,
        bool skipRedirection = false)
    {
        AppSettings.Email.Reload();

        bool IsFromFacebookOAuth = FacebookId != null ? true : false;

        // 0. Logged-in check
        if (Member.IsLogged)
            throw new MsgException(U5009.LOGOUTTOREGISTER);

        // 1. Validate if Username is not taken
        if (Member.Exists(InUsername))
            throw new MsgException(L1.ER_INVALIDUSERNAME3);

        // Or forbidden
        if (!SecurityManager.IsOkWithRules(SecurityRuleType.Username, InUsername) || InUsername.Contains("refback"))
            throw new MsgException(L1.UNAMEFORBID);

        // 2. Validate if Email is not taken
        if (Member.ExistsWithEmail(InEmail))
            throw new MsgException(L1.ER_DUPLICATEEMAIL);

        string currentip = IP.Current;
        CountryInformation CIService = new CountryInformation(currentip);

        // 0. Validate if Country/IP is not forbidden
        if (!SecurityManager.IsOkWithRules(SecurityRuleType.IP, currentip))
            throw new MsgException(L1.IPFORBID);

        if (!SecurityManager.IsOkWithRules(SecurityRuleType.Country, CIService.CountryName))
            throw new MsgException(L1.COUNTRYFORBID);

        // Validate if IP is not a proxy
        if (AppSettings.Proxy.IPPolicy == IPVerificationPolicy.EveryRegistration && ProxyManager.IsProxy(currentip))
            throw new MsgException(L1.IPAPROXY);

        if (AppSettings.Site.AllowOnlyOneRegisteredIP)
        {
            int membersRegisteredWithThisIP = (int)TableHelper.SelectScalar(
                String.Format("SELECT COUNT(*) FROM Users WHERE RegisteredWithIP = '{0}'", currentip));

            if (membersRegisteredWithThisIP > 0)
                throw new MsgException(U5005.IPALREADYREGISTERED);
        }

        AppSettings.DemoCheck();

        //Validate if country is OK
        FeatureManager Manager = null;
        if (IsFromFacebookOAuth)
            Manager = new FeatureManager(GeolocatedFeatureType.FacebookRegistration);
        else
            Manager = new FeatureManager(GeolocatedFeatureType.Registration);

        if (!Manager.IsAllowed)
            throw new MsgException(U4000.SORRYCOUNTRY);

        // All data is OK. Now lets register
        Member NewMember = new Member();
        NewMember.Name = InUsername;
        NewMember.Email = InEmail;
        NewMember.PIN = InPIN;
        NewMember.BirthYear = InBirthYear;
        NewMember.PrimaryPassword = MemberAuthenticationService.ComputeHash(InPassword);
        NewMember.FacebookOAuthId = FacebookId;

        //Detailed info?
        if (AppSettings.Authentication.DetailedRegisterFields)
        {
            NewMember.FirstName = FirstName;
            NewMember.SecondName = SecondName;
            NewMember.Address = Address;
            NewMember.City = City;
            NewMember.StateProvince = StateProvince;
            NewMember.ZipCode = ZipCode;
            NewMember.Gender = InGender;
        }

        //Custom fields?
        RegistrationFieldCreator.Save(NewMember, CustomFields);

        NewMember.CameFromUrl = (HttpContext.Current.Session == null || HttpContext.Current.Session["ReferralFrom"] == null) ? "Unknown" : HttpContext.Current.Session["ReferralFrom"].ToString().Replace("www.", "");
        NewMember.Country = CIService.CountryName;
        NewMember.CountryCode = CIService.CountryCode;
        NewMember.MessageSystemTurnedOn = true;
        NewMember.RevenueShareAdsWatchedYesterday = 1000; //No yesterday, that is why we assume that he watched all
        NewMember.PointsBalance = 0;

        //Slot Machine Modlue
        if (AppSettings.TitanFeatures.SlotMachineEnabled)
        {
            try
            {
                NewMember.PointsBalance = (int)HttpContext.Current.Session["anonymousSlotMachinePoints"];
            }
            catch (Exception slotError)
            {
                NewMember.SlotMachineChances = 1;
                ErrorLogger.Log(slotError);
            }
        }

        //Account type
        NewMember.IsEarner = isEarner;
        NewMember.IsAdvertiser = isAdvertiser;
        NewMember.IsPublisher = isPublisher;

        // Check if instant register or requires activation

        if (AppSettings.Authentication.IsInstantlyActivated || AppSettings.IsDemo || IsFromFacebookOAuth)
        {     
            //Adding referer and points bonus
            NewMember.TryAddReferer(InReferer);

            //Add history entry
            History.AddRegistration(NewMember.Name);

            //Representatives policy
            TrySetRepresentativeAsReferer(NewMember, CIService);

            if (AppSettings.Proxy.SMSType == ProxySMSType.EveryRegistration)
                NewMember.Register(MemberStatus.AwaitingSMSPIN);
            else
            {
                if (NewMember.HasReferer)
                {
                    var list = new List<RestrictionKind>(new RestrictionKind[] { RestrictionKind.DirectReferralsCount });
                    LeadershipSystem.CheckSystem(list, new Member(NewMember.ReferrerId));
                }
                NewMember.Register(MemberStatus.Active);
            }

            //Registration Bonus
            TryApplyRegistrationBonus(Manager, NewMember);

            //Matrix
            MatrixBase matrix = MatrixFactory.GetMatrix();
            if(matrix != null) matrix.TryAddMember(NewMember);

            if (!skipRedirection)
                HttpContext.Current.Response.Redirect("~/status.aspx?type=registerok&id=register1");
        }
        else
        {
            try
            {        
                Mailer.SendActivationLink(InUsername, InEmail);

                //Representatives policy
                TrySetRepresentativeAsReferer(NewMember, CIService);

                //Adding referer and points bonus
                NewMember.TryAddReferer(InReferer);

                NewMember.Register(MemberStatus.Inactive);
                
                //Add history entry
                History.AddRegistration(NewMember.Name);

                //Registration Bonus
                TryApplyRegistrationBonus(Manager, NewMember);

                //Matrix
                MatrixBase matrix = MatrixFactory.GetMatrix();
                if (matrix != null) matrix.TryAddMember(NewMember);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new MsgException("The system was unable to send the activation e-mail. Contact the administrator.");
            }

            if (!skipRedirection)
                HttpContext.Current.Response.Redirect("~/status.aspx?type=registerok&id=register2");
        }

    }

    private static void TryApplyRegistrationBonus(FeatureManager Manager, Member NewMember)
    {
        //Add initial Points
        if (Manager.Reward > 0)
        {
            NewMember = new Member(NewMember.Id);
            NewMember.AddToPointsBalance(Manager.Reward, "Registration bonus", BalanceLogType.RegistrationBonus, true);
            NewMember.SaveBalances();
        }

        RegistrationGiftCrediter.CreditGiftToUser(NewMember);
    }

    private static void TrySetRepresentativeAsReferer(Member NewMember, CountryInformation CIService)
    {
        if (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Automatic)
        {
            List<Representative> representatives = Representative.GetAllActiveFromCountry(CIService.CountryName);

            if (representatives.Count > 0)
            {
                Random random = new Random();
                Representative representative = representatives[random.Next(representatives.Count)];

                if (NewMember.ReferrerId == -1)
                    NewMember.ReferrerId = representative.UserId;

                NewMember.RepresentativeId = representative.UserId;
                
            }
        }
    }

}