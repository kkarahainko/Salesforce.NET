namespace SalesforceNET.Attributes
{
    using System;

    /// <summary>
    /// Ignore property for get operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForGetAttribute : IgnoreForAttribute { }
}
