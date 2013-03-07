namespace SalesforceNET.Entities
{
    using SalesforceNET.SalesforcePartnerAPI;

    /// <summary>
    /// Salesforce object's field descriptor class.
    /// </summary>
    public class FieldDescriptor
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public fieldType Type { get; set; }

        #endregion Public Properties
    }
}
