using System;

namespace iSign.Core.Services
{
    public interface IFileStorage
    {
        void DownloadFile (Action<string> afterDownloading);
        string Name { get; }
    }
}
