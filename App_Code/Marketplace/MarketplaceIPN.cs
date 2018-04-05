using System;
using Prem.PTC;
using System.Collections.Generic;
using System.Data;
using Prem.PTC.Advertising;
using System.Linq;

namespace Titan.Marketplace
{
    public class MarketplaceIPN : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MarketplaceIPNs"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("BuyerId")]
        public int BuyerId { get { return _BuyerId; } set { _BuyerId = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        [Column("ProductId")]
        public int ProductId { get { return _ProductrId; } set { _ProductrId = value; SetUpToDateAsFalse(); } }

        [Column("ProductQuantity")]
        public int ProductQuantity { get { return _ProductQuantity; } set { _ProductQuantity = value; SetUpToDateAsFalse(); } }

        [Column("DateAdded")]
        public DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

        [Column("DeliveryAddress")]
        public string DeliveryAddress { get { return _DeliveryAddress; } set { _DeliveryAddress = value; SetUpToDateAsFalse(); } }

        [Column("BuyerEmail")]
        public string BuyerEmail { get { return _BuyerEmail; } set { _BuyerEmail = value; SetUpToDateAsFalse(); } }

        [Column("Hash")]
        public string Hash { get { return _Hash; } set { _Hash = value; SetUpToDateAsFalse(); } }

        public MarketplaceIPNStatus Status
        {
            get { return (MarketplaceIPNStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        private int _Id, _BuyerId, _ProductrId, _ProductQuantity, _Status;
        private string _Hash, _DeliveryAddress, _BuyerEmail;
        private DateTime _DateAdded;

        #endregion Columns
        public MarketplaceIPN()
            : base() { }

        public MarketplaceIPN(int id) : base(id) { }

        public MarketplaceIPN(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        public MarketplaceIPN(int buyerId, int quantity, int productId, string deliveryAddress, string email)
        {
            BuyerId = buyerId;
            Status = MarketplaceIPNStatus.Pending;
            ProductId = productId;
            ProductQuantity = quantity;
            DateAdded = AppSettings.ServerTime;
            Hash = CreateHash();
            DeliveryAddress = deliveryAddress;
            BuyerEmail = email;
        }
        public static void Create(MarketplaceMember buyer, int quantity, MarketplaceProduct product)
        {
            MarketplaceIPN IPN = new MarketplaceIPN(buyer.Id, quantity, product.Id, buyer.DeliveryAddress, buyer.Email);
            IPN.Save();

            SendEmailNotification(buyer, product.Title, IPN.Hash);
        }

        private static void SendEmailNotification(MarketplaceMember buyer, string productTitle, string ipnHash)
        {
            try
            {
                Mailer.SendNewMarketplaceMessage(buyer.Email, productTitle, ipnHash);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ErrorLogger.Log(string.Format("Could not send Marketplace email to {0} ({1}). IPN Hash: {2}", buyer.Name, buyer.Email, ipnHash));
            }
        }

        string CreateHash()
        {
            string input = AppSettings.Offerwalls.UniversalHandlerPassword + ((int)AppSettings.ServerTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return HashingManager.SHA256(input);
        }
    }
}

