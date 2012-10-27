namespace SalesforceNET.Entities
{
    using Attributes;

    /// <summary>
    /// Base salesforce entity class.
    /// </summary>
    public class SalesforceEntity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets entity ID.
        /// </summary>
        [IgnoreForCreate]
        public string Id { get; set; }

        #endregion Public Properties
    }
}
