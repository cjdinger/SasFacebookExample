using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Facebook;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace SAS.FbFriends
{
    public partial class FacebookConnectorForm : Form
    {
        FbFriend me = new FbFriend();
        Dictionary<string, FbFriend> Friends = new Dictionary<string, FbFriend>();
        List<FbStatus> Statuses = new List<FbStatus>();
        List<FbSchool> Schools = new List<FbSchool>();
        List<string> FriendPairs = new List<string>();

        // for storing data about who's friends with who.
        string[] FriendIds = null;

        public FacebookConnectorForm()
        {
            InitializeComponent();
            tsProgressBar.Visible = false;

            btnGetDetails.Enabled = false;
            btnGetFriends.Enabled = false;
            btnGetStatus.Enabled = false;
            btnAreFriends.Enabled = false;

            lblStep1.Visible = false;
            lblStep2.Visible = false;
            lblStep3.Visible = false;
            lblStep4.Visible = false;
            lblStep5.Visible = false;
        }

        public FacebookConnectorForm(bool showCreateReport) :
            this()
        {
            btnReport.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblStep1.Visible = false;
            FbLoginBrowser dlg = new FbLoginBrowser();
            dlg.ExtendedPermissions.Add("read_stream");
            dlg.ExtendedPermissions.Add("friends_birthday");
            dlg.ExtendedPermissions.Add("friends_status");
            dlg.ExtendedPermissions.Add("friends_education_history");
            dlg.ExtendedPermissions.Add("friends_relationships");

            dlg.AppId = FbApp.ApplicationKey;
            dlg.ShowDialog();

            if (dlg.Success)
            {
                app = new FacebookApp(dlg.AccessToken);
                var result = app.Get("me");
                JObject jobject = JObject.Parse(result.ToString());
                tsStatusMessage.Text = string.Format("Connected as {0}", jobject["name"]);
                me.Name = (string)jobject["name"];
                me.Gender = (string)jobject["gender"];

                btnGetFriends.Enabled = true;
                btnConnect.Enabled = false;
                lblStep1.Visible = true;

            }
            else
                tsStatusMessage.Text = string.Format("FAILED to connect: {0}", dlg.ErrorReason);

        }

        FacebookApp app = null;
        private void btnGetFriends_Click(object sender, EventArgs e)
        {
            lblStep2.Visible = false;
            if (getLimit() == 0)
            {
                MessageBox.Show("Please enter a valid number for the Limit field.", "Limit is invalid");
                return;
            }

            lbFriends.DisplayMember = "Name";
            lbFriends.Items.Clear();
            Dictionary<string, object> parms = new Dictionary<string, object>();
            parms.Add("limit", getLimit());
            if (getOffset() > 0)
            {
                parms.Add("offset", getOffset());
            }

            var result = app.Get("me/friends",parms);

            JObject jobject = JObject.Parse(result.ToString());
            JArray friends = (JArray)jobject["data"];
            foreach (JObject friend in friends)
            {
                System.Diagnostics.Debug.WriteLine(friend.ToString());
                FbFriend fbFriend  = new FbFriend() 
                { 
                    Name = (string)friend["name"],
                    UserID=(string)friend["id"]
                };
                Friends.Add(fbFriend.UserID, fbFriend);

                lbFriends.Items.Add(fbFriend);
            }

            btnGetDetails.Enabled = true;
            btnGetStatus.Enabled = true;
            btnAreFriends.Enabled = true;
            lblStep2.Visible = true;

            tsStatusMessage.Text = string.Format("Collected {0} friends.", Friends.Count);
          
        }

        private void btnGetStatus_Click(object sender, EventArgs e)
        {
            lblStep4.Visible = false;
            tsProgressBar.Visible = true;
            tsProgressBar.Minimum = 0;
            tsProgressBar.Value = 0; 
            tsProgressBar.Maximum = Friends.Count;
            tsStatusMessage.Text = "Collecting recent status messages for each friend...";

            int counter = 0;
            tsProgressBar.Value = counter;

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString());
            foreach (FbFriend f in Friends.Values)
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                // get max of 1 status message for the past two weeks: FASTER.
                parms.Add("limit", 1);
                DateTime since = DateTime.Now.AddDays(-14);
                parms.Add("since", since.ToUnixTime());

                try
                {
                    app.GetAsync(f.UserID + "/statuses", parms, (val) =>
                        {
                            BeginInvoke(new MethodInvoker(
                                delegate()
                                {
                                    if (val != null && val.Result != null)
                                    {
                                        Statuses.AddRange(parseStatus(val.Result.ToString()));
                                    }
                                    if (tsProgressBar.Value++ == tsProgressBar.Maximum - 1)
                                    {
                                        tsProgressBar.Visible = false;
                                        tsStatusMessage.Text = string.Format("Status messages retrieved: {0}", Statuses.Count);
                                        lblStep4.Visible = true;
                                    }
                                }
                            ));

                            System.Diagnostics.Debug.WriteLine(val.Result.ToString());
                        });
                }
                catch
                {
                    tsProgressBar.Value++;
                }
            }
           

        }

        List<FbSchool> parseEducation(JArray schools, string userId)
        {
            List<FbSchool> result = new List<FbSchool>();
            foreach (JObject school in schools)
            {
                if (null != school["school"])
                {
                    FbSchool s = new FbSchool();
                    s.UserID = userId;
                    s.Name = (string)school["school"]["name"];
                    s.SchoolID = (string)school["school"]["id"];
                    s.Type = school["type"] !=null ? (string)school["type"] : "";
                    s.Year = school["year"] != null ? (string)school["year"]["name"] : "";
                    result.Add(s);
                }
            }
            return result;
        }

        List<FbStatus> parseStatus(string jsonStatusResult)
        {
            List<FbStatus> result = new List<FbStatus>();
            JObject jobject = JObject.Parse(jsonStatusResult.ToString());
            JArray statuses = (JArray)jobject["data"];
            foreach (JObject status in statuses)
            {
                string fromId = (string)status["from"]["id"];
                string msg = (string)status["message"];
                DateTime dt = DateTime.Parse((string)status["updated_time"]);
                FbStatus s = new FbStatus() { Status = msg, UpdateTime = dt, UserID=fromId };
                result.Add(s);
            }

            return result;
        }

        private void btnGetDetails_Click(object sender, EventArgs e)
        {
            lblStep3.Visible = false;
            tsProgressBar.Visible = true;
            tsProgressBar.Minimum = 0;
            tsProgressBar.Value = 0;
            tsProgressBar.Maximum = Friends.Count;
            tsStatusMessage.Text = "Collecting details for each friend...";

            foreach (FbFriend f in Friends.Values)
            {
                try
                {
                    app.GetAsync(f.UserID, (val) =>
                    {
                        BeginInvoke(new MethodInvoker(
                           delegate()
                           {
                               if (val != null && val.Result != null)
                               {
                                   JObject jobject = JObject.Parse(val.Result.ToString());
                                   FbFriend fb = Friends[(string)jobject["id"]];
                                   fb.FirstName = (string)jobject["first_name"];
                                   fb.LastName = (string)jobject["last_name"];
                                   fb.Gender = (string)jobject["gender"];
                                   fb.Link = (string)jobject["link"];
                                   fb.RelationshipStatus = (string)jobject["relationship_status"];

                                   // get birthday if available
                                   string bdayString = (string)jobject["birthday"];
                                   if (bdayString != null)
                                   {
                                       string[] bday = bdayString.Split(new char[] { '/' });
                                       if (bday.Length == 3)
                                       {
                                           fb.BirthdayYear = bday[2];
                                       }
                                       if (bday.Length >= 2)
                                       {
                                           fb.BirthdayMonth = bday[0];
                                           fb.BirthdayDay = bday[1];
                                       }
                                   }

                                   JArray schools = (JArray)jobject["education"];
                                   if (schools != null)
                                   {
                                       Schools.AddRange(parseEducation(schools, fb.UserID));
                                   }
                               }

                               if (tsProgressBar.Value++ == tsProgressBar.Maximum - 1)
                               {
                                   tsProgressBar.Visible = false;
                                   tsStatusMessage.Text = string.Format("Details retrieved for {0} friends", Friends.Values.Count);
                                   lblStep3.Visible = true;
                               }
                           }
                       ));

                        System.Diagnostics.Debug.WriteLine(val.Result.ToString());
                    });
                }
                catch
                {
                    tsProgressBar.Value++;
                }
            }
            
        }

        private void OnSaveAsSas(object sender, EventArgs e)
        {
            // Save the content of the friends data as a SAS program,
            // to the file system
            SaveFileDialog dlg = new SaveFileDialog();

            if (me != null)
            {
                dlg.FileName = string.Format("Friends_of_{0}", me.Name.Replace(' ','_'));
            }

            dlg.Filter = "SAS programs (.sas) | *.sas";
            dlg.OverwritePrompt = true;
            dlg.CheckPathExists = true;
            if (DialogResult.OK==dlg.ShowDialog(this))
            {
                try
                {
                    // create and save the file    
                    string program = FacebookToSas.GetSasProgram(me, Friends, Schools, Statuses, FriendPairs, chkUnicode.Checked);
                    System.IO.File.WriteAllText(dlg.FileName, program);                        
                    MessageBox.Show(string.Format("Your SAS program has been saved to:\n\n {0}\n\nYou can open your program in SAS Enterprise Guide or SAS display manager and run PROCs on your Facebook friends!", dlg.FileName));
                    System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(dlg.FileName));
                }
                catch
                {
                    MessageBox.Show(string.Format("Unable to save file to: {0}", dlg.FileName));
                }
            }
        }

        int getLimit()
        {
            try
            {
                int limit = Convert.ToInt16(txtNumber.Text);
                return limit;
            }
            catch
            {
                return 0;
            }
        }

        int getOffset()
        {
            try
            {
                int offset = Convert.ToInt16(txtOffset.Text);
                return offset;
            }
            catch
            {
                return 0;
            }
        }

        private void aboutThisExampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox dlg = new AboutBox();
            dlg.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public string GetSasProgram()
        {
            return FacebookToSas.GetSasProgram(me, Friends, Schools, Statuses, FriendPairs, chkUnicode.Checked);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnAreFriends_Click(object sender, EventArgs e)
        {
            // calculate total calls to Facebook for this
            int totalCalls = 0;

            // ((n) + (n-1) + (n-2) + ... 1) - n = number of combinations
            for (int i = Friends.Keys.Count; i > 0; i--)
                totalCalls += i;
           totalCalls -= Friends.Keys.Count;

            if (MessageBox.Show(
                string.Format("Warning: checking for friend relationships can take a long time! \n(Generates {0} calls to Facebook to check friend combinations).  \n\nDo you want to continue?", totalCalls),
                "Warning: Long operation", MessageBoxButtons.YesNo)
                  == System.Windows.Forms.DialogResult.Yes)
            {
                // init array of friend IDs
                FriendIds = new string[Friends.Keys.Count];
                Friends.Keys.CopyTo(FriendIds, 0);

                for (int i = 0; i < FriendIds.Length; i++)
                    for (int j = i+1; j < FriendIds.Length; j++)
                    {
                        try
                        {
                            tsStatusMessage.Text = string.Format("Checking {0} and {1}", Friends[FriendIds[i]].Name, Friends[FriendIds[j]].Name);
                            Application.DoEvents();

                            Dictionary<string, object> parms = new Dictionary<string, object>();
                            parms.Add("method", "friends.areFriends");
                            parms.Add("uids1", FriendIds[i]);
                            parms.Add("uids2", FriendIds[j]);
                            parms.Add("format", "json");
                            object obj = app.Api(parms);
                            Facebook.JsonArray resp = (Facebook.JsonArray)obj;
                            if ((bool)((Facebook.JsonObject)resp[0])["are_friends"] == true)
                            {
                                string friendId1 = ((Facebook.JsonObject)resp[0])["uid1"].ToString();
                                string friendId2 = ((Facebook.JsonObject)resp[0])["uid2"].ToString();
                                string friendName1 = Friends.ContainsKey(friendId1) ?
                                    Friends[friendId1].Name : "";
                                string friendName2 = Friends.ContainsKey(friendId2) ?
                                    Friends[friendId2].Name : "";

                                FriendPairs.Add(string.Format("{0},{1},{2},{3}",
                                    friendId1, friendId2, friendName1, friendName2));
                                FriendPairs.Add(string.Format("{0},{1},{2},{3}",
                                    friendId2, friendId1, friendName2, friendName1));

                            }
                        }
                        catch (Exception)
                        {

                        }

                    }
                lblStep5.Visible = true;
                tsStatusMessage.Text = "Check for who is friends with who: complete!";
            }
        }

      }
}
