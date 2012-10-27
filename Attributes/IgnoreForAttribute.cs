namespace SalesforceNET.Attributes
{
    using System;

    /// <summary>
    /// Ignore property for CRUD operation attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForAttribute : Attribute { }
}
