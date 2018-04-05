using System.Data;
using Prem.PTC;
using System.Collections.Generic;
using Prem.PTC.Offers;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Advertising;
using System;
using System.Linq;
using MarchewkaOne.Titan.Balances;

namespace Titan.Marketplace
{
    public class MarketplaceProduct : GeolocationBase
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MarketplaceProducts"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("SellerId")]
        public int SellerId { get { return _SellerId; } set { _SellerId = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("Description")]
        public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

        [Column("Price")]
        public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

        [Column("Quantity")]
        public int Quantity { get { return _Quantity; } set { _Quantity = value; SetUpToDateAsFalse(); } }

        [Column("Sold")]
        public int Sold { get { return _Sold; } set { _Sold = value; SetUpToDateAsFalse(); } }

        [Column("ImagePath")]
        public string ImagePath { get { return _ImagePath; } set { _ImagePath = value; SetUpToDateAsFalse(); } }

        [Column("Contact")]
        public string Contact { get { return _Contact; } set { _Contact = value; SetUpToDateAsFalse(); } }

        [Column("CategoryId")]
        public int CategoryId { get { return _CategoryId; } set { _CategoryId = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get
            {
                return (UniversalStatus)StatusInt;
            }
            set { StatusInt = (int)value; }
        }

        private int _Id, _SellerId, _Quantity, _CategoryId, _Sold, _Status;
        private string _Title, _Description, _ImagePath, _Contact;
        private Money _Price;

        public MarketplaceProduct()
            : base() { }

        public MarketplaceProduct(int id) : base(id) { }

        public MarketplaceProduct(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        #endregion Columns

        public static List<MarketplaceProduct> GetGeolocatedProducts(Member user)
        {
            string query = string.Format("SELECT * FROM MarketplaceProducts WHERE Status = {0} AND Sold < Quantity", (int)UniversalStatus.Active);
            List<MarketplaceProduct> MarketplaceProductsList = TableHelper.GetListFromRawQuery<MarketplaceProduct>(query);

            if (user == null)
                return MarketplaceProductsList
                                .Where(p => p.IsGeolocated == false)
                                .ToList();
            else
                return MarketplaceProductsList
                                .Where(p => p.IsGeolocationMeet(user))
                                .ToList();
        }

        protected Banner _bannerImage;
        public Banner BannerImage
        {
            get { return _bannerImage; }
            set
            {
                _bannerImage = value;
                SetUpToDateAsFalse();
            }
        }

        public void Save(bool forceSave = false, bool IsFromAdminPanel = false)
        {
            bannerImage_PreSave(IsFromAdminPanel);
            base.Save(forceSave);
        }

        private void bannerImage_PreSave(bool isFromAdminPanel = false)
        {
            if (_bannerImage != null)
            {
                if (!_bannerImage.IsSaved)
                {
                    if (isFromAdminPanel)
                        _bannerImage.SaveOnClient(AppSettings.FolderPaths.BannerAdvertImages);
                    else
                        _bannerImage.Save(AppSettings.FolderPaths.BannerAdvertImages);
                }

                ImagePath = _bannerImage.Path;
            }
        }

        public void BuyViaProcessor(string username, int quantity, string emailAddress, string deliveryAddress, Money paidAmount, int? promotorId)
        {
            Money totalAmount = this.Price * quantity;

            if (totalAmount <= Money.Zero)
                throw new MsgException(U5006.AMOUNTEQUALZERO);

            if (paidAmount < totalAmount)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            MarketplaceMember mBuyer;

            if (Member.Exists(username))
            {
                Member user = new Member(username);
                mBuyer = new MarketplaceMember(emailAddress, deliveryAddress, user.Name, user.Id);
            }
            else
            {
                mBuyer = new MarketplaceMember(emailAddress, deliveryAddress);
            }

            if(promotorId != null)
            {
                Member promotor = new Member((int)promotorId);
                var amount = totalAmount * AppSettings.Marketplace.MarketplacePromoteCommission / 100;
                promotor.AddToMainBalance(amount, "Marketplace commission");
                promotor.SaveBalances();
            }

            MarketplaceIPN.Create(mBuyer, quantity, this);

            this.Quantity -= quantity;
            this.Sold += quantity;
            this.Save();

            History.AddPurchase(mBuyer.Name, totalAmount, "Marketplace purchase");
        }

        public void Buy(Member buyer, int quantity, string deliveryAddress, string email, BalanceType targetBalance)
        {
            Money totalAmount = this.Price * quantity;

            if (totalAmount <= Money.Zero)
                throw new MsgException(U5006.AMOUNTEQUALZERO);

            if (targetBalance == BalanceType.PurchaseBalance)
            {
                if (totalAmount > buyer.PurchaseBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                //Take money and save the user
                buyer.SubtractFromPurchaseBalance(totalAmount, "Marketplace purchase", BalanceLogType.MarketplacePurchase);
            }
            else if (targetBalance == BalanceType.MarketplaceBalance)
            {
                if (totalAmount > buyer.MarketplaceBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                //Take money and save the user
                buyer.SubtractFromMarketplaceBalance(totalAmount, "Marketplace purchase", BalanceLogType.MarketplacePurchase);
            }
            else
                throw new ArgumentException("Invalid argument: " + targetBalance.ToString(), "targetBalance");
            buyer.SaveBalances();

            //Add history entry
            History.AddPurchase(buyer.Name, totalAmount, "Marketplace purchase");

            MarketplaceMember mBuyer = new MarketplaceMember(email, deliveryAddress, buyer.Name, buyer.Id);
            MarketplaceIPN.Create(mBuyer, quantity, this);

            this.Quantity -= Quantity;
            this.Sold += Quantity;
            this.Save();
        }
    }
}