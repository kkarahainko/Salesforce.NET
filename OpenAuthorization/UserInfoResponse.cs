namespace SalesforceNET.OpenAuthorization
{
    using System.Collections.Generic;

    /// <summary>
    /// OAuth user info response class.
    /// </summary>
    public class UserInfoResponse
    {
        #region Public Properties

        public string id { get; set; }

        public string user_id { get; set; }
        public string organization_id { get; set; }

        public Dictionary<string, string> urls { get; set; }

        public string username { get; set; }

        public string nick_name { get; set; }
        public string display_name { get; set; }

        public string first_name { get; set; }
        public string last_name { get; set; }

        public string email { get; set; }

        #endregion Public Properties
    }
}