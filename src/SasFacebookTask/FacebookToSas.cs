using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAS.FbFriends
{
    public class FacebookToSas
    {
        const string sasOdsPaths =
            "/* fix these paths for your OS */\n" +
            "%macro determineOutpath;\n" +
            "   %global outpath;\n" +
            "	%if &SYSSCP = WIN %then\n" +
            "	  %let outpath=c:\\temp\\;\n" +
            "	%else \n" +
            "	  %let outpath=/tmp/;\n" +
            "%mend;\n" +
            "%determineOutpath;\n" ;

        const string sasFriends =
            "/* FRIENDS */ \n" +
            "data friends; \n" +
            "length \n" +
            "  UserId $ 15 \n" +
            "  Name $ 40 \n" +
            "  ;\n" +
            "infile datalines4 dsd; \n" +
            "input UserId Name; \n" +
            "datalines4;\n"
            ;

        const string sasFriendDetails = 
            "/* FRIEND DETAILS */ \n" +
            "data FriendDetails; \n" +
	        "length  \n" +
	        "   UserId $ 15 \n" +
	        "	 FirstName $ 40 \n" +
	        "	 LastName $ 40 \n" +
	        "	 Gender $ 8 \n" +
	        "	 Link $ 200 \n" +
            "	 RelationshipStatus $ 30 \n" +
	        "	 BirthdayYear 8 \n" +
	        "	 BirthdayMonth 8 \n" +
	        "	 BirthdayDay 8 \n" +
	        "	 ; \n" +
	        "infile datalines4 dsd; \n" +
	        "input  \n" +
	        "	UserId  \n" +
	        "	LastName \n" +
            "	FirstName \n" +
	        "	Gender \n" +
	        "	Link \n" +
            "	RelationshipStatus \n" +
	        "   BirthdayYear \n" +
	        "	BirthdayMonth \n" +
	        "	BirthdayDay; \n" +
	        "datalines4;\n"
        ;

        const string sasStatus = 
            "/* STATUS MESSAGES */ \n" +
	        "data status; \n" +
	        "length  \n" +
	        "  UserId $ 15  \n" +
	        "  Date 8 \n" +
	        "	Message $ 1000; \n" +
	        "  ; \n" +
	        "informat Date anydtdte20.; \n" +
	        "format Date DATE9.; \n" +
	        "infile datalines4 dsd;  \n" +
	        "input UserId Date Message; \n" +
            "datalines4;\n"
            ;

        const string sasSchools =
            "/* SCHOOLS */ \n" +
            "data schools; \n" +
            "length  \n" +
            "  UserId $ 15  \n" +
            "  SchoolId $ 15  \n" +
            "  Name $ 50 \n" +
            "  Type $ 20 \n" +
            "  Year 8 \n" +
            "  ; \n" +
            "infile datalines4 dsd;  \n" +
            "input UserId SchoolId Name Type Year; \n" +
            "datalines4;\n"
            ;

        const string sasFriendMatrix =
            "/* FRIENDMATRIX */ \n" +
            "data friendMatrix; \n" +
            "attrib Friend1 Friend2 length=$20; \n" +
            "attrib FriendName1 FriendName2 length=$60; \n" +
            "infile datalines4 dsd;  \n" +
            "input Friend1 Friend2 FriendName1 FriendName2; \n" +
            "datalines4;\n"
            ;


        static public string GetSasProgram(FbFriend me, Dictionary<string, FbFriend> friends, List<FbSchool> schools, List<FbStatus> statuses, List<string> friendPairs, bool preserveEncoding)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(sasOdsPaths);
            sb.AppendFormat("%let myFacebookName = {0};\n", me.Name);
            sb.Append(sasFriends);
            foreach (string key in friends.Keys)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\"", friends[key].UserID, FixUpDatalinesString(friends[key].Name)));
            }
            sb.AppendLine(";;;;");
            sb.AppendLine("run;");

            sb.AppendLine();
            sb.Append(sasFriendDetails);
            foreach (string key in friends.Keys)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",{6},{7},{8}", 
                    friends[key].UserID, 
                    FixUpDatalinesString(friends[key].LastName),
                    FixUpDatalinesString(friends[key].FirstName),
                    friends[key].Gender,
                    FixUpDatalinesString(friends[key].Link),
                    friends[key].RelationshipStatus,
                    friends[key].BirthdayYear,
                    friends[key].BirthdayMonth,
                    friends[key].BirthdayDay
                    ));
            }

            sb.AppendLine(";;;;");
            sb.AppendLine("run;");


            sb.AppendLine();

            sb.Append(sasStatus);
            foreach (FbStatus s in statuses)
            {
                sb.AppendLine(buildStatusRecord(s));
            }

            sb.AppendLine(";;;;");
            sb.AppendLine("run;");

            sb.AppendLine();

            sb.Append(sasSchools);
            foreach (FbSchool s in schools)
            {
                sb.AppendLine(buildSchoolRecord(s));
            }
            sb.AppendLine(";;;;");
            sb.AppendLine("run;");

            if (friendPairs.Count>0)
            {
                sb.Append(buildFriendMatrix(friendPairs));
            }

            sb.Append(FacebookToSas.ReadFileFromAssembly("SAS.FbFriends.FbReport.sas"));

            if (!preserveEncoding)
            {
                Encoding enc = Encoding.ASCII;
                byte[] encoded = enc.GetBytes(sb.ToString());
                return enc.GetString(encoded);
            }
            else
                return sb.ToString();
        }

        private static string buildFriendMatrix(List<string> pairs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sasFriendMatrix);
            foreach (string pair in pairs)
            {
                sb.AppendLine(pair);
            }
            sb.AppendLine(";;;;");
            sb.AppendLine("run;");
            sb.AppendLine();
            return sb.ToString();
        }

        private static string buildSchoolRecord(FbSchool s)
        {
            string schoolLine = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",{4}", s.UserID, s.SchoolID, s.Name, s.Type, s.Year);
            return schoolLine;
        }

        static private string buildStatusRecord(FbStatus status)
        {
            string statusLine = string.Format("\"{0}\",\"{1}\",\"{2}\"", status.UserID, status.UpdateTime.ToShortDateString(), FixUpDatalinesString(status.Status));
            return statusLine;
        }

        private static string FixUpDatalinesString(string s)
        {
            if (s == null) return string.Empty;
            // escape double quotes
            s = s.Replace("\"", "\"\"");
            // remove carriage returns / line feeds
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            // and make sure we don't terminate the datalines4 early
            s = s.Replace(";;;;", ";");
            // make sure it's not too long
            if (s.Length > 32767)
                s = s.Substring(32767);
            return s;
        }

        #region Read embedded file from assembly
        /// <summary>
        /// Reads an embedded text file from an assembly.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        /// <returns>The contents of the file.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static string ReadFileFromAssembly(string filename)
        {
            string filecontents = String.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(filename);
            if (stream != null)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                filecontents = sr.ReadToEnd();
            }

            return filecontents;
        }
        #endregion
    }
}
