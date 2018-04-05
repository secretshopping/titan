using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.InvestmentPlatform;
using Titan.Marketplace;

namespace Prem.PTC.Payments
{
    /// <summary>
    /// Generates data necessary to create button.
    /// Each button generator has symmetric IIpnHandler to handle generated request.
    /// </summary>
    /// <seealso cref="IIpnHandler"/>
    public interface IButtonGenerator
    {
        /// <summary>
        /// Generates data necessary to create button.
        /// </summary>
        /// <returns>should return ready to use http link redirecting to Payza/PayPal/etc.
        /// website where user can finish payment. Optionally can return html fragment
        /// (for example html form) to create buttton.</returns>
        string Generate();
    }

    public abstract class BaseButtonGenerator : IButtonGenerator
    {
        /// <summary>
        /// Specifies technical details about button creation 
        /// (PayPal/Payza/etc. have different IPNs so buttons are different)
        /// This field should be set before calling Generate()
        /// </summary>
        public ButtonGenerationStrategy Strategy { get; set; }

        public abstract string Generate();
    }

    public class BuyAdvertButtonGenerator<AP> : BaseButtonGenerator where AP : IAdvertPack
    {
        public Advert<AP> Advert { get; set; }

        public BuyAdvertButtonGenerator() { }

        public BuyAdvertButtonGenerator(Advert<AP> advert)
        {
            this.Advert = advert;
        }

        public override string Generate()
        {
            IAdvertPack pack = Advert.Pack;

            return Strategy.Generate(
                Advert.TargetUrl + " (" + pack.Ends.ToString() + ')',
                Advert.Price,
                typeof(BuyAdvertIpnHandler),
                Advert.GetType().FullName, Advert.Id, this.Strategy.ToString(), Advert.Price.ToClearString());
        }
    }


    public class TransferToRentalBalanceButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public string Username { get; set; }

        public TransferToRentalBalanceButtonGenerator() { }

        public TransferToRentalBalanceButtonGenerator(Member member, Money amount)
            : this(member.Name, amount)
        { }

