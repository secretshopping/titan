using System;
using Prem.PTC.Payments;
using Prem.PTC.Members;
using Resources;
using System.Collections.Generic;

public partial class Controls_ProcessorSettings : System.Web.UI.UserControl, IProcessorSettingsObjectControl
{
    Member User;
    public PaymentAccountDetails BasicProcessor { get; set; }
    public CustomPayoutProcessor Processor { get; set; }
    public int UserId { get; set; }

    public string ImageURL { get; set; }

    private PaymentProcessorInfo _processorInfo;
    private PaymentProcessorInfo ProcessorInfo
    {
        get
        {
            if (Processor != null) //CustomProcessor
            {
                if (_processorInfo == null)
                    _processorInfo = new PaymentProcessorInfo(Processor.Id);
            }
            else  //Basic Processor
            {
                if (_processorInfo == null)
                    _processorInfo = new PaymentProcessorInfo((PaymentProcessor)Enum.Parse(typeof(PaymentProcessor), BasicProcessor.AccountType));
            }
            return _processorInfo;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        LangAdder.Add(REValidator, U3500.ILLEGALCHARS);

        if (Processor != null) //CustomProcessor
            Processor_Helper.Visible = (Processor.DaysToBlockWithdrawalsAfterAccounChange == 0) ? false : true;
        else //Basic Processor
            Processor_Helper.Visible = (BasicProcessor.DaysToBlockWithdrawalsAfterAccountChangename == 0) ? false : true;
    }

    public override void DataBind()
    {
        base.DataBind();
        int BannedDays = 0;

        if (Processor != null) //CustomProcessor
        {
            ImageURL = Processor.ImageURL;
            BannedDays = Processor.DaysToBlockWithdrawalsAfterAccounChange;

            var address = UsersPaymentProcessorsAddress.GetAddress(UserId, ProcessorInfo);
            if (address != null)
                ValueTextBox.Text = address.PaymentAddress.ToString();
        }
        else //Basic Processor
        {
            User = Member.CurrentInCache;
            var ProcessorType = (PaymentProcessor)Enum.Parse(typeof(PaymentProcessor), BasicProcessor.AccountType);

            ImageURL = String.Format("Images/OneSite/TransferMoney/{0}.png", BasicProcessor.AccountType);
            BannedDays = BasicProcessor.DaysToBlockWithdrawalsAfterAccountChangename;
            ValueTextBox.Text = User.GetPaymentAddress(ProcessorType);
        }

        if (!String.IsNullOrEmpty(ValueTextBox.Text))
            Processor_Helper.InnerText = (BannedDays > 0) ? String.Format(U6000.WALLETCHANGEWARNING, BannedDays) : "";

    }

    public void Save()
    {
        if (!string.IsNullOrEmpty(ValueTextBox.Text))
            UsersPaymentProcessorsAddress.TrySetAddress(UserId, ProcessorInfo, ValueTextBox.Text);
    }

}
