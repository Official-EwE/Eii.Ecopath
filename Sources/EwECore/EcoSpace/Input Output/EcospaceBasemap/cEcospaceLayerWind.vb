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
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace vector data.
''' </summary>
Public Class cEcospaceLayerWind
    Inherits cEcospaceLayerVelocity

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the wind layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)

        MyBase.New(theCore, manager, "", eVarNameFlags.LayerWind)
        Me.m_dataType = eDataTypes.EcospaceLayerWind
        Me.m_ccSecundaryIndex = eCoreCounterTypes.nMonths
        Me.m_coreComponent = eCoreComponentType.Ecospace

    End Sub

#End Region ' Construction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_WIND)
    End Function

#End Region ' Overrides

End Class
