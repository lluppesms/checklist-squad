# Schema Isolation in Shared Database Environments

> **Version:** 1.0  
> **Date:** 2026-04-22  
> **Author:** Dutch (Lead)  
> **Status:** Operational Guide

---

## Overview

The CheckList application has been designed to coexist in a shared Azure SQL Database alongside other applications. This migration from the `[dbo]` schema to a dedicated `[CheckList]` schema enables safe multi-tenant hosting while maintaining clear data boundaries and operational accountability.

### Why Schema Isolation?

When multiple applications share a single SQL Database:
- **Shared infrastructure cost** reduces per-app database overhead
- **Logical separation** prevents accidental cross-app data corruption
- **Clear ownership** makes it obvious which tables belong to CheckList
- **Future API access** easier to grant granular permissions per app

### What Changed

**EF Core DbContext:**
- DbContext fluent API now explicitly maps all entities to `[CheckList]` schema
- The schema is specified in `OnModelCreating()` but is invisible to entity class definitions
- No changes needed to entity POCO classes

**SQL Database Project:**
- All tables, stored procedures, views, and functions created in `[CheckList]` schema
- Pre-deployment script now creates the schema if it doesn't exist
- Seed data references use fully-qualified names: `[CheckList].[TableName]`
- No references to `[dbo]` except for excluded system objects

**Application Code:**
- Repository layer continues to work unchanged (EF Core handles schema routing)
- Connection strings point to the same database (schema is enforced via SQL permissions)
- No application-level schema awareness needed

---

## Connection String Guidance

### The Critical Distinction

**The connection string itself does NOT enforce schema isolation.**

A connection string with `User Id=CheckListAppUser` succeeds or fails based on:
1. Whether the SQL login exists
2. What database you're connecting to
3. What schema permissions the user has been granted

To truly restrict the CheckList application to only its schema, follow these steps:

### Step 1: Create a Dedicated Database User

```sql
-- Create a SQL login (do this ONCE per SQL Server)
CREATE LOGIN [CheckListAppLogin] WITH PASSWORD = 'ComplexPasswordHere!';

-- Create a database user mapped to the login (do this in the CheckList database)
USE [CheckListDatabase];
CREATE USER [CheckListAppUser] FOR LOGIN [CheckListAppLogin];
```

### Step 2: Grant Schema-Only Permissions

```sql
-- Grant only the permissions needed on the CheckList schema
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[CheckList] TO [CheckListAppUser];
GRANT EXECUTE ON SCHEMA::[CheckList] TO [CheckListAppUser];

-- Optional: Grant permission to use sequences if used
GRANT UPDATE ON SCHEMA::[CheckList] TO [CheckListAppUser];

-- CRITICAL: Do NOT grant permissions on dbo or other schemas
-- Do NOT grant db_owner or db_datawriter (too broad)
```

### Step 3: Use the Restricted User in Connection Strings

**For SQL Authentication:**
```
Server=myserver.database.windows.net;Database=CheckListDatabase;User Id=CheckListAppUser;Password=<secure-password>;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;
```

**For Managed Identity (Entra ID):**
```
Server=myserver.database.windows.net;Database=CheckListDatabase;Authentication=Active Directory Default;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;
```

Then assign the Managed Identity (service principal) to a database role with the same `[CheckList]` schema permissions.

### Step 4: Test the Connection

After deployment, verify the restricted user can only see CheckList data:

```sql
-- Connect as CheckListAppUser and run:
SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'CheckList';

-- You should see only CheckList tables. If you see dbo tables, permissions are too broad.
```

---

## Risk Analysis

### Risk 1: Identity Collisions — **LOW SEVERITY**

**What it is:**  
IDENTITY columns (auto-increment integers) in SQL Server are per-table, not per-schema. Two tables in different schemas can each have an IDENTITY(1,1) column without collision.

**Why it's not a problem for CheckList:**  
- Each table owns its IDENTITY independently
- No checksum or global uniqueness constraint on IDENTITY values
- Schema isolation doesn't introduce collision risk

**Mitigation:**  
No action needed. Continue using IDENTITY(1,1) for primary keys.

---

### Risk 2: Cross-Schema References — **MEDIUM SEVERITY**

**What it is:**  
If another app (e.g., a reporting service) needs to query CheckList data, it must use a cross-schema query with fully-qualified names like `SELECT * FROM [CheckList].[CheckList]`.

**Why it matters:**  
- The querying app needs explicit permissions on the CheckList schema
- Direct table references are fragile (brittle to schema renames)
- Each cross-app dependency is a manual permission grant

