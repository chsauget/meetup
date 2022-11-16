-- Remove seed table
DROP TABLE IF EXISTS hive_metastore.dbt_mro.ref_country

-- Remove 
DROP TABLE IF EXISTS hive_metastore.dbt_mro.dim_product_scd

-- Update source data to trigger snapshot update
UPDATE hive_metastore.adventureworks.product
SET Color = 'Blue'
WHERE Color = 'Red'