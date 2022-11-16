with adresses as (
    select ca.CustomerID
        ,a.CountryRegion
    from {{ source ('adventureworks', 'customeraddress') }} ca
    left join {{ source ('adventureworks', 'address') }} a ON ca.AddressId = a.AddressId
    where ca.AddressType = "Main Office"
)
select 
    c.CustomerID
    ,Title
    ,FirstName
    ,MiddleName
    ,LastName
    ,Suffix
    ,CompanyName
    ,EmailAddress
    ,Phone
    ,rc.CountryCode as MainCountryCode
from {{ source ('adventureworks', 'customer') }} c
left join adresses a on c.CustomerID = a.CustomerID
left join {{ ref ('ref_country') }} rc ON rc.CountryRegion = a.CountryRegion