using System;
using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Microsoft.Samples.SqlServer
{
    public static class StringSplitter
    {
        /// <summary>
        ///     The streaming table-valued function used to split the string into a relation
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        [SqlFunction(FillRowMethodName = "FillRow")]
        public static IEnumerable Split(SqlString argument)
        {
            var value = argument.IsNull ? string.Empty : argument.Value;

            return value.Split(',');
        }

        /// <summary>
        ///     The streaming table-valued function used to split the string into a relation
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        [SqlFunction(FillRowMethodName = "FillRow")]
        public static IEnumerable SplitBy(SqlString argument, SqlString by)
        {
            var value = argument.IsNull ? string.Empty : argument.Value;

            if (by.IsNull)
            {
                throw new ArgumentNullException(nameof(by));
            }
            if (by.Value.Length != 1)
            {
                throw new ArgumentException("by must be exactly one character", nameof(by));
            }

            var split = by.Value.ToCharArray()[0];

            return value.Split(split);
        }

        public static void FillRow(object obj, out string stringElement)
        {
            stringElement = (string) obj;
        }
    }
}