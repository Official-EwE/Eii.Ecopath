#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace depth data.
''' </summary>
Public Class cEcospaceLayerDepth
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal meta As cVariableMetaData)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerDepth, cCore.NULL_VALUE, meta)
        Me.m_dataType = eDataTypes.EcospaceLayerDepth
    End Sub

    Public Function IsWaterCell(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        Return CInt(Me.Cell(iRow, iCol)) > 0
    End Function

    Public Function IsLandCell(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        Return CInt(Me.Cell(iRow, iCol)) <= 0
    End Function

End Class
