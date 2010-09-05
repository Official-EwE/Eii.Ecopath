#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace MPA seed data.
''' </summary>
Public Class cEcospaceLayerMPASeed
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerMPASeed, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerMPASeed
    End Sub

End Class