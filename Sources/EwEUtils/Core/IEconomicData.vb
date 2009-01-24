'==============================================================================
'
' $Log: IEconomicData.vb,v $
' Revision 1.3  2009/01/24 17:46:16  joeb
' Added EmploymentValueByFleet and ProfitByFleet
'
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Employment value by fleet
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property EmploymentValueByFleet(ByVal FleetIndex As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Proifit by fleet
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property ProfitByFleet(ByVal FleetIndex As Integer) As Single

    End Interface

End Namespace ' Core
