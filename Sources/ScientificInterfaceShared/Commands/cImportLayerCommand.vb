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
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Import Ecospace Layer Data' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cImportLayerCommand
        Inherits cCommand

        Private m_alayers() As cRasterLayer = Nothing

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~importLayer"

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cImportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.Invoke(Nothing)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="alayers">The layers to import data into.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal alayers() As cRasterLayer)
            Me.m_alayers = alayers
            MyBase.Invoke()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the layers the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Layers() As cRasterLayer()
            Get
                Return Me.m_alayers
            End Get
        End Property

    End Class

End Namespace ' Commands
