using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.CryptocurrencyPlatform
{
    public enum CryptocurrencyOfferType
    {
        Null = 0,

        Buy = 1,
        Sell = 2
    }
    public enum CryptocurrencyOfferStatus
    {
        Null = 0,

        Active = 1,
        Paused = 2,
        Deleted = 3,
        Finished = 4
    }

    public enum CryptocurrencyOfferRating
    {
        Null = 0,

        OneStar = 1,
        TwoStars = 2,
        ThreeStars = 3,
        FourStars = 4,
        FiveStars = 5
    }


    public enum CryptocurrencyTransactionStatus
    {
        Null = 0,

        AwaitingPayment = 1,                //After transaction creation, waiting for client payment
        AwaitingPaymentConfirmation = 2,    //Waiting for offer creator to confirm received payment
        Finished = 3,                       //Offer Creator confirmed payment, transaction executed

        NotPaid = 4,                        //Escrow time finished, client didn't confirm payment 
        PaymentNotConfirmed = 5,            //Escrow time finished, offer creator didn't confirm payment income

        SolvedCryptocurrencyToOfferOwner = 10,   //Dispute solved by admin, Cryptocurrency released to offer's creator balance
        SolvedCryptocurrencyToOfferClient = 11   //Dispute solved by admin, Cryptocurrency released to client's balance
    }
}
