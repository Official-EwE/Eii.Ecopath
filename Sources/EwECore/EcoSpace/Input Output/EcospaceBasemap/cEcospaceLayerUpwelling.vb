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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace upwelling data.
''' </summary>
Public Class cEcospaceLayerUpwelling
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_UPWELLING, EwEUtils.Core.eVarNameFlags.LayerUpwelling, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerUpwelling
    End Sub

End Class
