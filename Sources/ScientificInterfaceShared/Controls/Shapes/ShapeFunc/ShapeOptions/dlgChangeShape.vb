'==============================================================================
'
' $Log: dlgChangeShape.vb,v $
' Revision 1.2  2009/02/26 06:33:51  sherman
' Enabled Y-end for all shapes.
'
' Revision 1.1  2008/12/15 15:36:38  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:42  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' Interface to set the contour of a given shape to a 'common' primitive
    ''' </summary>
    ''' <remarks>
    ''' EwE5: frmShaper.vb
    ''' </remarks>
    Public Class dlgChangeShape

#Region " Private vars "

        ''' <summary></summary>
        Private m_shape As cForcingFunction = Nothing
        ''' <summary>Copy of the original shape to work on.</summary>
        Private m_asDataWork As Single()
        ''' <summary></summary>
        Private m_fpYBase As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpYEnd As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpYZero As cEwEFormatProvider = Nothing
        ''' <summary></summary>
        Private m_fpSteep As cEwEFormatProvider = Nothing

        Private m_bRecalc As Boolean = True

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByRef shape As cForcingFunction)

            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Sanity check
            Debug.Assert(shape IsNot Nothing)

            ' Init
            Me.m_shape = shape
            Me.m_asDataWork = shape.ShapeData

        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub ForcingShape_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.CenterToParent()

            Me.m_fpYZero = New cEwEFormatProvider(Me.m_txbYZero, GetType(Single))
            Me.m_fpYZero.Value = Math.Max(0, Me.m_shape.YZero)

            Me.m_fpYBase = New cEwEFormatProvider(Me.m_txbYBase, GetType(Single))
            Me.m_fpYBase.Value = CSng(IIf(Me.m_shape.YBase <= 0, 0.5!, Me.m_shape.YBase))

            Me.m_fpYEnd = New cEwEFormatProvider(Me.m_txbYEnd, GetType(Single))
            Me.m_fpYEnd.Value = CSng(IIf(Me.m_shape.YEnd <= 0, 1.0!, Me.m_shape.YEnd))

            Me.m_fpSteep = New cEwEFormatProvider(Me.m_txbSteep, GetType(Single))
            Me.m_fpSteep.Value = CSng(IIf(Me.m_shape.Steep = 0, 3.0!, Me.m_shape.Steep))

            Me.m_lbShape.SelectedIndex = Me.m_shape.eShapeFunctionType

            Me.UpdatePreview()

        End Sub

        Private Sub lbShape_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_lbShape.SelectedIndexChanged
            Me.UpdateControls()
            Me.UpdatePreview()
        End Sub

        Private Sub OnOk(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOk.Click
            If Not Me.RecalcShape() Then
                ' MsgBox
                Return
            End If

            Me.m_shape.LockUpdates()

            ' Copy shape data back
            Me.m_shape.ShapeData = Me.m_asDataWork
            ' Store last used params
            Me.m_shape.YZero = CSng(Me.m_fpYZero.Value)
            Me.m_shape.YBase = CSng(Me.m_fpYBase.Value)
            Me.m_shape.YEnd = CSng(Me.m_fpYEnd.Value)
            Me.m_shape.Steep = CSng(Me.m_fpSteep.Value)
            Me.m_shape.eShapeFunctionType = Me.SelectedShapeType()

            ' Go johnny go
            Me.m_shape.UnlockUpdates(True)

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnCancel.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnInputValidated(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles m_txbSteep.Validated, m_txbYBase.Validated, m_txbYEnd.Validated, m_txbYZero.Validated
            Me.UpdatePreview()
        End Sub

        Private Sub OnPaintPreview(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
                Handles m_plPreview.Paint

            Dim sDataMax As Single = 0.0
            Dim g As Graphics = e.Graphics
            Dim rc As Rectangle = Me.m_plPreview.ClientRectangle

            If Me.m_bRecalc Then
                Me.RecalcShape()
                Me.m_bRecalc = False
            End If

            For Each s As Single In Me.m_asDataWork
                sDataMax = Math.Max(s, sDataMax)
            Next

            Using br As New SolidBrush(Me.m_plPreview.BackColor)
                g.FillRectangle(br, rc)
            End Using

            ShapeImage.DrawShapeDirect(Me.m_asDataWork, Me.m_shape.XMax, Me.m_shape.IsSeasonal, _
                Me.m_plPreview.ClientRectangle, e.Graphics, Color.Black, _
                eSketchDrawModeTypes.Line, _
                sDataMax / 0.8!, cCore.NULL_VALUE, cCore.NULL_VALUE)

        End Sub

#End Region ' Events

#Region " Private method helpers "

        Private Property SelectedShapeType() As eShapeFunctionType
            Get
                Return DirectCast(Me.m_lbShape.SelectedIndex, eShapeFunctionType)
            End Get
            Set(ByVal value As eShapeFunctionType)
                Me.m_lbShape.SelectedIndex = value
            End Set
        End Property

        ''' <summary>
        ''' Generate one common shape (linear, sigmoid, etc) based on the user's choice.
        ''' </summary>
        ''' <remarks>The formula here is extracted from EwE5 code</remarks>
        Private Sub UpdatePreview()
            Me.m_bRecalc = True
            Me.m_plPreview.Invalidate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update enabled state of controls
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            Dim bEnableSteep As Boolean = False
            Dim bEnableYBase As Boolean = False
            Dim bEnableYEnd As Boolean = False
            Dim bEnableYZero As Boolean = False

            Select Case Me.SelectedShapeType()
                Case eShapeFunctionType.NotSet
                    ' All input controls disabled
                Case eShapeFunctionType.Linear
                    bEnableYZero = True : bEnableYEnd = True
                Case eShapeFunctionType.Sigmoid
                    bEnableYBase = True : bEnableYEnd = True : bEnableYZero = True
                Case eShapeFunctionType.Hyperbolic
                    bEnableYBase = True : bEnableYEnd = True : bEnableYZero = True : bEnableSteep = True
                Case eShapeFunctionType.Exponential
                    bEnableYZero = True : bEnableYEnd = True : bEnableYBase = True
                Case Else
                    Debug.Assert(False)
            End Select

            ' Enable controls
            Me.m_fpYZero.Enabled = bEnableYZero
            Me.m_fpYBase.Enabled = bEnableYBase
            Me.m_fpYEnd.Enabled = bEnableYEnd
            Me.m_fpSteep.Enabled = bEnableSteep

        End Sub

        Private Function RecalcShape() As Boolean

            Dim nPoints As Integer = Me.m_shape.ShapeData.Length - 1

            If Me.m_shape.IsSeasonal Then
                nPoints = cCore.N_MONTHS
            End If

            Try

                Dim xBase As Single = 0.3 ' This original value is extracted from EwE5
                Dim xHalf, xPow, expK As Single
                Dim sYZero As Single = CSng(Me.m_fpYZero.Value)
                Dim sSteep As Single = CSng(Me.m_fpSteep.Value)
                Dim sYBase As Single = CSng(Me.m_fpYBase.Value)
                Dim sYEnd As Single = CSng(Me.m_fpYEnd.Value)

                Select Case Me.SelectedShapeType

                    Case eShapeFunctionType.NotSet
                        Me.m_asDataWork = Me.m_shape.ShapeData

                    Case eShapeFunctionType.Linear
                        For i As Integer = 0 To nPoints
                            Me.m_asDataWork(i) = sYZero + (sYEnd - sYZero) * i / nPoints
                        Next i

                    Case eShapeFunctionType.Sigmoid, eShapeFunctionType.Hyperbolic

                        If Me.SelectedShapeType = eShapeFunctionType.Sigmoid Then sSteep = 1

                        If sYBase <> sYZero Then
                            xHalf = CSng((sYEnd - sYZero) * ((xBase ^ sSteep) / (sYBase - sYZero)) - (xBase ^ sSteep))
                        Else
                            xHalf = 1000
                        End If
                        For i As Integer = 1 To nPoints
                            xPow = CSng((i / nPoints) ^ sSteep)
                            If (xHalf + xPow <> 0) Then
                                Me.m_asDataWork(i) = sYZero + ((sYEnd - sYZero) * xPow / (xHalf + xPow))
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
                            Me.m_asDataWork(i) = sTmp
                        Next i

                    Case Else
                        Debug.Assert(False)
                        Return False

                End Select

            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

#End Region ' Private method helpers

    End Class

End Namespace

