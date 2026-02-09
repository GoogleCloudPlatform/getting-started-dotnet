# Testing

## Running tests
1.  **Create a project in the Google Cloud Console**.
    If you haven't already created a project, create one now. Projects enable
    you to manage all Google Cloud resources for your app, including
    deployment, access control, billing, and services.
    1.  Open the [Cloud Console](https://console.cloud.google.com/).
    2.  In the drop-down menu at the top, select Create a project.
    3.  Give your project a name.
    4.  Make a note of the project ID, which might be different from the project
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
    resources on Google Cloud.

4.  **Enable APIs for your project**.
    Enable the following APIs in the Cloud Console:
    *   Cloud Firestore API
    *   Cloud Pub/Sub API
    *   Cloud Storage API
    *   Cloud Translation API
    *   Cloud SQL Admin API
    *   Cloud Key Management Service (KMS) API

5.  Download or clone this repo with

    ```sh
    git clone https://github.com/GoogleCloudPlatform/getting-started-dotnet
    ```

6.  Set the environment variables:
  * `GoogleCloudSamples:ProjectId` = your project id displayed on the Google Cloud Console.
  * `GoogleCloudSamples:BucketName` = the name of the Google Cloud Storage bucket you created.
  * `GoogleCloudSamples:ConnectionStringCloudSql` = the connection string for the Cloud SQL database you created, in the format of "Server=1.2.3.4;Database=bookshelf;Uid=dotnetapp;Pwd=password".

7.  Ensure you have the following tools installed and in your PATH:
  * [.NET Core SDK](https://dotnet.microsoft.com/download)
  * PowerShell

8.  Run the test script from the root of the repository:

    ```powershell
    .\BuildAndRunTests.ps1
    ```
