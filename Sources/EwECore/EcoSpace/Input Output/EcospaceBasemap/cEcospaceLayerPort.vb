' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace port data.
''' </summary>
Public Class cEcospaceLayerPort
    Inherits cEcospaceLayerBoolean

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerPort, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerPort
    End Sub

#Region " Cell interaction "

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Dim data As Boolean()(,) = DirectCast(Me.Data, Boolean()(,))
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    If data(iFleet)(iRow, iCol) Then Return True
                Next
                Return False
            Else
                Return data(Me.Index)(iRow, iCol)
            End If
        End Get
        Set(value As Object)
            Dim data As Boolean()(,) = DirectCast(Me.Data, Boolean()(,))
            ' ToDo: only allow coastal cells to be set
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    data(iFleet)(iRow, iCol) = CBool(value)
                Next
            Else
                data(Me.Index)(iRow, iCol) = CBool(value)
            End If
        End Set
    End Property

#End Region ' Cell interaction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        If (Me.Index = 0) Then Return My.Resources.CoreDefaults.CORE_ALL_FLEETS
        Return Me.m_core.EcopathFleetInputs(Me.Index).Name
    End Function

#End Region ' Overrides

End Class
