#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace relative contaminants data.
''' </summary>
Public Class cEcospaceLayerRelCin
    Inherits cEcospaceLayerSingleNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerRelCin, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerRelCin
    End Sub

End Class
