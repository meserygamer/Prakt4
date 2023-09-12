using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealizePythonList
{
    public class PythonList <T> : ICollection<T>
    {
        private List<T> listOfObject;
        public PythonList()
        {
            listOfObject = new List<T>();
        }

        public PythonList(List<T> listOfObject)
        { 
            this.listOfObject = listOfObject;
        }

        public PythonList(params T[] mas)
        {
            this.listOfObject = new List<T>(mas);
        }

        public T this[int index] 
        {
            set => this.listOfObject[index] = value;
            get
            {
                index = index % listOfObject.Count;
                if (index >= 0) return listOfObject[index];
                else return listOfObject[listOfObject.Count + index];
            }
        }

        public int Count => listOfObject.Count();

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            listOfObject.Add(item);
        }

        public void Clear()
        {
            listOfObject.Clear();
        }

        public bool Contains(T item) => listOfObject.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            for(int i = 0; i < listOfObject.Count; i++)
            {
                array[arrayIndex + i] = listOfObject[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return listOfObject.GetEnumerator();
        }

        public bool Remove(T item) => listOfObject.Remove(item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return listOfObject.GetEnumerator();
        }
    }
}
