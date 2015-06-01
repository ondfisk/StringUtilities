using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace Microsoft.Samples.SqlServer
{
    /// <summary>
    /// This class is provides regular expression operations for Transact-SQL callers
    /// </summary>
    public static class RegularExpression
    {
        /// <summary>
        /// This method returns a table of matches, groups, and captures based on the input
        /// string and pattern string provided.
        /// </summary>
        /// <param name="sqlInput">What to match against</param>
        /// <param name="sqlPattern">What to look for</param>
        /// <returns>An object which appears to be reading from SQL Server but which in fact is reading
        ///          from a memory based representation of the data.</returns>
        [SqlFunction(FillRowMethodName = "FillRow")]
        public static IEnumerable Matches(SqlString sqlInput, SqlString sqlPattern)
        {
            string input = (sqlInput.IsNull) ? string.Empty : sqlInput.Value;
            string pattern = (sqlPattern.IsNull) ? string.Empty : sqlPattern.Value;

            return GetMatches(input, pattern);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public static void FillRow(object obj, out int matchId, out int matchIndex, out string matchValue,
            out int groupId, out int groupIndex, out string groupValue, out int captureIndex,
            out string captureValue)
        {
            MatchResult result = (MatchResult)obj;
            matchId = result.MatchID;
            matchIndex = result.MatchIndex;
            matchValue = result.MatchValue;
            groupId = result.GroupID;
            groupIndex = result.GroupIndex;
            groupValue = result.GroupValue;
            captureIndex = result.CaptureIndex;
            captureValue = result.CaptureValue;
        }

        /// <summary>
        ///     Generates a list of Match/Group/Capture tuples represented using the
        ///     MatchResult struct based on the regular expression match of the input
        ///     string and pattern string provided.
        /// </summary>
        /// <param name="input">What to match</param>
        /// <param name="pattern">What to look for</param>
        /// <returns>A list of Match/Group/Capture tuples</returns>
        private static List<MatchResult> GetMatches(string input, string pattern)
        {
            List<MatchResult> result = new List<MatchResult>();
            int matchId = 0;
            int groupId = 0;
            foreach (Match m in Regex.Matches(input, pattern))
            {
                if (m.Groups.Count < 1)
                    result.Add(new MatchResult(matchId, m.Index, m.Value, -1, -1, string.Empty, -1, string.Empty));
                else
                {
                    groupId = 0;
                    foreach (Group g in m.Groups)
                    {
                        if (g.Captures.Count < 1)
                            result.Add(new MatchResult(matchId, m.Index, m.Value,
                                groupId, g.Index, g.Value, -1, string.Empty));
                        else
                        {
                            foreach (Capture c in m.Groups)
                            {
                                result.Add(new MatchResult(matchId, m.Index, m.Value,
                                    groupId, g.Index, g.Value, c.Index, c.Value));
                            }
                        }

                        groupId += 1;
                    }
                }

                matchId += 1;
            }

            return result;
        }

        /// <summary>
        ///     This method performs a pattern based substitution based on the provided input string, pattern
        ///     string, and replacement string.
        /// </summary>
        /// <param name="sqlInput">The source material</param>
        /// <param name="sqlPattern">How to parse the source material</param>
        /// <param name="sqlReplacement">What the output should look like</param>
        /// <returns></returns>
        public static string Replace(SqlString sqlInput, SqlString sqlPattern, SqlString sqlReplacement)
        {
            string input = (sqlInput.IsNull) ? string.Empty : sqlInput.Value;
            string pattern = (sqlPattern.IsNull) ? string.Empty : sqlPattern.Value;
            string replacement = (sqlReplacement.IsNull) ? string.Empty : sqlReplacement.Value;
            return Regex.Replace(input, pattern, replacement);
        }
    }

    /// <summary>
    /// This struct is used trepresents a Match/Group/Capture tuple.  Instances of this struct are
    /// created by the GetMatches method.
    /// </summary>
    internal struct MatchResult
    {
        /// <summary>
        /// Which match this is
        /// </summary>
        private int _matchID;
        public int MatchID
        {
            get
            {
                return this._matchID;
            }
        }

        /// <summary>
        /// Where the match starts in the input string
        /// </summary>
        private int _matchIndex;
        public int MatchIndex
        {
            get
            {
                return this._matchIndex;
            }
        }

        /// <summary>
        /// What string matched the pattern
        /// </summary>
        private string _matchValue;
        public string MatchValue
        {
            get
            {
                return this._matchValue;
            }
        }

        /// <summary>
        /// Which matching group this is
        /// </summary>
        private int _groupID;
        public int GroupID
        {
            get
            {
                return this._groupID;
            }
        }

        /// <summary>
        /// Where this group starts in the input string
        /// </summary>
        private int _groupIndex;
        public int GroupIndex
        {
            get
            {
                return this._groupIndex;
            }
        }

        /// <summary>
        /// What the group matched in the input string
        /// </summary>
        private string _groupValue;
        public string GroupValue
        {
            get
            {
                return this._groupValue;
            }
        }

        /// <summary>
        /// Where this capture starts in the input string
        /// </summary>
        private int _captureIndex;
        public int CaptureIndex
        {
            get
            {
                return this._captureIndex;
            }
        }

        /// <summary>
        /// What the capture matched in the input string
        /// </summary>
        private string _captureValue;
        public string CaptureValue
        {
            get
            {
                return this._captureValue;
            }
        }

        /// <summary>
        ///     A convenient constructor which fills in all the fields contained in this struct.
        /// </summary>
        /// <param name="matchID">Which match this is</param>
        /// <param name="matchIndex">Where the match starts in the input string</param>
        /// <param name="matchValue">What string matched the pattern</param>
        /// <param name="groupID">Which matching group this is</param>
        /// <param name="groupIndex">Where this group starts in the input string</param>
        /// <param name="groupValue">What the group matched in the input string</param>
        /// <param name="captureIndex">Where this capture starts in the input string</param>
        /// <param name="captureValue">What the capture matched in the input string</param>
        public MatchResult(int matchId, int matchIndex, string matchValue,
            int groupId, int groupIndex, string groupValue,
            int captureIndex, string captureValue)
        {
            this._matchID = matchId;
            this._matchIndex = matchIndex;
            this._matchValue = matchValue;
            this._groupID = groupId;
            this._groupIndex = groupIndex;
            this._groupValue = groupValue;
            this._captureIndex = captureIndex;
            this._captureValue = captureValue;
        }
    }
}
