using ConnectionCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WorkBook = Microsoft.Office.Interop.Excel;

namespace ConnectionCreator.BaseClass
{
    public class ExcelReader : IExcelReader
    {
        private WorkBook.Workbook _workBook;
        private WorkBook.Sheets _workSheets;
        private WorkBook.Worksheet _itemSheets;
        private WorkBook.Application _xls;
        
        private string[] legendColumns = { "A", "B", "C", "D" };
        
        private int _rowStart = 2;
        private int _columnStart = 2;

        public ExcelReader(string fileLocation)
        {
            _xls = new WorkBook.Application
            {
                Visible = false,
                DisplayAlerts = false
            };

            _workBook = _xls.Workbooks.Open(fileLocation, 0, false, 5, "", "", false, WorkBook.XlPlatform.xlWindows
                                                     , "", true, false, 0, true, false, false);

            _workSheets = _workBook.Worksheets;
        }

        public List<Component> GetComponentData()
        {
            int SheetCount = _workSheets.Count;
            List<Component> components = new List<Component>();
            try
            {
                for (int i = 1; i <= SheetCount; i++)
                {
                    _itemSheets = _workSheets[i];
                    var worksSheetName = _itemSheets.Name;

                    if (worksSheetName.Equals("Legend") || worksSheetName.Equals("General")) continue;

                    List<string> data = new List<string>();
                    WorkBook.Range excelRange = _itemSheets.UsedRange;

                    var rowCount = excelRange.Rows.Count;
                    var columnCount = excelRange.Columns.Count;


                    object[,] valueArray = (object[,])excelRange.get_Value(WorkBook.XlRangeValueDataType.xlRangeValueDefault);

                    for (int row = _rowStart; row <= rowCount; row++)
                    {
                        var str = Convert.ToString(valueArray[row, _columnStart]);
                        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str)) break;

                        var @out = "";
                        for (int col = _columnStart; col < columnCount; col++)
                        {
                            var val = Convert.ToString(valueArray[row, col]);

                            @out += $"{val},";

                        }
                        data.Add(@out.Trim(','));
                    }

                    components.AddRange(GetComponents(data, worksSheetName));
                }
            }
            catch (Exception x)
            {
                throw x;
            }

            return components;
        }

        public List<Component> GetComponents(List<string> data, string sheetName)
        {
            List<Component> components = new List<Component>();
            int componentNumber = 0;
            int.TryParse(sheetName, out componentNumber);

            if (componentNumber != 0 && data.Count > 0)
            {                                
                var attributes = data[0].Split(',');
                for (int i = 1; i < data.Count; i++)
                {
                    var values = data[i].Split(',');

                    Component component = new Component();
                    component.ComponentNumber = componentNumber;
                    component.Size = values[0];

                    if (string.IsNullOrEmpty(component.Size) || string.IsNullOrWhiteSpace(component.Size)) continue;
                    

                    for (int v = 1; v < values.Count(); v++)
                    {
                        component.Attributes.Add(attributes[v], values[v]);
                    }

                    components.Add(component);
                }
            }

            return components;
        }

        public List<AttributeModel> GetComponentDictionary()
        {
            _itemSheets = (WorkBook.Worksheet)_workSheets.Item["Legend"];
            int legendRowHeader = 1;
            int row = legendRowHeader;
            var attributeDictionary = new List<AttributeModel>();

            bool endLine = false;
            while (!endLine)
            {
                row++;

                string line = Convert.ToString(_itemSheets.Range[$"{legendColumns[0]}{row.ToString()}", Missing.Value].Value2);

                if (String.IsNullOrEmpty(line) || String.IsNullOrWhiteSpace(line))
                { endLine = true; }
                else
                {
                    AttributeModel attribute = new AttributeModel
                    {
                        AttributeId = Convert.ToString(_itemSheets.Range[$"{legendColumns[0]}{row.ToString()}", Missing.Value].Value2),
                        ParameterType = Convert.ToString(_itemSheets.Range[$"{legendColumns[1]}{row.ToString()}", Missing.Value].Value2),
                        Tab = Convert.ToString(_itemSheets.Range[$"{legendColumns[2]}{row.ToString()}", Missing.Value].Value2),
                        AttributeName = Convert.ToString(_itemSheets.Range[$"{legendColumns[3]}{row.ToString()}", Missing.Value].Value2)

                    };
                    attributeDictionary.Add(attribute);
                }
            }

            return attributeDictionary;
        }

        public void ForceDispose()
        {
            _workBook.Close();
            
            ReleaseObject(_itemSheets);
            ReleaseObject(_workSheets);
            ReleaseObject(_workBook);
            _xls.Quit();
            ReleaseObject(_xls);
        }

        private void ReleaseObject(object obj)
        {

            try
            {
                Marshal.ReleaseComObject(obj);

                obj = null;
            }
            catch (Exception err)
            {
                obj = null;
                throw new ArgumentException("Unable to release the object." + Environment.NewLine + err.ToString());
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }

        #region unused
        int dataRowHeader = 3;
        private string[] dataColumns = { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public List<string> IdentifyAttributes()
        {

            List<string> attributes = new List<string>();
            int checker = 0, index = 0;
            _itemSheets = (WorkBook.Worksheet)_workSheets.Item[1];
            while (checker <= 3)
            {
                string attr = _itemSheets.Range[$"{dataColumns[index]}{dataRowHeader.ToString()}", Missing.Value].Value2;
                if (String.IsNullOrEmpty(attr) || String.IsNullOrWhiteSpace(attr))
                { checker++; }
                else
                {
                    if (!attributes.Contains(attr))
                        attributes.Add(attr);
                }
                index++;
            }

            return attributes;
        }
        public List<Component> GetComponents()
        {
            List<Component> components = new List<Component>();
            var attributes = IdentifyAttributes();
            _itemSheets = (WorkBook.Worksheet)_workSheets.Item[1];

            int row = dataRowHeader;
            bool endLine = false;
            while (!endLine)
            {
                Component component = new Component();
                component.ComponentNumber = 141;
                row++;

                for (int i = 0; i <= attributes.Count - 1; i++)
                {
                    component.Size = _itemSheets.Range[$"{dataColumns[0]}{row.ToString()}", Missing.Value].Value2;

                    string value = Convert.ToString(_itemSheets.Range[$"{dataColumns[i]}{row.ToString()}", Missing.Value].Value2);

                    component.Attributes.Add(attributes[i], value);
                }

                if (String.IsNullOrEmpty(component.Size) || String.IsNullOrWhiteSpace(component.Size))
                { endLine = true; }
                else
                {
                    components.Add(component);
                }
            }

            return components;
        }
        #endregion



    }
}
