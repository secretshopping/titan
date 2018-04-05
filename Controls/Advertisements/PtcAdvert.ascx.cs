using System;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Prem.PTC.Memberships;

public partial class Controls_PtcAdvert : System.Web.UI.UserControl, IPtcAdvertObjectControl
{
    public PtcAdvert Object { get; set; }

    #region Control specifics

    public bool IsPreview { get; set; }
    public string AdColor
    {
        get
        {
            return Object.BackgroundColor;
        }
    }

    public bool IsActive
    {
        set { _IsActive = value; }

        get { return _IsActive; }
    }

    public string FavoriteCssClass { get; set; }
    public string ExposureType { get { return " " + Object.ExposureType.ToString().ToLower(); } }

    public string ImageClass { get; set; }

    public string ImageBackgroundStyle { get; set; }

    private bool _IsActive = true;

    public string ImageUrl
    {
        get
        {
            return Object.ImagePath;
        }
    }

    public string Info
    {
        get
        {
            if (AppSettings.PtcAdverts.CashLinkCrediterEnabled)
                return Amount.ToString();
            
            return FooterText;
        }
    }

    #endregion

    public override void DataBind()
    {
        base.DataBind();

        if (Member.IsLogged)
        {
            Member user = Member.CurrentInCache;

            UserEarnedMoney = PtcAdvert.CalculateNormalMemberEarnings(user, Object);
            EarningsDR = PtcAdvert.CalculateEarningsFromDirectReferral(user, Object);
            EarningsRR = PtcAdvert.CalculateEarningsFromRentedReferral(user, Object);
            EarningsPoints = user.Membership.AdvertPointsEarnings;
            EarningsAdCredits = user.Membership.PTCCreditsPerView;
        }
        else
        {
            //Public ad view
            UserEarnedMoney = PtcAdvert.CalculateNormalMemberEarnings(Membership.Standard, Object);
            EarningsDR = PtcAdvert.CalculateEarningsFromDirectReferral(Membership.Standard, Object);
            EarningsRR = PtcAdvert.CalculateEarningsFromRentedReferral(Membership.Standard, Object);
            EarningsPoints = Membership.Standard.AdvertPointsEarnings;
            EarningsAdCredits = Membership.Standard.PTCCreditsPerView;
        }

        //Display PTC Advert Type
        if (AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.PTC)
        {

            //Starred Ads
            if (Object.IsStarredAd)
            {
                starImg.Visible = true;
                starImg2.Visible = true;
            }
            else
            {
                starImg.Visible = false;
                starImg2.Visible = false;
            }

            //Favorite Ads
            if (AppSettings.PtcAdverts.FavoriteAdsEnabled && Member.IsLogged &&
                FavoriteAd.IsFavorite(Member.CurrentId, Object.Id, FavoriteAdType.PTC))
            {
                favoriteImg.Visible = true;
                favoriteImg2.Visible = true;
                FavoriteCssClass = " fav";
            }
            else
            {
                favoriteImg.Visible = false;
                favoriteImg2.Visible = false;
            }

            if (IsActive)
                ActiveAdvertPlaceHolder.Visible = true;
            else
                InactiveAdvertPlaceHolder.Visible = true;

            if (ImageUrl != null)
            {
                //    PtcImage.ImageUrl = PtcImage2.ImageUrl = ImageUrl;
                ImageBackgroundStyle = "background: url(" + ResolveUrl(ImageUrl) + ");";
            }


            if (AppSettings.PtcAdverts.PTCImagesEnabled)            
                ImageClass = "has-image";            
            else           
                ImageClass = "";
            
            CashLinkPlaceHolder.Visible = false;
        }
        //Display Cash Links Advert Type
        else if(AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.CashLink)
        {
            ActiveAdvertPlaceHolder.Visible = false;
            InactiveAdvertPlaceHolder.Visible = false;
            if (IsActive)
                CashLinkPlaceHolder.Visible = true;
            else
                CashLinkPlaceHolderInActive.Visible = true;

            if (IsPreview) //Preview fix
                CashLinkDiv.Attributes["class"] = "Abox CashLinkBox";
        }
    }

