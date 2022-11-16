{{ config(
    tags=['dimension']
) }}

with raw as (
    select distinct 
        SalesOrderID
        ,SalesOrderNumber
        ,PurchaseOrderNumber
        ,AccountNumber
    from {{ source ('adventureworks', 'salesorderheader') }}
)
SELECT 
    SalesOrderID
    ,SalesOrderNumber
    ,PurchaseOrderNumber
    ,AccountNumber
FROM raw