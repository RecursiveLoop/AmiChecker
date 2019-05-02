# AmiChecker

This checks your AWS environment to see if it is running public AMIs of a specific type.

It currently checks for EC2 instances running Windows 2003, Windows 2008, SQL Server 2005 and SQL Server 2008 AMIs and generates a CSV report. All regions except China and GovCloud are checked and reported on.

## Pre-Requisites

- Install the .NET Core 2.2 Runtime for your operating system (Windows, OSX, Linux) - https://dotnet.microsoft.com/download/dotnet-core/2.2


## Instructions 

- Navigate to the "AmiChecker" folder using your command prompt/shell.

- Type "dotnet run" to run the application.

- You will need to supply your AWS access key every time as this is not saved for security reasons.
