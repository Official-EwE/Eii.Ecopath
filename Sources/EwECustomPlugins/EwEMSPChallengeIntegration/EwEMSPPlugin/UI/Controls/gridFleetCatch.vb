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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEMSPPlugin.Emulator
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fleet catch configuration grid.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.cEwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridFleetCatch
        Inherits cEwEGrid

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridFleets">test set configuration</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub FillData()

        End Sub
    End Class

End Namespace
