using System;
using System.Collections.Generic;
using iSign.Core;

namespace iSign
{
    public class FileStorageChooser : IFileStorageChooser
    {
        public IEnumerable<IFileStorage> FileStorages => new List<IFileStorage> { new DropBoxFileStorage () };
    }
}
