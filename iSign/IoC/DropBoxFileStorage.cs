using System;
using System.IO;
using System.Net;
using Dropins.Chooser.iOS;
using iSign.Core.Services;
using UIKit;

namespace iSign.IoC
{
    public class DropBoxFileStorage : IFileStorage
    {
        public string Name => "Dropbox"; 

        public void DownloadFile (Action<string> afterDownloading)
        {
            var topVc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            DBChooser.DefaultChooser.OpenChooser (DBChooserLinkType.Direct, topVc, results => {
                if (results != null) {
                    string dropboxUrl = results [0].Link.ToString ();
                    var client = new WebClient ();
                    var filename = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), results [0].Name);
                    client.DownloadFile (dropboxUrl, filename);
                    afterDownloading(filename);
                }
            });
        }
    }
}
