
namespace EPSCoR.Web.Database.Services.FileConverter
{
    /// <summary>
    /// Define method for every fileconverter to implement.
    /// </summary>
    public interface IFileConverter
    {
        /// <summary>
        /// Converts the file to csv format and returns the full path the converted file.
        /// </summary>
        /// <returns></returns>
        string ConvertToCSV();
    }
}
