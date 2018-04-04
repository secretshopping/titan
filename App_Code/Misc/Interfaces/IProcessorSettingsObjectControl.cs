using Prem.PTC.Payments;

public interface IProcessorSettingsObjectControl
{
    PaymentAccountDetails BasicProcessor { get; set; } //Automatic Processor
    CustomPayoutProcessor Processor { get; set; } //Custom Processor
    int UserId { get; set; }
    void DataBind();
    void Save();
}