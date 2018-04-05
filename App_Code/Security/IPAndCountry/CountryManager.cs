using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public class CountryManager
{
    public static string[] countryCode =
    {
        "-","AP","EU","AD","AE","AF","AG","AI","AL","AM","AN","AO","AQ","AR","AS","AT","AU","AW","AZ","BA","BB","BD","BE","BF","BG","BH","BI","BJ","BM","BN","BO","BR","BS","BT","BV","BW","BY","BZ","CA","CC","CD","CF","CG","CH","CI","CK","CL","CM","CN","CO","CR","CU","CV","CX","CY","CZ","DE","DJ","DK","DM","DO","DZ",
        "EC","EE","EG","EH","ER","ES","ET","FI","FJ","FK","FM","FO","FR","FX","GA","GB","GD","GE","GF","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY","HK","HM","HN","HR","HT","HU","ID","IE","IL","IN","IO","IQ","IR","IS","IT","JM","JO","JP","KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
        "LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY","MA","MC","MD","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ","OM","PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY","QA",
        "RE","RO","RU","RW","SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","ST","SV","SY","SZ","TC","TD","TF","TG","TH","TJ","TK","TM","TN","TO","TL","TR","TT","TV","TW","TZ","UA","UG","UM","US","UY","UZ","VA","VC","VE","VG","VI","VN","VU","WF","WS","YE","YT","RS","ZA","ZM","ME","ZW","A1","A2",
        "O1","AX","GG","IM","JE","BL","MF", "UK"
    };

    public static string[] countryName =
    {
        "Unknown","Asia/Pacific Region","Europe","Andorra","United Arab Emirates","Afghanistan","Antigua & Barbuda","Anguilla","Albania","Armenia","Netherlands Antilles","Angola","Antarctica","Argentina","American Samoa","Austria","Australia","Aruba","Azerbaijan","Bosnia & Herzegovina","Barbados","Bangladesh","Belgium",
        "Burkina Faso","Bulgaria","Bahrain","Burundi","Benin","Bermuda","Brunei","Bolivia","Brazil","Bahamas","Bhutan","Bouvet Island","Botswana","Belarus","Belize","Canada","Cocos (Keeling) Islands","Congo, The Democratic Republic of the","Central African Republic","Congo","Switzerland","Ivory Coast",
        "Cook Islands","Chile","Cameroon","China","Colombia","Costa Rica","Cuba","Cape Verde","Christmas Island","Cyprus","Czech Republic","Germany","Djibouti","Denmark","Dominica","Dominican Republic","Algeria","Ecuador","Estonia","Egypt","Western Sahara","Eritrea","Spain","Ethiopia","Finland","Fiji","Falkland Islands (Malvinas)",
        "Micronesia","Faroe Islands","France","France, Metropolitan","Gabon","Great Britain","Grenada","Georgia","French Guiana","Ghana","Gibraltar","Greenland","Gambia","Guinea","Guadeloupe","Equatorial Guinea","Greece","South Georgia and the South Sandwich Islands","Guatemala","Guam","Guinea-Bissau","Guyana",
        "Hong Kong","Heard Island and McDonald Islands","Honduras","Croatia","Haiti","Hungary","Indonesia","Ireland","Israel","India","British Indian Ocean Territory","Iraq","Iran","Iceland","Italy","Jamaica","Jordan","Japan","Kenya","Kyrgyzstan","Cambodia","Kiribati","Comoros","St Kitts & Nevis",
        "Korea North","Korea South","Kuwait","Cayman Islands","Kazakhstan","Laos","Lebanon","St Lucia","Liechtenstein","Sri Lanka","Liberia","Lesotho","Lithuania","Luxembourg","Latvia","Libya","Morocco","Monaco","Moldova","Madagascar",
        "Marshall Islands","Macedonia","Mali","Myanmar","Mongolia","Macau","Northern Mariana Islands","Martinique","Mauritania","Montserrat","Malta","Mauritius","Maldives","Malawi","Mexico","Malaysia","Mozambique","Namibia","New Caledonia","Niger","Norfolk Island","Nigeria","Nicaragua","Netherlands",
        "Norway","Nepal","Nauru","Niue","New Zealand","Oman","Panama","Peru","French Polynesia","Papua New Guinea","Philippines","Pakistan","Poland","Saint Pierre and Miquelon","Pitcairn Islands","Puerto Rico","Palestinian Territory","Portugal","Palau","Paraguay","Qatar","Reunion","Romania","Russian Federation","Rwanda","Saudi Arabia",
        "Solomon Islands","Seychelles","Sudan","Sweden","Singapore","Saint Helena","Slovenia","Svalbard and Jan Mayen","Slovakia","Sierra Leone","San Marino","Senegal","Somalia","Suriname","Sao Tome & Principe","El Salvador","Syria","Swaziland","Turks and Caicos Islands","Chad","French Southern Territories","Togo",
        "Thailand","Tajikistan","Tokelau","Turkmenistan","Tunisia","Tonga","East Timor","Turkey","Trinidad & Tobago","Tuvalu","Taiwan","Tanzania","Ukraine","Uganda","United States Minor Outlying Islands","United States","Uruguay","Uzbekistan","Vatican City","Saint Vincent & the Grenadines",
        "Venezuela","Virgin Islands, British","Virgin Islands, U.S.","Vietnam","Vanuatu","Wallis and Futuna","Samoa","Yemen","Mayotte","Serbia","South Africa","Zambia","Montenegro","Zimbabwe","Anonymous Proxy","Satellite Provider",
        "Other","Aland Islands","Guernsey","Isle of Man","Jersey","Saint Barthelemy","Saint Martin", "United Kingdom"
    };

    public static Dictionary<string,string> languagesCodes = new Dictionary<string, string>
    {
        { "ar-EG", "Arabic - Egypt" },              //0x0C01  ARE
        { "zh-CN", "Chinese - China" },             //0x0804  CHS
        { "hr-HR", "Croatian - Croatia" },          //0x041A  HRV
        { "cs-CZ", "Czech - Czech Republic" },      //0x0405  CSY
        { "da-DK", "Danish - Denmark" },            //0x0406  DAN
        { "en-GB", "English - United Kingdom" },    //0x0809  ENG
        { "en-US", "English - United States" },     //0x0409  ENU
        { "fr-FR", "French - France" },             //0x040C
        { "de-DE", "German - Germany" },            //0x0407
        { "el-GR", "Greek - Greece" },              //0x0408  ELL
        { "hi-IN", "Hindi - India" },               //0x0439  HIN
        { "id-ID", "Indonesian - Indonesia" },      //0x0421
        { "it-IT", "Italian - Italy" },             //0x0410
        { "ja-JP", "Japanese - Japan" },            //0x0411  JPN
        { "pl-PL", "Polish - Poland" },             //0x0415  PLK
        { "pt-PT", "Portuguese - Portugal" },       //0x0816
        { "pt-BR", "Portuguese - Brazil" },         //0x0416  PTB
        { "ru-RU", "Russian - Russia" },            //0x0419  RUS
        { "es-ES", "Spanish - Spain" },             //0x0C0A
        { "tr-TR", "Turkish - Turkey" },            //0x041F  TRK
    };

    public static string GetCountryName(string cc)
    {
        for (int i = 0; i < countryCode.Length; ++i)        
            if (countryCode[i] == Regex.Replace(cc, @"\s+", ""))
                return countryName[i];        

        return String.Empty;
    }

    public static string GetCountryCode(string country)
    {
        for (int i = 0; i < countryName.Length; ++i)        
            if (countryName[i] == country)
                return countryCode[i];        

        return String.Empty;
    }

    public static CultureInfo GetCultureInfo(string cc)
    {
        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => c.Name.EndsWith(cc.ToUpper()));
        foreach (var info in cultureInfos)
            return info;

        return new CultureInfo("en-US");
    }

    /// <summary>
    /// Works with both: IPv4 and IPv6
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static string LookupCountryCode(string ip)
    {
        GeoIPReader Reader = new GeoIPReader();
        return Reader.LookupCode(ip);
    }
}
