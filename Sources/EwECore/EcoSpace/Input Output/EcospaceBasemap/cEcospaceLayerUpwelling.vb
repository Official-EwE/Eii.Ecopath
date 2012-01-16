#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace upwelling data.
''' </summary>
Public Class cEcospaceLayerUpwelling
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_UPWELLING, EwEUtils.Core.eVarNameFlags.LayerUpwelling, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerUpwelling
    End Sub

End Class
