'==============================================================================
'
' $Log: IEconomicData.vb,v $
' Revision 1.2  2009/01/22 17:38:36  jeroens
' Added  2 main economic values
'
' Revision 1.1  2009/01/21 19:25:00  jeroens
' Initial version
'
'
'==============================================================================

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Economic data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IEconomicData

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the total value of ???
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property TotalValue() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the total value of employment.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property EmploymentValue() As Single

    End Interface

End Namespace ' Core
