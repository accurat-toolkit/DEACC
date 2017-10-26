using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multithreading
{
    public static class Multithreader
    {
        public delegate double functionDelegateList(string x, string y);

        public static Dictionary<string, double> fork(string x, Hashtable yi, functionDelegateList function)
        {
            SyncronizedDictionary<string, double> ret = new SyncronizedDictionary<string, double>();
            string[] yiArray = new string[yi.Values.Count];
            string[] keyArray = new string[yi.Keys.Count];

            yi.Values.CopyTo(yiArray, 0);
            yi.Keys.CopyTo(keyArray, 0);

            Parallel.For(0, yi.Count, delegate(int i)
            {
                double result = function(x, yiArray[i]);
                ret.Add(keyArray[i], result);
            }
            );

            return ret.dic;
        }
    }

    public class SyncronizedDictionary<T1, T2>
    {
        public Dictionary<T1, T2> dic = new Dictionary<T1, T2>();

        object o = new object();

        public void Add(T1 t1, T2 t2)
        {
            lock (o)
            {
                dic.Add(t1, t2);
            }
        }
    }
}
