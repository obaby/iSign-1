using System.Collections.Generic;
using iSign.Core.Services;

namespace iSign.IoC
{
    public class FileStorageChooser : IFileStorageChooser
    {
        public IEnumerable<IFileStorage> FileStorages => new List<IFileStorage> { new DropBoxFileStorage () };
    }
}
