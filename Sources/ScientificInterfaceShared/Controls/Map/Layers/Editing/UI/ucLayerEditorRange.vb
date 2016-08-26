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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Layer editor interface for editing a layer that may contain a range of values.
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorRange

        Private m_fpValue As cEwEFormatProvider = Nothing

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            Try
                If bDisposing Then
                    If (Me.UIContext Is Nothing) Then Return

                    RemoveHandler Me.m_fpValue.OnValueChanged, AddressOf OnValueChanged
                    RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                    Me.m_fpValue.Release()

                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(bDisposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Overrides "

        Public Overrides Sub Initialize(ByVal editor As cLayerEditor)
            MyBase.Initialize(editor)

            Dim edt As cLayerEditor = Me.Editor
            Dim prop As cProperty = edt.Layer.GetProperty()

            If (edt.Layer.GetProperty() IsNot Nothing) Then
                Me.m_fpValue = New cPropertyFormatProvider(Me.UIContext, Me.m_nudValue, prop)
            Else
                ' Try to obtain editor metadata from layer
                Dim md As cVariableMetaData = Nothing

                If (edt.Layer.Data IsNot Nothing) Then
                    If (edt.Layer.Data.MetadataCell IsNot Nothing) Then
                        md = edt.Layer.Data.MetadataCell
                    End If
                End If

                ' No default metadata found?
                If (md Is Nothing) Then
                    ' #Yes: create metadata from layer editor settings
                    md = New cVariableMetaData(Convert.ToDecimal(Math.Max(-100000, Me.Editor.CellValueMin)),
                                            Convert.ToDecimal(Math.Min(100000, Me.Editor.CellValueMax)),
                                            cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo),
                                            cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
                End If
                Me.m_fpValue = New cEwEFormatProvider(Me.UIContext, Me.m_nudValue, edt.Layer.ValueType, md)

                ' Config numerical precision
                If edt.Layer.ValueType Is GetType(Integer) Then
                    Me.m_nudValue.DecimalPlaces = 0
                Else
                    Me.m_nudValue.DecimalPlaces = Me.UIContext.StyleGuide.NumDigits
                End If

                ' Config increment
                If (md.Max - md.Min) <= 1000 Then
                    Me.m_nudValue.Increment = CDec((md.Max - md.Min) / 100)
                End If

            End If

            AddHandler Me.m_fpValue.OnValueChanged, AddressOf OnValueChanged
            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity check
            If (Me.m_nudValue Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            ' Set control value
            Dim sValue As Single = Math.Max(Math.Min(CSng(Me.Editor.CellValue), CSng(Me.m_nudValue.Maximum)), _
                                            CSng(Me.m_nudValue.Minimum))

            Me.m_nudValue.Value = Convert.ToDecimal(sValue)
            Me.m_btnSmooth.Enabled = Me.Editor.CanSmooth

            Dim bmp As New Bitmap(Me.m_pbPreview.Width, Me.m_pbPreview.Height, Imaging.PixelFormat.Format32bppArgb)
            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim renderer As cRasterLayerRenderer = Nothing

            If Me.Layer IsNot Nothing Then
                renderer = DirectCast(Me.Layer.Renderer, cRasterLayerRenderer)
            End If

            If (renderer IsNot Nothing) Then
                renderer.RenderCell(g, New Rectangle(0, 0, bmp.Width, bmp.Height), _
                                    Me.Layer.Data, sValue, _
                                    cStyleGuide.eStyleFlags.Highlight)
            End If
            Me.m_pbPreview.Image = bmp

            g.Dispose()

            Dim bEditable As Boolean = editor.IsEditable
            Me.m_nudValue.Enabled = bEditable
            Me.m_btnReset.Enabled = bEditable
            Me.m_btnSmooth.Enabled = bEditable
            Me.m_lbValue.Enabled = bEditable

        End Sub

        Public Overrides Sub StartEdit(editor As cLayerEditor)
            ' Freeze renderer min/max values 
            Me.Layer.Renderer.ScaleMax = Me.Layer.Data.MaxValue
            Me.Layer.Renderer.ScaleMin = Me.Layer.Data.MinValue
            MyBase.StartEdit(editor)
        End Sub

        Public Overrides Sub EndEdit(editor As cLayerEditor)
            ' Release renderer min/max values
            Me.Layer.Renderer.ScaleMax = cCore.NULL_VALUE
            Me.Layer.Renderer.ScaleMin = cCore.NULL_VALUE
            MyBase.EndEdit(editor)
        End Sub

#End Region ' Overrides

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.UpdateContent(Me.Editor)

        End Sub

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.Editor.CellValue = Me.m_fpValue.Value
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
            If ((cf And cStyleGuide.eChangeType.NumberFormatting) > 0) Then
                Me.UpdateContent(Me.Editor)
            End If
        End Sub

        Private Sub OnSmooth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSmooth.Click
            Me.Editor.Smooth()
        End Sub

        Private Sub OnResetLayer(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnReset.Click
            Me.Editor.Reset()
        End Sub

        Private Sub OnClickPreview(sender As System.Object, e As System.EventArgs) _
            Handles m_pbPreview.Click
            Me.EditLayer(eLayerEditTypes.EditVisuals)
        End Sub

#End Region ' Events

#Region " Internals "

        Protected Sub EditLayer(ByVal edittype As eLayerEditTypes)
            Try
                Dim cmd As cEditLayerCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cEditLayerCommand.cCOMMAND_NAME), cEditLayerCommand)
                cmd.Invoke(Me.Layer, Nothing, edittype)
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Internals

    End Class

End Namespace
