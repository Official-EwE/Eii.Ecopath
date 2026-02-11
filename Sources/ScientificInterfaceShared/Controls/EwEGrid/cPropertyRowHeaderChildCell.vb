' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Properties

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderChildCell implements a PropertyRowHeaderCell rendered 
    ''' as a child cell in a hierarchy.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class cPropertyRowHeaderChildCell
        Inherits cPropertyRowHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwEChildRowHeader()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(pm As cPropertyManager,
                       Source As cCoreInputOutputBase,
                       VarName As eVarNameFlags,
                       Optional SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(pm.GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
        End Sub

#End Region ' Construction 

    End Class

End Namespace
