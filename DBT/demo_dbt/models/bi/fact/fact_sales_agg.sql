{% set colors = ["Red", "Blue", "Black"] %}

select 
	date_format(OrderDate, 'yyyy-MM') as month_id,
    {% for color in colors %}
    sum(case when p.Color = '{{color}}' then LineTotal end) as {{color}}_Total,
    {% endfor %}
    sum(LineTotal) as Total
from {{ ref ('fact_sales_order') }} so
left join {{ ref ('dim_product') }} p ON so.ProductID = p.ProductID
group by date_format(OrderDate, 'yyyy-MM')