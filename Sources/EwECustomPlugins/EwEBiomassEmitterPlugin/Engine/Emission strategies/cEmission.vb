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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'






''' <summary>
''' Time series for a specific MPA. A time series can hold relative B data points
''' for a specific group and target area (MPA or region), or can determine local
''' biomass fluctuations from emprical rules.
''' </summary>
Public MustInherit Class cEmission

    Public Sub New(data As cData)
        Me.Data = data
    End Sub

    Public ReadOnly Property Data As cData = Nothing
    Public MustOverride Property Enable As Boolean
    Public MustOverride Function IsValid() As Boolean

End Class
