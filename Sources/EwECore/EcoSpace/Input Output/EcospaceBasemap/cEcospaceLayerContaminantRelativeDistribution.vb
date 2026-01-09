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




Imports EwECore.Style



''' <summary>
''' Layer providing access to Ecospace relative contaminants data.
''' </summary>
Public Class cEcospaceLayerContaminantRelativeDistribution
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerContaminantRelativeDistribution, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerContaminantRelativeDistribution
    End Sub

    Protected Overrides Function DefaultName() As String
        Dim vnf As New cVarnameTypeFormatter()
        Return vnf.ToString(eVarNameFlags.LayerContaminantRelativeDistribution) ' My.Resources.CoreDefaults.CORE_DEFAULT_RELCIN
    End Function

End Class
