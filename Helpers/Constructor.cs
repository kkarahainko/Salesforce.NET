namespace SalesforceNET.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;

    using SalesforceNET.SalesforcePartnerAPI;

    /// <summary>
    /// Constructor class.
    /// </summary>
    internal static class Constructor
    {
        #region Public Methods

        /// <summary>
        /// Constructs sObject instance.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="values">Object values.</param>
        /// <returns>sObject instance.</returns>
        public static sObject ConstructSObject(string objectTypeName, Dictionary<string, IConvertible> values)
        {
            if (String.IsNullOrEmpty(objectTypeName))
            {
                throw (new ArgumentNullException("objectTypeName"));
            }
            else if (values == null)
            {
                throw (new ArgumentNullException("values"));
            }

            var sObj = new sObject();

            if (values.Count != 0)
            {
                var doc = new XmlDocument();

                var fields = new List<XmlElement>();
                var fieldsToNull = new List<string>();

                foreach (KeyValuePair<string, IConvertible> value in values)
                {
                    if (value.Value != null)
                    {
                        XmlElement field = doc.CreateElement(value.Key);

                        field.InnerText = Convert.ToString(value.Value);
                        field.IsEmpty = false;

                        fields.Add(field);
                    }
                    else
                    {
                        fieldsToNull.Add(value.Key);
                    }
                }

                sObj.type = objectTypeName;

                sObj.Any = fields.ToArray();
                sObj.fieldsToNull = fieldsToNull.ToArray();
            }

            return sObj;
        }

        /// <summary>
        /// Recursively constructs entity from sObejct.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="sObject"></param>
        /// <returns>Typed entity.</returns>
        public static dynamic ConstructEntity(Type entityType, XmlElement[] any)
        {
            if (entityType == null)
            {
                throw (new ArgumentNullException("entityType"));
            }
            if (any == null)
            {
                throw (new ArgumentNullException("any"));
            }

            dynamic entity = Activator.CreateInstance(entityType);

            foreach (XmlElement node in any)
            {
                PropertyInfo propertyInfo = entity.GetType().GetProperty(node.LocalName);

                if ((propertyInfo != null) && (!node.IsEmpty && node.NodeType == XmlNodeType.Element))
                {
                    Type propertyType = propertyInfo.PropertyType;

                    if (node.HasAttributes) // complex field with type (ex - sf:sObject) attribute
                    {
                        propertyInfo.SetValue(entity, Constructor.ConstructEntity(propertyType, node.ChildNodes.Cast<XmlElement>().ToArray()), null);
                    }
                    else // simple field with no type (ex - sf:sObject) attribute
                    {
                        Type underlyingType = Nullable.GetUnderlyingType(propertyType);

                        if (underlyingType != null)
                        {
                            propertyInfo.SetValue(entity, Convert.ChangeType(node.InnerText, underlyingType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(entity, Convert.ChangeType(node.InnerText, propertyType), null);
                        }
                    }
                }
            }

            return entity;
        }

        #endregion Public Methods
    }
}
