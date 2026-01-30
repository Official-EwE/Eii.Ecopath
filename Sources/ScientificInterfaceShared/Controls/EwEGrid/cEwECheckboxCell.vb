' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    Public Class cEwECheckboxCell
        Inherits SourceGrid2.Cells.Real.CheckBox
        Implements IEwECell
        Implements IDisposable

        ''' <summary>Default visualizer for EwECells</summary>
        Private Shared g_visualizer As New cEwECheckBoxVisualizer()
        ''' <summary>Behaviour model to catch [ENTER] key presses.</summary>
        Private m_bmCatchEnter As BehaviorModels.IBehaviorModel = Nothing
        Private m_uic As cUIContext = Nothing

        Public Sub New(bChecked As Boolean,
                       Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK)

            MyBase.New(bChecked)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            ' Set style
            Me.Style = style
        End Sub

        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            If Me.UIContext IsNot Nothing Then

                Me.UIContext = Nothing

                ' Remove all bahaviour models
                Me.Behaviors.Remove(Me.m_bmCatchEnter)
                Me.m_bmCatchEnter = Nothing

                ' Release any editors
                Me.DataModel.EnableEdit = False
                Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
                Me.DataModel = Nothing

            End If
            GC.SuppressFinalize(Me)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Custom cell style
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_style As cStyleGuide.eStyleFlags = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Allows to set a custom <see cref="cStyleGuide.eStyleFlags">style</see>,
        ''' triggering EwE colour feedback and EwE cell edit behaviour.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Style() As cStyleGuide.eStyleFlags _
            Implements IEwECell.Style

            Get
                Return Me.m_style
            End Get

            Set(s As cStyleGuide.eStyleFlags)
                Me.m_style = s
                If ((s And cStyleGuide.eStyleFlags.NotEditable) = 0) Then
                    Me.DataModel.EnableEdit = True
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.Default
                Else
                    Me.DataModel.EnableEdit = False
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
                End If
            End Set

        End Property

#Region " Updates (StyleGuide)"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' StyleGuide change event handler; makes sure cells are redrawn
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub OnStyleGuideChanged(changeType As cStyleGuide.eChangeType)
            Me.Invalidate()
        End Sub

#End Region ' Updated (StyleGuide)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connect a cell to a grid. Overridden to hook up to an existing 
        ''' <see cref="cUIContext">UI context</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnAddToGrid(e As System.EventArgs)
            MyBase.OnAddToGrid(e)
            If (TypeOf Me.Grid Is IUIElement) Then
                ' Grab UI context from parent grid
                Me.UIContext = DirectCast(Me.Grid, IUIElement).UIContext
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a cell from a grid. Overridden to disconnect from the current
        ''' <see cref="cUIContext">UI context</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnRemoveToGrid(e As System.EventArgs)
            MyBase.OnRemoveToGrid(e)
            Me.UIContext = Nothing
        End Sub

        Protected ReadOnly Property Core() As cCore _
            Implements IEwECell.Core
            Get
                If (Me.UIContext Is Nothing) Then Return Nothing
                Return Me.UIContext.Core
            End Get
        End Property

        Protected ReadOnly Property PropertyManager() As cPropertyManager _
            Implements IEwECell.PropertyManager
            Get
                If (Me.UIContext Is Nothing) Then Return Nothing
                Return Me.UIContext.PropertyManager
            End Get
        End Property

        Protected ReadOnly Property StyleGuide() As cStyleGuide _
            Implements IEwECell.StyleGuide
            Get
                If (Me.UIContext Is Nothing) Then Return Nothing
                Return Me.UIContext.StyleGuide
            End Get
        End Property

        Public Overridable Property UIContext() As cUIContext _
            Implements IEwECell.UIContext
            Get
                Return Me.m_uic
            End Get
            Protected Set(value As cUIContext)

                If (Me.m_uic IsNot Nothing) Then
                    ' Clean up
                    ' Release style guide event handler
                    RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If

                Me.m_uic = value

                If (Me.m_uic IsNot Nothing) Then
                    ' Release style guide event handler
                    AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If

            End Set

        End Property

    End Class

End Namespace
