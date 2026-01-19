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
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.VisualModels



Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwEColumnHeader implements a column header with EwE style
    ''' </summary>
    ''' -----------------------------------------------------------------------
    
    Public Class cEwEColumnHeaderCell
        Inherits cEwEHeaderCell

        Private m_vizDefault As IVisualModel = Nothing

#Region " Construction / destruction "

        Public Sub New(Optional strValue As String = "")
            MyBase.New(strValue)
            Me.VisualModel = New cEwEGridColumnHeaderVisualizer()
            Me.m_vizDefault = Me.VisualModel
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub New(strValue As String, strUnits As String)
            Me.New(strValue)
            Me.SetUnits(strUnits)
        End Sub

        ''' <summary>
        ''' Create a header cell with automated name and units.
        ''' </summary>
        ''' <param name="varname"></param>
        Public Sub New(varname As eVarNameFlags)
            Me.New(varname, eDescriptorTypes.Name)
        End Sub

        Public Sub New(varname As eVarNameFlags, detail As eDescriptorTypes, Optional bShowUnits As Boolean = True)
            Me.New(New cVarnameTypeFormatter().ToString(varname, detail) & "|" & New cVarnameTypeFormatter().ToString(varname, eDescriptorTypes.Description))
            If (bShowUnits) Then
                Dim md As cVariableMetaData = cVariableMetaData.Get(varname)
                If (md IsNot Nothing) Then
                    Me.SetUnits(md.Units)
                End If
            End If
        End Sub

        Public Overrides Sub Dispose()
            Me.VisualModel = Me.m_vizDefault
            MyBase.Dispose()
        End Sub

#End Region ' Construction / destruction

        Public Overrides Property Style As cStyleGuide.eStyleFlags
            Get
                Dim s As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable Or MyBase.Style)
                Dim sel As Selection = Me.Grid.Selection
                If sel IsNot Nothing Then
                    If sel.ContainsColumn(Me.Column) Then
                        s = s Or cStyleGuide.eStyleFlags.Checked
                    End If
                End If
                Return s
            End Get
            Set(value As cStyleGuide.eStyleFlags)
                MyBase.Style = value
            End Set
        End Property

    End Class

End Namespace
