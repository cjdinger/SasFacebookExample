using System;
using System.Text;
using SAS.Shared.AddIns;
using SAS.Tasks.Toolkit;
using SAS.FbFriends;

namespace SAS.FbFriends
{
    // unique identifier for this task
    [ClassId("24bc9bad-4d33-4a2f-bec4-63b5f8e577e6")]
    // location of the task icon to show in the menu and process flow
    [IconLocation("SAS.FbFriends.facebook.ico")]
    [InputRequired(InputResourceType.None)]
    public class SasFacebookTask : SAS.Tasks.Toolkit.SasTask
    {

        #region Initialization
        public SasFacebookTask()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 
            // SasFacebookTaskTask
            // 
            this.TaskCategory = "SAS Examples";
            this.TaskDescription = "Run PROCs on your Facebook Friends";
            this.TaskName = "PROCs on Facebook Friends";

        }
        #endregion

        #region overrides

        string sasProgram;

        public override string GetXmlState()
        {
            return sasProgram;
        }

        public override void RestoreStateFromXml(string xmlState)
        {
            sasProgram = xmlState;
        }

        /// <summary>
        /// Show the task user interface
        /// </summary>
        /// <param name="Owner"></param>
        /// <returns>whether to cancel the task, or run now</returns>
        public override ShowResult Show(System.Windows.Forms.IWin32Window Owner)
        {
            FacebookConnectorForm dlg = new FacebookConnectorForm(true);
            if (dlg.ShowDialog(Owner) == System.Windows.Forms.DialogResult.OK)
            {
                sasProgram = dlg.GetSasProgram();
                return ShowResult.RunNow;
            }
            return ShowResult.Canceled;
        }

        /// <summary>
        /// Get the SAS program that this task should generate
        /// based on the options specified.
        /// </summary>
        /// <returns>a valid SAS program to run</returns>
        public override string GetSasCode()
        {
            return sasProgram;
        }

        /// <summary>
        /// Return all data sets into the project
        /// </summary>
        public override int OutputDataCount
        {
            get { return -1; }
        }
        #endregion

    }
}
