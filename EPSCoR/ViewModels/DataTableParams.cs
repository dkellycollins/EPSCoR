using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.ViewModels
{
    [ModelBinder(typeof(DataTablesParamsBinder))]
    public class DataTableParams
    {
        /// <summary>
        /// Display start point in the current data set.
        /// </summary>
        public int DisplayStart { get; set; }

        /// <summary>
        /// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
        /// </summary>
        public int DisplayLength { get; set; }

        /// <summary>
        /// Number of columns being displayed (useful for getting individual column search info)
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Global search field
        /// </summary>
        public string GlobalSearch { get; set; }

        /// <summary>
        /// True if the global filter should be treated as a regular expression for advanced filtering, false if not.
        /// </summary>
        public bool GlobalRegex { get; set; }

        /// <summary>
        /// Indicator for if a column is flagged as searchable or not on the client-side
        /// </summary>
        public bool[] Searchable { get; set; }

        /// <summary>
        /// Individual column filter
        /// </summary>
        public string[] ColumnSearch { get; set; }

        /// <summary>
        /// True if the individual column filter should be treated as a regular expression for advanced filtering, false if not
        /// </summary>
        public bool[] ColumnRegex { get; set; }

        /// <summary>
        /// Indicator for if a column is flagged as sortable or not on the client-side
        /// </summary>
        public bool[] Sortable { get; set; }

        /// <summary>
        /// Number of columns to sort on
        /// </summary>
        public int SortingCols { get; set; }

        /// <summary>
        /// Column being sorted on (you will need to decode this number for your database)
        /// </summary>
        public int[] SortCol { get; set; }

        /// <summary>
        /// Direction to be sorted - "desc" or "asc".
        /// </summary>
        public string[] SortDir { get; set; }

        /// <summary>
        /// The value specified by mDataProp for each column. This can be useful for ensuring that the processing of data is independent from the order of the columns.
        /// </summary>
        public string[] DataProp { get; set; }

        /// <summary>
        /// Information for DataTables to use for rendering.
        /// </summary>
        public string Echo { get; set; }

        /// <summary>
        /// Custom parameter that contains the name of the table.
        /// </summary>
        public string TableName { get; set; }

        public class DataTablesParamsBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.HttpContext.Request;

                DataTableParams result = new DataTableParams();
                if (request["iDisplayStart"] != null)
                    result.DisplayStart = Int32.Parse(request["iDisplayStart"]);
                if (request["iDisplayLength"] != null)
                    result.DisplayLength = Int32.Parse(request["iDisplayLength"]);
                if (request["iColumns"] != null)
                    result.Columns = Int32.Parse(request["iColumns"]);
                if (request["sSearch"] != null)
                    result.GlobalSearch = request["sSearch"];
                if (request["bRegex"] != null)
                    result.GlobalRegex = Boolean.Parse(request["bRegex"]);
                if (request["bSearchable_0"] != null)
                {
                    result.Searchable = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.Searchable[i] = Boolean.Parse(request["bSearchable_" + i]);
                    }
                }
                if (request["sSearch_0"] != null)
                {
                    result.ColumnSearch = new string[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.ColumnSearch[i] = request["sSearch_" + i];
                    }
                }
                if (request["bRegex_0"] != null)
                {
                    result.ColumnRegex = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.ColumnRegex[i] = Boolean.Parse(request["bRegex_" + i]);
                    }
                }
                if (request["bSortable_0"] != null)
                {
                    result.Sortable = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.Sortable[i] = Boolean.Parse(request["bSortable_" + i]);
                    }
                }
                if (request["iSortingCols"] != null)
                    result.SortingCols = Int32.Parse(request["iSortingCols"]);
                if (request["iSortCol_0"] != null)
                {
                    result.SortCol = new int[result.SortingCols];
                    for (int i = 0; i < result.SortingCols; i++)
                    {
                        result.SortCol[i] = Int32.Parse(request["iSortCol_" + i]);
                    }
                }
                if (request["sSortDir_0"] != null)
                {
                    result.SortDir = new string[result.SortingCols];
                    for (int i = 0; i < result.SortingCols; i++)
                    {
                        result.SortDir[i] = request["sSortDir_" + i];
                    }
                }
                if (request["mDataProp_0"] != null)
                {
                    result.DataProp = new string[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.DataProp[i] = request["mDataProp_" + i];
                    }
                }
                if (request["sEcho"] != null)
                    result.Echo = request["sEcho"];
                if (request["tableName"] != null)
                    result.TableName = request["tableName"];

                return result;
            }
        }
    }
}