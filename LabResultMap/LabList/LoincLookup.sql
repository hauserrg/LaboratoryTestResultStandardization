SELECT     u.LOINC_NUM AS UserLoinc, l.COMPONENT, l.SCALE_TYP, l.SHORTNAME
FROM         UserLoincs AS u LEFT OUTER JOIN
                      Loinc AS l ON u.LOINC_NUM = l.LOINC_NUM