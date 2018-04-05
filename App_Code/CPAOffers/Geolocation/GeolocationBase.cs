using System;
using System.Data;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC.Utils.NVP;
using System.Text;

namespace Prem.PTC.Offers
{
    [Serializable]
    public abstract class GeolocationBase : BaseTableObject
    {
        public static readonly char Delimiter = ','; //Default delimiter for country codes & cities

        public GeolocationBase()
            : base()
        {
            GeolocatedCC = ""; GeolocatedCities = ""; GeolocatedAgeMin = 0; GeolocatedAgeMax = 0; GeolocatedGenderInt = 0; GeolocationProfileString = "";
        }

        public GeolocationBase(int id)
            : base(id)
        {
        }

        public GeolocationBase(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
        }

        [Column("GeolocatedCC")]
        public string GeolocatedCC { get { return geo1; } set { geo1 = value; SetUpToDateAsFalse(); } }

        [Column("GeolocatedCities")]
        public string GeolocatedCities { get { return geo2; } set { geo2 = value; SetUpToDateAsFalse(); } }

        [Column("GeolocatedAgeMin")]
        public int GeolocatedAgeMin { get { return geo3; } set { geo3 = value; SetUpToDateAsFalse(); } }

        [Column("GeolocatedAgeMax")]
        public int GeolocatedAgeMax { get { return geo4; } set { geo4 = value; SetUpToDateAsFalse(); } }

        [Column("GeolocatedGender")]
        protected int GeolocatedGenderInt { get { return geo5; } set { geo5 = value; SetUpToDateAsFalse(); } }
        public Gender GeolocatedGender { get { return (Gender)GeolocatedGenderInt; } set { GeolocatedGenderInt = (int)value; } }

        [Column("GeolocationProfile")]
        protected string GeolocationProfileString { get { return geo6; } set { geo6 = value; SetUpToDateAsFalse(); } }
        public NotNullNameValuePairs GeolocationProfile { get { return NotNullNameValuePairs.Parse(geo6); } set { geo6 = value.ToString(); } }

        private int geo3, geo4, geo5;
        private string geo1, geo2, geo6;