**Mitigation:**  
1. **Prefer API access** over direct SQL queries when possible
   - Expose a REST or GraphQL endpoint for external access to CheckList data
   - Gives you control over data contract and query performance

2. **Use views or stored procedures** for stable external interfaces
   - Create a view in the CheckList schema that exports the needed data
   - Grant SELECT on the view, not on base tables
   - Example: `CREATE VIEW [CheckList].[PublicActiveLists] AS SELECT [CheckListId], [Name] FROM [CheckList].[CheckList] WHERE ...`

3. **Document the cross-schema dependency**
   - If another app must access CheckList tables directly, add a note in the data dictionary
   - Include the required permissions grant

---

### Risk 3: Backup and Restore Granularity — **HIGH SEVERITY**

**What it is:**  
SQL Server backups are at the **database level**, not the schema level. A backup of `CheckListDatabase` includes all schemas in that database.

**Why it matters:**  
- If you restore a backup, you restore ALL apps' data in that database, not just CheckList
- Restoring a backup from 6 hours ago rolls back every app 6 hours
- One app's data corruption could force a full-database restore, affecting other apps

**Mitigation:**  
1. **Use point-in-time restore carefully**
   - Azure SQL Database supports point-in-time restore, but it restores the entire database
   - Coordinate with other app owners before initiating a restore
   - Document the restore window and affected apps

2. **Implement schema-aware backup export for CheckList data**
   - Write a PowerShell or T-SQL script that exports CheckList schema to a file
   - Run nightly; store in a separate blob storage account
   - Allows schema-only recovery without affecting other apps
   - Example: `SELECT * INTO [Backup_CheckList_Snapshot] FROM [CheckList].[CheckList]` (in a separate database)

3. **Monitor backup retention and costs**
   - Every app in the shared database increases backup volume
   - Align backup retention policies with compliance requirements

---

### Risk 4: Resource Contention — **MEDIUM SEVERITY**

**What it is:**  
The CheckList app and other apps in the shared database compete for:
- **DTUs/vCores** (compute capacity)
- **tempdb** (temporary workspace for sorts, hashing)
- **Transaction log** (write throughput)
- **Connection pool** (max concurrent connections)

**Why it matters:**  
- One app's heavy query (e.g., scanning millions of rows) consumes shared resources
- Other apps experience slowdowns or timeouts
- No automatic resource isolation; it's "best effort" sharing

**Mitigation:**  
1. **Monitor DTU usage and query performance**
   - Set up Azure Monitor alerts for DTU > 80%
   - Query `sys.dm_db_resource_stats` to identify expensive queries
   - Log slow queries (add `OPTION (RECOMPILE)` or adjust indexes as needed)

2. **Use Resource Governor (Enterprise Edition only)**
   - If the SQL Server is Enterprise, create resource pools per app
   - Example:
     ```sql
     CREATE RESOURCE POOL CheckListPool WITH (MAX_CPU_PERCENT=30, MAX_MEMORY_PERCENT=25);
     CREATE WORKLOAD GROUP CheckListWorkgroup USING CheckListPool;
     CREATE CLASSIFIER_FUNCTION CheckListClassifier() RETURNS sysname 
     AS BEGIN RETURN 'CheckListWorkgroup' END;
     ```
   - Less critical for small apps; mainly useful for predictable SLAs

3. **Design for efficient queries**
   - Add indexes on frequently-queried columns
   - Use pagination for large result sets (avoid SELECT * without LIMIT)
   - Profile queries in development; don't deploy unknowns

4. **Right-size the database tier**
   - General Purpose or Business Critical tier?
   - Standard or Premium?
   - Start conservative; scale up if contention becomes a problem

---

### Risk 5: Schema Migration Coordination — **HIGH SEVERITY**

**What it is:**  
Deploying schema changes (e.g., adding a column) involves publishing a DACPAC to the database. If multiple apps' deployments overlap, one app's publish can interfere with another's.

**Why it matters:**  
- DACPAC publish scans the entire database, not just the target schema
- A conflict during publish can leave the database in an inconsistent state
- Different apps on different release schedules create deployment windows

**Mitigation:**  
1. **Test DACPAC publish in a staging database first**
   - Replicate the shared database structure with multiple schemas
   - Test your DACPAC publish against the replica
   - Verify that no other app's tables were modified
   - Example: Staging DB has `[dbo]`, `[CheckList]`, `[ReportingApp]` schemas; test publish targets only `[CheckList]`

2. **Use schema-scoped compare options in DACPAC**
   - In the Database Project properties, ensure the DACPAC source and target schemas are correctly specified
   - SQL Data Tools can filter compare to a specific schema
   - Reduces risk of accidental changes to other schemas

