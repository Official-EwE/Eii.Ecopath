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

<HideModuleName()>
Public Module modDefinitions

    Public Enum eTrendType As Integer
        TimeSeries = 0
        Rule
    End Enum

    Public Enum eTargetType As Integer
        Region = 0
        MPA
        Habitat
    End Enum

    Public Enum eApplicationType As Integer
        Relative = 0
        Absolute
        Additive
    End Enum

    Public Enum eProtectionType As Integer
        Full = 0
        High
        Moderate
        Poor
        None
        '''' <summary>Let EwE decide the type of protection</summary>
        'Automatic
    End Enum

End Module
