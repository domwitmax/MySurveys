using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MySurveys.Shared.Collections
{
    public class ObservableCollection<T>
    {
        public List<T> List;
        public ObservableCollection(int size, T defaultValue)
        {
            List = new List<T>();
            for (int i = 0; i < size; i++)
            {
                List.Add(defaultValue);
            }
        }
        public T this[int key]
        {
            get => List[key];
            set
            {
                List[key] = value;
                PropertyChanged.Invoke(this, (key, value));
            }
        }
        public event EventHandler<(int, T)> PropertyChanged;
    }
}
