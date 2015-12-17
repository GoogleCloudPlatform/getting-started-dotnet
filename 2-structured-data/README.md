## 2-structured data

This directory contains a sample bookshelf MVC application that can be deployed to Google Cloud Platform Managed VM.

### Build and Run

#### Configure your database connection string

Set a database connection string in your environment.  One of the following
must be set:

```
Data:Npgsql:ConnectionString
Data:SqlServer:ConnectionString
```

Then build and run:

```
dnvm use 1.0.0-rc1-final -r coreclr
dnu restore
dnu build
dnx web
```

See our other [Google Cloud Platform github
repos](https://github.com/GoogleCloudPlatform) for sample applications and
scaffolding for other frameworks and use cases.

## Contributing changes

* See [CONTRIBUTING.md](../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../LICENSE)
