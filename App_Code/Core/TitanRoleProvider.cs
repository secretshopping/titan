using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Security;

namespace Prem.PTC.Members
{
    sealed public class TitanRoleProvider : System.Web.Security.RoleProvider
    {

        private string _ApplicationName;
        private string _RemoteProviderName;

        public static readonly string MODERATORS = "Moderators";
        public static readonly string ADMINISTRATORS = "Administrators";
        public static readonly string REGISTERED = "Registered";

        /// <summary>
        /// required implementation
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            return configValue;
        }

        /// <summary>
        /// handle the Initiate override and extract our parameters
        /// </summary>
        /// <param name="name">name of the provider</param>
        /// <param name="config">configuration collection</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "TitanRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample TitanRoleProvider");
            }

            // Initialize the abstract base class. 
            base.Initialize(name, config);


            if (config["applicationName"] == null || config["applicationName"].Trim() == "")
            {
                _ApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                _ApplicationName = config["applicationName"];
            }


        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="usernames">a list of usernames</param>
        /// <param name="roleNames">a list of roles</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            SetupRoles(usernames, roleNames, true);
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="roleName">a role name</param>
        public override void CreateRole(string roleName)
        {
            throw new Exception("Opperation forbidden");
            throw new NotImplementedException();
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="roleName">a role</param>
        /// <param name="throwOnPopulatedRole">get upset of users are in a role</param>
        /// <returns></returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("Opperation forbidden");
            throw new NotImplementedException();
        }

        /// <summary>
        /// required implemention
        /// </summary>
        /// <param name="roleName">a role</param>
        /// <param name="usernameToMatch">a username to look for in the role</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            List<string> result = new List<string>();
            List<Member> listOfUsernames = TableHelper.GetListFromQuery<Member>("WHERE Username LIKE '%" + usernameToMatch + "%'");

            foreach (var user in listOfUsernames)
            {
                if (roleName == MODERATORS && user.IsForumModerator)
                    result.Add(user.Name);

                if (roleName == ADMINISTRATORS && user.IsForumAdministrator)
                    result.Add(user.Name);
            }

            return result.ToArray();
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            var list = new List<string>();
            list.Add(REGISTERED);
            list.Add(ADMINISTRATORS);
            list.Add(MODERATORS);

            return list.ToArray();
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="username">a username</param>
        /// <returns>a list of roles</returns>
        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            try
            {
                Member user = new Member(username);

                roles.Add(REGISTERED); // to all members

                if (user.IsForumModerator)
                    roles.Add(MODERATORS);

                if (user.IsForumAdministrator)
                    roles.Add(ADMINISTRATORS);
            }
            catch (Exception ex) { }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var result = new List<string>();

            if (roleName == REGISTERED)
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    var dt = bridge.Instance.ExecuteRawCommandToDataTable("SELECT Username FROM Users");

                    foreach (DataRow dr in dt.Rows)
                        result.Add(dr[0].ToString());
                }
            }
            else if (roleName == ADMINISTRATORS)
            {
                var resultList = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary(Member.Columns.IsForumAdministrator, true));
                foreach (var member in resultList)
                    result.Add(member.Name);
            }
            else if (roleName == MODERATORS)
            {
                var resultList = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary(Member.Columns.IsForumModerator, true));
                foreach (var member in resultList)
                    result.Add(member.Name);
            }

            return result.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                Member User = new Member(username);

                if (roleName == REGISTERED)
                    return true;

                else if (roleName == ADMINISTRATORS)
                    return User.IsForumAdministrator;

                else if (roleName == MODERATORS)
                    return User.IsForumModerator;
            }
            catch (Exception ex) { }

            return false;
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="usernames">a list of usernames</param>
        /// <param name="roleNames">a list of roles</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SetupRoles(usernames, roleNames, false);
        }

        /// <summary>
        /// required implementation
        /// </summary>
        /// <param name="roleName">a role</param>
        /// <returns>true or false</returns>
        public override bool RoleExists(string roleName)
        {
            if (roleName == ADMINISTRATORS || roleName == REGISTERED || roleName == MODERATORS)
                return true;
            return false;
        }

        private void SetupRoles(string[] usernames, string[] roleNames, bool roleValue)
        {
            foreach (string role in roleNames)
            {
                foreach (string username in usernames)
                {
                    Member User = new Member(username);

                    if (role == MODERATORS)
                        User.IsForumModerator = roleValue;
                    else if
                        (role == ADMINISTRATORS)
                        User.IsForumAdministrator = roleValue;

                    User.Save();
                }
            }
        }

        public static string GetListOfModerators()
        {
            return GetListOfRoles(TitanRoleProvider.MODERATORS) == "" ? "<i>This list is empty</i>" : GetListOfRoles(TitanRoleProvider.MODERATORS);
        }

        public static string GetListOfAdministrators()
        {
            return GetListOfRoles(TitanRoleProvider.ADMINISTRATORS) == "" ? "<i>This list is empty</i>" : GetListOfRoles(TitanRoleProvider.ADMINISTRATORS);
        }

        private static string GetListOfRoles(string role)
        {
            string result = "";

            TitanRoleProvider Provider = new TitanRoleProvider();
            var moderators = Provider.GetUsersInRole(role);

            foreach (var elem in moderators)
                result += elem + ", ";

            return result;
        }
    }
}