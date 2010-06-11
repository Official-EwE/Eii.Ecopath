#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Network Analysis data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface INetworkAnalysisData

        ReadOnly Property Ascendancy() As Single(,)

    End Interface

End Namespace
