using System;
using System.Collections.Generic;

namespace iSign.Core
{
    public interface IFileStorageChooser
    {
        IEnumerable<IFileStorage> FileStorages { get; }
    }
}
