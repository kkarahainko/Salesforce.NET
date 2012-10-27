namespace SalesforceNET.OpenAuthorization
{
    /// <summary>
    /// OAuth token response class.
    /// </summary>
    public class TokenResponse
    {
        #region Public Properties

        public string id { get; set; }
        public string issued_at { get; set; }
        public string refresh_token { get; set; }
        public string instance_url { get; set; }
        public string signature { get; set; }
        public string access_token { get; set; }

        #endregion Public Properties
    }
}