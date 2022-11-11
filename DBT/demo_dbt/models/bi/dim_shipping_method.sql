with raw as (
    select distinct 
        ShipMethod
    from {{ source ('adventureworks', 'salesorderheader') }}
)
SELECT 
    row_number() over (partition by ShipMethod ORDER BY 1) as ShippingMethodID,
    ShipMethod as ShippingMethod
FROM raw