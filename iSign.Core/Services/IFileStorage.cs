using System;

namespace iSign.Core
{
    public interface IFileStorage
    {
        void DownloadFile (Action<string> afterDownloading);
        string Name { get; }
    }
}
