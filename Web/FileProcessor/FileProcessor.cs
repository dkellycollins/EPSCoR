using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPSCoR.Common;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;
using EPSCoR.Web.Database.Services;
using EPSCoR.Web.Database.Services.Log;
using EPSCoR.Web.FileProcessor.FileConverter;

namespace EPSCoR.Web.FileProcessor
{
    /// <summary>
    /// The file processor coverts any uploaded files into csv file and adds the file to the database.
    /// </summary>
    public static class FileProcessor
    {
        private static DbContextFactory _contextFactory = new DbContextFactory();

        /// <summary>
        /// Starts a task that converts the file to csv and add the result to database.
        /// Note that the conversion is stored in DirectoryManager.ConversionDir so DirectoryManager.SetRootDirectory must be called be fore calling this method.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Task ProcessFileAsync(string filePath)
        {
            string fileKey = FileKeyGenerator.GenerateKey(filePath);
            TableIndex tableIndex = null;
            using (ModelDbContext context = _contextFactory.GetModelDbContext())
            {
                tableIndex = context.GetAllModels<TableIndex>().Where((i) => i.FileKey == fileKey).FirstOrDefault();
            }

            if (tableIndex == null)
            {
                LoggerFactory.GetLogger().Log("Could not find table index for " + filePath);
            }

            Task task = Task.Factory.StartNew(() => convertFile(filePath, tableIndex), TaskCreationOptions.LongRunning);
            task.ContinueWith((t) => cleanUp(t, tableIndex, filePath));

            return task;
        }

        private static void convertFile(string filePath, TableIndex tableIndex)
        {
            LoggerFactory.GetLogger().Log("Begining to process file: " + filePath);

            //Convert the file.
            updateTableStatus(tableIndex, "Converting uploaded file.");
            string conversionPath = FileConverterFactory.GetConverter(filePath, tableIndex.UploadedByUser).ConvertToCSV();

            //Add converted file to the database.
            tableIndex.FileKey = FileKeyGenerator.GenerateKey(conversionPath);
            updateTableStatus(tableIndex, "Creating table in database.");
            using (TableDbContext tableContext = _contextFactory.GetTableDbContextForUser(tableIndex.UploadedByUser))
            {
                tableContext.AddTableFromFile(conversionPath);
                tableContext.PopulateTableFromFile(conversionPath);
            }
        }

        private static void cleanUp(Task task, TableIndex tableIndex, string filePath)
        {
            if (task.IsFaulted)
            {
                handleError(tableIndex, filePath, task.Exception);
            }
            else if (task.IsCanceled)
            {
                handleCancel(tableIndex, filePath);
            }
            else
            {
                handleSuccess(tableIndex, filePath);
            }
        }

        private static void handleError(TableIndex tableIndex, string filePath, Exception taskException)
        {
            updateTableStatus(tableIndex, "An error occured while processing the file.", false, true);

            LoggerFactory.GetLogger().Log("Exception while processing file: " + filePath, taskException);

            string invlaidPath = Path.Combine(DirectoryManager.InvalidDir, tableIndex.UploadedByUser, Path.GetFileName(filePath));
            File.Move(filePath, invlaidPath);
        }

        private static void handleCancel(TableIndex tableIndex, string filePath)
        {
            updateTableStatus(tableIndex, "Processing canceled by user.", false, false);

            LoggerFactory.GetLogger().Log("Task canceled while processing: " + filePath);

            string invlaidPath = Path.Combine(DirectoryManager.InvalidDir, tableIndex.UploadedByUser, Path.GetFileName(filePath));
            File.Move(filePath, invlaidPath);
        }

        private static void handleSuccess(TableIndex tableIndex, string filePath)
        {
            updateTableStatus(tableIndex, "Table created.", true, false);

            LoggerFactory.GetLogger().Log("File processed: " + filePath);

            string archivePath = Path.Combine(DirectoryManager.ArchiveDir, tableIndex.UploadedByUser, Path.GetFileName(filePath));
            File.Move(filePath, archivePath);
        }

        /// <summary>
        /// Ensures that File.Move and File.Copy will not throw an exception becuase a directory does not exist or a file of the same name does.
        /// </summary>
        /// <param name="dest">The destination path.</param>
        private static void validateDestination(string dest)
        {
            string destDir = Directory.GetParent(dest).FullName;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            if (File.Exists(dest))
                File.Delete(dest);
        }

        private static void updateTableStatus(TableIndex index, string status, bool processed = false, bool error = false)
        {
            using (ModelDbContext context = _contextFactory.GetModelDbContext())
            {
                index.Status = status;
                index.Processed = processed;
                index.Error = error;
                context.UpdateModel(index);

                context.CreateModel(new DbEvent()
                {
                    ActionCode = (int)EPSCoR.Web.Database.Models.Action.Updated,
                    EntryID = index.ID,
                    TableName = "TableIndexes"
                });
            }
        }
    }
}
