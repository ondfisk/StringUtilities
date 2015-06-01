USE [<database>]
GO

DECLARE @Assembly nvarchar(250) 

SET @Assembly = 'C:\Users\rasmusl\Documents\GitHub\StringUtilities\StringUtilities\bin\Debug\StringUtilities.dll'

ALTER DATABASE MAP SET TRUSTWORTHY ON;
GO

CREATE ASSEMBLY StringUtilities FROM @Assembly
WITH permission_set=Unsafe;
GO

CREATE AGGREGATE Concatenate(@input nvarchar(4000))
RETURNS nvarchar(4000)
EXTERNAL NAME [StringUtilities].[Microsoft.Samples.SqlServer.Concatenate];
GO

CREATE AGGREGATE ConcatenateDistinct(@input nvarchar(4000))
RETURNS nvarchar(4000)
EXTERNAL NAME [StringUtilities].[Microsoft.Samples.SqlServer.ConcatenateDistinct]; 
GO

CREATE FUNCTION [Split](@input nvarchar(4000)) 
RETURNS TABLE(StringElement nvarchar(128))
AS EXTERNAL NAME [StringUtilities].[Microsoft.Samples.SqlServer.StringSplitter].[Split];
GO

CREATE FUNCTION [RegexMatches] (@input nvarchar(max), @pattern nvarchar(max))
RETURNS TABLE(
	MatchID int,
    MatchIndex int,
    MatchValue nvarchar(4000),
	GroupID int,
	GroupIndex int,
	GroupValue nvarchar(4000),
	CaptureIndex int,
	CaptureValue nvarchar(4000))
AS EXTERNAL NAME [StringUtilities].[Microsoft.Samples.SqlServer.RegularExpression].[Matches];
GO

CREATE FUNCTION [RegexReplace] (@input nvarchar(max), @pattern nvarchar(max), @replacement nvarchar(max))
RETURNS nvarchar(max)
AS EXTERNAL NAME [StringUtilities].[Microsoft.Samples.SqlServer.RegularExpression].[Replace]
GO


		

