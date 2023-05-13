<h1 align="center">OnlineSellingStore</h1>


1. [Short Description]("https://github.com/Polclard/OnlineSellingStore/tree/master/#short-description") :ledger:
2. [How to run the application]("#how-to-run-the-application") :running_man:
3. [Resources]("#resources") :bookmark_tabs:
4. [Techonlogies used in this project]("#techologies-used-in-this-project") :computer:


<div id="short-description"  style="text-align: justify">
  
  # Short Description
  
  <p>- This application is about Online Selling Store for books (for now) and the main purpose is to be used for people such as 
    <ul>
      <li>Customer</li>
      <li>Companies</li>
    </ul>
    for buying online books. It is simple to use and provides different functionalities so the user feels comfortable using the application. The companies also can buy products and pay later, as they have period to make the payment before the order is shipped.
  </p>
  <p>- This WEB application also provides tables whih are easy to use and can help the user/admin to easily navigate the products, orders, users, categories etc. This is easily achievable by just simply searching trough the table or setting filters.</p>
</div>

<div id="how-to-run-the-application" style="text-align: justify">
  
  # How to run the application
Use these instructions to get the project up and running.

### Prerequisites
You will need the following tools:

* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
* [.Net Core 7.0 or later](https://dotnet.microsoft.com/download/dotnet-core/2.2](https://dotnet.microsoft.com/en-us/download/dotnet/7.0))
* EF Core 

### Installing
Follow these steps to get your development environment set up:
1. Clone the repository
2. At the root directory, restore required packages by running:
```csharp
dotnet restore
```
3. Next, build the solution by running:
```csharp
dotnet build
```
4. Next, within the AspnetRun.Web directory, launch the back end by running:
```csharp
dotnet run
```
5. Launch http://localhost:5281 in your browser to view the Web UI.

If you have **Visual Studio** after cloning Open solution with your IDE, **OnlineSellingStoreWeb** should be the start-up project. Directly run this project on Visual Studio with **F5 or Ctrl+F5**. You will see index page of project, you can navigate product and category pages and you can perform crud operations on your browser.

### Usage
  
After cloning or downloading the sample you should be able to run it using an In Memory database immediately. The default configuration of Entity Framework Database is **"OnlineSellingStoreDeploy21"**.
  
---------------------------------------------------------------------------------------------------------------------------
-Plus Section
  
If you wish to use the project with a persistent database, you will need to run its Entity Framework Core **migrations** before you will be able to run the app, and update the ConfigureDatabases method in **Startup.cs** (see below).

```csharp
public void ConfigureDatabases(IServiceCollection services)
{
            // add real database dependecy
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
```

1. Ensure your connection strings in ```appsettings.json``` point to a local SQL Server instance.

2. Open a command prompt in the Web folder and execute the following commands:

```csharp
dotnet restore
dotnet ef database update -c ApplicationDbContext
```
Or you can direct call ef commands from Visual Studio **Package Manager Console**. Open Package Manager Console, set default project to OnlineSellingStore.Data and run below command;
```csharp
update-database
```
These commands will create aspnetrun database. You can see from **ApplicationDbContext.cs**.
1. Run the application.
The first time you run the application, it will seed aspnetrun sql server database with a few data.

If you modify-change or add new some of entities to Core project, you should run ef migrate commands in order to update your database as the same way but below commands;
```csharp
add-migration YourCustomEntityChanges
update-database
```
    
  </div>
  
</div>

<div id="resources" style="text-align: justify">
  
  # Resources
  
 1. [ASP .NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)
 2. [ASP .NET Core Assistance](https://www.tutorialsteacher.com/core)
 3. [ASP .NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0)
  
</div>


  # Technologies used in this project

<div id="techologies-used-in-this-project" style="text-align: justify">
  
    1. C#
    2. ASP .NET Core
    3. Visual Studio 2022
    4. HTML
    5. CSS
    6. JavaScript
    7. Bootstrap
    8. Bootswatch
  
</div>

