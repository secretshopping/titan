using Prem.PTC.Members;
using System;

namespace Prem.PTC.Advertising
{
    public struct Advertiser
    {
        public enum Creator { Null = 0, Member = 1, Stranger = 2, Admin = 3 }

        private string _memberUsername;
        public string MemberUsername
        {
            get
            {
                if (CreatedBy != Creator.Member) return string.Empty;

                return _memberUsername;
            }
            private set
            {
                if (CreatedBy != Creator.Member) return;

                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _memberUsername = value;
            }
        }

        private string _strangerEmail;
        public string StrangerEmail
        {
            get
            {
                if (CreatedBy != Creator.Stranger) return string.Empty;

                return _strangerEmail;
            }
            private set
            {
                if (CreatedBy != Creator.Stranger) return;

                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _strangerEmail = value;
            }
        }

        public Creator CreatedBy { get; private set; }


        #region Constructors

        private Advertiser(Creator createdBy, string creatorUsername, string creatorEmail)
            : this()
        {
            CreatedBy = createdBy;
            MemberUsername = creatorUsername;
            StrangerEmail = creatorEmail;
        }

        private Advertiser(Creator createdBy) : this(createdBy, string.Empty, string.Empty) { }

        #endregion Constructors


        public static Advertiser Null
        {
            get { return new Advertiser(Creator.Null); }
        }

        public static Advertiser AsMember(string memberUsername)
        {
            return new Advertiser(Creator.Member, memberUsername, string.Empty);
        }

        public static Advertiser AsStranger(string strangerEmail)
        {
            return new Advertiser(Creator.Stranger, string.Empty, strangerEmail);
        }

        public static Advertiser AsAdmin
        {
            get { return new Advertiser(Creator.Admin); }
        }

        /// <summary>
        /// Helper factory method to easily instantiate Advertiser 
        /// from table columns using all properties.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="InvalidOperationException" />
        public static Advertiser FromColumns(int createdBy, string creatorUsername, string creatorEmail)
        {
            if (!Enum.IsDefined(typeof(Creator), createdBy))
                throw new ArgumentException("There is no Creator with ordinal \'" + createdBy + "\'");

            Creator creator = (Creator)createdBy;

            switch (creator)
            {
                case Creator.Null: return Advertiser.Null;
                case Creator.Admin: return Advertiser.AsAdmin;
                case Creator.Member: return Advertiser.AsMember(creatorUsername);
                case Creator.Stranger: return Advertiser.AsStranger(creatorEmail);

                default: throw new NotImplementedException("Not all options are implemented");
            }
        }

        public bool Is(Creator creator) { return CreatedBy == creator; }

        public override string ToString()
        {
            switch (CreatedBy)
            {
                case Creator.Null: return "N/A";
                case Creator.Admin: return "Admin";
                case Creator.Member: return MemberUsername;
                case Creator.Stranger: return StrangerEmail;

                default: throw new NotImplementedException("Not all options are implemented");
            }
        }
    }
}