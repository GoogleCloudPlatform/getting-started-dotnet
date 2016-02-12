## Getting Started for .NET

This repo contains sample applications written in C# that run on Google Cloud Platform.

See our other [Google Cloud Platform github
repos](https://github.com/GoogleCloudPlatform) for sample applications and
scaffolding for other frameworks and use cases.

### Building and Testing

#### Configure your database connection string

Set a database connection string in your environment.  One of the following
must be set:

```
Data:Npgsql:ConnectionString
Data:SqlServer:ConnectionString
```

#### Install PhantomJS and CasperJS

Download zip file from here and unpack:
http://phantomjs.org/download.html

Download latest archive, stable version from here:
http://docs.casperjs.org/en/latest/installation.html

Add new folders to your PATH, run casperjs and hit this issue:
https://github.com/n1k0/casperjs/issues/1150
Copy and paste the 4 lines of code, as described in the issue.

#### Run the test

```.\buildAndRunTests.ps1```

## Contributing changes

* See [CONTRIBUTING.md](CONTRIBUTING.md)

## Licensing

* See [LICENSE](LICENSE)
