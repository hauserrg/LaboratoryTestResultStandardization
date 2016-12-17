--0: table = 1Extract
--1: loinc = e.g. loinc_code

select distinct {1} as Loinc
from {0}
where {1} is not null