3. **Coordinate deployments with other app teams**
   - If you know another app is deploying, wait for their publish to complete
   - Use a deployment calendar or Slack channel for visibility
   - Most critical for schema DDL changes (ALTER TABLE, CREATE PROCEDURE)

4. **Automate DACPAC validation in CI/CD**
   - Before publishing to production, validate that the DACPAC only touches the CheckList schema
   - Script: Compare the DACPAC's object list against the expected schema
   - Fail the build if unexpected schemas are included

---

### Risk 6: Connection Pooling Limits — **MEDIUM SEVERITY**

**What it is:**  
Each application maintains a connection pool (default: min 0, max 100 in .NET). With multiple apps and possibly multiple replicas, connection limits can be exceeded.

**Why it matters:**  
- Connection pooling is per connection string
- Different apps use different connection strings (different users, if following best practices)
- Each connection string gets its own pool
- Azure SQL Database has a per-database connection limit (e.g., 30,000 for Standard)

**Mitigation:**  
1. **Monitor connection pool usage**
   - Query `sys.dm_exec_sessions` to see current connection count
   - Alert if connections approach the database limit
   - Example: `SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE database_id = DB_ID();`

2. **Configure Max Pool Size appropriately**
   - Set `Max Pool Size=100` (or lower if you have many apps) in connection string
   - Balance between responsiveness (higher = more resources held) and throughput
   - Example: `Connection String; Max Pool Size=50;`

3. **Monitor and reuse connections**
   - Entity Framework Core connection pooling is automatic; verify it's working
   - Avoid opening/closing connections in tight loops
   - Use `using` statements to ensure connections return to the pool

---

### Risk 7: Deployment Permissions — **MEDIUM SEVERITY**

**What it is:**  
The deployment principal (the user/service account running the DACPAC publish) needs `ALTER SCHEMA` permission on the CheckList schema. This is a powerful permission.

**Why it matters:**  
- `ALTER SCHEMA` allows renaming, dropping, or modifying schema objects
- The deploying principal could accidentally drop all CheckList tables
- Overly broad deployment principals are a security risk

**Mitigation:**  
1. **Use a dedicated deployment principal**
   - Create a separate SQL login for CI/CD deployments
   - Do NOT use the application user (CheckListAppUser) for deployments
   - Do NOT use `sa` or database owner account
   - Example: `[CheckListDeployBot]`

2. **Grant scoped permissions**
   ```sql
   -- Create the deployment user
   CREATE USER [CheckListDeployBot] FOR LOGIN [CheckListDeployBot];

   -- Grant only the permissions needed for DACPAC publish
   GRANT ALTER ON SCHEMA::[CheckList] TO [CheckListDeployBot];
   GRANT CREATE TABLE ON DATABASE::[CheckListDatabase] TO [CheckListDeployBot];
   GRANT CREATE PROCEDURE ON DATABASE::[CheckListDatabase] TO [CheckListDeployBot];
   GRANT CREATE VIEW ON DATABASE::[CheckListDatabase] TO [CheckListDeployBot];
   ```

3. **Revoke permissions after deployment**
   - If the deployment principal is temporary (e.g., a managed identity with time-limited access), revoke permissions once the deployment succeeds
   - This limits the window of exposure

4. **Audit deployment activity**
   - Enable SQL Auditing on the server or database
   - Log all DDL changes (CREATE, ALTER, DROP)
   - Review audit logs monthly to catch unauthorized changes

---

## What Didn't Change

### Entity Class Files
Entity classes (e.g., `CheckList.cs`, `CheckSetAction.cs`) have **no schema references**. The schema is specified in the EF Core DbContext, not in the entities themselves.

**Why:** Keeps entity classes simple and portable. Schema is an infrastructure concern, not a business model concern.

### Repository Files
Repository implementations (e.g., `CheckListRepository.cs`) are **unchanged**. They use EF Core DbContext methods, and the schema is invisible to the repository.

**Why:** EF Core's fluent API handles schema mapping. Repositories don't need to know about schemas.

### Connection Strings
Connection strings **do not include schema information**. They specify the database name and credentials, but schema isolation is enforced via SQL permissions.

**Why:** SQL Server doesn't have a "schema parameter" in connection strings. Schema is a database-level concept enforced at the permission level.

### Test Infrastructure
The in-memory test database (used by MSTest) **ignores schema**. InMemoryDatabase doesn't enforce schema boundaries; it's a simple in-memory data store.

**Why:** For unit tests, schema isolation isn't the concern. We test business logic, not database infrastructure. Integration tests run against a real database and respect schema permissions.

---

