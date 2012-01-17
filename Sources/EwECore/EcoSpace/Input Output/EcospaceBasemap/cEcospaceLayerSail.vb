#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports DefaultRes = EwECore.My.Resources.CoreDefaults

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace sailing cost data.
''' </summary>
Public Class cEcospaceLayerSail
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, _
                   String.Format(DefaultRes.CORE_DEFAULT_SAILCOST, iIndex), _
                   EwEUtils.Core.eVarNameFlags.LayerSail, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerSail
    End Sub

#Region " Cell interaction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of a sailing cost cell.
    ''' </summary>
    ''' <param name="iRow">Row index of the cell to access.</param>
    ''' <param name="iCol">Column index of the cell to access.</param>
    ''' <remarks>
    ''' Note that cells will be accessed for the currently selected fleet index.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            Return data(Me.Index, iRow, iCol)
        End Get
        Set(ByVal value As Object)
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            data(Me.Index, iRow, iCol) = CSng(value)
        End Set
    End Property

#End Region ' Cell interaction

End Class
