
GO

/****** Object:  UserDefinedFunction [dbo].[ufn_SplitStringToTable]    Script Date: 2016/7/22 10:40:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ufn_SplitStringToTable]
(
  @str VARCHAR(MAX) ,
  @split VARCHAR(10)
)
RETURNS TABLE
    AS 
RETURN
    ( SELECT    B.id
      FROM      ( SELECT    [value] = CONVERT(XML , '<v>' + REPLACE(@str , @split , '</v><v>')
                            + '</v>')
                ) A
      OUTER APPLY ( SELECT  id = N.v.value('.' , 'varchar(100)')
                    FROM    A.[value].nodes('/v') N ( v )
                  ) B
    )
GO


