# SAS custom task example: PROCs on Facebook Friends
***
This repository contains one of a series of examples that accompany
_Custom Tasks for SAS Enterprise Guide using Microsoft .NET_ 
by [Chris Hemedinger](http://support.sas.com/hemedinger).

This example is featured in **Chapter 15: Running PROCs on Your Facebook Friends**.  It
is also described in:
- [this blog post](http://blogs.sas.com/content/sasdummy/2011/04/03/running-procs-on-your-facebook-friends-2011-version/)
- [this SAS Global Forum paper](http://support.sas.com/documentation/onlinedoc/guide/examples/SASGF2011/Hemedinger_Slaughter_315-2011.pdf)
- [and this sasCommunity.org page](http://www.sascommunity.org/wiki/Social_Networking_and_SAS:_Running_PROCs_on_Your_Facebook_Friends)

## About this example
The source code for this example is missing one key item: an application key, which is 
required for a Facebook application to connect to and work with the Facebook APIs.  If you
want to adapt the example for your own use, you must [first register as
a developer on Facebook](developers.facebook.com), and then create your own application name and key to include
in the application.  There is a placeholder for the application key in the **FbClasses.cs**
file.

This task uses the Facebook C# SDK library to provide access to
the Facebook APIs.  The downloads and documentation for this library
are available at [the Facebook C# project page](http://csharpsdk.org/).

