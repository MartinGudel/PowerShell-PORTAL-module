# PowerShell-PORTAL-module
PowerShell Core msonline replacement module

This símple PowerShell Core module can be used as a replacement for Microsofts msonline module in such cases where using the new az module is not satisfiying and graph CLI commands are not applicable. 

## WARNING 1: 
Be careful when using this module as cmdlets may not have been imlemented in a way you are used to use it. 

## WARNING 2: 
As I'm not an experienced programmer, this code is more a sample for getting familiar with 
* workflows when authenticating to Azure AD
* workflows, how to manage user, group and many other objects in Azure AD if you play around with Office 365

Don't use this module in production if you do not fully understand what you're doing! At the current state, cmdlets may be incomplete, contain bugs and the code provided in this repository is not trustworthy enough to be run in your production. 

## Common notes
Feel free to add comments to  coding style and process implementation as I may learn from your comments as well.

The current version is developed by using netstandard 2.0 .NET Standard versions are separate from PSCore6 NetCoreApp version.  
Even though PSCore6.1 and 6.2 are on NetCoreApp2.1, .NET Core 2.1 implements .NET Standard 2.0. 
see also https://twitter.com/Steve_MSFT/status/1114571560571027457

PORTAL is short for 
PowerShell 
Online
Remote
Tenant
Administration
Light
Anyway, as we started to create empty cmdlets so I will not miss any existing cmdlet from the original msonline Module, original files are still including the "msol" token until the commands have been implemented. 

At this point in time we do not provide any manifest, no tests and no PScriptChecker results.
