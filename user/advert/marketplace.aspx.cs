using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using MarchewkaOne.Titan.Balances;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using System.Text;
using Prem.PTC.Payments;
using Titan.Marketplace;
using Prem.PTC.Memberships;

public partial class About : System.Web.UI.Page
{
    public static int MaxImageHeight = 1000;
    public static int MaxImageWidth = 670;
    public string ItemAffiliateLink;

    Member _user;
    Member user
    {
        get
        {
            if (Member.IsLogged && _user == null)
                _user = Member.CurrentInCache;
            return _user;
        }
    }

    bool senderIsMenuButton;
    public bool SenderIsMenuButton
    {
        get
        { return ViewState["SenderIsMenuButton"] == null ? false : (bool)ViewState["SenderIsMenuButton"]; }
        set
        {
            ViewState["SenderIsMenuButton"] = senderIsMenuButton = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertMarketplaceEnabled);        

        if (Request.Params.Get("pid") != null && !SenderIsMenuButton) //open detail info about product
        {
            int ProductId = Convert.ToInt32(Request.Params.Get("pid"));
            MarketplaceProduct SelectedProduct = new MarketplaceProduct(ProductId);

            //insert data to info window
            MenuMultiView.ActiveViewIndex = 4;

            if (AppSettings.Marketplace.MarketplaceUsersPromoteByLinkEnabled)
            {
                if (Request.Params.Get("ref") == null && Session["MarketplacePromotorId"] == null && Member.IsLogged)
                {
                    if (user.Membership.Id > Membership.Standard.Id)
                    {
                        AffiliateLinkPlaceHolder.Visible = true;
                        ItemAffiliateLink = Request.Url.AbsoluteUri + "&ref=" + user.Id;
                    }
                }
                else if (Request.Params.Get("ref") != null)
                    Session["MarketplacePromotorId"] = int.Parse(Request.Params.Get("ref"));
            }

            ProductInfoImage.ImageUrl = SelectedProduct.ImagePath;
            ProductInfoTitle.Text = SelectedProduct.Title;
            ProductInfoDescription.Text = Mailer.ReplaceNewLines(SelectedProduct.Description);
            ProductInfoQuantity.Text = SelectedProduct.Quantity.ToString();
            ProductInfoContact.Text = Mailer.ReplaceNewLines(SelectedProduct.Contact);
            BuyProductButton.Text = SelectedProduct.Price.ToString();
            BuyProductButton.CommandArgument = ProductId.ToString();
            BuyProductFromAdBalance.CommandArgument = ProductId.ToString();
            BuyProductFromMarketplaceBalance.CommandArgument = ProductId.ToString();
        }
        else if (Request.Params.Get("s") == "t") //success bought
        {
            SuccMessage.Text = U5006.YOUSUCCBOUGHTONMARKETPLACE;
            SuccMessagePanel.Visible = true;
        }
        else if (!string.IsNullOrWhiteSpace(Request.Params.Get("h"))) //confirm purchase
        {
            MenuMultiView.ActiveViewIndex = 5;
            foreach (Button b in MenuButtonPlaceHolder.Controls)
            {
                b.CssClass = "";
            }
            SubMenuButton_MarketplaceConfirmBuying.CssClass = "ViewSelected";

            ConfirmProductButton.Text = L1.CONFIRM;
            string hash = Request.Params.Get("h");


            MarketplaceIPN ipn = TableHelper.SelectRows<MarketplaceIPN>(TableHelper.MakeDictionary("Hash", hash)).FirstOrDefault();

            MarketplaceProduct product = new MarketplaceProduct(ipn.ProductId);
            ConfirmProductImage.ImageUrl = product.ImagePath;
            ConfirmProductTitle.Text = product.Title;
            ConfirmProductDescription.Text = Mailer.ReplaceNewLines(product.Description);
            ConfirmProductContact.Text = Mailer.ReplaceNewLines(product.Contact);

            if (ipn.Status == MarketplaceIPNStatus.Pending)
                ConfirmProductButton.CommandArgument = ipn.Id.ToString();
            else if (ipn.Status == MarketplaceIPNStatus.Confirmed)
            {
                ConfirmProductButton.Visible = false;
                AlreadyConfirmedLiteral.Visible = true;
                AlreadyConfirmedLiteral.Text = string.Format("<div class='whitebox' style='width:220px'>{0}<div>", U5006.PRODUCTCONFIRMED);
            }
            if (ipn == null)
                Response.Redirect("~/user/default.aspx");
        }

        if(!Member.IsLogged)
        {
            Master.HideSidebars();
        }

        //Lang & Hint   
        //GridView1.EmptyDataText = L1.NODATA;
        LangAdder.Add(SubMenuButton_MarketplaceProducts, U5006.MARKETPLACE);
        LangAdder.Add(SubMenuButton_MarketplaceAddProduct, U6005.ADDNEWPRODUCT);
        LangAdder.Add(SubMenuButton_MarketplaceConfirmBuying, U5006.PURCHASEDPRODUCTS);
        LangAdder.Add(MarketplaceAddButton, L1.ADDNEW);
        LangAdder.Add(MarketplaceAdd_BannerUploadValidCustomValidator, L1.ER_BANNERISNOTVALID, true);
        LangAdder.Add(MarketplaceAdd_BannerUploadSelectedCustomValidator, U5006.MUSTUPLOADIMAGE, true);
        LangAdder.Add(SubSubMenuButton_MarketplaceMyProducts, U5006.MYPRODUCTS);

        LangAdder.Add(RequiredFieldValidator1, L1.REQ_TITLE, true);
        LangAdder.Add(RequiredFieldValidator2, L1.REQ_DESC, true);
        LangAdder.Add(RequiredFieldValidator5, L1.CONTACT + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(DeliveryAddressRequired, U6005.DELIVERYADDRESSREQUIRED, true);
        LangAdder.Add(EmailRequired, L1.REG_REQ_EMAIL, true);
        LangAdder.Add(EmailRegex, L1.ER_BADEMAILFORMAT, true);

        BuyProductFromAdBalance.Text = string.Format(U6005.PAYWITH, U6012.PURCHASEBALANCE);
        BuyProductFromMarketplaceBalance.Text = string.Format(U6005.PAYWITH, U6008.MARKETPLACEBALANCE);
        SubSubMenuButton_MarketplaceMyProducts.Visible =
            SubMenuButton_MarketplaceAddProduct.Visible = AppSettings.Marketplace.CanUsersAddMarketplaceProducts && Member.IsLogged;

        PurchasedProductsGridView.Visible = Member.IsLogged;
        SubMenuButton_MarketplaceConfirmBuying.Visible = Member.IsLogged || !string.IsNullOrEmpty(Request.Params.Get("h"));

        BuildMarketplaceGrid();

        if (!Page.IsPostBack)
        {
            MakeCategoriesList();
            BindDataCountriesToDDL();
            GeolocationCheckBox.Attributes.Add("onclick", "ManageGeoEvent();");
        }
        ScriptManager.GetCurrent(this).RegisterPostBackControl(MarketplaceAdd_BannerUploadSubmit);
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SenderIsMenuButton = true;
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
        {
            SenderIsMenuButton = false;
            Response.Redirect("~/user/advert/marketplace.aspx");

        }
        if (viewIndex == 1)
            ClearAll();
        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    #region MarketplaceProducts

    public void BuildMarketplaceGrid()
    {
        List<MarketplaceProduct> MarketplaceProductsList = MarketplaceProduct.GetGeolocatedProducts(user);

        if (MarketplaceProductsList.Count < 1)
        {
            MarketEmptyPlaceHolder.Visible = true;
            MarketplaceProductsPlaceholder.Visible = false;
        }
        else
        {
            try
            {
                foreach (MarketplaceProduct Product in MarketplaceProductsList)
                {
                    MarketplaceProductsPlaceholder.Controls.Add(GetMarketplaceProduct(Product));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected UserControl GetMarketplaceProduct(MarketplaceProduct Product)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/MarketplaceProduct.ascx");
        var parsedControl = objControl as MarketplaceProductObjectControl;
        parsedControl.Object = Product;
        parsedControl.DataBind();

        return objControl;
    }

    #endregion

    #region Geolocation
    private void BindDataCountriesToDDL()
    {
        AllCountries.DataSource = GeolocationUtils.GetCountriesData();
        AllCountries.DataTextField = "Value";
        AllCountries.DataValueField = "Key";
        AllCountries.DataBind();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.Add(new ListItem(AllCountries.SelectedItem.Text, AllCountries.SelectedItem.Value));
            OrderItems(GeoCountries);
            AllCountries.Items.Remove(AllCountries.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.Add(new ListItem(GeoCountries.SelectedItem.Text, GeoCountries.SelectedItem.Value));
            OrderItems(AllCountries);
            GeoCountries.Items.Remove(GeoCountries.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void btnAddAll_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.AddRange(AllCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(GeoCountries);
            AllCountries.Items.Clear();
        }
        catch (Exception ex) { }
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.AddRange(GeoCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(AllCountries);
            GeoCountries.Items.Clear();

        }
        catch (Exception ex) { }
    }

    private void OrderItems(ListBox list)
    {
        var items = list.Items.Cast<ListItem>().Select(x => x).OrderBy(x => x.Value).ToList();
        list.Items.Clear();
        list.Items.AddRange(items.ToArray());
    }
    #endregion
    #region MarketplaceAdd

    public void MakeCategoriesList()
    {
        var list = new Dictionary<string, string>();
        List<MarketplaceCategory> List = MarketplaceCategory.AllActiveCategories();
        if (List.Count < 1)
        {
            AdvertisePlaceholder.Visible = false;
            AdvertisingUnavailable.Visible = true;
        }
        else
        {
            AdvertisePlaceholder.Visible = true;
            AdvertisingUnavailable.Visible = false;
        }
        foreach (MarketplaceCategory elem in List)
        {
            list.Add((elem.Id).ToString(), elem.Title);
        }
        MarketplaceAddCategoriesList.DataSource = list;
        MarketplaceAddCategoriesList.DataTextField = "Value";
        MarketplaceAddCategoriesList.DataValueField = "Key";
        MarketplaceAddCategoriesList.DataBind();
    }

    //Creatning new campaign
    private MarketplaceProduct _newProduct;
    protected MarketplaceProduct NewProduct
    {
        get
        {
            if (_newProduct == null)

                if (Session["Marketplace_NewProduct"] is MarketplaceProduct)
                    _newProduct = Session["Marketplace_NewProduct"] as MarketplaceProduct;
                else
                {
                    NewProduct = new MarketplaceProduct();
                    Session["Marketplace_NewProduct"] = NewProduct;
                }
            return _newProduct;
        }

        set { Session["Marketplace_NewProduct"] = _newProduct = value; }
    }

    protected void MarketplaceAdd_BannerUploadSubmit_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && MarketplaceAdd_BannerUpload.HasFile)
        {
            NewTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);

            deleteOldImageIfExists();

            NewProduct.BannerImage = NewTemporaryBanner;

            MarketplaceAdd_BannerImage.ImageUrl = NewProduct.BannerImage.Path;

            //Hide upload
            MarketplaceAdd_BannerUpload.Visible = false;
            MarketplaceAdd_BannerUploadSubmit.Visible = false;

            MarketplaceAdd_BannerUpload.Dispose();
        }
    }

    private void deleteOldImageIfExists()
    {
        if (NewProduct != null && NewProduct.BannerImage != null && NewProduct.BannerImage.IsSaved && newTemporaryBanner != null)
        {
            newTemporaryBanner.Delete();
            NewProduct.BannerImage = null;
        }
    }

    Banner newTemporaryBanner;
    Banner NewTemporaryBanner
    {
        get
        {
            if (newTemporaryBanner == null)

                if (Session["NewTemporaryBanner"] is Banner)
                    NewTemporaryBanner = Session["NewTemporaryBanner"] as Banner;
                else
                {
                    NewTemporaryBanner = null;
                    Session["NewTemporaryBanner"] = NewTemporaryBanner;
                }
            return newTemporaryBanner;
        }

        set { Session["NewTemporaryBanner"] = newTemporaryBanner = value; }
    }
    protected void MarketplaceAdd_BannerUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid =
            Banner.TryFromStream(MarketplaceAdd_BannerUpload.PostedFile.InputStream, out newTemporaryBanner)
            && (newTemporaryBanner.Width <= MaxImageWidth && newTemporaryBanner.Height <= MaxImageHeight);
        if (newTemporaryBanner != null)
            NewTemporaryBanner = newTemporaryBanner;
    }

    protected void MarketplaceAdd_BannerUploadSelectedCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NewProduct.BannerImage != null;
    }

    protected void ClearAll()
    {
        //Clear all
        MarketplaceAddTitle.Text = string.Empty;
        MarketplaceAddDescription.Text = string.Empty;
        MarketplaceAddContact.Text = string.Empty;
        deleteOldImageIfExists();
        MarketplaceAdd_BannerUpload.Visible = true;
        MarketplaceAdd_BannerUploadSubmit.Visible = true;
        MarketplaceAdd_BannerImage.ImageUrl = null;
        GeolocationCheckBox.Checked = false;
        AllCountries.Items.AddRange(GeoCountries.Items.Cast<ListItem>().ToArray());
        OrderItems(AllCountries);
        GeoCountries.Items.Clear();
        GeoAgeMin.Text = "0";
        GeoAgeMax.Text = "0";
        NewProduct = null;
    }

    protected void MarketplaceAddButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Member.IsLogged && Page.IsValid)
        {
            try
            {
                NewProduct.SellerId = user.Id;
                NewProduct.Title = InputChecker.HtmlEncode(MarketplaceAddTitle.Text, MarketplaceAddTitle.MaxLength, L1.TITLE);
                NewProduct.Description = InputChecker.HtmlEncode(MarketplaceAddDescription.Text, MarketplaceAddDescription.MaxLength, L1.DESCRIPTION);
                NewProduct.Price = Money.Parse(MarketplaceAddPrice.Text);
                NewProduct.Quantity = Convert.ToInt32(MarketplaceAddQuantity.Text);
                NewProduct.Contact = InputChecker.HtmlEncode(MarketplaceAddContact.Text, MarketplaceAddContact.MaxLength, L1.CONTACT);
                NewProduct.Status = UniversalStatus.Active;
                NewProduct.CategoryId = Convert.ToInt32(MarketplaceAddCategoriesList.SelectedValue);

                if (GeolocationCheckBox.Checked)
                {
                    var validCountries = GeolocationUtils.GeoCountData.Keys;
                    var countryNames = new StringBuilder();

                    foreach (ListItem item in GeoCountries.Items)
                    {
                        if (validCountries.Contains<string>(item.Value))
                        {
                            countryNames.Append(item.Value);
                            countryNames.Append("#");
                        }
                    }

                    var minAge = Convert.ToInt32(GeoAgeMin.Text);
                    var maxAge = Convert.ToInt32(GeoAgeMax.Text);
                    var gender = (Gender)Convert.ToInt32(GeoGenderList.SelectedValue);
                    var countryCodes = GeolocationUnit.ParseFromCountriesString(countryNames.ToString());
                    var cities = string.Empty;
                    NewProduct.AddGeolocation(new GeolocationUnit(countryCodes, cities, minAge, maxAge, gender));
                }

                NewProduct.Save();

                ClearAll();
                BuildMarketplaceGrid();

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U5006.MARKETPLACEADDED;
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    #endregion

    protected void ConfirmProductButton_Click(object sender, EventArgs e)
    {
        int ipnId = Convert.ToInt32(((Button)sender).CommandArgument);

        MarketplaceManager.TryConfirmIPN(ipnId);

        ConfirmProductButton.Visible = false;
        AlreadyConfirmedLiteral.Visible = true;
        AlreadyConfirmedLiteral.Text = string.Format("<div class='whitebox' style='width:220px'>{0}<div>", U5006.PRODUCTCONFIRMED);
    }

    protected void PurchasedProductsGridViewDataSource_Init(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            PurchasedProductsGridViewDataSource.SelectCommand = string.Format(@"SELECT ipn.Id, p.ImagePath, p.Title, p.Price, ipn.ProductQuantity, p.SellerId, ipn.Status 
FROM MarketplaceProducts p 
JOIN MarketplaceIPNs ipn ON ipn.ProductId = p.Id 
WHERE ipn.BuyerId = {0}", user.Id);
        }
    }

    protected void PurchasedProductsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var Img = new Image();
            if (e.Row.Cells[1].Text != "&nbsp;")
            {
                Img.ImageUrl = e.Row.Cells[1].Text;
                Img.Width = Unit.Pixel(20);
                Img.Height = Unit.Pixel(20);
                e.Row.Cells[1].Text = "";
                e.Row.Cells[1].Controls.Add(Img);
            }

            var status = (MarketplaceIPNStatus)Convert.ToInt32(e.Row.Cells[6].Text);
            MarketplaceIPN ipn = new MarketplaceIPN(Convert.ToInt32(e.Row.Cells[0].Text));

            var link = string.Format("{0}user/advert/marketplace.aspx?h={1}", AppSettings.Site.Url, ipn.Hash);

            e.Row.Cells[2].Text = string.Format("<a href={0}>{1}</a>", link, e.Row.Cells[2].Text);

            e.Row.Cells[6].Text = HtmlCreator.GetColoredStatus(status);

            if (status != MarketplaceIPNStatus.Pending)
                e.Row.Cells[7].Text = String.Empty;
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.IMAGE;
            e.Row.Cells[2].Text = L1.TITLE;
            e.Row.Cells[3].Text = L1.PRICE;
            e.Row.Cells[4].Text = L1.AMOUNT;
            e.Row.Cells[5].Text = U5006.SELLER;
            e.Row.Cells[6].Text = L1.STATUS;
        }
    }

    protected void PurchasedProductsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "Sort" && e.CommandName != "Page")
        {
            if (e.CommandName == "ConfirmIPNCommand")
            {
                var ipnId = Convert.ToInt32(PurchasedProductsGridView.Rows[Convert.ToInt32(e.CommandArgument)].Cells[0].Text);

                MarketplaceManager.TryConfirmIPN(ipnId);

                PurchasedProductsGridView.DataBind();
            }
        }
    }

    #region MY PRODUCTS GRIDVIEW
    protected void MyProductsGridViewDataSource_Init(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            MyProductsGridViewDataSource.SelectCommand = string.Format("SELECT * FROM MarketplaceProducts WHERE SellerId = {0}", user.Id);
        }
    }
    #endregion

    protected void MyProductsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var Img = new Image();
            if (e.Row.Cells[1].Text != "&nbsp;")
            {
                Img.ImageUrl = e.Row.Cells[1].Text;
                Img.Width = Unit.Pixel(20);
                Img.Height = Unit.Pixel(20);
                e.Row.Cells[1].Text = "";
                e.Row.Cells[1].Controls.Add(Img);
            }

            MarketplaceProduct product = new MarketplaceProduct(Convert.ToInt32(e.Row.Cells[0].Text));

            var link = string.Format("{0}user/advert/marketplace.aspx?pid={1}", AppSettings.Site.Url, product.Id);

            e.Row.Cells[2].Text = string.Format("<a href={0}>{1}</a>", link, e.Row.Cells[2].Text);

        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.IMAGE;
            e.Row.Cells[2].Text = L1.TITLE;
            e.Row.Cells[3].Text = L1.PRICE;
            e.Row.Cells[4].Text = U5006.SOLD + "/" + U5006.ADVERTISED;
        }
    }

    protected void View4_Activate(object sender, EventArgs e)
    {
        MyProductsGridView.DataBind();
    }

    protected void View3_Activate(object sender, EventArgs e)
    {
        PurchasedProductsGridView.DataBind();
    }

    #region Purchase
    protected void BuyProduct_Click(object sender, EventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        ErrorMessagePanel.Visible = false;

        try
        {
            int productId = Convert.ToInt32(((Button)sender).CommandArgument);
            int quantity = Convert.ToInt32(ProductInfoBuyCount.Text);
            string deliveryAddress = InputChecker.HtmlEncode(DeliveryAddressTextBox.Text, DeliveryAddressTextBox.MaxLength, U6005.DELIVERYADDRESS);
            string email = EmailTextBox.Text.Trim();
            int? promotorId = (int?)Session["MarketplacePromotorId"];

            MarketplaceProduct product = new MarketplaceProduct(productId);

            if (Member.IsLogged)
            {
                Member buyer = user;

                if (!product.IsGeolocationMeet(buyer))
                    throw new MsgException("You don't meet Geolocation constraints for this product.");

                if (buyer.Id == product.SellerId)
                    throw new MsgException(U5006.BUYFROMYOURSELF);
            }

            if (quantity > product.Quantity)
                throw new MsgException(U5006.SELLERDOESNTHAVEENOUGHT);

            if (AppSettings.Marketplace.MarketplaceAllowPurchaseFromPaymentProcessors)
            {
                string username = Member.IsLogged ? user.Name : MarketplaceMember.AnonymousUsername;

                PaymentProcessorButtonsPlaceHolder.Visible = true;
                var bg = new BuyMarketplaceProductButtonGenerator(username, productId, quantity, deliveryAddress, email, promotorId);
                PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(bg);
            }
            if (Member.IsLogged && AppSettings.Marketplace.MarketplaceAllowPurchaseFromAdBalance)
            {
                BuyProductFromAdBalance.Visible = true;
            }
            if(Member.IsLogged && AppSettings.Marketplace.AllowPurchaseFromMarketplaceBalance)
            {
                BuyProductFromMarketplaceBalance.Visible = true;
            }
            BuyProductButton.Visible = false;
        }
        catch (MsgException ex)
        {
            ErrorMessage.Text = ex.Message;
            ErrorMessagePanel.Visible = true;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }


    protected void BuyProductFromAdOrMarketplaceBalance_Click(object sender, EventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        ErrorMessagePanel.Visible = false;
        var targetBalance = BalanceType.PurchaseBalance;

        if (((Button)sender).ID == "BuyProductFromMarketplaceBalance")
            targetBalance = BalanceType.MarketplaceBalance;
        try
        {
            int productId = Convert.ToInt32(((Button)sender).CommandArgument);
            int quantity = Convert.ToInt32(ProductInfoBuyCount.Text);
            string deliveryAddress = InputChecker.HtmlEncode(DeliveryAddressTextBox.Text, DeliveryAddressTextBox.MaxLength, U6005.DELIVERYADDRESS);
            string email = EmailTextBox.Text.Trim();

            int? promotorId;

            if (Request.Params.Get("ref") != null)
                promotorId = int.Parse(Request.Params.Get("ref"));
            else
                promotorId = null;

            MarketplaceProduct product = new MarketplaceProduct(productId);

            product.Buy(Member.Current, quantity, deliveryAddress, email, targetBalance);

            MarketplaceAdd_BannerImage.ImageUrl = null;
            Response.Redirect("~/user/advert/marketplace.aspx?s=t");
        }
        catch (MsgException ex)
        {
            ErrorMessage.Text = ex.Message;
            ErrorMessagePanel.Visible = true;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    #endregion
}
