Task: PROCs on Your Facebook Friends

This task connects to Facebook and gathers your friends data.
It brings the data into a SAS program and runs some simple
reports on it.

Requirements:
- a Facebook account
- optional: SAS Enterprise Guide 4.2 or 4.3 (if running as a task)
- PC with Microsoft Windows and Microsoft .NET Framework 3.5 SP1
  (will already be present if you have SAS Enterprise Guide 4.3)
- SAS 9.2 (to process the SAS program)

There are two ways to use this task:

- from SAS Enterprise Guide, as a custom task that integrates with the
  application

- as a standalone program, that connects to Facebook and saves a 
  SAS program with the results.  You can then run the SAS program
  in SAS Enterprise Guide or in SAS display manager: your choice.

How to install this task:

1. Copy the contents of the ZIP file to one of these locations:

- For use by all users on a machine (requires administrative privileges to install):
  C:\Program Files\SAS\EnterpriseGuide\4.2\Custom\FB  (create the Custom\FB folder if needed)
  or
  C:\Program Files\SAS\EnterpriseGuide\4.3\Custom\FB  (create the Custom\FB folder if needed)

- For use by just the current user:
  %appdata%\SAS\EnterpriseGuide\4.2\Custom\FB (create the Custom\FB folder if needed)
  or
  %appdata%\SAS\EnterpriseGuide\4.3\Custom\FB (create the Custom\FB folder if needed)
  
  Note: The "%appdata%" environment variable is a Windows variable that 
  maps to something like "C:\Users\<yourAccount>\AppData\Roaming" or 
  "C:\Documents and Settings\<yourAccount>\Application Data"
	  
2. Start SAS Enterprise Guide.  The new task appears in the menus
   as Tools->Add-Ins->PROCs On Facebook Friends 
   (or Tools->Add-Ins->SAS Examples->PROCs On Facebook Friends)

To use this task as a standalone program:

1. Copy the contents of the ZIP file to a directory on your machine 
   (for example, "C:\FB").

2. Run "FacebookConnector.exe".

Using the Task/Application
---------------------------------------------------
The following steps describe how to use the task user interface
to gather selected data from Facebook.

  1. Click the "Connect to Facebook" button.  You might be prompted to log in to 
     the Facebook site, and grant certain permissions to the "PROCs on Friends" application.
	 
  2. Examine Limit and Offset values.
     The default "limit" for how many friends to analyze is set to 50.  Gathering information
     for hundreds of friends can take a long time, so try 50 at first.  If you want to try
	 again later and get a different "batch" of friends, set the Offset value to something higher.
	 
	 For example, a limit of 50 and Offset of 50 will retrieve friends "51" through "100".
	 The sequence of friends is internal to Facebook - don't expect a logical ordering.
	 
  3. Click Get Friends to get a list of friends; the list will show in the box on the right.
  
  4. Optional: Click Get Friends Details to get more information such as birthdays, relationship
     status, and gender.  The progress/status area will show when this is complete. 
	 (The list of friends will not change.)
	 
	 Note that not all Facebook users share these fields.  
	 
  5. Optional: Click Get Statuses to get the most recent Facebook status message
     from those friends who have shared one.  This fetch can take several moments.
	 The progress/status area will show when this is complete. 

  6. If running as a task in SAS Enterprise Guide, click "Create Report" to create the
     SAS data and report output within your project.

  7. Select File->Save as SAS Program to save all of the information gathered
     into a SAS program.  You can then run this program in SAS Display Manager or 
	 SAS Enterprise Guide


Allowing this Application to Run on Facebook
---------------------------------------------------
When you run the task, whether within SAS Enterprise Guide, or as a 
standalone program, you must answer prompts to:

- provide your Facebook account user ID and password.  This allows the 
  application to connect to your account via the Facebook APIs.  This
  information is not stored in the application at all -- it's just
  like using an application from the Facebook web site.

- grant permission for this registered application, "PROCs on Friends", to
  have access to your Facebook data.  The application will prompt
  you to grant the specific permissions needed to gather the 
  information you can see about friends' birthdays, education history,
  and so on.

  Again, this permission is *not* stored in the application, 
  and works just like any application that you might use via Facebook.

Notes: 
This task is provided as an example of the data and
reporting capabilities within SAS Enterprise Guide and SAS.  
If you want to provide feedback or
ask questions related to this task, please post your comments to 
The SAS Dummy blog at http://blogs.sas.com/sasdummy.

This task uses the Facebook C# SDK library to provide access to
the Facebook APIs.  The downloads and documentation for this library
is available at http://facebooksdk.codeplex.com.

Copyright (c) 2011 SAS Institute Inc.   
 