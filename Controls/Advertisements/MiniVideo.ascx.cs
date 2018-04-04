using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Titan.MiniVideos;

public partial class Controls_Advertisements_MiniVideo : System.Web.UI.UserControl, IMiniVideoObjectControl
{
    public MiniVideoCampaign Object { get; set; }

    public int Id
    {
        get { return Object.Id; }
    }

    public string Title
    {
        get { return Object.Title; }
    }

    public string ImageURL
    {
        get { return Object.ImageURL; }
    }

    public string VideoURL
    {
        get { return Object.VideoURL; }
    }

    public string Description
    {
        get { return Object.Description; }
    }

    public string VideoCategory
    {
        get { return new MiniVideoCategory(Object.VideoCategory).Name; }
    }

    public Money Price
    {
        get { return Member.Current.Membership.MiniVideoWatchPrice; }
    }

    public override void DataBind()
    {
        base.DataBind();
        BuyButton.Text = L1.BUY;        
    }

    protected void BuyButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        try
        {
            var user = Member.Current;
            
            PurchaseOption.ChargeBalance(user, Price, TargetBalanceRadioButtonList.Feature, TargetBalanceRadioButtonList.TargetBalance, "Mini Video Bought");

            var boughtVideo = new UsersMiniVideoCampaign
            {
                BoughtDate = DateTime.Now,
                VideoId = Id,
                Username = user.Name
            };
            boughtVideo.Save();

            SuccMessagePanel.Visible = true;
            SuccMessage.Text = U6008.MINIVIDEOCREATED;
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}