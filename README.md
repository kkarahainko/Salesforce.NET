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