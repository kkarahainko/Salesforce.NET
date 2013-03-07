namespace SalesforceNET.Constants
{
    using System;

    /// <summary>
    /// Field attributes enum.
    /// </summary>
    [Flags]
    public enum FieldAttribute
    {
        DontCare = 1,
        Updatable = 2,
        Filterable = 4,
        Custom = 8
    }
}