        public bool IsGeolocated
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCC) && String.IsNullOrWhiteSpace(GeolocatedCities) && 
                    !IsGeolocatedByProfile && GeolocatedAgeMin <= 0 && GeolocatedAgeMax <= 0 && GeolocatedGenderInt <= 0)
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByCountry
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCC))
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByCity
        {
            get
            {
                if (String.IsNullOrWhiteSpace(GeolocatedCities))
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByGender
        {
            get
            {
                if (GeolocatedGender == Gender.Null)
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByAge
        {
            get
            {
                if (GeolocatedAgeMax == 0 && GeolocatedAgeMin == 0)
                    return false;
                return true;
            }
        }

        public bool IsGeolocatedByProfile
        {
            get
            {
                if (GeolocationProfile.Count > 0 && !String.IsNullOrWhiteSpace(GeolocationProfile.Keys.First<string>())) 
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Adds specified GeolocationUnit to the geolocation. Requires Save()
        /// </summary>
        public void AddGeolocation(GeolocationUnit unit)
        {
            GeolocatedCC = (String.IsNullOrEmpty(unit.CountryCodes)) ? GeolocatedCC : unit.CountryCodes;
            GeolocatedCities = (String.IsNullOrEmpty(unit.Cities)) ? GeolocatedCities : unit.Cities;
            GeolocationProfile = (unit.Profile.Count == 0) ? GeolocationProfile : unit.Profile;
            GeolocatedAgeMax = (unit.MaxAge == 0) ? GeolocatedAgeMax : unit.MaxAge;
            GeolocatedAgeMin = (unit.MinAge == 0) ? GeolocatedAgeMin : unit.MinAge;
            GeolocatedGender = (unit.Gender == Gender.Null) ? GeolocatedGender : unit.Gender;
        }

        /// <summary>
        /// Clears all kind of geolocation. Requires Save()
        /// </summary>
        public void ClearGeolocation()
        {
            GeolocatedCC = ""; GeolocatedCities = ""; GeolocatedAgeMin = 0; GeolocatedAgeMax = 0; GeolocatedGenderInt = 0; GeolocationProfileString = "";
        }

        /// <summary>
        /// Clears country geolocation. Requires Save()
        /// </summary>
        public void ClearCountryGeolocation()
        {
            GeolocatedCC = ""; 
        }
        
        /// <summary>
        /// Checks whether the member passes the geolocation requirements or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsGeolocationMeet(Member user)
        {
            if (IsGeolocated)
            {
                try
                {
                    if (IsGeolocatedByCountry && !GeolocatedCC.Contains(user.CountryCode))
                        return false;

                    if (IsGeolocatedByCity && !GeolocatedCities.Contains(user.City))
                        return false;

                    if (IsGeolocatedByGender && user.Gender != GeolocatedGender)
                        return false;

                    if (GeolocatedAgeMin > 0 && user.Age < GeolocatedAgeMin)
                        return false;

                    if (GeolocatedAgeMax > 0 && user.Age > GeolocatedAgeMax)
                        return false;

                    if (!IsGeolocationProfileMeet(GeolocationProfile, user))
                        return false;
                }
                catch (Exception ex) { ErrorLogger.Log(ex); }
            }

            return true;
        }

        public bool IsGeolocationMet(string countryCode, int? age, Gender gender)
        {
            if (IsGeolocated)
            {
                try
                {
                    if (IsGeolocatedByCountry && !GeolocatedCC.Contains(countryCode))
                        return false;

                    //Geolocation by city is not implemented

                    //if (IsGeolocatedByCity && !GeolocatedCities.Contains(user.City))
                    //    return false;

                    if (IsGeolocatedByGender && gender != GeolocatedGender)
                        return false;

                    if (GeolocatedAgeMin > 0 && (!age.HasValue || age < GeolocatedAgeMin))
                        return false;

                    if (GeolocatedAgeMax > 0 && (!age.HasValue || age > GeolocatedAgeMax))
                        return false;
                }
                catch (Exception ex) { ErrorLogger.Log(ex); }
            }

            return true;
        }

        /// <summary>
        /// Checks whether the member passes the geolocation requirements in GeolocationUnit or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsGeolocationMeet(GeolocationUnit unit, Member user)
        {
            CPAOffer offer = new CPAOffer();
            offer.AddGeolocation(unit);
            return offer.IsGeolocationMeet(user);
        }

        /// <summary>
        /// Copies all geolocation records between two GeolocationBase objects
        /// </summary>
        /// <param name="from"></param>
        public void CopyGeolocation(GeolocationBase from)
        {
            this.GeolocatedCC = from.GeolocatedCC.Replace(':',',');
            this.GeolocatedCities = from.GeolocatedCities;
            this.GeolocatedGender = from.GeolocatedGender;
            this.GeolocatedAgeMax = from.GeolocatedAgeMax;
            this.GeolocatedAgeMin = from.GeolocatedAgeMin;
            this.GeolocationProfileString = from.GeolocationProfileString;
        }

        /// <summary>
        /// Checks whether geolocated countries are the same for both offers or not
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool AreGeolocatedCountriesTheSame(GeolocationBase compare)
        {
            if (this.GeolocatedCC == compare.GeolocatedCC)
                return true;
            return false;
        }

        /// <summary>
        /// Generates geolocation-SQL insert statement part, for SyncedNetwork class
        /// </summary>
        /// <returns></returns>
        public string GetGeolocationSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GeolocatedAgeMin)
              .Append(", ")
              .Append(this.GeolocatedAgeMax)
              .Append(", ")
              .Append(this.GeolocatedGenderInt)
              .Append(", '")
              .Append(this.GeolocatedCC.Replace("'", "''"))
              .Append("', '")
              .Append(this.GeolocatedCities.Replace("'", "''"))
              .Append("', '")
              .Append(this.GeolocationProfileString.Replace("'", "''"))
              .Append("'");
            return sb.ToString();
        }

        public void Save(bool forceSave = false)
        {
            base.Save(forceSave);
        }

        public List<string> GetGeolocatedCountries()
        {
            List<string> countriesList = new List<string>();
            var str = GeolocatedCC.TrimEnd(',');
            var charsToRemove = new string[] { "[", "]", "\"", "\"" };
            foreach (var c in charsToRemove)
                str = str.Replace(c, string.Empty);

            var ccs = str.Split(Delimiter);

            foreach (var cc in ccs)
                countriesList.Add(CountryManager.GetCountryName(cc));
            
            return countriesList;
        }

        private static bool IsGeolocationProfileMeet(NotNullNameValuePairs profile, Member user)
        {
            if (profile.Count > 0 && !String.IsNullOrEmpty(profile.Keys.First<string>())) 
            {
                foreach (var pair in profile)
                {
                    if (!string.IsNullOrEmpty(pair.Key))
                    {
                        bool exists = false;
                        foreach (var custom in user.Custom)
                            if (pair.Key == custom.Key)
                            {
                                exists = true;
                                if (GeolocationNumber.IsGeolocationNumber(pair.Value) && !GeolocationNumber.Parse(pair.Value).IsBetweenMinAndMax(Convert.ToInt32(custom.Value)))
                                    return false;
                                else if (!GeolocationNumber.IsGeolocationNumber(pair.Value) && pair.Value != custom.Value)
                                    return false;
                            }

                        if (exists == false)
                            return false;
                    }
                }
            }

            return true;
        }

    }
}