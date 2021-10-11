using System.IO;

namespace Paradox_Editor.D_Static_Variables
{
    public static class ProgramProperties
    {
        public static string ProvinceDirectory { get; set; }

        public static string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22");
        //Stored opener will be subject to change for user convienence
    }
}
