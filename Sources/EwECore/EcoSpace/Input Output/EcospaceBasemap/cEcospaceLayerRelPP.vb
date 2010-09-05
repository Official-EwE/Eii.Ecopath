#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace Primarey Production data.
''' </summary>
Public Class cEcospaceLayerRelPP
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerRelPP)
        Me.m_dataType = eDataTypes.EcospaceLayerRelPP
    End Sub

End Class
