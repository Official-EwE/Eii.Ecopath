#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports DefaultRes = EwECore.My.Resources.CoreDefaults

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace habitat capacity data.
''' </summary>
Public Class cEcospaceLayerHabitatCapacity
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal dt As eDataTypes, ByVal vn As eVarNameFlags, iIndex As Integer)
        MyBase.New(theCore, manager, _
                   String.Format(CStr(IIf(vn = eVarNameFlags.LayerHabitatCapacity, DefaultRes.CORE_DEFAULT_HABCAP, DefaultRes.CORE_DEFAULT_HABCAP_INPUT)), iIndex), _
                   vn, iIndex)
        Me.m_dataType = dt
    End Sub

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return data(iRow, iCol, Me.Index)
            Return 0
        End Get
        Set(ByVal value As Object)
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then data(iRow, iCol, Me.Index) = CSng(value)
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

#End Region ' Cell interaction

End Class
