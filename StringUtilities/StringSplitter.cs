/*=====================================================================

  File:      Split.cs for Streaming TVF SQLCLR Example
  Summary:   Defines a streaming table valued function to split a string into a one-column table
  Date:	     July 21, 2003

---------------------------------------------------------------------
  This file is part of the Microsoft SQL Server Code Samples.
  Copyright (C) Microsoft Corporation.  All rights reserved.

  This source code is intended only as a supplement to Microsoft
  Development Tools and/or on-line documentation.  See these other
  materials for detailed information regarding Microsoft code samples.

  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
======================================================= */

using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.Server;

namespace Microsoft.Samples.SqlServer
{
    public static class StringSplitter
    {

        /// <summary>
        /// The streaming table-valued function used to split the string into a relation
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        [SqlFunction(FillRowMethodName = "FillRow")]
        public static IEnumerable Split(SqlString argument)
        {
            string value;
            if (argument.IsNull)
                value = "";
            else
                value = argument.Value;
            return value.Split(',');
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public static void FillRow(Object obj, out string stringElement)
        {
            stringElement = (string)obj;
        }
    }
}
