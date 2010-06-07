#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports


Public Class cEcospaceLayerDistribution
    Inherits cEcospaceLayerIntegerNxM
    Implements ICoreGroupFilter

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerDistribution)
        Me.m_dataType = eDataTypes.EcospaceLayerDistribution
    End Sub

#Region " Cell interaction "

    Private m_iGroup As Integer

    ''' <summary>
    ''' Index of the currently selected group
    ''' </summary>
    Public Property Group() As Integer Implements EwEUtils.Core.ICoreGroupFilter.Group
        Get
            Return Me.m_iGroup
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(value > 0 And value <= Me.m_core.nGroups, Me.ToString & ".Group() Invalid group index!")
            If value > 0 And value <= Me.m_core.nGroups Then
                Me.m_iGroup = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Is this cell in the distribution envelope for the currently selected group
    ''' </summary>
    ''' <param name="iRow">Row index of the cell to access.</param>
    ''' <param name="iCol">Column index of the cell to access.</param>
    ''' <remarks>
    ''' Note that cells will be accessed for the currently selected 
    ''' <see cref="group">group index</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Try
                Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
                If Me.m_iGroup > 0 Or Me.m_iGroup <= Me.m_core.nGroups Then
                    'valid group index
                    Return CSng(IIf(data(Me.m_iGroup, iRow, iCol), 1.0!, 0.0!))
                Else
                    Return cCore.NULL_VALUE ' or 0.0f
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

        Set(ByVal value As Single)

            Try

                'bark if the group index has not been set correctly
                Debug.Assert(Me.m_iGroup > 0 And Me.m_iGroup <= Me.m_core.nGroups, Me.ToString & ".Group() Invalid group index!")

                Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
                If Me.m_iGroup > 0 Or Me.m_iGroup <= Me.m_core.nGroups Then
                    data(iRow, iCol, Me.m_iGroup) = (value <> 0.0!)
                End If

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Set
    End Property
#End Region ' Cell interaction


End Class
