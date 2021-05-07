' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Public Class cPeriodSorter
    Implements IComparer(Of cPeriod)

    Public Function Compare(x As cPeriod, y As cPeriod) As Integer Implements IComparer(Of cPeriod).Compare
        If x.EndYear < y.StartYear Then Return -1
        If x.StartYear > y.EndYear Then Return 1
        If x.StartYear < y.StartYear Then Return -1
        If x.StartYear = y.StartYear Then Return 0
        Return 1
    End Function

End Class
