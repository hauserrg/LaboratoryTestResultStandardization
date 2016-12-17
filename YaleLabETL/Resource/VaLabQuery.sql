SELECT Loinc.LOINC, Loinc.ScaleType, plc.Sta3n, plc.PatientSID, plc.LabChemSpecimenDateTime, plc.RequestingStaffSID, plc.RequestingLocationSID, plc.Units, plc.Abnormal, plc.RefHigh, plc.RefLow, plc.LabChemSID, Loinc.LOINCSID, plc.LabChemResultValue, plc.LabChemResultNumericValue, plc.LabChemCompleteDateTime 
into [PCS_LABMed].[Dflt].[{0}]  
FROM [PCS_LABMed].[Dflt].ETL_LabList LabList 
     JOIN [RDWWork].Dim.LOINC Loinc 
         ON LabList.Loinc = Loinc.LOINC 
     JOIN [RDWWork].Chem.PatientLabChem plc 
         ON plc.LOINCSID=Loinc.LOINCSID 
WHERE LabChemCompleteDateTime > '2013-01-01'
 and LabChemCompleteDateTime < '2014-01-01'