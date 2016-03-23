using AttributeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WorkBook = Microsoft.Office.Interop.Excel;

namespace AttributeManager.BaseClass
{
    public class ExcelReader : IExcelReader
    {
        private WorkBook.Workbook _workBook;
        private WorkBook.Sheets _workSheets;
        private WorkBook.Worksheet _itemSheets;
        private WorkBook.Application _xls;

        private string[] dataColumns = { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private string[] legendColumns = { "A", "B", "C", "D" };
        int dataRowHeader = 3;
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

                for (int i = 0; i < attributes.Count - 1; i++)
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

        public List<AttributeModel> GetComponentDictionary()
        {
            _itemSheets = (WorkBook.Worksheet)_workSheets.Item[2];
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
                        AttridbuteId = Convert.ToString(_itemSheets.Range[$"{legendColumns[0]}{row.ToString()}", Missing.Value].Value2),
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
    }
}
