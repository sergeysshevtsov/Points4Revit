using System.IO;

namespace Points4Revit.Core
{
    public class Common
    {
        public static readonly string pathToTmpFile = Path.Combine(Path.GetTempPath(), "p4r");
    }
}