        public TransferToRentalBalanceButtonGenerator(string username, Money amount)
        {
            this.Username = username;
            this.Amount = amount;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Money transfer to Traffic Balance: " + Amount.ToString(),
                Amount,
                typeof(TransferToRentalBalanceIpnHandler),
                Username, Amount.ToClearString(), this.Strategy.ToString()
                );
        }
    }

    public class TransferToAdvertisingBalanceButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public string Username { get; set; }

        public TransferToAdvertisingBalanceButtonGenerator() { }

        public TransferToAdvertisingBalanceButtonGenerator(Member member, Money amount)
            : this(member.Name, amount)
        { }

        public TransferToAdvertisingBalanceButtonGenerator(string username, Money amount)
        {
            this.Username = username;
            this.Amount = amount;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Money transfer to Advertising Balance: " + Amount.ToString(),
                Amount,
                typeof(TransferToAdvertisingBalanceIpnHandler),
                Username, Amount.ToClearString(), this.Strategy.ToString()
                );
        }
    }

    public class TransferToPointsBalanceButtonGenerator : BaseButtonGenerator
    {
        public int Points { get; set; }
        public Money Amount { get; set; }
        public string Username { get; set; }

        public TransferToPointsBalanceButtonGenerator() { }

        public TransferToPointsBalanceButtonGenerator(Member member, Money amount)
            : this(member.Name, amount)
        { }

        public TransferToPointsBalanceButtonGenerator(string username, Money amount)
        {
            this.Username = username;
            this.Points = amount.ConvertToPoints();
            this.Amount = amount;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                String.Format("Deposit {1} {0}", AppSettings.PointsName, Points.ToString()),
                Amount,
                typeof(TransferToPointsBalanceIpnHandler),
                
                Username,
                Points.ToString(),
                this.Strategy.ToString()
                );
        }
    }

    public class BuyMarketplaceProductButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public string Username { get; set; }
        private readonly int productId;
        private readonly int quantity;
        private readonly string deliveryAddress;
        private readonly string email;
        private readonly MarketplaceProduct product;
        private readonly int? promotorId;

        public BuyMarketplaceProductButtonGenerator(string username, int productId, int quantity, string deliveryAddress, string email, int? promotorId)
        {
            product = new MarketplaceProduct(productId);
            Amount = product.Price * quantity;
            Username = username;
            this.productId = product.Id;            
            this.quantity = quantity;
            this.deliveryAddress = deliveryAddress;
            this.email = email;
            this.promotorId = promotorId;
        }

        public override string Generate()
        {
            string itemName = string.Format("Buy {0} x {1} from Marketplace ({2})", quantity, product.Title, Amount);
            string emailWithPromotorId = HashingManager.Base64Encode(email) + "#" + HashingManager.Base64Encode(promotorId.ToString());
            return Strategy.Generate(
                itemName: itemName,
                amount: Amount,
                handlerCommand: typeof(BuyMarketplaceProductIpnHandler),
                args: new object[]
                {
                    Username,
                    productId,
                    this.Strategy.ToString(),
                    quantity,
                    deliveryAddress,
                    emailWithPromotorId
                });
        }
    }

    public class TransferToMarketplaceBalanceButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public string Username { get; set; }

        public TransferToMarketplaceBalanceButtonGenerator() { }

        public TransferToMarketplaceBalanceButtonGenerator(Member member, Money amount)
            : this(member.Name, amount)
        { }

        public TransferToMarketplaceBalanceButtonGenerator(string username, Money amount)
        {
            this.Username = username;
            this.Amount = amount;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Money transfer to Marketplace Balance: " + Amount.ToString(),
                Amount,
                typeof(TransferToMarketplaceBalanceIpnHandler),
                Username, Amount.ToClearString(), this.Strategy.ToString()
                );
        }
    }

    public class TransferToCashBalanceButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public string Username { get; set; }

        public TransferToCashBalanceButtonGenerator() { }

        public TransferToCashBalanceButtonGenerator(Member member, Money amount)
            : this(member.Name, amount)
        { }

        public TransferToCashBalanceButtonGenerator(string username, Money amount)
        {
            this.Username = username;
            this.Amount = amount;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Money transfer to Cash Balance: " + Amount.ToString(),
                Amount,
                typeof(TransferToCashBalanceIpnHandler),
                Username, Amount.ToClearString(), this.Strategy.ToString()
                );
        }
    }

    public class UpgradeMembershipButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public Member Member { get; set; }

        public MembershipPack MembershipPack { get; set; }

        public UpgradeMembershipButtonGenerator(Member member, Money amount, MembershipPack membershipPack)
        {
            this.Amount = amount;
            this.Member = member;
            this.MembershipPack = membershipPack;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Upgrade membership: " + Amount.ToString(),
                Amount,
                typeof(BuyMembershipIpnHandler),
                args: new object[] { Member.Name, MembershipPack.Id,
                this.Strategy.ToString() }
                );
        }
    }

    public class DepositCryptocurrencyButtonGenerator : BaseButtonGenerator
    {
        public Money Amount { get; set; }
        public Member Member { get; set; }        

        public DepositCryptocurrencyButtonGenerator(Member member, Money amount)
        {
            Amount = amount;
            Member = member;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Deposit Money: " + Amount.ToString(),
                Amount,
                typeof(DepositCryptocurrencyIpnHandler),
                args: new object[] { Member.Name, AppSettings.Site.CurrencyCode, Strategy.ToString() }
                );
        }
    }

    public class DepositToWalletCryptocurrencyButtonGenerator : BaseButtonGenerator
    {
        public CryptocurrencyMoney Amount { get; set; }
        public Member Member { get; set; }

        public DepositToWalletCryptocurrencyButtonGenerator(Member member, CryptocurrencyMoney amount)
        {
            Amount = amount;
            Member = member;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Deposit " + Amount.ToString(),
                Amount,
                typeof(WalletDepositCryptocurrencyIpnHandler),
                args: new object[] { Member.Name, Amount.cryptocurrencyType.ToString(), Strategy.ToString() }
                );
        }
    }

    public class ActivateAccountButtonGenerator : BaseButtonGenerator
    {
        public Member Member { get; set; }

        public ActivateAccountButtonGenerator(Member member)
        {
            this.Member = member;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Account activation ",
                AppSettings.Registration.AccountActivationFee,
                typeof(ActivateAccountIpnHandler),
                Member.Id, null, this.Strategy.ToString());
        }
    }

    public class BuyInvestmentPlanButtonGenerator : BaseButtonGenerator
    {
        public Member Member { get; set; }
        public InvestmentPlatformPlan Plan { get; set; }      
        public Money Price { get; set; }

        public BuyInvestmentPlanButtonGenerator(Member member, InvestmentPlatformPlan plan, Money price)
        {
            Member = member;
            Plan = plan;
            Price = price;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                "Investment plan: " + Plan.Name.ToString(),
                Price,
                typeof(BuyInvestmentPlanIpnHandler),
                args: new object[] { Member.Name, Plan.Id, Strategy.ToString() }
                );
        }
    }

    public class BuyJackpotTicketsButtonGenerator : BaseButtonGenerator
    {
        public Member Member { get; set; }
        public Jackpot Jackpot { get; set; }
        public Money Price { get; set; }
        public int Tickets { get; set; }

        public BuyJackpotTicketsButtonGenerator(Member member, Jackpot jackpot, Money price, int tickets)
        {
            Member = member;
            Jackpot = jackpot;
            Price = price;
            Tickets = tickets;
        }

        public override string Generate()
        {
            return Strategy.Generate(
                String.Format("Jackpot - {0}: {1} ticket(s)", Jackpot.Name, Tickets),
                Price,
                typeof(BuyJuckpotTicketsIpnHandler),
                args: new object[] { Member.Name, Jackpot.Id, Strategy.ToString(), Tickets }
                );
        }
    }
}