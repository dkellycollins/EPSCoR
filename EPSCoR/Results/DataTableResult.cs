using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Results
{
    public class DataTableResult : ActionResult
    {
        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned in this result set)
        /// </summary>
        public int iTotalDisplayRecords { get; set; }

        /// <summary>
        /// An unaltered copy of sEcho sent from the client side. This parameter will change with each draw (it is basically a draw count) - so it is important that this is implemented. Note that it strongly recommended for security reasons that you 'cast' this parameter to an integer in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public string sEcho { get; set; }

        /// <summary>
        /// The data in a 2D array. Note that you can change the name of this parameter with sAjaxDataProp.
        /// </summary>
        public object[,] aaData { get; set; }

        public DataTableResult(int totalRecords, int totalDisplayRecords, int echo, DataTable data)
        {
            iTotalRecords = totalRecords;
            iTotalDisplayRecords = totalDisplayRecords;
            sEcho = echo.ToString();
            int numCol = data.Columns.Count;
            int numRow = data.Rows.Count;
            aaData = new object[numRow, numCol];
            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    aaData[i, j] = data.Rows[i][j];
                }
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string json = JsonConvert.SerializeObject(this);

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(json);
        }
    }
}