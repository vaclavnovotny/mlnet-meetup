using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;

namespace MLNET.Basics
{
    public static class DataLoader
    {
        public static IDataView LoadFromFile<T>(MLContext mlContext, string path, bool hasHeader = true) {
            return mlContext.Data.LoadFromTextFile<T>(path: path, hasHeader: hasHeader);
        }

        public static IDataView LoadFromEnumerable(string path)
        {
            return null;
        }

        public static IDataView LoadFromDatabase()
        {
            return null;
        }
    }
}
