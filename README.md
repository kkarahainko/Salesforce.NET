Salesforce.NET
==============

Salesforce .NET SOAP wrapper.

For first you have to define your Salesforce entity derived from SalesforceEntity, for ex:

```CSharp

public class Task : SalesforceEntity
{
	#region Public Properties
	
	public string Subject { get; set; }

	[IgnoreForCreateUpdate]
	public DateTime? CreatedDate { get; set; }

	public DateTime? ActivityDate { get; set; }

	public string Status { get; set; }
	
	public string Priority { get; set; }

	public string Description { get; set; }

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

1. ```CSharp List<Task> tasks = this._salesforceService.GetObjects<Task>(whereClause); ```
2. ```CSharp List<Task> tasks = this._salesforceService.GetObjects<Task>(new Liist<string>() { "Id", "Subject" }, whereClause); ```

or you can use sObject ("entityless") call:

3. ```CSharp List<sObject> tasks = this._salesforceService.GetObjects("Task", new Liist<string>() { "Id", "Subject" }, whereClause); ```

Creating
--------

1.

```CSharp

var task = new Task()
{
	Subject = "Hello!"
};

this._salesforceService.CreateObject(task);

```

2. ```CSharp this._salesforceService.CreateObject<Task>(new Dictionary<string, IConvertible>() {{ "Subject", "Hello!" }} ); ```

or you can use sObject ("entityless") call:

3. ```CSharp this._salesforceService.CreateObject("Task", new Dictionary<string, IConvertible>() {{ "Subject", "Hello!" }} ); ```

Same with update and delete ...