    protected void FavoriteAdsImageButton_Click(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            if (FavoriteAd.IsFavorite(Member.CurrentId, Object.Id, FavoriteAdType.PTC))
            {
                FavoriteAd.RemoveFromFavorites(Member.CurrentId, Object.Id, FavoriteAdType.PTC);
                favoriteImg.Visible = false;
                favoriteImg2.Visible = false;
            }
            else
            {
                FavoriteAd.AddToFavorites(Member.CurrentId, Object.Id, FavoriteAdType.PTC);
                favoriteImg.Visible = true;
                favoriteImg2.Visible = true;
            }
        }
    }

    #region Usable properties

    public string HoverHintText
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(L1.BYVIEWINGEARN);
            sb.Append(":<br/> <div class=ABlist>");

            if (!AppSettings.PtcAdverts.DisableMoneyEarningsInPTC)
                sb.Append("&bull; <b>" + UserEarnedMoney.ToString() + "</b> ");

            if (AppSettings.Points.PointsEnabled)
            {
                sb.Append("<br/>&bull; ");
                sb.Append("<b>" + EarningsPoints + "</b> ");
                sb.Append(AppSettings.PointsName);
            }

            if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            {
                sb.Append("<br/>&bull; ");
                sb.Append("<b>" + EarningsAdCredits + "</b> ");
                sb.Append(U5006.ADCREDITS);
            }

            //if (!AppSettings.PtcAdverts.DisableMoneyEarningsInPTC)
            //{
            //    sb.Append("<br/><br/>&bull; ");
            //    sb.Append("<b>" + EarningsDR.ToString() + "</b> " + L1.FROMDR);
            //}

            //if (AppSettings.TitanFeatures.ReferralsRentedEnabled && !AppSettings.PtcAdverts.DisableMoneyEarningsInPTC)
            //{
            //    sb.Append("<br/>&bull; ");
            //    sb.Append("<b>" + EarningsRR.ToString() + "</b> " + L1.FROMRR);
            //}

            sb.Append("<br/></div><br/>");
            sb.Append(L1.THISADLASTS + " ");
            sb.Append(Object.DisplayTime.TotalSeconds);
            sb.Append(" " + L1.SECONDS);
            sb.Append(Flags);
            sb.Append("\"");
            return sb.ToString();
        }
    }

    public Money UserEarnedMoney { get; set; }
    public Money EarningsDR { get; set; }
    public Money EarningsRR { get; set; }
    public int EarningsPoints { get; set; }
    public decimal EarningsAdCredits { get; set; }
    public int EarningsXP { get; set; }

    public string Title
    {
        get
        {
            if (Object.HasBoldTitle)
                return "<b>" + Object.Title + "</b>";

            return Object.Title;
        }
    }

    public string Description
    {
        get
        {
            return Object.Description;
        }
    }

    public int CategoryId
    {
        get
        {
            return Object.CategoryId;
        }
    }

    public string Flags
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            if (AppSettings.PtcAdverts.ShowGeolocatedCountryFlags && !String.IsNullOrEmpty(Object.GeolocatedCC))
            {
                string[] countries = Object.GeolocatedCC.Split(',');
                int maxLength = 11;
                int length = countries.Length > maxLength ? maxLength : countries.Length;
                sb.Append("</br></br>");
                for (int i = 0; i < length; i++)
                {
                    if (i == length - 1 && countries.Length > maxLength)
                        sb.Append("&nbsp;...");
                    else if (!String.IsNullOrWhiteSpace(countries[i]))
                        sb.Append("&nbsp;<img src='Images/Flags/" + countries[i] + ".png' />");
                }
            }

            return sb.ToString();
        }
    }

    public string FooterText
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            if (!AppSettings.PtcAdverts.DisableMoneyEarningsInPTC)            
                sb.Append(UserEarnedMoney.ToString());

            if (AppSettings.Points.PointsEnabled)
            {
                if (sb.Length != 0)
                    sb.Append(" + ");
                sb.Append(EarningsPoints);
                sb.Append(" <span style='font-weight:normal'>");
                sb.Append(AppSettings.PointsName);
                sb.Append("</span>");
            }

            if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            {
                if (sb.Length != 0)
                    sb.Append(" + ");
                sb.Append(EarningsAdCredits);
                sb.Append(" <span style='font-weight:normal'>");
                sb.Append(U5006.ADCREDITS);
                sb.Append("</span>");
            }

            sb.Append(" <span class='seconds pull-right' style='font-weight:normal; '>");
            sb.Append(Object.DisplayTime.TotalSeconds);
            sb.Append("s");
            sb.Append("</span>");

            return sb.ToString().TrimEnd('+');
        }
    }
    #endregion

    public Money Amount
    {
        get
        {
            if (IsPreview)
                return new Money(0.5);

            return Object.MoneyToClaimAsCashLink;
        }
    }

}
