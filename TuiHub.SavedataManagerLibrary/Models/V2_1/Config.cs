namespace TuiHub.SavedataManagerLibrary.Models.V2_1
{
    public class Config
    {
        public Platform Platform { get; set; }
        public List<object>? Entries { get; set; }
    }
    public enum Platform
    {
        Windows
    }
}
