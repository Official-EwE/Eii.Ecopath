' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace habitat data.
''' </summary>
Public Class cEcospaceLayerHabitat
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerHabitat, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerHabitat
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return data(Me.Index)(iRow, iCol) Else Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
            Dim s As Single = Convert.ToSingle(value)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    data(Me.Index)(iRow, iCol) = s
                    Me.Invalidate()
                End If
            End If
        End Set
    End Property

    Protected Overrides Function DefaultName() As String
        If (Me.Index = 0) Then Return My.Resources.CoreDefaults.CORE_DEFAULT_HABITAT_ALL
        Return Me.m_core.EcospaceHabitats(Me.Index).Name
    End Function

End Class
