' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a EwERowHeaderCell to implement row headers. 
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class cEwERowHeaderCell
        Inherits cEwEHeaderCell

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell with an optional static value.
        ''' </summary>
        ''' <param name="strValue">The value to set.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(Optional strValue As String = "")
            MyBase.New(strValue)
            ' Set visualizer
            Me.VisualModel = New cEwEGridRowHeaderVisualizer()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell displaying a single unit.
        ''' </summary>
        ''' <param name="strUnitMask">The mask should contain ONE {0} placeholder where
        ''' the <paramref name="strUnit">unit</paramref> will be displayed.</param>
        ''' <param name="strUnit">The unit to dynamically substitute in the cell display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(strUnitMask As String, strUnit As String)
            Me.New(strUnitMask)
            Me.SetUnits(strUnit)
        End Sub

        Public Sub New(varname As eVarNameFlags)
            Me.New(New cVarnameTypeFormatter().ToString(varname, eDescriptorTypes.Name))
        End Sub

        Public Sub New(varname As eVarNameFlags, strUnitMask As String, strUnit As String)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE,
                                 New cVarnameTypeFormatter().ToString(varname, eDescriptorTypes.Name),
                                 strUnitMask), strUnit)
        End Sub

        Public Overrides Property Style As cStyleGuide.eStyleFlags
            Get
                Dim s As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable Or MyBase.Style)
                Dim sel As Selection = Me.Grid.Selection
                If sel IsNot Nothing Then
                    If sel.ContainsRow(Me.Row) Then
                        s = s Or cStyleGuide.eStyleFlags.Checked
                    End If
                End If
                Return s
            End Get
            Set(value As cStyleGuide.eStyleFlags)
                MyBase.Style = value
            End Set
        End Property

#End Region ' Construction 

    End Class

End Namespace
