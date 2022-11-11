select 
    CustomerID
    ,Title
    ,FirstName
    ,MiddleName
    ,LastName
    ,Suffix
    ,CompanyName
    ,EmailAddress
    ,Phone
from {{ source ('adventureworks', 'customer') }}