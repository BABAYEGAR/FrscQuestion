namespace FrscQuestion.Models
{
    public class AppConfig
    {
        #region SocialMediaLogin

        public string GoogleClientId => "566533178136-v4pn69g7o5i3g2cif8nsiqjokp2ieefg.apps.googleusercontent.com";
        public string GoogleClientSecret => "aaGVzKs8s0JxKsFJnoDV-QBB";

        #endregion

        #region PaymentCharges

        public string LivePaystackBaseUrl => "https://api.paystack.co/";
        //public string TestPaystackBaseUrl => "https://api.paystack.co/";
        public string PaystackPaymentBaseUrl => LivePaystackBaseUrl + "flwv3-pug/getpaidx/api/charge";
        public string PaystackFetchBanksBaseUrl => LivePaystackBaseUrl + "bank";
        public string PaystackCreateAccountReceipient => LivePaystackBaseUrl + "transferrecipient";
        public string PaystackFundTransfer => LivePaystackBaseUrl + "transfer";
        public double PaystackFixedCharge => 100;
        public double PaystackPercentageCharge => 1.5;

        public string PaystackVerifyBankAccountBaseUrl =>
            LivePaystackBaseUrl + "bank/resolve?";

        public string PaystackValidateUrl => LivePaystackBaseUrl + "transaction/verify/";
        public string PaystackRefundUrl => LivePaystackBaseUrl + "refund";

        #endregion

        #region Mailer

        public string EmailServer => "smtp.ionos.com";
        public string SupportEmail => "support@afriplugz.com";
        public string SupportEmailPassword => "Afriplugz20018#";
        public int Port => 465;
        public string NewUserHtml => "wwwroot/EmailTemplates/NewUser.html";
        public string NewUserTokenHtml => "wwwroot/EmailTemplates/NewUserToken.html";
        public string NewUserSocialHtml => "wwwroot/EmailTemplates/NewUserSocial.html";
        public string NewOrderHtml => "wwwroot/EmailTemplates/NewOrder.html";
        public string NewTicketHtml => "wwwroot/EmailTemplates/Ticket.html";
        public string ForgotPasswordHtml => "wwwroot/EmailTemplates/ForgotPassword.html";
        public string ContactHtml => "wwwroot/EmailTemplates/Contact.html";
        public string NewsletterHtml => "wwwroot/EmailTemplates/NewsLetter.html";
        public string GeneralNoticeHtml => "wwwroot/EmailTemplates/GeneralNotice.html";
        public string TokenVerificationHtml => "wwwroot/EmailTemplates/TokenVerification.html";
        public string TicketVerificationHtml => "wwwroot/EmailTemplates/TicketVerification.html";
        public string BankAccountUpdateHtml => "wwwroot/EmailTemplates/BankAccountUpdate.html";
        public string AppUrl => "https://afriplugz.com/";
        //public string AppUrl => "http://212.227.11.252/";
        public string PrivacyPolicy => new AppConfig().AppUrl + "Home/PrivacyPolicy";
        public string Terms => new AppConfig().AppUrl + "Home/Terms";
        public string AppLogo => new AppConfig().AppUrl + "images/email.png";

        #endregion

        #region Cloudinary

        public string CloudinaryBaseUrl => "https://res.cloudinary.com/";
        public string CloudinaryBaseUrlImageFetch => "image/fetch/";
        public string CloudinaryAccoutnName => "afriplugz/";
        public string CloudinaryApiKey => "439175294525735";
        public string CloudinaryApiSecret => "BkkTAASwQbEFze3oasAYqksV7Pg";

        public string UpcomingEvents =>
            CloudinaryBaseUrl + CloudinaryAccoutnName + CloudinaryBaseUrlImageFetch +
            "q_auto:low,f_auto/w_360,h_270,c_fill,g_auto,dpr_auto/";
        public string Slider =>
            CloudinaryBaseUrl + CloudinaryAccoutnName + CloudinaryBaseUrlImageFetch +
            "q_auto:low,f_auto/w_1895,h_710,c_fill,g_auto,dpr_auto/";
        public string SignUp =>
            CloudinaryBaseUrl + CloudinaryAccoutnName + CloudinaryBaseUrlImageFetch +
            "q_auto:low,f_auto/w_1895,h_487,c_fill,g_auto,dpr_auto/";
        public string CartImage =>
            CloudinaryBaseUrl + CloudinaryAccoutnName + CloudinaryBaseUrlImageFetch +
            "q_auto:low,f_auto/w_160,h_160,c_fill,g_auto,dpr_auto/";

        public string EventHeader =>
            CloudinaryBaseUrl + CloudinaryAccoutnName + CloudinaryBaseUrlImageFetch +
            "q_auto:low,f_auto/w_1985,h_241,c_fill,g_auto,dpr_auto/";

        #endregion
    }
}