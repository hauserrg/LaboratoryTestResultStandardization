if object_id('{1}') is not null begin drop table {1} end;

with simpleResults as (
select * 
from (
select {2}, Result, count(1) [Count]
from {0}
group by {2}, Result  
) as resGroup
pivot 
(
max(Count)
for [Result] in (Pos, Neg, Other, Error)
) as piv
)

select {2}, isnull(Pos,0) [Pos], isnull(Neg,0) [Neg], isnull(Other,0) [Other], isnull(Error,0) [Error]
	, (isnull(Pos,0)+isnull(Neg,0)) [Total_PosNeg]
	, case 
		when isnull(Pos,0) + isnull(Neg,0) = 0 then 0
		else round(isnull(Pos,0)/(isnull(Pos,0)+cast(isnull(Neg,0) as float))*100,1) 
		end as [PosPct]
into {1}
from simpleResults
order by (isnull(Pos,0)+isnull(Neg,0)+isnull(Other,0)+isnull(Error,0)) desc;