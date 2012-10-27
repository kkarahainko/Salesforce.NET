namespace SalesforceNET.Attributes
{
    using System;

    /// <summary>
    /// Ignore property for update operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForUpdateAttribute : IgnoreForAttribute { }
}
