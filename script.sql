DECLARE @SchemaName NVARCHAR(128) = 'IntuneMobilityViolence';
DECLARE @SQL NVARCHAR(MAX) = '';

SELECT @SQL = @SQL +
'CREATE TYPE ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(t.name + 'Type') + ' AS TABLE' + CHAR(13) +
'(' + CHAR(13) +
STUFF((
    SELECT CHAR(13) + '    ' + QUOTENAME(c.name) + ' ' + 
           UPPER(tp.name) + 
           CASE 
               WHEN tp.name IN ('varchar', 'nvarchar', 'char', 'nchar') THEN '(' + 
                    CASE WHEN c.max_length = -1 THEN 'MAX' 
                         ELSE CAST(c.max_length AS NVARCHAR(10)) 
                    END + ')'
               WHEN tp.name IN ('decimal', 'numeric') THEN '(' + 
                    CAST(c.precision AS NVARCHAR(10)) + ',' + 
                    CAST(c.scale AS NVARCHAR(10)) + ')'
               ELSE ''
           END + ' ' +
           CASE WHEN c.is_nullable = 0 THEN 'NOT NULL' ELSE 'NULL' END + ','
    FROM sys.columns c
    JOIN sys.types tp ON c.user_type_id = tp.user_type_id
    WHERE c.object_id = t.object_id
    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 0, '') + CHAR(13) +
');' + CHAR(13) + CHAR(13)
FROM sys.tables t
WHERE t.schema_id = SCHEMA_ID(@SchemaName);

-- Output the generated SQL script
PRINT @SQL;

-- Optionally, execute the generated SQL script
-- EXEC sp_executesql @SQL;