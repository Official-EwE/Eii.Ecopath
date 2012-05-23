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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Storage class for system-wide, model independent EwE core settings.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class cCoreSettings

    ''' <summary>Autosave flags</summary>
    Public Autosave() As Boolean
    ''' <summary>Path for EwE core processes to write output information to.</summary>
    Public OutputPath As String = ""
    ''' <summary>Path for the core to write backup files to.</summary>
    Public BackupFileMask As String = ""

    ''' <summary>Default author name.</summary>
    Public Author As String = ""
    ''' <summary>Default author contact information.</summary>
    Public Contact As String = ""

    Public Sub New()
        ReDim Autosave([Enum].GetValues(GetType(eAutosaveTypes)).Length)
    End Sub

End Class
