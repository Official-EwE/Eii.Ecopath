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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace advection data.
''' </summary>
Public Class cEcospaceLayerAdvection
    Inherits cEcospaceLayerSingle

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the advection layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iIndex As Integer)

        MyBase.New(theCore, manager, "", eVarNameFlags.LayerAdvection, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerAdvection

    End Sub

#End Region ' Construction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_ADVECTION,
                                     cSystemUtils.IIF(Me.Index = 1, My.Resources.CoreDefaults.CORE_DEFAULT_X_VELOCITY, My.Resources.CoreDefaults.CORE_DEFAULT_Y_VELOCITY))
    End Function

#End Region ' Overrides

End Class
