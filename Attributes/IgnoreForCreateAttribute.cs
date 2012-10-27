namespace SalesforceNET.Attributes
{
    using System;

    /// <summary>
    /// Ignore property for create operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForCreateAttribute : IgnoreForAttribute { }
}
