using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon_editor {
    public static class ArrayUtils {
        public static T[] InsertElemAt<T>(T elem, T[] array, int n) {
            T[] newElems = new T[array.Length + 1];
            Array.Copy(array, newElems, n);
            Array.Copy(array, n, newElems, n + 1, array.Length - n);
            newElems[n] = elem;
            return newElems;
        }

        public static T[] RemoveNthElem<T>(int n, T[] array) {
            T[] newElems = new T[array.Length - 1];
            Array.Copy(array, newElems, n);
            Array.Copy(array, n + 1, newElems, n, array.Length - n - 1);
            return newElems;
        }
    }
}
