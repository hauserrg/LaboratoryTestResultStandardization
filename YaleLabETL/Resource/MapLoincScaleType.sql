update [Dflt].[{0}]
set ScaleType = 
	(
		case 
			when ScaleType = '* ' then null
			when ScaleType = 'SET' then null
			when ScaleType = 'Nominal' then 'Nom'
			when ScaleType = 'Narrative' then 'Nar'
			when ScaleType = '*Unknown at this time*' then null
			when ScaleType = 'Ordinal' then 'Ord'
			when ScaleType = 'NULL' then null
			when ScaleType = 'Document' then 'Doc'
			when ScaleType = 'Multi' then 'Multi'
			when ScaleType = 'Quantitative' then 'Qn'
			when ScaleType = '*Missing*' then null
			when ScaleType = 'Quantitative Or Ordinal' then 'OrdQn'
			else null end 
	)
