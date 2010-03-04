#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace region data.
''' </summary>
Public Class cEcospaceLayerRegion
    Inherits cEcospaceLayerIntegerNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerRegion)
        Me.m_dataType = eDataTypes.EcospaceLayerRegion
    End Sub

End Class
