using Newtonsoft.Json;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;

namespace Titan.WelcomeTour
{
    public enum WelcomeTourStepStatus
    {
        Disabled = 0,
        Enabled  = 1
    }

    //EVERY STEP MUST BE DEFINED IN DB!!
    public enum ContentIdentifierForMainSite
    {
        //Global
        Null = 0,
        TopPanelBasicFunctions = 1,
        LanguageChange = 2,
        CurrencyChange = 3,
        UserSettings = 4,
        Balances = 5,
        Menu = 6,

        //Earn menu
        EarnCPAGPT = 7
    }

    public enum ContentIdentifierForAdminPanel
    {
        Null = 0
    }


    public static class WelcomeTourStepManager
    {
        public static String WelcomeTourJsonString()
        {
            var stepsList = new List<Dictionary<String, String>>();
            var EnabledTourSteps = TableHelper.GetListFromQuery<WelcomeTourStep>("WHERE Status=1 ORDER BY StepOrder ASC");

            foreach (WelcomeTourStep Step in EnabledTourSteps)
                if (Step.ContentIdentifier != ContentIdentifierForMainSite.Null)
                    stepsList.Add(CreateStepBody(Step));

            return JsonConvert.SerializeObject(stepsList);
        }

        private static Dictionary<String, String> CreateStepBody(WelcomeTourStep stepToBuild)
        {
            var newStepString = new Dictionary<String, String>();

            newStepString.Add("element",    GetStringSelector(stepToBuild.ContentIdentifier));
            newStepString.Add("content",    stepToBuild.Content);
            newStepString.Add("title",      stepToBuild.Title);
            newStepString.Add("placement",  GetStepDescriptionBoxPlacement(stepToBuild.ContentIdentifier));
            newStepString.Add("path",       GetSiteUrlForStep(stepToBuild.ContentIdentifier));

            return newStepString;
        }

        public static String GetContentPlaceDescription(ContentIdentifierForMainSite identifier)
        {
            switch (identifier)
            {
                #region Global
                case ContentIdentifierForMainSite.TopPanelBasicFunctions:
                    return "Top Left Panel";
                case ContentIdentifierForMainSite.LanguageChange:
                    return "Language change";
                case ContentIdentifierForMainSite.CurrencyChange:
                    return "Currency change";
                case ContentIdentifierForMainSite.UserSettings:
                    return "User settings";
                case ContentIdentifierForMainSite.Balances:
                    return "Balances window";
                case ContentIdentifierForMainSite.Menu:
                    return "Main menu";
                #endregion
                #region Earn
                case ContentIdentifierForMainSite.EarnCPAGPT:
                    return "Earn CPA/GPT tab";
                #endregion

                default:
                    return "NOT FOUND";
            }
        }

        public static String GetStringSelector(ContentIdentifierForMainSite identifier)
        {
            switch (identifier)
            {
                #region Global
                case ContentIdentifierForMainSite.TopPanelBasicFunctions:
                    return "#Global_TopNavbar";
                case ContentIdentifierForMainSite.LanguageChange:
                    return "#flags";
                case ContentIdentifierForMainSite.CurrencyChange:
                    return "#Global_CurrencyChange > div";
                case ContentIdentifierForMainSite.UserSettings:
                    return "#Global_UserSettings > a";
                case ContentIdentifierForMainSite.Balances:
                    return "#User_Balances";
                case ContentIdentifierForMainSite.Menu:
                    return "#Global_MainMenu";
                #endregion
                #region Earn
                case ContentIdentifierForMainSite.EarnCPAGPT:
                    return "#QSG_CPA_LIST";
                #endregion
                default:
                    return String.Empty;
            }
        }

        public static String GetSiteUrlForStep(ContentIdentifierForMainSite identifier)
        {
            const String MainSitePath = "/user/default.aspx";
            const String EarnCPAPath  = "/user/earn/cpaoffers.aspx";

            switch (identifier)
            {
                #region Global
                case ContentIdentifierForMainSite.TopPanelBasicFunctions:
                case ContentIdentifierForMainSite.LanguageChange:
                case ContentIdentifierForMainSite.CurrencyChange:
                case ContentIdentifierForMainSite.UserSettings:
                case ContentIdentifierForMainSite.Menu:
                    return String.Empty;
                #endregion
                #region Main Site
                case ContentIdentifierForMainSite.Balances:
                    return MainSitePath;
                #endregion
                #region Earn
                case ContentIdentifierForMainSite.EarnCPAGPT:
                    return EarnCPAPath;
                #endregion
                default:
                    return String.Empty;
            }
        }

        private static String GetStepDescriptionBoxPlacement(ContentIdentifierForMainSite identifier)
        {
            // auto - top - bottom - left - right 
            switch (identifier)
            {
                case ContentIdentifierForMainSite.Menu:
                    return "right";
                case ContentIdentifierForMainSite.UserSettings:
                    return "left";
                default:
                    return "auto";
            }
        }

        public static void SwapStepsOrders(WelcomeTourStep currentStep, WelcomeTourStep versusStep)
        {
            int tmpSaveOrder = currentStep.Order;
            currentStep.Order = versusStep.Order;
            versusStep.Order = tmpSaveOrder;

            currentStep.Save();
            versusStep.Save();           
        }
    }
}

