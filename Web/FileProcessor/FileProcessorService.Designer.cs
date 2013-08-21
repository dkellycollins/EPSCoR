namespace EPSCoR.Web.FileProcessor
{
    partial class FileProcessorService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fileWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).BeginInit();
            // 
            // fileWatcher
            // 
            this.fileWatcher.IncludeSubdirectories = true;
            this.fileWatcher.Created += new System.IO.FileSystemEventHandler(this._fileWatcher_Created);
            this.fileWatcher.Error += new System.IO.ErrorEventHandler(this._fileWatcher_Error);
            // 
            // FileProcessorService
            // 
            this.ServiceName = "EPSCoR File Processor";
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).EndInit();

        }

        #endregion

        private System.IO.FileSystemWatcher fileWatcher;
    }
}
