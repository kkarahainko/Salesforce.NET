namespace SalesforceNET
{
    using System;
    using System.Collections.Generic;

    using Entities;
    using OpenAuthorization;
    using SalesforcePartnerAPI;

    /// <summary>
    /// Salesforce service interface.
    /// </summary>
    public interface ISalesforceService
    {
        #region Properties

        /// <summary>
        /// Binding is logged in.
        /// </summary>
        bool IsLogged { get; }

        /// <summary>
        /// Gets the user info.
        /// </summary>
        UserInfo UserInfo { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// True - if logged in.
        /// </returns>
        bool Login(string username, string password, string token);

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>
        /// True - if logged in.
        /// </returns>
        bool Login(string username, string password, string token, out UserInfo userInfo);

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="userInfoResponse">The user info response.</param>
        /// <returns>
        /// True - if logged in.
        /// </returns>
        bool Login(TokenResponse token, UserInfoResponse userInfoResponse);

        /// <summary>
        /// Logins current binding.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="userInfoResponse">The user info response.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>
        /// True - if logged in.
        /// </returns>
        bool Login(TokenResponse token, UserInfoResponse userInfoResponse, out UserInfo userInfo);

        /// <summary>
        /// Logouts current user.
        /// </summary>
        void Logout();

        /// <summary>
        /// Gets the user info.
        /// </summary>
        /// <returns>User Info.</returns>
        GetUserInfoResult GetUserInfo();

        /// <summary>
        /// Gets fields list of specified object.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <returns>List of fields.</returns>
        List<FieldDescriptor> GetFields(string objectTypeName);

        /// <summary>
        /// Gets object type by object ID.
        /// </summary>
        /// <param name="objectTypeName">Object ID.</param>
        /// <returns>Object type.</returns>
        string GetObjectType(string objectId);

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="fieldNames">Fields for selection.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>List of sObject.</returns>
        List<sObject> GetObjects(string objectTypeName, List<string> fieldNames, string whereClause = null);

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="fieldNames">Fields for selection.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>List of entities.</returns>
        List<T> GetObjects<T>(List<string> fieldNames, string whereClause = null) where T : SalesforceEntity;

        /// <summary>
        /// Gets Salesforce objects.
        /// </summary>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>List of entities.</returns>
        List<T> GetObjects<T>(string whereClause = null) where T : SalesforceEntity;

        /// <summary>
        /// Creates Salesforce object.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        bool CreateObject(string objectTypeName, Dictionary<string, IConvertible> values);

        /// <summary>
        /// Creates Salesforce object.
        /// </summary>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        bool CreateObject<T>(Dictionary<string, IConvertible> values) where T : SalesforceEntity;

        /// <summary>
        /// Creates sObject on Salesforce.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Operation result.</returns>
        bool CreateObject(SalesforceEntity entity);

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="objectTypeName">Object type name.</param>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        bool UpdateObject(string objectTypeName, Dictionary<string, IConvertible> values);

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="values">Object values.</param>
        /// <returns>Operation result.</returns>
        bool UpdateObject<T>(Dictionary<string, IConvertible> values) where T : SalesforceEntity;

        /// <summary>
        /// Updates Salesforce object.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Operation result.</returns>
        bool UpdateObject(SalesforceEntity entity);

        /// <summary>
        /// Deletes Salesforce object by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Operation result.</returns>
        bool DeleteObject(string id);

        /// <summary>
        /// Counts records of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectTypeName">Type of sObject.</param>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>Count of records with where clause if it set otherwise all record.</returns>
        int CountRecords(string objectTypeName, string whereClause = null);

        /// <summary>
        /// Counts records of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereClause">Where clause.</param>
        /// <returns>Count of records with where clause if it set otherwise all record.</returns>
        int CountRecords<T>(string whereClause = null) where T : SalesforceEntity;

        #endregion Methods
    }
}
