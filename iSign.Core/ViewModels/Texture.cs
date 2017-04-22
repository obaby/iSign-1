using iSign.Services;

namespace iSign.Core
{
    public class Texture
    {
        public bool IsColor => Color != null;
        public Color? Color { get; set; }
        public string Path { get; set; }
    }
}
