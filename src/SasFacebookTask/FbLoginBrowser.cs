using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAS.FbFriends
{
    public partial class FbLoginBrowser : Form
    {
        string uri = "https://graph.facebook.com/oauth/authorize?" +
                "client_id={0}&" +
                "redirect_uri=http://www.facebook.com/connect/login_success.html&" +
                "type=user_agent&" +
                "display=popup&" +
                "{1}" ;

        public List<string> ExtendedPermissions {  get; set; }
        public string AppId { get; set; }
        public string AccessToken { get; private set;  }
        public string ErrorReason { get; private set; }
        public bool Success { get; private set; }

        public FbLoginBrowser()
        {
            Success = false;
            ExtendedPermissions = new List<string>();
            InitializeComponent();
            wbFacebookSite.Navigated += new WebBrowserNavigatedEventHandler(wbFacebookSite_Navigated);
        }

        void wbFacebookSite_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsolutePath.EndsWith("/connect/login_success.html"))
            {
                string[] parts = e.Url.OriginalString.Split(new char[] { '&' });
                foreach (string part in parts)
                {
                    if (part.Contains("access_token="))
                    {
                        string[] vals = part.Split(new char[] { '=' });
                        if (vals.Length > 1) AccessToken = vals[1];
                        Success = true;
                        break;
                    }
                    if (part.Contains("error_reason="))
                    {
                        string[] vals = part.Split(new char[] { '=' });
                        if (vals.Length > 1) ErrorReason = vals[1];
                        Success = false;
                        break;
                    }
                }
                this.Close();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            string scope = "";
            if (ExtendedPermissions.Count > 0)
            {
                StringBuilder scopeBuilder = new StringBuilder();
                scopeBuilder.Append("scope=");
                foreach (string perm in ExtendedPermissions)
                {
                    scopeBuilder.AppendFormat("{0},", perm);
                }
                scope = scopeBuilder.ToString().TrimEnd(new char[] { ',' });
            }

            wbFacebookSite.Navigate(string.Format(uri, AppId, scope));
            base.OnLoad(e);
        }
    }

    #region Possible known permissions
    public static class ExtendedPermissions
    {
        public static string[] Permissions = {
            // Publishing Permissions
            "publish_stream",
            "create_event",
            "rsvp_event",
            "sms",
            "offline_access",
            // User Permissions
            "user_about_me",
            "user_activities",
            "user_birthday",
            "user_education_history",
            "user_events",
            "user_groups",
            "user_hometown",
            "user_interests",
            "user_likes",
            "user_location",
            "user_notes",
            "user_online_presence",
            "user_photo_video_tags",
            "user_photos",
            "user_relationships",
            "user_relationship_details",
            "user_religion_politics",
            "user_status",
            "user_videos",
            "user_website",
            "user_work_history",
            "email",
            "read_friendlists",
            "read_insights",
            "read_mailbox",
            "read_requests",
            "read_stream",
            "xmpp_login",
            "ads_management",
            "user_checkins",
            // Friends Permissions
            "friends_about_me",
            "friends_activities",
            "friends_birthday",
            "friends_education_history",
            "friends_events",
            "friends_groups",
            "friends_hometown",
            "friends_interests",
            "friends_likes",
            "friends_location",
            "friends_notes",
            "friends_online_presence",
            "friends_photo_video_tags",
            "friends_photos",
            "friends_relationships",
            "friends_relationship_details",
            "friends_religion_politics",
            "friends_status",
            "friends_videos",
            "friends_website",
            "friends_work_history",
            "friends_checkins",
            // Page Permissions
            "manage_pages",
        };
    }
    #endregion

}
