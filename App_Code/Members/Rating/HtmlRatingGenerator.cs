using Prem.PTC.Members;
using System;

public static class HtmlRatingGenerator
{
    const int MaxStarsAmount = 5;

    public static String GenerateHtmlRating(RatingType ratingType, int userId)
    {
        if (ratingType == RatingType.Representative)
            return GenerateRatingHtmlStringForRepresentative(userId);

        if (ratingType == RatingType.CryptocurrencyTrading)
            return GenerateRatingHtmlStringForCryptocurrencyTradingUser(userId);
    
        return String.Empty;
    }

    private static String GenerateRatingHtmlStringForRepresentative(int userId)
    {
        RatingMemberInfo UserRatingInfo = RepresentativeRatingMemberDictionary.Get(userId);
        if(UserRatingInfo.TotalActions == 0)
            return GenerateHtmlRatingStarsString(0) + "(0)";

        double Rating = Convert.ToDouble(UserRatingInfo.TotalPositiveActions) / Convert.ToDouble(UserRatingInfo.TotalActions);

        return GenerateHtmlRatingStarsString(Rating) + GenerateHtmlRatingDetailsString(Rating, UserRatingInfo.TotalActions);
    }

    private static String GenerateRatingHtmlStringForCryptocurrencyTradingUser(int userId)
    {
        RatingMemberInfo UserRatingInfo = CryptocurrencyRatingMemberDictionary.Get(userId);
       
        if(UserRatingInfo.TotalActions == 0)
            return GenerateHtmlRatingStarsString(0) + "(0)";

        double Rating = Convert.ToDouble(UserRatingInfo.TotalPositiveActions) / Convert.ToDouble(UserRatingInfo.TotalActions);
        return GenerateHtmlRatingStarsString(Rating) + GenerateHtmlRatingDetailsString(Rating, UserRatingInfo.TotalActions);
    }

    private static String GenerateHtmlRatingDetailsString(double rating, int totalActions)
    {
        return String.Format("({0}% <i class=\"fa fa-thumbs-up\" style=\"color: #00B700\" aria-hidden=\"true\"></i> of {1})", Math.Round(rating * 100, 0), totalActions);
    }

    private static String GenerateHtmlRatingStarsString(double rating, bool floatLeft=true)
    {
        String RatingHtml = String.Empty;
        int FilledStars = (int)Math.Round(rating * MaxStarsAmount, 0);

        String Float = floatLeft == true ? "style =\"float: left;\"" : String.Empty;

        //Generating filled stars
        for (int i=0; i<FilledStars; i++)
            RatingHtml += String.Format("<span id=\"Star_{0}\" class=\"ratingStar filledRatingStar\" {1}>&nbsp;</span>", i, Float);

        //Generating empty stars
        for (int i = 0; i < MaxStarsAmount - FilledStars; i++)
            RatingHtml += String.Format("<span id=\"Star_{0}\" class=\"ratingStar emptyRatingStar\" {1}>&nbsp;</span>", i+rating, Float);

        return RatingHtml;
    }

}