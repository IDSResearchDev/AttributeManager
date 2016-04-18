using ConnectionCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionCreator.BaseClass
{
    public interface IExcelReader
    {
        List<Component> GetComponents();
        List<Component> GetComponentData();
        List<Component> GetComponents(List<string> data, string sheetName);
        List<string> IdentifyAttributes();

        List<AttributeModel> GetComponentDictionary();
        void ForceDispose();
        
    }
}
