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






Namespace Commands

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch a the interface to select which groups and fleets to
    ''' display in the UI.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cShowHideItemsCommand
        Inherits cCommand


        Public Shared COMMAND_NAME As String = "~showhideitems"



        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="cmdh">The <see cref="cCommandHandler"/> to associate this command with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cShowHideItemsCommand.COMMAND_NAME, My.Resources.COMMAND_DISPLAYGROUPS)
        End Sub

    End Class

End Namespace
