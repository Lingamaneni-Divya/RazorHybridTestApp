DECLARE @SchemaName NVARCHAR(128) = 'IntuneMobilityViolence';
DECLARE @TableName NVARCHAR(128);
DECLARE @SQL NVARCHAR(MAX);

-- Cursor to iterate over each table in the specified schema
DECLARE TableCursor CURSOR FOR
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = @SchemaName;

OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Generate and execute the dynamic SQL for each table
    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName);
    PRINT @SQL; -- For debugging purposes, print the SQL statement
    EXEC sp_executesql @SQL;
    
    FETCH NEXT FROM TableCursor INTO @TableName;
END

CLOSE TableCursor;
DEALLOCATE TableCursor;

DECLARE @SchemaName NVARCHAR(128) = 'IntuneMobilityViolence';
DECLARE @TableName NVARCHAR(128);
DECLARE @SQL NVARCHAR(MAX);

-- Cursor to iterate over each table in the specified schema
DECLARE TableCursor CURSOR FOR
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = @SchemaName;

OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Generate and execute the dynamic SQL for each table
    SET @SQL = 'DELETE FROM ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName);
    PRINT @SQL; -- For debugging purposes, print the SQL statement
    EXEC sp_executesql @SQL;
    
    FETCH NEXT FROM TableCursor INTO @TableName;
END

CLOSE TableCursor;
DEALLOCATE TableCursor;