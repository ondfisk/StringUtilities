using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Server;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Samples.SqlServer")]

namespace Microsoft.Samples.SqlServer
{
    [Serializable]
    [SqlUserDefinedAggregate(
        Format.UserDefined, //use clr serialization to serialize the intermediate result
        IsInvariantToNulls = true,			//optimizer property
        IsInvariantToDuplicates = false,		//optimizer property
        IsInvariantToOrder = false,			//optimizer property
        MaxByteSize = 8000)					//maximum size in bytes of persisted value
        ]
    public class Concatenate : IBinarySerialize
    {
        /// <summary>
        /// The variable that holds the intermediate result of the concatenation
        /// </summary>
        private StringBuilder _intermediateResult;

        /// <summary>
        /// Initialize the internal data structures
        /// </summary>
        public void Init()
        {
            _intermediateResult = new StringBuilder();
        }

        /// <summary>
        /// Accumulate the next value, nop if the value is null
        /// </summary>
        /// <param name="value"></param>
        public void Accumulate(SqlString value)
        {

            if (value.IsNull)
            {
                return;
            }
            _intermediateResult.Append(value.Value).Append(',');

        }

        /// <summary>
        /// Merge the partially computed aggregate with this aggregate.
        /// </summary>
        /// <param name="other"></param>
        public void Merge(Concatenate other)
        {
            _intermediateResult.Append(other._intermediateResult);
        }

        /// <summary>
        /// Called at the end of aggregation, to return the results of the aggregation
        /// </summary>
        /// <returns></returns>
        public SqlString Terminate()
        {
            string output = string.Empty;
            //delete the trailing comma, if any
            if (_intermediateResult != null && _intermediateResult.Length > 0)
            {
                output = _intermediateResult.ToString(0, _intermediateResult.Length - 1);
            }
            return new SqlString(output);
        }

        public void Read(BinaryReader r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }
            _intermediateResult = new StringBuilder(r.ReadString());
        }

        public void Write(BinaryWriter w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
			w.Write(_intermediateResult.ToString());
        }
    }
}