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

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style
Imports System.Drawing.Drawing2D
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' Interface to set the contour of a given shape to a 'common' primitive
    ''' </summary>
    ''' <remarks>
    ''' This code is loosely based on frmShaper.vb in EwE5.
    ''' </remarks>
    Public Class dlgChangeShape

#Region " Private vars "

        ''' <summary></summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary></summary>
        Private m_shape As cForcingFunction = Nothing
        ''' <summary></summary>
        Private m_handler As cShapeGUIHandler = Nothing
        ''' <summary>Format providers to handle user input.</summary>
        Private m_fps As New List(Of cEwEFormatProvider)

        Private Const cMAX_PARAM As Integer = 5

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, ByVal shape As cForcingFunction)

            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

            Me.InitializeComponent()

            ' Sanity checks
            Debug.Assert(uic IsNot Nothing)
            Debug.Assert(shape IsNot Nothing)

            ' Init
            Me.m_uic = uic
            Me.m_shape = shape
            Me.m_handler = cShapeGUIHandler.GetShapeUIHandler(shape)

        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Add shape name 
            Me.m_tbxName.Text = Me.m_shape.Name

            ' Show available options
            For Each sft As IShapeFunction In cShapeFunctionFactory.GetShapeFunctions(Me.m_shape, Me.m_uic.Core.PluginManager)
                Me.m_lbShapeFunctionTypes.Items.Add(sft)

                ' This selection logic will have to change when plug-in provided shape function types become available
                If (TypeOf sft Is cShapeFunction) Then
                    If (DirectCast(sft, cShapeFunction).ShapeFunctionType = Me.m_shape.ShapeFunctionType) Then
                        Me.SelectedShapeFunction = sft
                    End If
                End If
            Next

            ' Initialize shape function
            Me.m_lbShapeFunctionTypes.SelectedIndex = Me.GetShapeTypeIndex(Me.m_shape.ShapeFunctionType)

            Me.UpdatePreview()
            Me.UpdateControls()
            Me.CenterToScreen()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.SelectedShapeFunction = Nothing
            MyBase.OnFormClosed(e)

        End Sub

        Private Sub OnDefaults(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btDefaults.Click

            Dim fs As IShapeFunction = Me.SelectedShapeFunction()
            If (fs Is Nothing) Then Return

            fs.Defaults()
            For i As Integer = 0 To Math.Min(cMAX_PARAM, fs.nParameters) - 1
                Me.m_fps(i).Value = fs.ParamValue(i + 1)
            Next

            Me.UpdatePreview()

        End Sub

        Private Sub OnOk(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOk.Click

            Dim fs As IShapeFunction = Me.SelectedShapeFunction()
            If (fs Is Nothing) Then Return

            Me.m_shape.Name = Me.m_tbxName.Text
            fs.Apply(Me.m_shape)

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnFormatShapeFunction(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbShapeFunctionTypes.Format
            Dim fmt As New cShapeFunctionFormatter()
            e.Value = fmt.GetDescriptor(e.ListItem)
        End Sub

        Private Sub OnShapeFunctionTypeSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbShapeFunctionTypes.SelectedIndexChanged

            Me.SelectedShapeFunction = DirectCast(Me.m_lbShapeFunctionTypes.SelectedItem, IShapeFunction)
        End Sub

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim fs As IShapeFunction = Me.SelectedShapeFunction()
            If (fs Is Nothing) Then Return

            For i As Integer = 0 To Me.m_fps.Count - 1
                fs.ParamValue(i + 1) = CSng(Me.m_fps(i).Value)
            Next
            Me.UpdatePreview()

        End Sub

        Private Sub OnNameChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_tbxName.TextChanged

            Me.UpdateControls()

        End Sub

        Private Sub OnPaintPreview(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_plPreview.Paint

            Try

                Dim fs As IShapeFunction = Me.SelectedShapeFunction()
                If (fs Is Nothing) Then Return

                Dim sDataMax As Single = 0.0
                Dim g As Graphics = e.Graphics
                Dim rc As Rectangle = Me.m_plPreview.ClientRectangle
                Dim data As Single() = fs.Shape(Me.nDataPoints)
                Dim iNumPoints As Integer = data.Length - 1

                For Each s As Single In data
                    sDataMax = Math.Max(s, sDataMax)
                Next

                Using br As New SolidBrush(Me.m_plPreview.BackColor)
                    g.FillRectangle(br, rc)
                End Using

                cShapeImage.DrawShapeDirect(Me.m_uic, _
                                            data, Me.nDisplayPoints, Me.m_shape.IsSeasonal, _
                                            Me.m_plPreview.ClientRectangle, e.Graphics, Me.m_handler.Color, _
                                            Me.m_handler.SketchDrawMode, _
                                            sDataMax / 0.8!, cCore.NULL_VALUE, cCore.NULL_VALUE)

                Using br As New HatchBrush(HatchStyle.SmallConfetti, Color.FromArgb(100, 0, 0, 0), Color.Transparent)
                    Dim x As Integer = CInt(Math.Ceiling(rc.Width * Me.nDataPoints / Me.nDisplayPoints))
                    g.FillRectangle(br, New Rectangle(x, 0, rc.Width, rc.Height))
                End Using

            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnRefreshShape(sender As System.Object, e As System.EventArgs) _
            Handles m_btnRefresh.Click
            Me.UpdatePreview()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdateControls()

            Dim bHasName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxName.Text)
            Me.m_btnOk.Enabled = bHasName

        End Sub

        Private Property SelectedShapeFunction As IShapeFunction
            Get
                Return DirectCast(Me.m_lbShapeFunctionTypes.SelectedItem, IShapeFunction)
            End Get
            Set(value As IShapeFunction)
                'If (Object.ReferenceEquals(Me.SelectedShapeFunction, value)) Then Return

                For Each fp As cEwEFormatProvider In Me.m_fps
                    RemoveHandler fp.OnValueChanged, AddressOf OnValueChanged
                    fp.Release()
                Next

                Me.m_fps.Clear()

                If (value IsNot Nothing) Then

                    ' ToDo: Make max no params a flexible number
                    For i As Integer = 1 To cMAX_PARAM

                        Dim lblName As Control = Nothing
                        Dim lblUnit As Control = Nothing
                        Dim tbxValue As Control = Nothing
                        Dim fp As cEwEFormatProvider = Nothing

                        Select Case i
                            Case 1 : lblName = Me.m_lblA : tbxValue = Me.m_tbxA : lblUnit = Me.m_lblAUnit
                            Case 2 : lblName = Me.m_lblB : tbxValue = Me.m_tbxB : lblUnit = Me.m_lblBUnit
                            Case 3 : lblName = Me.m_lblC : tbxValue = Me.m_tbxC : lblUnit = Me.m_lblCUnit
                            Case 4 : lblName = Me.m_lblD : tbxValue = Me.m_tbxD : lblUnit = Me.m_lblDUnit
                            Case 5 : lblName = Me.m_lblE : tbxValue = Me.m_tbxE : lblUnit = Me.m_lblEUnit
                        End Select

                        If (i <= value.nParameters) Then
                            ' Configure labels
                            lblName.Visible = True
                            lblName.Text = cStyleGuide.ToControlLabel(value.ParamName(i))
                            lblUnit.Visible = True
                            lblUnit.Text = value.ParamUnit(i)
                            ' Configure textbox and format provider
                            tbxValue.Visible = True

                            fp = New cEwEFormatProvider(Me.m_uic, tbxValue, GetType(Single))
                            fp.Tag = i
                            fp.Value = value.ParamValue(i)
                            AddHandler fp.OnValueChanged, AddressOf OnValueChanged

                            Me.m_fps.Add(fp)
                        Else
                            lblName.Visible = False
                            lblUnit.Visible = False
                            tbxValue.Visible = False
                        End If
                    Next

                End If

                Me.UpdatePreview()

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Redraw the shape
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdatePreview()
            Me.m_plPreview.Invalidate()
        End Sub

        Private Function nDisplayPoints() As Integer
            If Me.m_shape.IsSeasonal Then Return cCore.N_MONTHS
            Return Me.m_shape.ShapeData.Length - 1
        End Function

        Private Function nDataPoints() As Integer
            If Me.m_shape.IsSeasonal Then Return cCore.N_MONTHS
            If Me.m_shape.DataType = EwEUtils.Core.eDataTypes.Forcing Then
                Return Me.m_uic.Core.nEcosimYears * cCore.N_MONTHS
            End If
            Return Me.m_shape.nPoints
        End Function

        Private Function GetShapeTypeIndex(shapeType As Long) As Integer

            ' JS 3dec14: do not rely on for each to return items in a known order
            'For Each sft As Object In Me.m_lbShapeFunctionTypes.Items
            For iShp As Integer = 0 To Me.m_lbShapeFunctionTypes.Items.Count - 1
                Dim sft As IShapeFunction = DirectCast(Me.m_lbShapeFunctionTypes.Items(iShp), IShapeFunction)
                If (sft.ShapeFunctionType = shapeType) Then
                    Return iShp
                End If
            Next
            Return 0

        End Function

#End Region ' Internals

    End Class

End Namespace

