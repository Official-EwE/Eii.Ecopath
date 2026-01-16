' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore.Style



''' <summary>
''' Layer providing access to Ecospace Primarey Production data.
''' </summary>
Public Class cEcospaceLayerRelPP
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerRelPP, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerRelPP
    End Sub

    Protected Overrides Function DefaultName() As String
        Dim vnf As New cVarnameTypeFormatter()
        Return vnf.ToString(eVarNameFlags.LayerRelPP)
    End Function

End Class
