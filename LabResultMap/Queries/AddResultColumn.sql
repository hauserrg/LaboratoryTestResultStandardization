alter table [{0}]
add Result as
(
	case
		when General is not null 			
			then 'Other'
		when {1} is not null  --Abnormal				
			then 'Pos'
		when {1} is null
			then 'Neg'
		else 'Error' end
)