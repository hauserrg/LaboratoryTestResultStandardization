if object_id('{2}') is not null begin drop table {2} end;

select  e.*,  m.MappedYN, m.Number, m.Inequality, m.AfterDecimal, m.Field1, m.General
into {2}
from [{0}] e
left join [{1}] m
	on m.Result = e.{3}
	and m.Loinc = e.{4}
