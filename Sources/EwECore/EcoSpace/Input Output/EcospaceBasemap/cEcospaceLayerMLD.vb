#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace mixed layer depth data.
''' </summary>
Public Class cEcospaceLayerMLD
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_MIXEDLAYERDEPTH, _
                   EwEUtils.Core.eVarNameFlags.LayerMLD, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerMLD
    End Sub



End Class
