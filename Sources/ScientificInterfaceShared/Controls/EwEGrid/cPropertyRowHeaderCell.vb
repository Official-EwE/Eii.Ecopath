' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real



Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Cell class to implement a row header in an <see cref="cEwEGrid">EWE grid</see>, 
    ''' that dynamically derives its <see cref="Cell.DisplayText">display text</see>
    ''' from the core.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class inherits from <see cref="cPropertyHeaderCell">PropertyHeaderCell</see> 
    ''' to implement basic, standardized formatting for row header cells. The
    ''' display text of the cell is tracked 'live' using <see cref="cProperty">properties</see>.</para>
    ''' <para>Additionally, the cell offers capabilities to incorporate units
    ''' that are updated whenever the system display units change.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------

    Public Class cPropertyRowHeaderCell
        Inherits cPropertyHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As cEwEGridVisualizerBase

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see>.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(prop As cProperty)
            MyBase.New(prop)
            Me.VisualModel = New cEwEGridRowHeaderVisualizer
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that derives its 
        ''' <see cref="DisplayText">display text</see> from a 
        ''' <see cref="cProperty">cProperty</see>. The property value is 
        ''' inserted in the cell display text via a format mask.
        ''' </summary>
        ''' <param name="prop">cProperty to deliver the cell value.</param>
        ''' <param name="strUnit">The format mask to apply. This mask must
        ''' contain a '{0}' field where the property value is to be inserted.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(prop As cProperty,
                       strUnit As String)
            Me.New(prop)
            Me.SetUnits(strUnit)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor to create a row header cell that synchronizes 
        ''' its <see cref="DisplayText">display text</see> live with core data.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> 
        ''' object to deliver the core data.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">variable</see> 
        ''' of the <paramref name="Source">Source</paramref> to display in the cell.</param>
        ''' <param name="SourceSec">An optional secundary index in the 
        ''' <paramref name="VarName">variable</paramref>, or 
        ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when this variable
        ''' does not require an index.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(pm As cPropertyManager,
                       Source As cCoreInputOutputBase,
                       VarName As eVarNameFlags,
                       Optional SourceSec As cCoreInputOutputBase = Nothing,
                       Optional strUnit As String = "")
            Me.New(pm.GetProperty(Source, VarName, SourceSec), strUnit)
        End Sub

#End Region ' Construction 

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Row headers always use <see cref="cStyleGuide.eStyleFlags.Names">Names</see> style.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Style As cStyleGuide.eStyleFlags
            Get
                Dim s As cStyleGuide.eStyleFlags = MyBase.Style Or cStyleGuide.eStyleFlags.Names
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

    End Class

End Namespace
