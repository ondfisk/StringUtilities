USE Test
GO

-- Invoke the tvf
SELECT * FROM dbo.Split('will,this,work');
GO

-- Invoke the tvf
SELECT * FROM dbo.SplitBy('will;this;work', ';');
GO

-- Invoke the aggregate over the results of the tvf
SELECT dbo.Concatenate(StringElement) FROM dbo.Split('will,this,also,work');
GO

-- Invoke distinct aggregate
SELECT dbo.ConcatenateDistinct(StringElement) FROM dbo.Split('1,1,2,2,3,1,2,3')

-- Find two word pairs where the first word contains an 'r'
SELECT MatchID, MatchIndex, MatchValue, 
  GroupID, GroupIndex, GroupValue, 
  CaptureIndex, CaptureValue
FROM dbo.RegexMatches('The quick red fox jumped over the lazy brown dog', '(\w*r\w*)\s(\w+)');
GO

-- A variant of the above with no backtracking
SELECT MatchID, MatchIndex, MatchValue, 
  GroupID, GroupIndex, GroupValue, 
  CaptureIndex, CaptureValue
FROM dbo.RegexMatches('The quick red fox jumped over the lazy brown dog', '(?>\w*r\w*)\s(?>\w+)');
GO

-- Swap the subject of the sentence with the object of the sentence.
SELECT dbo.RegexReplace('The quick red fox jumped over the lazy brown dog', 
						'^The (?<fox>(?:[\w]+\s){3})jumped over the (?<dog>(?:[\w]+\s){2}(?:[\w]+))$',
						'The ${dog} jumped over the ${fox}');


						 

