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

' ********************************************************************************
' ********************************************************************************
'
' This dialog needs a major overhaul as follows:
' - All shape functions will be provided by individual shape function classes, one
'   for each type of function (sketched, linear, sigmoid, etc)
' - Each shape function instance with have its OWN buffer for shape points and 
'   configuration parameters.
' - The controls in this form will interact only with the selected shape function class;
'   when the selection changes the controls will be rerouted to work with this new 
'   selection.
' - The dialog must retain full OK / Cancel behaviour. This functionality is now 
'   partially destroyed. On Cancel, the original shape must be unaffected
' - IShapeFunctions classes will be available via the EwE Core, and can be obtained
'   from plug-in points as well.
' ********************************************************************************
' ********************************************************************************

Namespace Controls

    ''' <summary>
    ''' Interface to set the contour of a given shape to a 'common' primitive
    ''' </summary>
    ''' <remarks>
    ''' EwE5: frmShaper.vb
    ''' </remarks>
    Public Class dlgChangeShape

#Region "Private internal class definitions"

        ''' <summary>
        ''' Internal class to hold the shape values in a buffer. This class will be replaced by
        ''' IShapeFunction-derived instances for each shape type, which can then also be derived
        ''' from plug-in points.
        ''' </summary>
        ''' <remarks>
        ''' For now this is only used in the data validation. 
        ''' It could be extended so the interface always uses this as its data source.
        '''  </remarks>
        Private Class cShapeBuffer
            Public A As Single
            Public B As Single
            Public C As Single
            Public D As Single
            Public ShapeData As Single()
            Public ShapeType As eShapeFunctionType

            Public Sub New(ByVal shape As cForcingFunction)
                Me.A = shape.YZero
                Me.B = shape.YEnd
                Me.C = shape.YBase
                Me.D = shape.Steep
                Me.ShapeType = shape.ShapeFunctionType
                Me.ShapeData = shape.ShapeData
            End Sub

            Public Function Apply(shape As cForcingFunction) As Boolean

                shape.LockUpdates()

                shape.YZero = Me.A
                shape.YEnd = Me.B
                shape.YBase = Me.C
                shape.Steep = Me.D
                shape.ShapeData = Me.ShapeData

                shape.UnlockUpdates()

            End Function

            Public ReadOnly Property Max As Single
                Get
                    Dim sDataMax As Single = Single.MinValue
                    For Each s As Single In Me.ShapeData
                        sDataMax = Math.Max(s, sDataMax)
                    Next
                    Return sDataMax
                End Get
            End Property
        End Class

#End Region

#Region " Private vars "

        ''' <summary></summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary></summary>
        Private m_shape As cForcingFunction = Nothing
        ''' <summary></summary>
        Private m_handler As cShapeGUIHandler = Nothing

        ''' <summary></summary>
        Private m_fpC As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpB As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpA As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpD As cEwEFormatProvider = Nothing

        Private m_fpMax As cEwEFormatProvider = Nothing

        Private m_bRecalc As Boolean = True

        Private MAXIT As Integer = 100
        Private EPS As Single = 0.0000003
        Private FPMIN As Single = 1.0E-30

        Private m_shpBuff As cShapeBuffer = Nothing

        ' --------------- NEW STUFF -------------------
        Private m_fps As New List(Of cEwEFormatProvider)

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, ByVal shape As cForcingFunction, ByVal handler As cShapeGUIHandler)

            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

            Me.InitializeComponent()

            ' Sanity checks
            Debug.Assert(uic IsNot Nothing)
            Debug.Assert(shape IsNot Nothing)

            ' Init
            Me.m_uic = uic
            Me.m_shape = shape
            Me.m_handler = handler

            'Keep the shape in a buffer
            Me.m_shpBuff = New cShapeBuffer(Me.m_shape)

        End Sub


