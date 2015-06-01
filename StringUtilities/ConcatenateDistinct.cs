using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace Microsoft.Samples.SqlServer
{
    [Serializable]
    [SqlUserDefinedAggregate(
        Format.UserDefined, //use clr serialization to serialize the intermediate result
        IsInvariantToNulls = true, //optimizer property
        IsInvariantToDuplicates = false, //optimizer property
        IsInvariantToOrder = false, //optimizer property
        MaxByteSize = 8000)] //maximum size in bytes of persisted value
    public class ConcatenateDistinct : IBinarySerialize
    {
        /// <summary>
        ///     The variable that holds the intermediate result of the concatenation
        /// </summary>
        private IList<string> _intermediateResult;

        public void Read(BinaryReader r)
        {
            if (r == null)
            {
                throw new ArgumentNullException(nameof(r));
            }
            _intermediateResult = new List<string>();
            while (r.PeekChar() != -1)
            {
                _intermediateResult.Add(r.ReadString());
            }
        }

        public void Write(BinaryWriter w)
        {
            if (w == null)
            {
                throw new ArgumentNullException(nameof(w));
            }
            foreach (var value in _intermediateResult)
            {
                w.Write(value);
            }
        }

        /// <summary>
        ///     Initialize the internal data structures
        /// </summary>
        public void Init()
        {
            _intermediateResult = new List<string>();
        }

        /// <summary>
        ///     Accumulate the next value, nop if the value is null
        /// </summary>
        /// <param name="value"></param>
        public void Accumulate(SqlString value)
        {
            if (value.IsNull)
            {
                return;
            }
            _intermediateResult.Add(value.Value);
        }

        /// <summary>
        ///     Merge the partially computed aggregate with this aggregate.
        /// </summary>
        /// <param name="other"></param>
        public void Merge(ConcatenateDistinct other)
        {
            foreach (var value in other._intermediateResult)
            {
                _intermediateResult.Add(value);
            }
        }

        /// <summary>
        ///     Called at the end of aggregation, to return the results of the aggregation
        /// </summary>
        /// <returns></returns>
        public SqlString Terminate()
        {
            var output = string.Empty;
            //delete the trailing comma, if any
            if (_intermediateResult != null && _intermediateResult.Count > 0)
            {
                output = string.Join(", ",
                    _intermediateResult.Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s).Distinct());
            }
            return new SqlString(output);
        }
    }
}