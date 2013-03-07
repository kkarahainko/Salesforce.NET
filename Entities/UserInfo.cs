namespace SalesforceNET.Entities
{
    /// <summary>
    /// Salesforce user info class.
    /// </summary>
    public class UserInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets user SFID.
        /// </summary>
        public string UserSFID { get; set; }

        /// <summary>
        /// Gets or sets organization SFID.
        /// </summary>
        public string OrganizationSFID { get; set; }

        /// <summary>
        /// Gets or sets name of the organization.
        /// </summary>
        public string OrganizationName { get; set; }

        #endregion Public Properties
    }
}
