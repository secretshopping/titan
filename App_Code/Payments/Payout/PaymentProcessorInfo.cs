using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PaymentProcessorInfo
{
    public PaymentProcessor ProcessorType;
    public int CustomPayoutProcessorId;

    public PaymentProcessorInfo(PaymentProcessor processor)
    {
        if (processor == PaymentProcessor.CustomPayoutProcessor)
            throw new FormatException("You can't call this construction for custom payout processors");

        ProcessorType = processor;
    }

    public PaymentProcessorInfo(PaymentProcessor processor, int custompayoutProcessorId)
    {
        CustomPayoutProcessorId = custompayoutProcessorId;
        ProcessorType = processor;

        if (processor != PaymentProcessor.CustomPayoutProcessor)
            custompayoutProcessorId = -1;
    }

    public PaymentProcessorInfo(int custompayoutProcessorId)
    {
        ProcessorType = PaymentProcessor.CustomPayoutProcessor;
        CustomPayoutProcessorId = custompayoutProcessorId;
    }

    public bool IsCustomProcessor
    {
        get
        {
            return ProcessorType == PaymentProcessor.CustomPayoutProcessor;
        }
    }

    public bool IsAutomaticProcessor
    {
        get
        {
            return ProcessorType != PaymentProcessor.CustomPayoutProcessor;
        }
    }
}