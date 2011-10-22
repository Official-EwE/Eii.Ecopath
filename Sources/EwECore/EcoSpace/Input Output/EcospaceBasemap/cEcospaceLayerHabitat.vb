#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace habitat data.
''' </summary>
Public Class cEcospaceLayerHabitat
    Inherits cEcospaceLayerSingle

    Private m_iHabitat As Integer = 0

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iHabitat As Integer)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_HABITAT, EwEUtils.Core.eVarNameFlags.LayerHabitat, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerHabitat
        Me.m_iHabitat = iHabitat
    End Sub

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return d(iRow, iCol, Me.m_iHabitat) Else Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
            Dim s As Single = Convert.ToSingle(value)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol, Me.m_iHabitat) = s
                    Me.Invalidate()
                End If
            End If
        End Set
    End Property


End Class
