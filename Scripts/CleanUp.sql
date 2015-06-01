USE Test
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'Split' and (type = 'FS' or type = 'FT'))  
DROP FUNCTION [Split];
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'SplitBy' and (type = 'FS' or type = 'FT'))  
DROP FUNCTION [SplitBy];
GO

IF EXISTS (SELECT * from sys.objects WHERE name = N'Concatenate' and (type = 'AF'))  
DROP AGGREGATE [Concatenate];
GO

IF EXISTS (SELECT * from sys.objects WHERE name = N'ConcatenateDistinct' and (type = 'AF'))  
DROP AGGREGATE [ConcatenateDistinct];
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'RegexMatches' and (type = 'FS' or type = 'FT'))
DROP FUNCTION RegexMatches;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'RegexReplace' and (type = 'FS' or type = 'FT'))
DROP FUNCTION RegexReplace;
GO

IF EXISTS (select * from sys.assemblies where name = N'StringUtilities') 
DROP ASSEMBLY StringUtilities;
GO
