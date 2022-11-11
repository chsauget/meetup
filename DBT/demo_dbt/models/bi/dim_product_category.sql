select 
    pc.ProductCategoryID
    ,pc.Name as ProductCategoryName
    ,parent.ProductCategoryID as ParentProductCategoryID
    ,parent.Name as ParentProductCategoryName
from {{ source ('adventureworks', 'productcategory') }} pc
left join {{ source ('adventureworks', 'productcategory') }} parent on pc.ParentProductCategoryID = pc.ProductCategoryID