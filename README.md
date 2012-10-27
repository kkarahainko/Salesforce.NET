Salesforce.NET
==============

Salesforce .NET SOAP wrapper.

For first you have to define your Salesforce entity derived from SalesforceEntity, for ex:

```CSharp
public class Task : SalesforceEntity
{
	#region Public Properties
	
	public string Subject { get; set; }
	public string Description { get; set; }

	[IgnoreForCreateUpdate]
	public DateTime? CreatedDate { get; set; }

	public DateTime? ActivityDate { get; set; }

	public string Status { get; set; }	
	public string Priority { get; set; }

	public string OwnerId { get; set; }

	[ExtractRecursively]
	[IgnoreForCreateUpdate]
	public User Owner { get; set; }

	public string WhoId { get; set; }

	[ExtractRecursively]
	[IgnoreForCreateUpdate]
	public OwnerRelation Who { get; set; }

	public string WhatId { get; set; }

	[ExtractRecursively]
	[IgnoreForCreateUpdate]
	public OwnerRelation What { get; set; }

	[IgnoreForCreateUpdate]
	public bool? IsDeleted { get; set; }

	#endregion Public Properties
}
```

After that you can get, create, update and delete tasks, for ex:

Getting
-------

```CSharp 
List<Task> tasks = this._salesforceService.GetObjects<Task>(whereClause); 
```
```CSharp 
List<Task> tasks = this._salesforceService.GetObjects<Task>(new List<string>() { "Id", "Subject" }, whereClause); 
```

or you can use sObject ("entityless") call:

```CSharp 
List<sObject> tasks = this._salesforceService.GetObjects("Task", new List<string>() { "Id", "Subject" }, whereClause); 
```

Creating
--------

```CSharp
var task = new Task()
{
	Subject = "Hello!"
};

this._salesforceService.CreateObject(task);
```
```CSharp 
this._salesforceService.CreateObject<Task>(new Dictionary<string, IConvertible>() {{ "Subject", "Hello!" }} ); 
```

or you can use sObject ("entityless") call:

```CSharp 
this._salesforceService.CreateObject("Task", new Dictionary<string, IConvertible>() {{ "Subject", "Hello!" }} ); 
```

Same with update and delete ...

OAuth
-----

Send OAuth request for getting code

```CSharp 
string OAauthUrl = String.Format
	(
		"{0}?response_type=code&client_id={1}&redirect_uri={2}",
		this._configurationHelper.OAuthAuthorizeUrl,
		this._configurationHelper.ConsumerKey,
		this._configurationHelper.OAuthRedirectUrl
	);

return (new RedirectResult(OAauthUrl));
```

After getting code you have to get and fill:

1. TokenResponse (https://login.salesforce.com/services/oauth2/token)
2. UserInfoResponse (https://login.salesforce.com/id/)

After that you can use login function on this maner:

```CSharp
TokenResponse token = this.RequestToken(code);

try
{
	TokenResponse token = this.RequestToken(code);

	if (token != null)
	{
		// Request user info by token

		UserInfoResponse userInfoResponse = this.RequestUserInfo(token);

		if (userInfoResponse != null)
		{
			// Try to login to Salesfeorce

			if (this._salesforceService.Login(token, userInfoResponse))
			{
				this._isAuthorized = true;
			}
		}
	}
}
catch (Exception ex)
{
	// Log exception
}
```