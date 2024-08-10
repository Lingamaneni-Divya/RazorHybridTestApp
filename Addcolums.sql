DECLARE @TableName NVARCHAR(128);
DECLARE @SchemaName NVARCHAR(128) = 'IntuneMobilityViolence';

DECLARE TableCursor CURSOR FOR
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = @SchemaName AND TABLE_TYPE = 'BASE TABLE';

OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    SET @SQL = N'
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N''CreatedDate'' AND Object_ID = Object_ID(N''' + @SchemaName + '.' + @TableName + '''))
    BEGIN
        ALTER TABLE ' + @SchemaName + '.' + @TableName + ' ADD CreatedDate DATETIME2 NOT NULL DEFAULT (SYSDATETIME());
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N''UpdatedDate'' AND Object_ID = Object_ID(N''' + @SchemaName + '.' + @TableName + '''))
    BEGIN
        ALTER TABLE ' + @SchemaName + '.' + @TableName + ' ADD UpdatedDate DATETIME2 NOT NULL DEFAULT (SYSDATETIME());
    END
    ';
    
    EXEC sp_executesql @SQL;
    FETCH NEXT FROM TableCursor INTO @TableName;
END;

CLOSE TableCursor;
DEALLOCATE TableCursor;