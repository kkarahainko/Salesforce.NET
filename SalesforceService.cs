namespace SalesforceNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SalesforceNET.Attributes;
    using SalesforceNET.Constants;
    using SalesforceNET.Entities;
    using SalesforceNET.Helpers;
    using SalesforceNET.OpenAuthorization;
    using SalesforceNET.SalesforcePartnerAPI;

    /// <summary>
    /// Base Salesforce entity class.
    /// </summary>
    public class SalesforceEntityBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets entity ID.
        /// </summary>
        [IgnoreForCreate]
        public string Id { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Salesforce service implementation.
    /// </summary>
    public class SalesforceService : ISalesforceService
    {
        #region Defines

        private const string ID_FIELD_NAME = "Id";

        #endregion Defines

        #region Private Fields

        /// <summary>
        /// Salesforce service binding.
        /// </summary>
        private SforceService _binding = null;

        /// <summary>
        /// Logged in flag.
        /// </summary>
        private bool _loggedIn = false;

        /// <summary>
        /// Current Salesforce user ID.
        /// </summary>
        private UserInfo _userInfo = null;

        /// <summary>
        /// Described Salesforce globals.
        /// </summary>
        private DescribeGlobalResult _salesforceGlobals = null;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets binding is logged in.
        /// </summary>
        public bool IsLogged
        {
            get { return this._loggedIn; }
        }

        /// <summary>
        /// Gets the user info.
        /// </summary>
        public UserInfo UserInfo
        {
            get { return this._userInfo; }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Checks binding connection state.
        /// </summary>
        /// <param name="throwIfNotConnected">Throw exception flag.</param>
        /// <returns>True if connected otherwise false.</returns>
        private bool CheckConnected(bool throwIfNotConnected = true)
        {
            if (!this._loggedIn)
            {
                if (throwIfNotConnected)
                {
                    throw (new InvalidOperationException(Errors.ERR_BINDING_IS_NOT_LOGGED_IN));
                }
                else { return false; }
            }

            return true;
        }

        #endregion Private Methods

        #region Public Methods

        #region Authorization

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>Login result.</returns>
        public bool Login(string username, string password, string token, out UserInfo userInfo)
        {
            if (!this._loggedIn)
            {
                // Try to get login result

                LoginResult loginResult = this._binding.login(username, String.Concat(password, token));

                // Set connection data

                this._binding.Url = loginResult.serverUrl;
                this._binding.SessionHeaderValue = new SessionHeader()
                {
                    sessionId = loginResult.sessionId
                };

                // Describe Salesforce globals

                this._salesforceGlobals = this._binding.describeGlobal();

                // Fill user info

                this._userInfo = new UserInfo()
                {
                    UserSFID = loginResult.userId,
                    OrganizationSFID = loginResult.userInfo.organizationId,
                    OrganizationName = loginResult.userInfo.organizationName
                };

                userInfo = this._userInfo;

                // Set logged in flag

                this._loggedIn = true;
            }
            else
            {
                userInfo = null;
            }

            return this._loggedIn;
        }

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <returns>Login result.</returns>
        public bool Login(string username, string password, string token)
        {
            if (!this._loggedIn)
            {
                // Try to get login result

                LoginResult loginResult = this._binding.login(username, String.Concat(password, token));

                // Set connection data

                this._binding.Url = loginResult.serverUrl;
                this._binding.SessionHeaderValue = new SessionHeader()
                {
                    sessionId = loginResult.sessionId
                };

                // Describe Salesforce globals

                this._salesforceGlobals = this._binding.describeGlobal();

                // Fill user info

                this._userInfo = new UserInfo()
                {
                    UserSFID = loginResult.userId,
                    OrganizationSFID = loginResult.userInfo.organizationId,
                    OrganizationName = loginResult.userInfo.organizationName
                };

                // Set logged in flag

                this._loggedIn = true;
            }

            return this._loggedIn;
        }

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="userInfoResponse">The user info response.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>Login result.</returns>
        public bool Login(TokenResponse token, UserInfoResponse userInfoResponse, out UserInfo userInfo)
        {
            if (!this._loggedIn)
            {
                // Set connection data

                if (userInfoResponse.urls.ContainsKey("partner"))
                {
                    this._binding.Url = userInfoResponse.urls["partner"].Replace("{version}", "26.0");
                    this._binding.SessionHeaderValue = new SessionHeader()
                    {
                        sessionId = token.access_token
                    };
                }
                else
                {
                    throw (new InvalidOperationException());
                }

                // Describe Salesforce globals

                this._salesforceGlobals = this._binding.describeGlobal();

                // Get user info from Salesforce

                var info = this.GetUserInfo();

                this._userInfo = new UserInfo()
                {
                    UserSFID = userInfoResponse.user_id,
                    OrganizationSFID = userInfoResponse.organization_id,
                    OrganizationName = info.organizationName
                };

                userInfo = this._userInfo;

                // Set logged in flag

                this._loggedIn = true;
            }
            else
            {
                userInfo = null;
            }

            return this._loggedIn;
        }

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="userInfoResponse">The user info response.</param>
        /// <returns>Login result.</returns>
        public bool Login(TokenResponse token, UserInfoResponse userInfoResponse)
        {
            if (!this._loggedIn)
            {
                // Set connection data

                if (userInfoResponse.urls.ContainsKey("partner"))
                {
                    this._binding.Url = userInfoResponse.urls["partner"].Replace("{version}", "25.0");
                    this._binding.SessionHeaderValue = new SessionHeader()
                    {
                        sessionId = token.access_token
                    };
                }
                else
                {
                    throw (new InvalidOperationException());
                }

                // Describe Salesforce globals

                this._salesforceGlobals = this._binding.describeGlobal();

                // Get user info from Salesforce

                var info = this.GetUserInfo();

                this._userInfo = new UserInfo()
                {
                    UserSFID = userInfoResponse.user_id,
                    OrganizationSFID = userInfoResponse.organization_id,
                    OrganizationName = info.organizationName
                };

                // Set logged in flag

                this._loggedIn = true;
            }

            return this._loggedIn;
        }

        /// <summary>
        /// Logouts current user.
        /// </summary>
        public void Logout()
        {
            this._loggedIn = false;
        }

        #endregion Authorization

        #region Support Methods

        /// <summary>
        /// Gets the user info.
        /// </summary>
        /// <returns>User Info.</returns>
        public GetUserInfoResult GetUserInfo()
        {
            GetUserInfoResult result = null;

            if (this.CheckConnected())
            {
                result = this._binding.getUserInfo();
            }

            return result;
        }

        /// <summary>
        /// Gets fields list of specified object.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="attribute">Field attribute flags.</param>
        /// <returns>List of fields.</returns>
        public List<FieldDescriptor> GetFields(string objectTypeName, FieldAttribute attribute = FieldAttribute.DontCare)
        {
            var result = new List<FieldDescriptor>();

            if (this.CheckConnected())
            {
                if (String.IsNullOrEmpty(objectTypeName))
                {
                    throw (new ArgumentNullException("objectTypeName"));
                }

                // Get fields of specified object type

                DescribeSObjectResult describeResult = this._binding.describeSObject(objectTypeName);

                // Get field descriptors

                if (describeResult != null)
                {
                    IEnumerable<Field> filteredFields = describeResult.fields;

                    if (attribute != FieldAttribute.DontCare)
                    {
                        if (attribute.HasFlag(FieldAttribute.Custom))
                        {
                            filteredFields = filteredFields.Where(f => f.custom);
                        }
                        if (attribute.HasFlag(FieldAttribute.Updatable))
                        {
                            filteredFields = filteredFields.Where(f => f.updateable);
                        }
                        if (attribute.HasFlag(FieldAttribute.Filterable))
                        {
                            filteredFields = filteredFields.Where(f => f.filterable);
                        }
                    }

                    result = filteredFields.Select
                        (
                            q => new FieldDescriptor() { Name = q.name, Type = q.type }
                        )
                        .ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets object type by object ID.
        /// </summary>
        /// <param name="objectTypeName">Object ID.</param>
        /// <returns>Object type.</returns>
        public string GetObjectType(string objectId)
        {
            if (String.IsNullOrEmpty(objectId))
            {
                throw (new ArgumentNullException("objectId"));
            }

            // Get key prefix

            var keyPrefix = objectId.Substring(0, 3);

            // Get object descriptor by prefix

            var objectDescriptor = this._salesforceGlobals.sobjects
                .Where(x => (x.keyPrefix == keyPrefix))
                .FirstOrDefault();

            // Return object type name

            return (objectDescriptor != null)
                ? objectDescriptor.name
                : null;
        }

        /// <summary>
        /// Counts records of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectTypeName">Type of sObject.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>Count of records with where clause if it set otherwise all record.</returns>
        public int CountRecords(string objectTypeName, string whereClause = null)
        {
            if (this.CheckConnected())
            {
                string queryString = String.Format("select COUNT() from {0}", objectTypeName);

                // If where clause is not null or empty when add it to query string

                if (!String.IsNullOrEmpty(whereClause))
                {
                    queryString = String.Format("{0} {1}", queryString, whereClause);
                }

                // Try to request data

                QueryResult qr = this._binding.query(queryString);

                return qr.size;
            }

            return 0;
        }

        /// <summary>
        /// Counts records of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>Count of records with where clause if it set otherwise all record.</returns>
        public int CountRecords<T>(string whereClause = null) where T : SalesforceEntityBase
        {
            if (this.CheckConnected())
            {
                // Get object type name

                string objectTypeName = typeof(T).Name;

                // Build query string

                string queryString = String.Format("select COUNT() from {0}", objectTypeName);

                // If where clause is not null or empty 
                // when add it to query string

                if (!String.IsNullOrEmpty(whereClause))
                {
                    queryString = String.Format("{0} where {1}", queryString, whereClause);
                }

                // Try to request data

                QueryResult qr = this._binding.query(queryString);

                return qr.size;
            }

            return 0;
        }

        #endregion Support Methods

        #region CRUD: Get Objects

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="queryString">Query string.</param>
        /// <returns>List of Salesforce objects.</returns>
        public List<sObject> GetObjects(string queryString)
        {
            var result = new List<sObject>();

            if (this.CheckConnected())
            {
                QueryResult qr = this._binding.queryAll(queryString);

                while (qr.size > 0)
                {
                    result.AddRange(qr.records.ToList()); qr.size = 0;

                    if (!qr.done)
                    {
                        qr = this._binding.queryMore(qr.queryLocator);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="fieldNames">Fields for selection.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <param name="orderByClause">Order by clause.</param>
        /// <returns>List of sObject.</returns>
        public List<sObject> GetObjects(string objectTypeName, List<string> fieldNames, string whereClause = null, string orderByClause = null)
        {
            var result = new List<sObject>();

            if (this.CheckConnected())
            {
                if (fieldNames == null)
                {
                    throw (new ArgumentNullException("fieldNames"));
                }
                if (String.IsNullOrEmpty(objectTypeName))
                {
                    throw (new ArgumentNullException("objectTypeName"));
                }

                // Field names should not be empty

                if (fieldNames.Count > 0)
                {
                    // Add ID field to query if it not exists

                    if (!fieldNames.Exists(q => q.Equals(ID_FIELD_NAME, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fieldNames.Insert(0, ID_FIELD_NAME);
                    }

                    // Get query string

                    string queryString = String.Format("SELECT {0} FROM {1}", String.Join(", ", fieldNames), objectTypeName);

                    // Append 'where' clause && 'order by' clause

                    if (!String.IsNullOrEmpty(whereClause))
                    {
                        queryString = String.Format("{0} WHERE {1}", queryString, whereClause);
                    }
                    if (!String.IsNullOrEmpty(orderByClause))
                    {
                        queryString = String.Format("{0} ORDER BY {1}", queryString, orderByClause);
                    }

                    // Try to get query result

                    result = this.GetObjects(queryString);
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="queryString">Query string.</param>
        /// <returns>List of Salesforce objects.</returns>
        public List<T> GetObjects<T>(string queryString)
        {
            var result = new List<T>();

            if (this.CheckConnected())
            {
                QueryResult qr = this._binding.queryAll(queryString);

                while (qr.size > 0)
                {
                    foreach (sObject sObject in qr.records.ToList())
                    {
                        result.Add(Constructor.ConstructEntity(typeof(T), sObject.Any));
                    }

                    qr.size = 0;

                    if (!qr.done)
                    {
                        qr = this._binding.queryMore(qr.queryLocator);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="fieldNames">Fields for selection.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <param name="orderByClause">Order by clause.</param>
        /// <returns>List of entities.</returns>
        public List<T> GetObjects<T>(List<string> fieldNames, string whereClause = null, string orderByClause = null) where T : SalesforceEntityBase
        {
            var result = new List<T>();

            if (this.CheckConnected())
            {
                if (fieldNames == null)
                {
                    throw (new ArgumentNullException("fieldNames"));
                }

                // Field names should not be empty

                if (fieldNames.Count > 0)
                {
                    // Add ID field to query if it not exists

                    if (!fieldNames.Exists(q => q.Equals(ID_FIELD_NAME, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fieldNames.Insert(0, ID_FIELD_NAME);
                    }

                    // Build query string

                    string queryString = String.Format("select {0} from {1}", String.Join(", ", fieldNames), typeof(T).Name);

                    // Append 'where' clause && 'order by' clause

                    if (!String.IsNullOrEmpty(whereClause))
                    {
                        queryString = String.Format("{0} WHERE {1}", queryString, whereClause);
                    }
                    if (!String.IsNullOrEmpty(orderByClause))
                    {
                        queryString = String.Format("{0} ORDER BY {1}", queryString, orderByClause);
                    }

                    // Try to get query result

                    result = this.GetObjects<T>(queryString);
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>List of entities.</returns>
        public List<T> GetObjects<T>(string whereClause = null, string orderByClause = null) where T : SalesforceEntityBase
        {
            var result = new List<T>();

            if (this.CheckConnected())
            {
                result = this.GetObjects<T>(Extractor.ExtractFieldNames(typeof(T)), whereClause, orderByClause);
            }

            return result;
        }

        #endregion CRUD: Get Objects

        #region CRUD: Create Object

        /// <summary>
        /// Creates Salesforce.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        public bool CreateObject(string objectTypeName, Dictionary<string, IConvertible> values)
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (String.IsNullOrEmpty(objectTypeName))
                {
                    throw (new ArgumentNullException("objectTypeName"));
                }
                if (values == null)
                {
                    throw (new ArgumentNullException("values"));
                }

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (!values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.create
                            (
                                new sObject[] { Constructor.ConstructSObject(objectTypeName, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_AUTOGENERATED));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates Salesforce object.
        /// </summary>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        public bool CreateObject<T>(Dictionary<string, IConvertible> values) where T : SalesforceEntityBase
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (values == null)
                {
                    throw (new ArgumentNullException("values"));
                }

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (!values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.create
                            (
                                new sObject[] { Constructor.ConstructSObject(typeof(T).Name, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_AUTOGENERATED));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates Salesforce object.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Operation result.</returns>
        public bool CreateObject(SalesforceEntityBase entity)
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (entity == null)
                {
                    throw (new ArgumentNullException("entity"));
                }

                // Extract current entity values

                Dictionary<string, IConvertible> values = Extractor.ExtractFieldValues
                    (
                        entity, new ExtractionOptions() { ExcludeCreateIgnored = true }
                    );

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (!values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.create
                            (
                                new sObject[] { Constructor.ConstructSObject(entity.GetType().Name, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_AUTOGENERATED));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        #endregion CRUD: Create Object

        #region CRUD: Update Object

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        public bool UpdateObject(string objectTypeName, Dictionary<string, IConvertible> values)
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (String.IsNullOrEmpty(objectTypeName))
                {
                    throw (new ArgumentNullException("objectTypeName"));
                }
                if (values == null)
                {
                    throw (new ArgumentNullException("values"));
                }

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.update
                            (
                                new sObject[] { Constructor.ConstructSObject(objectTypeName, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_NOT_SET));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        public bool UpdateObject<T>(Dictionary<string, IConvertible> values) where T : SalesforceEntityBase
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (values == null)
                {
                    throw (new ArgumentNullException("values"));
                }

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.update
                            (
                                new sObject[] { Constructor.ConstructSObject(typeof(T).Name, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_NOT_SET));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Operation result.</returns>
        public bool UpdateObject(SalesforceEntityBase entity)
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (entity == null)
                {
                    throw (new ArgumentNullException("entity"));
                }

                // Extract current values of entity

                Dictionary<string, IConvertible> values = Extractor.ExtractFieldValues
                    (
                        entity, new ExtractionOptions() { ExcludeUpdateIgnored = true }
                    );

                // Values should not be empty

                if (values.Count > 0)
                {
                    // Construct Salesforce object

                    if (values.ContainsKey(ID_FIELD_NAME))
                    {
                        SaveResult[] saveResults = this._binding.update
                            (
                                new sObject[] { Constructor.ConstructSObject(entity.GetType().Name, values) }
                            );

                        result = ((saveResults.Length > 0) && (saveResults[0].success));
                    }
                    else
                    {
                        throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_NOT_SET));
                    }
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_FIELDS_ARE_EMPTY));
                }
            }

            return result;
        }

        #endregion CRUD: Update Object

        #region CRUD: Delete Object

        /// <summary>
        /// Deletes Salesforce object by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if deleted.</returns>
        public bool DeleteObject(string id)
        {
            bool result = false;

            if (this.CheckConnected())
            {
                if (!String.IsNullOrEmpty(id))
                {
                    // Try to delete record

                    DeleteResult[] deleteResults = this._binding.delete(new string[] { id });

                    // Get result

                    result = ((deleteResults.Length > 0) && (deleteResults[0].success));
                }
                else
                {
                    throw (new InvalidOperationException(Errors.ERR_ID_FIELD_IS_NOT_SET));
                }
            }

            return result;
        }

        #endregion CRUD: Delete Object

        #endregion Public Methods

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SalesforceService()
        {
            this._binding = new SforceService { Timeout = 60000 };
        }

        #endregion Constructors
    }
}