' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' HACK WARNING: This is a place holder ONLY used for the IBM Age 1 forcing
''' There is no IBM biomass layer in the core 
''' We need a placeholder so the IBM forcing will look like other Ecospace Basemap layers, even though it's not.
''' If you need this to work as a cEcospaceBasemap 
''' you will need to construct an array in the core that holds Multi stanza biomass 
''' </summary>
Public Class cEcospaceLayerIBMAge1Forcing
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerIBMAge1Forcing, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerIBMAge1Forcing
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return d(iRow, iCol, Me.Index)
            Catch ex As Exception

            End Try
            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Dim s As Single = Convert.ToSingle(value)
                d(iRow, iCol, Me.Index) = s
                Me.Invalidate()
            Catch ex As Exception

            End Try
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overriden to include the group name into this layer's name
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function DefaultName() As String
        '  Dim iSt As Integer = m_core.getStanzaIndexForGroup(Me.Index)
        Return Me.m_core.StanzaGroups(Me.Index - 1).Name
        ' Return Me.m_core.EcoPathGroupInputs(iSt).Name
    End Function

End Class

