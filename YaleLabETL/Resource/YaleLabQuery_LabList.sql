select o.PAT_ENC_CSN_ID
	, o.order_proc_id
	, o.proc_code
	, o.display_name
	, r.line
	, r.component_id
	, cc.loinc_code
	, loincScale.Name LoincScale
	, cc.EXTERNAL_NAME
	, opi.PARENT_ORDER_ID
	, r.ord_value
	, r.ord_num_value
	, r.REFERENCE_UNIT
	, r.REFERENCE_LOW
	, r.REFERENCE_HIGH
	, r.result_in_range_yn
	, zrf.Name result_flag_c
	, zrs.Name result_status_c	
	, zrdt.Name data_type_c
	, o.ORDER_INST
	, o.INSTANTIATED_TIME
	, o2.specimn_taken_time
	, r.result_time
	, llb.LLB_NAME
	, zoc.Name order_class_c
	, zop.Name order_priority_c
	, o.session_key
into [Reds3_Dev].[dbo].[{0}]
from [EPICCLARSQLP].[CLARITY].[DBO].[order_proc] o 
left join [EPICCLARSQLP].[CLARITY].[DBO].[ORDER_PARENT_INFO] opi on opi.order_id = o.order_proc_id
left join [EPICCLARSQLP].[CLARITY].[DBO].[order_results] r on r.order_proc_id = o.order_proc_id
left join [EPICCLARSQLP].[CLARITY].[DBO].[CLARITY_COMPONENT] cc on cc.component_id = r.component_id
left join [EPICCLARSQLP].[CLARITY].[DBO].[ORDER_PROC_2] o2 on o2.order_proc_id = o.order_proc_id
left join [EPICCLARSQLP].[CLARITY].[DBO].[CLARITY_LLB] llb on llb.resulting_lab_id = o.result_lab_id
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_ORDER_CLASS] zoc on zoc.order_class_c = o.order_class_c
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_ORDER_PRIORITY] zop on zop.order_priority_c = o.order_priority_c
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_RESULT_FLAG] zrf on zrf.result_flag_c = r.result_flag_c
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_RESULT_STATUS] zrs on zrs.result_status_c = r.result_status_c
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_RES_DATA_TYPE] zrdt on zrdt.res_data_type_c = r.data_type_c
left join [EPICCLARSQLP].[CLARITY].[DBO].[LNC_DB_MAIN] loinc on loinc.lnc_code = cc.loinc_code
left join [EPICCLARSQLP].[CLARITY].[DBO].[ZC_LNC_SCALE] loincScale on loincScale.LNC_SCALE_C = loinc.LNC_SCALE_C

where o.proc_code like 'LAB%'
and cc.loinc_code is not null --requires loinc, we know that many labs do not have a loinc code
and r.ord_value is not null	  --requires non-null lab result
and o.ORDER_INST >= '{2}' and o.ORDER_INST < '{3}'
and	o.order_proc_id != opi.parent_order_id	
and exists
(
	select *
	from [dbo].[{1}] ll --Proj_*_ETL_LabList
	where ll.Loinc = cc.loinc_code
)

