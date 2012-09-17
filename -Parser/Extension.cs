namespace ParsingTables.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections;

    static class ListExtension
    {

        public static List<T> ToList<T>(this IList iList)
        {
            List<T> result = new List<T>();
            foreach (T value in iList)
            {
                result.Add(value);
            }
            return result;
        }


        public static List<T> ToList<T>(this IList<T> iList)
        {
            //List<T> result = new List<T>();
            //foreach (T value in iList)
            //{
            //    result.Add(value);
            //}
            //return result;
            return (List<T>)iList;
        }

        
    }

  
}
