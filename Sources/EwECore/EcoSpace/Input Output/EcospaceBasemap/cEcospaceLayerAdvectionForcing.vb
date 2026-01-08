' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

#End Region ' Imports

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
