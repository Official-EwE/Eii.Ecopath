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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwECore

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="cCommand">Command</see> to invoke the ecospace data connections
    ''' interface.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEcospaceExternalDataCommand
        Inherits cCommand

        Private m_layer As cEcospaceLayer = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared cCOMMAND_NAME As String = "~ecospaceexternaldata"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, cCOMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(Optional ByVal layer As cEcospaceLayer = Nothing)
            Me.m_layer = layer
            MyBase.Invoke()
            Me.m_layer = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cEcospaceLayer"/> this command was invoked for,
        ''' if any.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Layer() As cEcospaceLayer
            Get
                Return Me.m_layer
            End Get
        End Property

    End Class

End Namespace
