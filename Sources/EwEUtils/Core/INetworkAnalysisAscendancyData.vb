'==============================================================================
'
' $Log: INetworkAnalysisAscendancyData.vb,v $
' Revision 1.1  2009/01/21 19:25:00  jeroens
' Initial version
'
'
'==============================================================================

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Network Analysis data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface INetworkAnalysisData

        ReadOnly Property Ascendancy() As Single(,)

    End Interface

End Namespace
