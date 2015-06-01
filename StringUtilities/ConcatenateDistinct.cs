using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    public class ConcatenateDistinct : IBinarySerialize
    {
        /// <summary>
        /// The variable that holds the intermediate result of the concatenation
        /// </summary>
        private IList<string> _intermediateResult;

        /// <summary>
        /// Initialize the internal data structures
        /// </summary>
        public void Init()
        {
            _intermediateResult = new List<string>();
        }

        /// <summary>
        /// Accumulate the next value, nop if the value is null
        /// </summary>
        /// <param name="value"></param>
        public void Accumulate(SqlString value)
        {

            if (value.IsNull || string.IsNullOrWhiteSpace(value.Value))
            {
                return;
            }

            _intermediateResult.Add(value.Value.Trim());
        }

        /// <summary>
        /// Merge the partially computed aggregate with this aggregate.
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
        /// Called at the end of aggregation, to return the results of the aggregation
        /// </summary>
        /// <returns></returns>
        public SqlString Terminate()
        {
            var output = string.Empty;
            if (_intermediateResult != null && _intermediateResult.Any())
            {
                output = string.Join(", ", _intermediateResult.OrderBy(s => s).Distinct());
            }
            return new SqlString(output);
        }

        public void Read(BinaryReader r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }
            var formatter = new BinaryFormatter();
            _intermediateResult = (string[]) formatter.Deserialize(r.BaseStream);
        }

        public void Write(BinaryWriter w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, _intermediateResult.ToArray());

                w.Write(stream.ToArray());
            }
        }
    }
}