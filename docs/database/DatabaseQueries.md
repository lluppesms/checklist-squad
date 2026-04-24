# Database Queries

## Count Rows in Tables

``` sql
select 'TemplateSet' as TableName, count(*) as Rows from Checklist.TemplateSet
UNION
select 'TemplateList' as TableName, count(*) as Rows from Checklist.TemplateList
UNION
select 'TemplateCategory' as TableName, count(*) as Rows from Checklist.TemplateCategory
UNION
select 'TemplateAction' as TableName, count(*) as Rows from Checklist.TemplateAction
UNION
select 'CheckSet' as TableName, count(*) as Rows from Checklist.CheckSet
UNION
select 'CheckList' as TableName, count(*) as Rows from Checklist.CheckList
UNION
select 'CheckCategory' as TableName, count(*) as Rows from Checklist.CheckCategory
UNION
select 'CheckAction' as TableName, count(*) as Rows from Checklist.CheckAction
UNION
select 'CheckSet' as TableName, count(*) as Rows from Checklist.CheckSet
UNION
select 'CheckSetShare' as TableName, count(*) as Rows from Checklist.CheckSetShare
UNION
select 'AppUser' as TableName, count(*) as Rows from Checklist.AppUser
UNION
select 'UserPartnership' as TableName, count(*) as Rows from Checklist.UserPartnership
```
