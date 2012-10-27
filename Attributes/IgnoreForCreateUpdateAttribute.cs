namespace SalesforceNET.Attributes
{
    using System;

    /// <summary>
    /// Ignore property for create and update operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForCreateUpdateAttribute : IgnoreForAttribute { }
}