#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.CenterToParent()

            Me.m_tbxName.Text = Me.m_shape.Name

            'jb 24-May-11 removed data validation to fix ticket 975
            Me.m_fpA = New cEwEFormatProvider(Me.m_uic, Me.m_tbxA, GetType(Single))
            Me.m_fpA.Value = Me.m_shpBuff.A

            Me.m_fpB = New cEwEFormatProvider(Me.m_uic, Me.m_tbxB, GetType(Single))
            Me.m_fpB.Value = Me.m_shpBuff.B

            Me.m_fpC = New cEwEFormatProvider(Me.m_uic, Me.m_tbxC, GetType(Single))
            Me.m_fpC.Value = Me.m_shpBuff.C

            Me.m_fpD = New cEwEFormatProvider(Me.m_uic, Me.m_tbxD, GetType(Single))
            Me.m_fpD.Value = Me.m_shpBuff.D

            Me.m_fpMax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxMaxValue, GetType(Single))
            Me.m_fpMax.Value = Me.m_shpBuff.Max

            ' Show available options
            For Each sft As eShapeFunctionType In [Enum].GetValues(GetType(eShapeFunctionType))
                If Me.IsRelevantShapeType(sft) Then
                    Me.m_lbShapeFunctionTypes.Items.Add(sft)
                End If
            Next

            Me.SelectedShapeType = Me.m_shape.ShapeFunctionType

            Me.UpdatePreview()
            Me.UpdateControls()

            AddHandler Me.m_fpA.OnValueChanged, AddressOf OnValueChanged
            AddHandler Me.m_fpB.OnValueChanged, AddressOf OnValueChanged
            AddHandler Me.m_fpC.OnValueChanged, AddressOf OnValueChanged
            AddHandler Me.m_fpD.OnValueChanged, AddressOf OnValueChanged

            AddHandler Me.m_fpMax.OnValueChanged, AddressOf OnMaxValueChanged

        End Sub


        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            RemoveHandler Me.m_fpA.OnValueChanged, AddressOf OnValueChanged
            RemoveHandler Me.m_fpB.OnValueChanged, AddressOf OnValueChanged
            RemoveHandler Me.m_fpC.OnValueChanged, AddressOf OnValueChanged
            RemoveHandler Me.m_fpD.OnValueChanged, AddressOf OnValueChanged

            RemoveHandler Me.m_fpMax.OnValueChanged, AddressOf OnMaxValueChanged

            Me.m_fpD.Release()
            Me.m_fpC.Release()
            Me.m_fpB.Release()
            Me.m_fpA.Release()
            Me.m_fpMax.Release()

            MyBase.OnFormClosed(e)

        End Sub

        Private Sub OnDefaults(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btDefaults.Click

            Dim sA As Single = 0.0
            Dim sB As Single = 0.0
            Dim sC As Single = 0.0
            Dim sD As Single = 0.0

            Dim max As Single = Me.m_shape.YMax

            Select Case Me.SelectedShapeType
                Case eShapeFunctionType.NotSet
                    sA = Me.m_shape.YZero : sB = Me.m_shape.YEnd : sC = Me.m_shape.YBase : sD = Me.m_shape.Steep
                Case eShapeFunctionType.Linear
                    sA = 1.0 : sB = 1.0
                Case eShapeFunctionType.Exponential
                    sA = 1.0 : sB = 5.0 : sC = 0.2
                Case eShapeFunctionType.Hyperbolic
                    sA = 1.0 : sB = 3.0 : sC = 0.75
                Case eShapeFunctionType.Sigmoid
                    sA = 0.0 : sB = 2.0 : sC = 0.5 : sD = 3.0
                Case eShapeFunctionType.Betapdf
                    sA = 2.0F
                    sB = 3.0F
                Case eShapeFunctionType.Normal
                    sA = 1.0 : sB = 1.0 : sC = 10.0
                Case eShapeFunctionType.LeftShoulder
                    sA = 1.0 : sB = 2.0 : sC = 3.0
                Case eShapeFunctionType.RightShoulder
                    sA = 1.0 : sB = 2.0 : sC = 3.0

                Case eShapeFunctionType.Trapezoid
                    sA = 1.0 : sB = 2.0 : sC = 3.0 : sD = 4.0

            End Select

            Me.m_fpA.Value = sA
            Me.m_fpB.Value = sB
            Me.m_fpC.Value = sC
            Me.m_fpD.Value = sD

            Me.UpdatePreview()
            Me.UpdateControls()

        End Sub

        Private Sub OnOk(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOk.Click

            If Not Me.RecalcShape() Then
                ' MsgBox
                Return
            End If

            Me.m_shpBuff.Apply(Me.m_shape)
            Me.m_shape.Name = Me.m_tbxName.Text

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnCancel.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnNameChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_tbxName.TextChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnFormatShapeFunction(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbShapeFunctionTypes.Format
            Dim fmt As New cShapeFunctionTypeFormatter()
            e.Value = fmt.GetDescriptor(e.ListItem)
        End Sub

        Private Sub OnShapeFunctionTypeSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbShapeFunctionTypes.SelectedIndexChanged
            Me.UpdateControls()
            Me.UpdatePreview()
        End Sub

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

            Me.ValidateInput()
            Me.UpdateShape()
            Me.UpdatePreview()

        End Sub

        Private Sub OnMaxValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim scale As Single = CSng(Me.m_fpMax.Value)
            Me.ScaleShape(scale)
            Me.UpdatePreview()

        End Sub

        Private Sub OnPaintPreview(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_plPreview.Paint

            Try

                Dim sDataMax As Single = 0.0
                Dim g As Graphics = e.Graphics
                Dim rc As Rectangle = Me.m_plPreview.ClientRectangle
                Dim iNumPoints As Integer = Me.m_shape.ShapeData.Length

                If Me.m_bRecalc Then
                    Me.RecalcShape()
                    Me.m_bRecalc = False
                End If

                sDataMax = Me.m_shpBuff.Max

                Using br As New SolidBrush(Me.m_plPreview.BackColor)
                    g.FillRectangle(br, rc)
                End Using

                cShapeImage.DrawShapeDirect(Me.m_uic, _
                                            Me.m_shpBuff.ShapeData, Me.NumDisplayPoints, Me.m_shape.IsSeasonal, _
                                            Me.m_plPreview.ClientRectangle, e.Graphics, Me.m_handler.Color, _
                                            Me.m_handler.SketchDrawMode, _
                                            sDataMax / 0.8!, cCore.NULL_VALUE, cCore.NULL_VALUE)

                Using br As New HatchBrush(HatchStyle.SmallConfetti, Color.FromArgb(100, 0, 0, 0), Color.Transparent)
                    Dim x As Integer = CInt(Math.Ceiling(rc.Width * Me.NumDataPoints / Me.NumDisplayPoints))
                    g.FillRectangle(br, New Rectangle(x, 0, rc.Width, rc.Height))
                End Using

            Catch ex As Exception

            End Try

        End Sub

#End Region ' Events

#Region " Private method helpers "

        Private Property SelectedShapeFunction As IShapeFunction
            Get
                Return DirectCast(Me.m_lbShapeFunctionTypes.SelectedItem, IShapeFunction)
            End Get
            Set(value As IShapeFunction)
                If (Object.ReferenceEquals(Me.SelectedShapeType, value)) Then Return

                For Each fp As cEwEFormatProvider In Me.m_fps
                    RemoveHandler fp.OnValueChanged, AddressOf OnValueChanged
                    fp.Release()
                Next

                Me.m_fps.Clear()

                For i As Integer = 1 To 4 ' Will become flexible to max number

                    Dim lbl As Control = Nothing
                    Dim tbx As Control = Nothing
                    Dim fp As cEwEFormatProvider = Nothing

                    Select Case i
                        Case 1 : lbl = Me.m_lblA : tbx = Me.m_tbxA
                        Case 2 : lbl = Me.m_lblB : tbx = Me.m_tbxB
                        Case 3 : lbl = Me.m_lblC : tbx = Me.m_tbxC
                        Case 4 : lbl = Me.m_lblD : tbx = Me.m_tbxD
                    End Select

                    If (i < value.nParameters) Then
                        ' Configure label
                        lbl.Visible = True
                        lbl.Text = cStyleGuide.ToLabel(value.ParamName(i))
                        ' Configure textbox and format provider
                        tbx.Visible = True

                        fp = New cEwEFormatProvider(Me.m_uic, tbx, GetType(Single))
                        fp.Tag = i
                        fp.Value = value.ParamValue(i)
                        AddHandler fp.OnValueChanged, AddressOf OnValueChanged

                        Me.m_fps.Add(fp)
                    Else
                        lbl.Visible = False
                        tbx.Visible = False
                    End If
                Next

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected shape function type in the type selection controls
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property SelectedShapeType() As eShapeFunctionType
            Get
                Dim item As Object = Me.m_lbShapeFunctionTypes.SelectedItem
                If (item Is Nothing) Then Return eShapeFunctionType.NotSet
                Return DirectCast(item, eShapeFunctionType)
            End Get
            Set(ByVal value As eShapeFunctionType)
                For Each item As Object In Me.m_lbShapeFunctionTypes.Items
                    If (DirectCast(item, eShapeFunctionType) = value) Then
                        Me.m_lbShapeFunctionTypes.SelectedItem = item
                        Return
                    End If
                Next
                Me.m_lbShapeFunctionTypes.SelectedItem = Nothing
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generate one common shape (linear, sigmoid, etc) based on the user's choice.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdatePreview()
            Me.m_bRecalc = True
            Me.m_plPreview.Invalidate()
        End Sub

        Private Sub UpdateShape()
            Me.m_shpBuff.A = CSng(Me.m_fpA.Value)
            Me.m_shpBuff.B = CSng(Me.m_fpB.Value)
            Me.m_shpBuff.C = CSng(Me.m_fpC.Value)
            Me.m_shpBuff.D = CSng(Me.m_fpD.Value)
        End Sub

        Private m_bInValidation As Boolean = False

        Private Sub ValidateInput()

            If (Me.m_bInValidation) Then Return

            Me.m_bInValidation = True

            Dim a As Single = CSng(Me.m_fpA.Value)
            Dim b As Single = CSng(Me.m_fpB.Value)
            Dim c As Single = CSng(Me.m_fpC.Value)
            Dim d As Single = CSng(Me.m_fpD.Value)

            Dim shift As Single

            Select Case Me.SelectedShapeType

                Case eShapeFunctionType.Trapezoid
                    'This only sort of works
                    'The idea is to translate the object
                    'if one of the points is to far to the right.
                    'Because we don't know the point positions before the edit 
                    'we can't figure out the shift for the translate
                    'So just fake it...
                    If a > b Then
                        shift = a - Me.m_shpBuff.A
                        Me.m_fpB.Value = b + shift
                        Me.m_fpC.Value = c + shift
                        Me.m_fpD.Value = d + shift
                        Return
                    End If

                    If b > c Then
                        shift = b - Me.m_shpBuff.B
                        Me.m_fpC.Value = c + shift
                        Me.m_fpD.Value = d + shift
                        Return
                    End If

                    If c > d Then
                        shift = c - Me.m_shpBuff.C
                        Me.m_fpD.Value = d + shift
                        Return
                    End If

            End Select

            Me.m_bInValidation = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enable shape type options that make sense for the selected shape.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function IsRelevantShapeType(ByVal FuncType As eShapeFunctionType) As Boolean

            If (FuncType = eShapeFunctionType.NotSet) Then Return True

            Select Case Me.m_shape.DataType

                Case EwEUtils.Core.eDataTypes.Forcing
                    Return (FuncType <> eShapeFunctionType.Betapdf And FuncType <> eShapeFunctionType.Normal And _
                            FuncType <> eShapeFunctionType.Normal And FuncType <> eShapeFunctionType.RightShoulder _
                            And FuncType <> eShapeFunctionType.LeftShoulder And FuncType <> eShapeFunctionType.Trapezoid)

                Case EwEUtils.Core.eDataTypes.Mediation, EwEUtils.Core.eDataTypes.PriceMediation
                    Return True

            End Select

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update enabled state of controls
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()
            Me.SuspendLayout()

            Dim bHasName As Boolean = (Me.m_tbxName.Text.Length > 0)
            Dim bEnableA As Boolean = False
            Dim bEnableB As Boolean = False
            Dim bEnableC As Boolean = False
            Dim bEnableD As Boolean = False

            Dim strLabelA As String = My.Resources.LABEL_YZERO
            Dim strLabelB As String = My.Resources.LABEL_YEND
            Dim strLabelC As String = My.Resources.LABEL_YBASE
            Dim strLabelD As String = My.Resources.LABEL_STEEPNESS

            Select Case Me.SelectedShapeType()
                Case eShapeFunctionType.NotSet
                    ' All input controls disabled
                Case eShapeFunctionType.Linear
                    bEnableA = True : bEnableB = True

                Case eShapeFunctionType.Sigmoid
                    bEnableA = True : bEnableB = True : bEnableC = True : bEnableD = True

                Case eShapeFunctionType.Hyperbolic

                    '                 bEnableA = True : bEnableB = True : bEnableC = True : bEnableD = True

                    bEnableA = True : bEnableB = True : bEnableC = True


                Case eShapeFunctionType.Exponential
                    bEnableA = True : bEnableC = True

                Case eShapeFunctionType.Betapdf
                    bEnableA = True : bEnableB = True
                    strLabelA = My.Resources.LABEL_A
                    strLabelB = My.Resources.LABEL_B

                Case eShapeFunctionType.Normal
                    bEnableA = True : bEnableB = True : bEnableC = True : bEnableD = True
                    strLabelA = My.Resources.LABEL_SD_LEFT
                    strLabelB = My.Resources.LABEL_SD_RIGHT
                    strLabelC = My.Resources.LABEL_SD_WIDTH
                    strLabelD = My.Resources.LABEL_MEAN

                Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder
                    bEnableA = True : bEnableB = True : bEnableC = True
                    strLabelA = My.Resources.LABEL_LEFTPOINT
                    strLabelB = My.Resources.LABEL_RIGHTPOINT
                    strLabelC = My.Resources.LABEL_WIDTH

                Case eShapeFunctionType.Trapezoid

                    bEnableA = True : bEnableB = True : bEnableC = True : bEnableD = True
                    strLabelA = "Left bottom"
                    strLabelB = "Left top"
                    strLabelC = "Right top"
                    strLabelD = "Right bottom"

                Case Else
                    Debug.Assert(False)
            End Select

            ' Show/hide controls
            Me.m_lblA.Visible = bEnableA : Me.m_tbxA.Visible = bEnableA
            Me.m_lblB.Visible = bEnableB : Me.m_tbxB.Visible = bEnableB
            Me.m_lblC.Visible = bEnableC : Me.m_tbxC.Visible = bEnableC
            Me.m_lblD.Visible = bEnableD : Me.m_tbxD.Visible = bEnableD

            ' Update labels
            Me.m_lblA.Text = strLabelA
            Me.m_lblB.Text = strLabelB
            Me.m_lblC.Text = strLabelC
            Me.m_lblD.Text = strLabelD

            Me.ResumeLayout(True)

            Me.m_btnOk.Enabled = bHasName

        End Sub

        Private Function RecalcShape() As Boolean

            Dim nPoints As Integer = Me.NumDataPoints

            Try

                Dim xBase As Single = 0.3 ' This original value is extracted from EwE5
                Dim xHalf, xPow, expK As Single
                Dim sYZero As Single = CSng(Me.m_fpA.Value)
                Dim sSteep As Single = CSng(Me.m_fpD.Value)
                Dim sYBase As Single = CSng(Me.m_fpC.Value)
                Dim sYEnd As Single = CSng(Me.m_fpB.Value)


                Select Case Me.SelectedShapeType

                    Case eShapeFunctionType.NotSet
                        ' Obtain original shape data again
                        Me.m_shpBuff.ShapeData = Me.m_shape.ShapeData

                    Case eShapeFunctionType.Linear
                        For i As Integer = 1 To nPoints
                            Me.m_shpBuff.ShapeData(i) = sYZero + (sYEnd - sYZero) * (i - 1) / (nPoints - 1)
                        Next i

                    Case eShapeFunctionType.Sigmoid, eShapeFunctionType.Hyperbolic

                        If Me.SelectedShapeType = eShapeFunctionType.Hyperbolic Then sSteep = 1

                        If sYBase <> sYZero Then
                            xHalf = CSng((sYEnd - sYZero) * ((xBase ^ sSteep) / (sYBase - sYZero)) - (xBase ^ sSteep))
                        Else
                            xHalf = 1000
                        End If
                        For i As Integer = 1 To nPoints
                            xPow = CSng((i / nPoints) ^ sSteep)
                            If (xHalf + xPow <> 0) Then
                                Me.m_shpBuff.ShapeData(i) = sYZero + ((sYEnd - sYZero) * xPow / (xHalf + xPow))
                            End If
                        Next i

                    Case eShapeFunctionType.Exponential
                        If sYZero > 0 Then
                            expK = CSng((1 / xBase) * Math.Log(sYBase / sYZero))
                        Else
                            expK = 10
                        End If

                        For i As Integer = 1 To nPoints
                            Dim sTmp As Single = CSng(sYZero * Math.Exp(expK * i / nPoints))
                            If sTmp > 1 Then sTmp = 1
                            Me.m_shpBuff.ShapeData(i) = sTmp
                        Next i

                    Case eShapeFunctionType.Betapdf

                        'Beta probability distribution function
                        For i As Integer = 1 To nPoints
                            Dim x As Single = CSng(i / (nPoints + 1))
                            Me.m_shpBuff.ShapeData(i) = CSng(Me.betaPDF(sYZero, sYEnd, x))
                        Next i

                    Case eShapeFunctionType.Normal

                        'normal distribution with a mean of Zero
                        'User defines 
                        '   Standard deviation on the left and right
                        '   Width of the data in standard deviations 
                        '   Width is important because values outside the bounds 
                        '       are just the first or last value in the shape

                        'Normal and Beta shapes are not used for Forcing functions
                        'so it is only the shape we are interested in not that actual data
                        'how the shape affects the data is defined by the user by where they place the baseline
                        'If these are to be used as Forcing Function then we will need a way to 'scale' the data
                        'as there is no way to in the Forcing Function interface to select where the baseline is.
                        Dim nPtHalf As Integer = nPoints \ 2
                        'SD left
                        Dim sd As Single = sYZero + 0.0000001F
                        'width in SD
                        Dim Wsd As Single = sYBase

                        'Delta X 
                        Dim dx As Single = Wsd / (nPoints - 1)
                        'Start X
                        Dim x0 As Single = -Wsd * 0.5F
                        Dim x As Single
                        For i As Integer = 1 To nPoints
                            If i > nPtHalf Then
                                sd = sYEnd + 0.0000001F
                            End If
                            x = x0 + dx * (i - 1)
                            Me.m_shpBuff.ShapeData(i) = CSng(Math.Exp(-0.5 * (x / sd) ^ 2))
                        Next

                        'xxxxxxALTERNATIVE WAY TO USE THE PARAMETERS NOT IMPLEMENTED HERE xxxxxxxxxxxx
                        'Case eShapeFunctionType.Normal

                        '    'normal distribution with a mean of Zero
                        '    'User defines 
                        '    '   Standard deviation on the left and right
                        '    '   Width of the data in standard deviations 
                        '    '   Width is important because values outside the bounds 
                        '    '       are just the first or last value in the shape

                        '    'Normal and Beta shapes are not used for Forcing functions
                        '    'so it is only the shape we are interested in not that actual data
                        '    'how the shape affects the data is defined by the user by where they place the baseline
                        '    'If these are to be used as Forcing Function then we will need a way to 'scale' the data
                        '    'as there is no way to in the Forcing Function interface to select where the baseline is.
                        '    Dim nPtHalf As Integer = nPoints \ 2
                        '    'SD left
                        '    Dim SDLeft As Single = sYZero '+ 0.0000001F
                        '    Dim SDRight As Single = sYEnd ' + 0.0000001F
                        '    If SDLeft = 0 Then SDLeft = 0.0000001F
                        '    If SDRight = 0 Then SDLeft = 0.0000001F

                        '    Dim Mean As Single = sSteep
                        '    'width in SD
                        '    Dim Wsd As Single = sYBase

                        '    'width in user defined units
                        '    Dim Wvals As Single = Math.Max(SDLeft, SDRight) * Wsd
                        '    'Delta X 
                        '    Dim dx As Single = Wvals / (nPoints - 1)
                        '    'Start X
                        '    Dim x0 As Single = (-Wvals * 0.5F)
                        '    Dim x As Single
                        '    Dim sd As Single = SDLeft
                        '    For i As Integer = 1 To nPoints
                        '        If i > nPtHalf Then
                        '            sd = SDRight ' + 0.0000001F
                        '        End If
                        '        x = x0 + dx * (i - 1)
                        '        Me.m_asDataWork(i) = CSng(Math.Exp(-0.5 * (x / sd) ^ 2))
                        '    Next
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder

                        Dim xpt As Single
                        Dim width As Single = sYBase
                        'x0 is the value of the first x point
                        'if the user set the first point to < zero 
                        'then shift x0 over by one point to get a bit of room for the shoulder 
                        Dim x0 As Single = 0
                        If sYZero < 0 Then
                            x0 = sYZero - 1.0F
                            width = sYBase - x0
                        End If

                        Dim dx As Single = width / nPoints

                        If sYBase = 0 Then sYBase = 1
                        If sYZero > sYEnd Then sYEnd = sYZero
                        If sYBase < sYZero Or sYBase < sYEnd Then sYBase = sYEnd + 1

                        Dim yVal() As Single
                        If Me.SelectedShapeType = eShapeFunctionType.LeftShoulder Then

                            yVal = New Single() {1, 1, 0, 0}
                        Else
                            yVal = New Single() {0, 0, 1, 1}
                        End If

                        Dim xVal() As Single = New Single() {x0, sYZero, sYEnd, sYBase}
                        'Break the line up into segments based on the xpoints the user entered
                        'The location of the shoulder in the response function is determined by it's index position in the points array
                        Dim iSegment() As Integer = New Integer() {0, Me.getIndex(sYZero, x0, sYBase, nPoints), Me.getIndex(sYEnd, x0, sYBase, nPoints), nPoints}

                        ' JS 160914: This is not right; the original shape cannot be modified until the user clicks 'OK'
                        Dim shape As cEnviroResponseFunction = TryCast(Me.m_shape, cEnviroResponseFunction)
                        If shape IsNot Nothing Then
                            'set the extent of the data in the shape
                            shape.ResponseLeftLimit = x0
                            shape.ResponseRightLimit = sYBase
                        End If

                        'loop over the segments and interpolate the points on the line
                        For i As Integer = 0 To 2
                            xpt = xVal(i)
                            For j As Integer = iSegment(i) To iSegment(i + 1)
                                Me.m_shpBuff.ShapeData(j) = Me.LinearInterp(xpt, xVal(i), xVal(i + 1), yVal(i), yVal(i + 1))
                                xpt += dx
                            Next j
                        Next i


                    Case eShapeFunctionType.Trapezoid

                        Dim xpt As Single
                        Dim width As Single = sSteep
                        Dim x0 As Single = 0
                        If sYZero < 0 Then
                            x0 = sYZero
                            width = sSteep - sYZero
                        End If

                        Dim dx As Single = width / nPoints

                        If sYBase = 0 Then sYBase = 1
                        If sYZero > sYEnd Then sYEnd = sYZero
                        If sYBase < sYZero Or sYBase < sYEnd Then sYBase = sYEnd + 1

                        Dim yVal() As Single = New Single() {0, 0, 1, 1, 0, 0}
                        Dim xVal() As Single = New Single() {x0, sYZero, sYEnd, sYBase, sSteep, width}

                        'Break the line up into segments based on the xpoints the user entered
                        'The location of the shoulder in the response function is determined by it's index position in the points array
                        Dim iSegment() As Integer = New Integer() {0, Me.getIndex(sYZero, x0, sSteep, nPoints), Me.getIndex(sYEnd, x0, sSteep, nPoints), Me.getIndex(sYBase, x0, sSteep, nPoints), Me.getIndex(sSteep, x0, sSteep, nPoints), nPoints}

                        ' JS 160914: This is not right; the original shape cannot be modified until the user clicks 'OK'
                        Dim shape As cEnviroResponseFunction = TryCast(Me.m_shape, cEnviroResponseFunction)
                        If shape IsNot Nothing Then
                            'set the extent of the data in the shape
                            shape.ResponseLeftLimit = x0
                            shape.ResponseRightLimit = sSteep
                        End If

                        'loop over the segments and interpolate the points on the line
                        For i As Integer = 0 To 4
                            xpt = xVal(i)
                            'loop from the start to the end position in this segment
                            'and interpolate the y point on the line
                            For j As Integer = iSegment(i) To iSegment(i + 1)
                                Me.m_shpBuff.ShapeData(j) = Me.LinearInterp(xpt, xVal(i), xVal(i + 1), yVal(i), yVal(i + 1))
                                xpt += dx
                            Next j
                        Next i


                    Case Else
                        Debug.Assert(False)
                        Return False

                End Select

                Dim ScaleMax As Single = CSng(Me.m_fpMax.Value)
                Me.ScaleShape(ScaleMax)

            Catch ex As Exception
                Return False
            End Try

            ' Is not displaying original shape?
            If (Me.SelectedShapeType <> eShapeFunctionType.NotSet) Then

                ' Complete rest of shape
                For i As Integer = nPoints + 1 To Me.NumDisplayPoints
                    Me.m_shpBuff.ShapeData(i) = Me.m_shpBuff.ShapeData(nPoints)
                Next

            End If

            Return True

        End Function

        Private Function getIndex(Xvalue As Single, x0 As Single, x1 As Single, TotalNPoints As Integer) As Integer
            'Debug.Assert(Xvalue >= x0 And Xvalue <= x1, Me.ToString + ".getIndex() value out of bounds.")
            'use the linear interpolator to find the index positon of Value
            'In this case we are interpolating the number of data points Xvalue is along the line
            'x0 and x1 are the first and last values of the x axis
            '0 and TotalNPoints are the number of data points/array indexes
            Return CInt(LinearInterp(Xvalue, x0, x1, 0, TotalNPoints))
        End Function

        Private Function LinearInterp(ByVal x As Single, x0 As Single, x1 As Single, y0 As Single, y1 As Single) As Single
            If ((x1 - x0) = 0) Then
                'mid point on the y axis
                Return (y0 + y1) / 2.0F
            Else
                Return y0 + (y1 - y0) * ((x - x0) / (x1 - x0))
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gamma function
        ''' </summary>
        ''' <param name="xx"></param>
        ''' -------------------------------------------------------------------
        Private Function Gamma(ByVal xx As Double) As Double
            'HACK gammln(x) returns the log n gamma used by Numeric Recipies in C betai(a,b,x) 
            'we need gamma for beta(x) so remove the log
            Return Math.Exp(Me.gammln(xx))
        End Function


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gamma Log n from Numeric Recipies in C
        ''' </summary>
        ''' <param name="xx"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function gammln(ByVal xx As Double) As Double
            'from NRC-2
            Dim x As Double, y As Double, tmp As Double, ser As Double
            Dim cof() As Double = {76.180091729471457, -86.505320329416776, _
                                  24.014098240830911, -1.231739572450155, _
                                  0.001208650973866179, -0.000005395239384953}
            Dim j As Integer
            x = xx
            tmp = x + 5.5
            tmp -= (x + 0.5) * Math.Log(tmp)
            ser = 1.0000000001900149

            For j = 0 To 5
                y += 1
                ser += cof(j) / (x + y)
            Next

            Return -tmp + Math.Log(2.5066282746310007 * ser / x)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cumulative Beta distribution function from Numeric Recipies in C
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>Not used here but left in because it works!!!</remarks>
        ''' -------------------------------------------------------------------
        Private Function betacf(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double

            Dim m As Integer, m2 As Integer
            Dim aa As Double, c As Double, d As Double, del As Double, h As Double, qab As Double, qam As Double, qap As Double
            qab = a + b ' These q's will be used in factors that occur
            qap = a + 1.0F ' in the coecients (6.4.6).
            qam = a - 1.0F '
            c = 1.0 ' First step of Lentz's method.
            d = 1.0F - qab * x / qap '
            If (Math.Abs(d) < FPMIN) Then d = FPMIN
            d = 1.0F / d
            h = d

            For m = 1 To MAXIT ' - 1 '(m=1;m<=MAXIT;m++) 
                m2 = 2 * m
                aa = m * (b - m) * x / ((qam + m2) * (a + m2))
                d = 1.0F + aa * d ' One step (the even one) of the recurrence.
                If (Math.Abs(d) < FPMIN) Then d = FPMIN
                c = 1.0F + aa / c
                If (Math.Abs(c) < FPMIN) Then c = FPMIN 'if (fabs(c) < FPMIN) c=FPMIN;
                d = 1.0F / d
                h *= d * c
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
                d = 1.0F + aa * d ' Next step of the recurrence (the odd one).
                If (Math.Abs(d) < FPMIN) Then d = FPMIN
                c = 1.0F + aa / c
                If (Math.Abs(c) < FPMIN) Then c = FPMIN
                d = 1.0F / d
                del = d * c
                h *= del
                If (Math.Abs(del - 1.0) < EPS) Then Exit For ' Are we done?'if (fabs(del-1.0) < EPS) break; Are we done?

            Next

            'if (m > MAXIT) nrerror("a or b too big, or MAXIT too small in betacf");
            Return h

        End Function

        Private Function beta(ByVal a As Single, ByVal b As Single) As Single
            'Beta function from Wikipedia
            'http://en.wikipedia.org/wiki/Beta_function
            Return CSng(Gamma(a) * Gamma(b) / Gamma(a + b))

        End Function

        Private Function betaPDF(ByVal a As Single, ByVal b As Single, ByVal x As Single) As Single
            'Beta Distribution pdf from Wikipedia
            'http://en.wikipedia.org/wiki/Beta_distribution
            Return CSng((x ^ (a - 1) * (1 - x) ^ (b - 1)) / beta(a, b))

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cumulative Beta distribution from Numberic Recipies in C
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function betai(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double

            Dim bt As Double
            ' if (x < 0.0 || x > 1.0) nrerror("Bad x in routine betai");
            If (x <= 0.0 Or x >= 1.0) Then
                bt = 0.0
            Else
                bt = Math.Exp(gammln(a + b) - gammln(a) - gammln(b) + a * Math.Log(x) + b * Math.Log(1.0 - x))
            End If

            If (x < (a + 1.0) / (a + b + 2.0)) Then 'Use continued fraction directly.
                Return bt * betacf(a, b, x) / a
            Else 'Use continued fraction after making the symmetry transformation.
                Return 1D - bt * betacf(b, a, 1D - x) / b ' 
            End If

            '            float bt;
            'if (x < 0.0 || x > 1.0) nrerror("Bad x in routine betai");
            'if (x == 0.0 || x == 1.0) bt=0.0;
            'else Factors in front of the continued fraction.
            'bt=exp(gammln(a+b)-gammln(a)-gammln(b)+a*log(x)+b*log(1.0-x));
            'if (x < (a+1.0)/(a+b+2.0)) Use continued fraction directly.
            'return bt*betacf(a,b,x)/a;
            'else Use continued fraction after making the sym-
            'return 1.0-bt*betacf(b,a,1.0-x)/b; metry transformation.
        End Function

        Private Function NumDisplayPoints() As Integer
            If Me.m_shape.IsSeasonal Then Return cCore.N_MONTHS
            Return Me.m_shape.ShapeData.Length - 1
        End Function

        Private Function NumDataPoints() As Integer
            If Me.m_shape.IsSeasonal Then Return cCore.N_MONTHS
            If Me.m_shape.DataType = EwEUtils.Core.eDataTypes.Forcing Then
                Return Me.m_uic.Core.nEcosimYears * cCore.N_MONTHS
            End If
            Return Me.m_shape.nPoints
        End Function

        ''' <summary>
        ''' Scale the internal shape buffer to a new maximum.
        ''' </summary>
        ''' <param name="sNewMax"></param>
        Private Sub ScaleShape(sNewMax As Single)

            If (sNewMax = 0) Then Return

            Dim scalar As Single = sNewMax / Me.m_shpBuff.Max
            For ipt As Integer = 1 To Me.m_shpBuff.ShapeData.Length - 1
                Me.m_shpBuff.ShapeData(ipt) *= scalar
            Next

        End Sub

#End Region ' Private method helpers

    End Class

End Namespace

