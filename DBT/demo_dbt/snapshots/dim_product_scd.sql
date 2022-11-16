{% snapshot dim_product_scd %}

    {{
        config(
          target_schema="dbt_mro",
          strategy='check',
          unique_key='ProductID',
          check_cols=['Color', 'Size', 'Weight', 'SellStartDate', 'SellEndDate'],
        )
    }}

    select 
        p.ProductID
        ,p.Name
        ,p.ProductNumber
        ,p.Color
        ,p.Size
        ,p.Weight
        ,cast(p.SellStartDate as date) as SellStartDate
        ,cast(p.SellEndDate as date) as SellEndDate
        ,pm.Name as ModelName
        ,pd.Description as ModelDescriptionEN
        ,p.ProductCategoryID
    from {{ source ('adventureworks', 'product') }} p
    left join {{ source ('adventureworks', 'productmodel') }} pm on p.ProductModelID = pm.ProductModelID
    left join {{ source ('adventureworks', 'productmodelproductdescription') }} pmpd on 
        pm.ProductModelID = pmpd.ProductModelID
        and pmpd.Culture = 'en'
    left join {{ source ('adventureworks', 'productdescription') }} pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID
{% endsnapshot %}