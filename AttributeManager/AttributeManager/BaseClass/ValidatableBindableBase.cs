using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeManager
{
    public class ValidatableBindableBase : BindableBase, INotifyDataErrorInfo
    {
        public bool HasErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
