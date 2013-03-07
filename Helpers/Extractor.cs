namespace SalesforceNET.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SalesforceNET.Attributes;

    /// <summary>
    /// Field extraction options class.
    /// </summary>
    internal class ExtractionOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets ExcludeGetIgnored flag.
        /// </summary>
        public bool ExcludeGetIgnored { get; set; }

        /// <summary>
        /// Gets or sets ExcludeCreateIgnored flag.
        /// </summary>
        public bool ExcludeCreateIgnored { get; set; }

        /// <summary>
        /// Gets or sets ExcludeUpdateIgnored flag.
        /// </summary>
        public bool ExcludeUpdateIgnored { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Extractor class.
    /// </summary>
    internal static class Extractor
    {
        #region Private Methods

        /// <summary>
        /// Gets ignore attrbutes of proprty.
        /// </summary>
        /// <param name="property">Property info.</param>
        /// <returns>List of ignore attributes.</returns>
        private static List<IgnoreForAttribute> GetIgnoreAttributes(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(IgnoreForAttribute), true).Cast<IgnoreForAttribute>().ToList();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Recursively extracts properties names.
        /// </summary>
        /// <param name="type">Type to extract.</param>
        /// <param name="typePrefix">String prefix of type.</param>
        /// <param name="options">Extraction options.</param>
        /// <returns>List of filed names.</returns>
        public static List<string> ExtractFieldNames(Type type, string typePrefix = null, ExtractionOptions options = null)
        {
            var result = new List<string>();

            // Get public properties

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Get properties values

            foreach (PropertyInfo property in properties)
            {
                if ((options != null) && (options.ExcludeGetIgnored))
                {
                    var ignoreAttributes = Extractor.GetIgnoreAttributes(property);

                    if (ignoreAttributes.Any(x => x.GetType() == typeof(IgnoreForGetAttribute)))
                    {
                        continue; // skip cycle iteration for ignored field
                    }
                }

                if (property.GetCustomAttributes(typeof(ExtractRecursivelyAttribute), true).Length > 0)
                {
                    result.AddRange(Extractor.ExtractFieldNames(property.PropertyType, property.Name, options));
                }
                else
                {
                    // Resolve result field name

                    if (!String.IsNullOrEmpty(typePrefix))
                    {
                        result.Add(String.Format("{0}.{1}", typePrefix, property.Name));
                    }
                    else
                    {
                        result.Add(property.Name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts entity's properties values.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        /// <param name="options">Extract options.</param>
        /// <returns>Dictionary of property name - property value.</returns>
        public static Dictionary<string, IConvertible> ExtractFieldValues(SalesforceEntityBase entity, ExtractionOptions options = null)
        {
            var result = new Dictionary<string, IConvertible>();

            // Get public properties

            PropertyInfo[] properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Get properties values

            foreach (PropertyInfo property in properties)
            {
                // Check for ignored properties

                bool ignoreProperty = false;

                if (options != null)
                {
                    var ignoredAttributes = Extractor.GetIgnoreAttributes(property);

                    if (options.ExcludeGetIgnored)
                    {
                        ignoreProperty = (ignoredAttributes.Any(x => x.GetType() == typeof(IgnoreForGetAttribute)));
                    }
                    else if ((options.ExcludeCreateIgnored) || (options.ExcludeUpdateIgnored))
                    {
                        if (ignoredAttributes.Any(x => x.GetType() == typeof(IgnoreForCreateUpdateAttribute)))
                        {
                            ignoreProperty = true;
                        }
                        else if (options.ExcludeCreateIgnored)
                        {
                            ignoreProperty = (ignoredAttributes.Any(x => x.GetType() == typeof(IgnoreForCreateAttribute)));
                        }
                        else if (options.ExcludeUpdateIgnored)
                        {
                            ignoreProperty = (ignoredAttributes.Any(x => x.GetType() == typeof(IgnoreForUpdateAttribute)));
                        }
                    }
                }

                // Ignore property if needed

                if (!ignoreProperty)
                {
                    // Get property value

                    object propertyValue = property.GetValue(entity, null);

                    if (propertyValue != null) // if value is not null
                    {
                        string resultValue = propertyValue.ToString();

                        Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                        if (underlyingType != null) // if has nullable type
                        {
                            if (underlyingType == typeof(DateTime))
                            {
                                resultValue = (propertyValue as DateTime?).Value.ToString("s");
                            }
                        }
                        else // if not nullable type
                        {
                            if (property.PropertyType == typeof(DateTime))
                            {
                                resultValue = (propertyValue as DateTime?).Value.ToString("s");
                            }
                        }

                        // Add property name - value to result

                        result.Add(property.Name, resultValue);
                    }
                    else
                    {
                        // Add null value to result

                        result.Add(property.Name, null);
                    }
                }
            }

            return result;
        }

        #endregion Public Methods
    }
}
