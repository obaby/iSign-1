using System.Collections.Generic;

namespace iSign.Core.Services
{
    public interface IFileStorageChooser
    {
        IEnumerable<IFileStorage> FileStorages { get; }
    }
}