## DACPAC Deployment Notes

### Pre-Deployment Schema Creation

The DACPAC includes a pre-deployment script that creates the `[CheckList]` schema if it doesn't exist:

```sql
-- Pre.Deployment.sql
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'CheckList')
BEGIN
    EXEC sp_executesql N'CREATE SCHEMA [CheckList];';
END
```

This ensures the first deployment to a new database succeeds without manual intervention.

### Schema Object Definition

All CheckList objects (tables, stored procedures, views) are defined in the `[CheckList]` schema. The DACPAC model includes:

```xml
<!-- Example: CheckList.Schema.sql -->
<Element Type="SqlSchema" Name="[CheckList]" />
```

When the DACPAC is published, SQL Server creates all dependent objects within this schema.

### First Deployment to a Shared Database

When deploying the CheckList DACPAC to a shared database for the first time:

1. **Deployment principal must have CREATE SCHEMA permission** on the database
   ```sql
   GRANT CREATE SCHEMA ON DATABASE::[CheckListDatabase] TO [CheckListDeployBot];
   ```

2. **Subsequent deployments** only need `ALTER SCHEMA` permission (schema already exists)

3. **Test the deploy** in a staging environment first to catch any conflicts

---

## Operational Checkpoints

### Pre-Deployment Validation

Before deploying to production:

- [ ] DACPAC publish tested in staging with all other app schemas present
- [ ] Deployment principal has correct permissions (ALTER SCHEMA on CheckList, not broader)
- [ ] No hardcoded references to `[dbo]` in application code or SQL scripts
- [ ] Connection string uses the restricted application user (CheckListAppUser), not sa or owner
- [ ] Backup retention policy is aligned with compliance requirements

### Post-Deployment Verification

After deploying to production:

- [ ] Verify CheckList tables are present: `SELECT * FROM [CheckList].[CheckList]` succeeds
- [ ] Verify schema isolation: `SELECT * FROM [dbo].[SomeTable]` returns an error for restricted user
- [ ] Application can read/write data normally
- [ ] Monitor DTU usage for 24 hours; alert if trending upward

### Ongoing Monitoring

- **Monthly:** Query slow query logs; optimize queries with high execution count
- **Quarterly:** Review connection pool usage; adjust Max Pool Size if needed
- **Yearly:** Audit deployment permissions; revoke unused principals

---

## Troubleshooting

### Symptom: "Login failed for user 'CheckListAppUser'"

**Diagnosis:** The user doesn't have SELECT permission on any tables.

**Fix:**  
```sql
-- Verify the user exists in the database
SELECT * FROM sys.database_principals WHERE name = 'CheckListAppUser';

-- Grant permissions on the CheckList schema
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::[CheckList] TO [CheckListAppUser];
```

### Symptom: "Invalid object name '[dbo].[CheckList]'"

**Diagnosis:** Application or script is trying to reference CheckList tables in the `[dbo]` schema, but they're in `[CheckList]`.

**Fix:**  
- Update application code to remove hardcoded `[dbo]` references (shouldn't have any if using EF Core)
- Update any T-SQL scripts to use fully-qualified names: `[CheckList].[TableName]`

### Symptom: "DACPAC publish failed: cannot drop object in use"

**Diagnosis:** Application is connected to the database during deployment, preventing DDL changes.

**Fix:**  
- Ensure application is shut down or connection pool is closed before publishing
- Some Azure App Service deployments require app slot swap to avoid active connections

### Symptom: "Application sees tables from other schemas"

**Diagnosis:** Deployment principal or application user has granted permissions on other schemas.

**Fix:**  
- Review user permissions: `EXEC sp_helprolemember`
- Revoke overly broad permissions: `REVOKE SELECT ON SCHEMA::[dbo] FROM [CheckListAppUser];`
- Verify user is not a member of `db_datareader` or `db_datawriter` role

---

## References

- [Microsoft: CREATE SCHEMA (Transact-SQL)](https://learn.microsoft.com/en-us/sql/t-sql/statements/create-schema-transact-sql)
- [Microsoft: GRANT Schema Permissions (Transact-SQL)](https://learn.microsoft.com/en-us/sql/t-sql/statements/grant-transact-sql)
- [Microsoft: Entity Framework Core Schema Separation](https://learn.microsoft.com/en-us/ef/core/modeling/relational/schemas)
- [DACPAC Publishing Best Practices](https://learn.microsoft.com/en-us/sql/ssdt/how-to-use-microsoft-sql-server-data-tools)

---

**Document Status:** Approved for operational use  
**Last Updated:** 2026-04-22  
**Next Review:** 2026-07-22
