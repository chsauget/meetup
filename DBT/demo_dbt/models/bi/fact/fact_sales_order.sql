{{
    config(
        materialized='incremental',
        unique_key='SalesOrderDetailID',
        on_schema_change='fail'
    )
}}

with headers as (
	select * from {{ source ('adventureworks', 'salesorderheader') }}
), details as (
	select * from {{ source ('adventureworks', 'salesorderdetail') }}
)

select
	o.SalesOrderID
	,cast(headers.OrderDate as date) as OrderDate
	,cast(headers.DueDate as date) as DueDate
	,cast(headers.ShipDate as date) as ShipDate
	,headers.RevisionNumber
	,headers.Status
	,headers.CustomerID
    ,sp.ShippingMethodID
	,headers.ShipToAddressID
	,headers.BillToAddressID
	,details.SalesOrderDetailID
	,details.OrderQty
	,details.ProductID
	,details.UnitPrice
	,details.LineTotal
	,details.ModifiedDate
from headers
left join details on headers.SalesOrderID = details.SalesOrderID
left join {{ ref ('dim_order') }} o ON o.SalesOrderID = headers.SalesOrderID
left join {{ ref ('dim_shipping_method') }} sp on sp.ShippingMethod = headers.ShipMethod
left join {{ ref ('dim_customer') }} c on c.CustomerID = headers.CustomerID

{% if is_incremental() %}
  -- this filter will only be applied on an incremental run
  where details.ModifiedDate > (select max(ModifiedDate) from {{ this }})
{% endif %}