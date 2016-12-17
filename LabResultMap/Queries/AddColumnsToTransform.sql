--select * from [{0}]
--select * from [{1}]

alter table [{0}] add N int
alter table [{0}] add N_Total int
alter table [{0}] add Pct float

/************  N  *********************************/
select t.Loinc, t.Result, count(1) Num
into #N
from [{0}] t
left join [{1}] l
	on t.Loinc = l.{2} and t.Result = l.{3}	
group by t.Loinc, t.Result		


update [{0}]
set N = Num
from #N
where #N.Loinc = [{0}].Loinc 
and #N.Result = [{0}].Result


/************  N_Total  *********************************/
select t.Loinc, count(1) Num
into #N_Total
from [{0}] t
left join [{1}] l
	on t.Loinc = l.{2} and t.Result = l.{3}	
group by t.Loinc


update [{0}]
set N_Total = Num
from #N_Total
where #N_Total.Loinc = [{0}].Loinc 


/************  Pct  *********************************/
update [{0}]
set Pct = round(N / cast(N_Total as float) * 100 , 2)
