# Testing

## Running tests
1.  **Create a project in the Google Cloud Platform Console**.
    If you haven't already created a project, create one now. Projects enable
    you to manage all Google Cloud Platform resources for your app, including
    deployment, access control, billing, and services.
    1.  Open the [Cloud Platform Console](https://console.cloud.google.com/).
    2.  In the drop-down menu at the top, select Create a project.
    3.  Click Show advanced options. Under App Engine location, select a
        United States location.
    4.  Give your project a name.
    5.  Make a note of the project ID, which might be different from the project
        name. The project ID is used in commands and in configurations.

2.  **Enable billing for your project**.
    If you haven't already enabled billing for your project,
    [enable billing now](https://console.cloud.google.com/project/_/settings).
    Enabling billing allows the application to consume billable resources such
    as running instances and storing data.

3.  **Install the Google Cloud SDK**.
    If you haven't already installed the Google Cloud SDK, [install and
    initialize the Google Cloud SDK](https://cloud.google.com/sdk/docs/) now.
    The SDK contains tools and libraries that enable you to create and manage
    resources on Google Cloud Platform.

4.  **Enable APIs for your project**.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=datastore,pubsub,storage_api,logging,plus&showconfirmation=true)
    to visit Cloud Platform Console and enable the APIs.

5.  Download or clone this repo with

    ```sh
    git clone https://github.com/GoogleCloudPlatform/getting-started-dotnet
    ```

6.  Set the environment variables:
 - GoogleCloudSamples:ProjectId = your project id displayed on the Google Developers Console.
 - GoogleCloudSamples:BucketName = the name of the Google Cloud Storage bucket you created.
 - GoogleCloudSamples:ApplicationName = the name for your application.
 - GoogleCloudSamples:AuthClientId = the service account id of the service account you created.
 - GoogleCloudSamples:AuthClientSecret = the local path to the JSON file containing the service account's private key.
 - GoogleCloudSamples:ConnectionStringCloudSql= the connection string for the Cloud SQL database you created, in the format of "Server=1.2.3.4;Database=bookshelf;Uid=dotnetapp;Pwd=password".
 - GoogleCloudSamples:ConnectionStringSqlServer= the connection string for the SQL Server database you created, in the format of "Data Source=ServerName;Initial Catalog=DatabaseName;Integrated Security=False;User Id=userid;Password=password".

7.  Add the following tools to your path:
 - MSBuild
 - MSTest
 - Nuget

8.  Run the test script:

    ```
    C:\...\getting-started-dotnet\aspnet> powershell ..\BuildAndRunTests.ps1
    ```

