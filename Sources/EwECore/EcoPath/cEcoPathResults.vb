''' <summary>
''' WARNING
''' Due to a change is structure this class is NOT used as is
''' but could be changed to return only summary data from the EcoPath model
''' 
''' Class to encapsulate the results of an EcoPath parameter estimation
''' this class 
''' </summary>
''' <remarks></remarks>
Public Class cEcoPathResults

    Public B() As Double            'Biomass
    Public BH() As Double           'Biomass per habitat area
    Public BA() As Double           'Biomass accumulation
    Public PB() As Double           'Production/biomass
    Public QB() As Double           'Consumption/biomass
    Public EE() As Double           'Ecotrophic efficiency
    Public GE() As Single

    'summary stats
    Public RTZ As Single 'sum of respiration
    Public Consum As Single
    Public SumBio As Single
    Public CatchSum As Single 'sum of catch
    Public GEff As Single 'gross efficiency
    Public Totpp As Single
    Public TLcatch As Single
    Public Dt As Single 'total flow of detritus
    Public SumEx As Single 'sum of exports
    Public SumP As Single 'Sum of all production
    Public Conn As Single 'Connectance Index
    Public SysOm As Single
    Public PProd As Single

    Public Function RedimVariables(ByVal NumGroups As Integer) As Boolean

        ReDim B(NumGroups)
        ReDim BH(NumGroups)    'habitat biomass
        ReDim BA(NumGroups)
        ReDim PB(NumGroups)
        ReDim EE(NumGroups)
        ReDim GE(NumGroups)
        ReDim QB(NumGroups)
        ReDim PB(NumGroups)


    End Function

End Class
