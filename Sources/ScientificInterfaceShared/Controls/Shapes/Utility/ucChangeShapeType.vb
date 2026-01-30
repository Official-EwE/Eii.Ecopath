' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports EwECore.Common
Imports ScientificInterfaceShared.Style



' ToDo: Document this class

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control to change and parameterize the <see cref="IShapeFunction">
    ''' shape function</see> that defines a shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucChangeShapeType
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_shape As cForcingFunction = Nothing
        Private m_bInUpdate As Boolean = False

#End Region ' Private vars 

#Region " Construction / destruction "

        Public Sub New()
            Me.InitializeComponent()
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso Me.components IsNot Nothing Then
                    RemoveHandler Me.m_grid.OnShapeFunctionChanged, AddressOf Me.OnShapeParametersChanged
                    Me.UIContext = Nothing
                    Me.components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            AddHandler Me.m_grid.OnShapeFunctionChanged, AddressOf Me.OnShapeParametersChanged
        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Event OnShapeFunctionChanged()

        Public Property UIContext As cUIContext Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                Me.m_uic = value
                Me.m_grid.UIContext = value
            End Set
        End Property

        Public Property Shape As cForcingFunction
            Get
                Return Me.m_shape
            End Get
            Set(value As cForcingFunction)
                Me.m_shape = value
                Me.Init()
            End Set
        End Property

        Public Property SelectedShapeFunction As IShapeFunction
            Get
                Return DirectCast(Me.m_lbShapeFunctionTypes.SelectedItem, IShapeFunction)
            End Get
            Private Set(template As IShapeFunction)
                Me.m_lbShapeFunctionTypes.SelectedItem = template
                Me.m_grid.ShapeFunction = template
            End Set
        End Property

        Public Sub Defaults()
            Dim template As IShapeFunction = Me.SelectedShapeFunction()
            If (template Is Nothing) Then Return
            template.Defaults()
            Me.m_grid.ShapeFunction = template
        End Sub

#End Region ' Public access

#Region " Event handlers "

        Private Sub OnFormatShapeFunction(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbShapeFunctionTypes.Format
            Dim fmt As New cShapeFunctionFormatter()
            e.Value = fmt.ToString(e.ListItem)
        End Sub

        Private Sub OnShapeFunctionTypeSelected(sender As System.Object, e As System.EventArgs) _
            Handles m_lbShapeFunctionTypes.SelectedIndexChanged

            If (Me.m_bInUpdate) Then Return

            Me.SelectedShapeFunction = DirectCast(Me.m_lbShapeFunctionTypes.SelectedItem, IShapeFunction)
            Try
                RaiseEvent OnShapeFunctionChanged()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnShapeParametersChanged()
            Try
                RaiseEvent OnShapeFunctionChanged()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub Init()

            Me.m_bInUpdate = True

            Me.m_lbShapeFunctionTypes.Items.Clear()

            If (Me.m_shape IsNot Nothing) Then

                Dim selection As IShapeFunction = Nothing
                For Each sft As IShapeFunction In cShapeFunctionFactory.GetShapeFunctions(Me.m_shape, Me.m_uic.Core.PluginManager)
                    Me.m_lbShapeFunctionTypes.Items.Add(sft)
                    If (sft.ShapeFunctionType = Me.m_shape.ShapeFunctionType) Then
                        selection = sft
                    End If

                Next
                Me.SelectedShapeFunction = selection
            End If

            Me.m_bInUpdate = False
            Me.UpdateControls()

        End Sub

        Private Sub UpdateControls()
            Me.m_lbShapeFunctionTypes.Enabled = (Me.Shape IsNot Nothing)
            Me.m_grid.Enabled = (Me.Shape IsNot Nothing)
        End Sub

#End Region ' Internals

    End Class

End Namespace
