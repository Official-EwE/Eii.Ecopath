' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace advection forcing data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerAdvectionForcing
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerAdvectionForcing, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerAdvectionForcing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overriden to include the advection layer name. Indexes are one-based
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function DefaultName() As String
        Select Case Me.Index
            Case 1 : Return My.Resources.CoreDefaults.CORE_DEFAULT_X_VELOCITY
            Case 2 : Return My.Resources.CoreDefaults.CORE_DEFAULT_Y_VELOCITY
            Case 3 : Return My.Resources.CoreDefaults.CORE_DEFAULT_UPWELLING
            Case Else
                Debug.Assert(False)
        End Select
        Return "?"
    End Function

End Class
