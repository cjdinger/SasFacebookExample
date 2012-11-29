/* prep a data set for PROC REPORT list of statuses */
proc sql;
   create table work.statusprep as 
   select t1.name, 
          t2.link, 
          t3.date, 
          t3.message, 
          /* namelink */
            ('<a href="' || trim(t2.link) || '">' || trim(t1.name) || '</a>') as namelink
      from work.friends t1, work.frienddetails t2, work.status t3
      where (t1.userid = t2.userid and t1.userid = t3.userid)
      order by t3.date desc;

   create table work.schoolfriends as 
   select t2.name, 
          t1.name as schoolname, 
          t1.type, 
          t1.year
      from work.schools t1 left join work.friends t2 on (t1.userid = t2.userid)
      order by t1.schoolid;
quit;

/* fix this path for your system, if necessary */
ods html file="&outpath.FbReport.html" style=plateau gpath="&outpath.";

ods noproctitle;
title "Total Friends for &myFacebookName";
proc sql;
     select count(distinct(UserId)) as Number_Of_Friends into: NumberOfFriends
		 from friends;
quit;

title "Count of friends by gender";
footnote "Not all Facebook users make Gender public";
ods graphics on / width=400 height=400;
proc freq data=frienddetails
	order=internal;
	tables gender / 
		nocum missprint  
		scores=table plots(only)=freqplot;
run;

title "Count of friends by Relationship Status";
footnote "Not all Facebook users make relationships public";
proc freq data=frienddetails
	order=internal;
	tables relationshipstatus / 
		nocum missprint  
		scores=table plots(only)=freqplot;
run;

title "Recent status messages";
footnote;
proc report data=statusprep nowd;
columns namelink date message;
run;

data birthdays (drop=year);
	set friendDetails (keep=LastName FirstName Gender BirthdayYear BirthdayDay BirthdayMonth);
	length Birthday 8 Age 8;
	format Birthday date5.;
	format Age 6.2;
	if BirthdayYear = . then
		year = 1899;
	else year=BirthdayYear;
	Birthday = MDY(BirthdayMonth, BirthdayDay, year);
	if BirthdayYear = . then
		Age = .;
	else Age = yrdif(Birthday, today(), 'act/act');
run;

proc sort data=birthdays (where=(Birthday <> .))
  out=birthdates;
by BirthdayMonth BirthdayDay;
run;

title "Known birthdays in calendar order";
proc print data=birthdates;
var LastName FirstName Birthday;
run;

title "Analysis of Friend published ages";
footnote "Note: many Facebook users do not share their birth year";

proc means data=birthdays
	min max mean median n nmiss;
	var age;
run;

title1 "Schools attended by friends";
footnote1 "Note: Not all Facebook users share education history";

proc report data=schoolfriends nowd;
	column SchoolName Name Type Year cv1;
	define SchoolName / group 'SchoolName' missing;
	define Name / group 'Name' missing;
	compute Name;
		if Name ne ' ' then hold1=Name;
		if Name eq ' ' then Name=hold1;
	endcomp;
	define Type / group 'Type' missing;
	compute Type;
		if Type ne ' ' then hold2=Type;
		if Type eq ' ' then Type=hold2;
	endcomp;
	define Year / group missing noprint;
	define cv1 / computed 'Year' missing;
	compute cv1;
		if Year ne . then hold3=Year;
		cv1=hold3;
	endcomp;
	run;
quit;

/* format for classifying graduates */
proc format 
	lib=work
;
	value yrsago
		low - 0 = "not yet"
		1 - <5 = "less than 5 years ago"
		5 - <10 = "5 to 10 years ago"
		10 - <15 = "10 to 20 years ago"
		15 - <20 = "15 to 20 years ago"
		20 - <25 = "20 to 25 years ago"
		25 - <30 = "25 to 30 years ago"
		30 - <35 = "30 to 35 years ago"
		35 - high = "more than 35 years ago";
run;

proc sql;
	create table work.hsyears as 
		select t1.year, 
			/* yearsago */
		(year(today())-t1.year) as yearsago, 
		/* age */
		(year(today())-(t1.year-18)) as age from work.schools t1 
		where t1.type = 'High School' and t1.year not is missing;
	create table work.collegeyears as 
		select t1.year, 
			/* yearsago */
		(year(today())-t1.year) as yearsago, 
		/* age */
		(year(today())-(t1.year-22)) as age from work.schools t1 
	where t1.type = 'College' and t1.year not is missing;
quit;

ods graphics /width=500 height=350;
title "When friends graduated";
title2 "high school";

proc sgplot data=hsyears;
	label yearsago="Years ago";
	format yearsago yrsago.;
	hbar yearsago / stat=freq;
run;

title "When friends graduated";
title2 "college";

proc sgplot data=collegeyears;
	label yearsago="Years ago";
	format yearsago yrsago.;
	hbar yearsago / stat=freq;
run;

data ages;
	length how $ 15;
	label how = "How determined?";
	set hsyears(keep=age in=high_school) 
		collegeyears(keep=age in=college)
		birthdays(keep=age in=published where=(age NOT = .));
	if (high_school) then
		how = 'High School';
	if (college) then
		how = 'College';
	if (published) then
		how = 'Published';
run;

title "Calculated Ages based on Graduation years";

proc means data=ages
	min max mean median n;
	var age;
	class how;
run;

title; footnote;
ods html close;
