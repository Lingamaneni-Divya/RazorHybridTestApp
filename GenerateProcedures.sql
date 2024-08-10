USE YourDatabaseName;
GO

-- Replace 'YourDatabaseName' with the actual name of your database

DECLARE @tableName NVARCHAR(128);
DECLARE @columnName NVARCHAR(128);
DECLARE @sql NVARCHAR(MAX);
DECLARE @updateColumns NVARCHAR(MAX);
DECLARE @insertColumns NVARCHAR(MAX);
DECLARE @insertValues NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'IntuneMobilityViolence'
AND TABLE_TYPE = 'BASE TABLE';

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @updateColumns = '';
    SET @insertColumns = 'Id, CreatedDate, UpdatedDate';
    SET @insertValues = 'Source.Id, GETDATE(), GETDATE()';

    -- Get all column names except Id, CreatedDate, and UpdatedDate
    DECLARE column_cursor CURSOR FOR
    SELECT COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'IntuneMobilityViolence'
    AND TABLE_NAME = @tableName
    AND COLUMN_NAME NOT IN ('Id', 'CreatedDate', 'UpdatedDate');

    OPEN column_cursor;
    FETCH NEXT FROM column_cursor INTO @columnName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @updateColumns = @updateColumns + 'Target.' + @columnName + ' = Source.' + @columnName + ', ';
        SET @insertColumns = @insertColumns + ', ' + @columnName;
        SET @insertValues = @insertValues + ', Source.' + @columnName;

        FETCH NEXT FROM column_cursor INTO @columnName;
    END;

    CLOSE column_cursor;
    DEALLOCATE column_cursor;

    -- Remove the trailing comma and space from @updateColumns
    SET @updateColumns = LEFT(@updateColumns, LEN(@updateColumns) - 2);

    SET @sql = '
    -- Create or alter procedure
    IF OBJECT_ID(''IntuneMobilityViolence.SaveOrUpdate_' + @tableName + ''') IS NOT NULL
        DROP PROCEDURE IntuneMobilityViolence.SaveOrUpdate_' + @tableName + ';
    
    CREATE PROCEDURE IntuneMobilityViolence.SaveOrUpdate_' + @tableName + '
        @InputData IntuneMobilityViolence.' + @tableName + 'Type READONLY
    AS
    BEGIN
        SET NOCOUNT ON;

        MERGE INTO IntuneMobilityViolence.' + @tableName + ' AS Target
        USING @InputData AS Source
        ON Target.Id = Source.Id
        WHEN MATCHED THEN
            UPDATE SET
                ' + @updateColumns + ',
                Target.UpdatedDate = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (' + @insertColumns + ')
            VALUES (' + @insertValues + ');
    END;
    ';

    -- Print the SQL statement for debugging purposes (optional)
    PRINT @sql;

    -- Execute the generated SQL to create the procedure
    EXEC sp_executesql @sql;

    FETCH NEXT FROM table_cursor INTO @tableName;
END;

CLOSE table_cursor;
DEALLOCATE table_cursor;