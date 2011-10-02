#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace habitat data.
''' </summary>
Public Class cEcospaceLayerHabitat
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_HABITAT, EwEUtils.Core.eVarNameFlags.LayerHabitat, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerHabitat
    End Sub

End Class
