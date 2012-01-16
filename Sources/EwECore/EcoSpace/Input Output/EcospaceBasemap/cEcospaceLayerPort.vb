#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace port data.
''' </summary>
Public Class cEcospaceLayerPort
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_PORT, EwEUtils.Core.eVarNameFlags.LayerPort, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerPort
    End Sub

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    If data(iFleet, iRow, iCol) Then Return 1.0!
                Next
                Return cCore.NULL_VALUE
            Else
                Return CSng(IIf(data(Me.Index, iRow, iCol), 1.0!, 0.0!))
            End If
        End Get
        Set(ByVal value As Object)
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            ' ToDo: only allow coastal cells to be set
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    data(iFleet, iRow, iCol) = (CSng(value) <> 0.0!)
                Next
            Else
                data(Me.Index, iRow, iCol) = (CSng(value) <> 0.0!)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0.0!
        End Get
    End Property

#End Region ' Cell interaction

End Class
