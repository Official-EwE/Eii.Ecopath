#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style

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
        Private m_uic As cUIContext = Nothing
        ''' <summary></summary>
        Private m_shape As cForcingFunction = Nothing
        ''' <summary></summary>
        Private m_handler As cShapeGUIHandler = Nothing

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

        Private MAXIT As Integer = 100
        Private EPS As Single = 0.0000003
        Private FPMIN As Single = 1.0E-30

#End Region ' Private vars

#Region " Private class "

        Private Class cShapeFunctionTypeItem

            Private m_sft As eShapeFunctionType = eShapeFunctionType.NotSet

            Public Sub New(ByVal sft As eShapeFunctionType)
                Me.m_sft = sft
            End Sub

            Public Overrides Function ToString() As String
                Dim fmt As New cShapeFunctionTypeFormatter()
                Return fmt.GetDescriptor(Me.m_sft)
            End Function

            ReadOnly Property ShapeFunctionType() As eShapeFunctionType
                Get
                    Return Me.m_sft
                End Get
            End Property

        End Class
#End Region ' Private class

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
            Me.m_asDataWork = shape.ShapeData

        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.CenterToParent()

            'jb 24-May-11 removed data validation to fix ticket 975
            Me.m_fpYZero = New cEwEFormatProvider(Me.m_uic, Me.m_txbYZero, GetType(Single))
            Me.m_fpYZero.Value = Me.m_shape.YZero 'Math.Max(0, Me.m_shape.YZero)

            Me.m_fpYBase = New cEwEFormatProvider(Me.m_uic, Me.m_txbYBase, GetType(Single))
            Me.m_fpYBase.Value = Me.m_shape.YBase 'CSng(IIf(Me.m_shape.YBase <= 0, 0.5!, Me.m_shape.YBase))

            Me.m_fpYEnd = New cEwEFormatProvider(Me.m_uic, Me.m_txbYEnd, GetType(Single))
            Me.m_fpYEnd.Value = Me.m_shape.YEnd 'CSng(IIf(Me.m_shape.YEnd < 0, 1.0!, Me.m_shape.YEnd))

            Me.m_fpSteep = New cEwEFormatProvider(Me.m_uic, Me.m_txbSteep, GetType(Single))
            Me.m_fpSteep.Value = Me.m_shape.Steep ' CSng(IIf(Me.m_shape.Steep = 0, 3.0!, Me.m_shape.Steep))

            ' Show available options
            For Each sft As eShapeFunctionType In [Enum].GetValues(GetType(eShapeFunctionType))
                If Me.IsRelevantShapeType(sft) Then
                    Me.m_lbShapeFunctionTypes.Items.Add(New cShapeFunctionTypeItem(sft))
                End If
            Next

            ' Integrate shape name into the dialog title
            Dim fmt As New cShapeDataFormatter()
            Me.Text = String.Format(Me.Text, fmt.GetDescriptor(Me.m_shape))

            Me.SelectedShapeType = Me.m_shape.ShapeFunctionType

            Me.UpdatePreview()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_fpSteep.Release()
            Me.m_fpYBase.Release()
            Me.m_fpYEnd.Release()
            Me.m_fpYZero.Release()

            MyBase.OnFormClosed(e)

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
            Me.m_shape.ShapeFunctionType = Me.SelectedShapeType()

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

        Private Sub OnShapeFunctionTypeSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbShapeFunctionTypes.SelectedIndexChanged
            Try
                Me.UpdateControls()
                Me.UpdatePreview()
            Catch ex As Exception

            End Try
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

            cShapeImage.DrawShapeDirect(Me.m_uic, _
                                       Me.m_asDataWork, Me.m_shape.XMax, Me.m_shape.IsSeasonal, _
                                       Me.m_plPreview.ClientRectangle, e.Graphics, Me.m_handler.Color, _
                                       Me.m_handler.SketchDrawMode, _
                                       sDataMax / 0.8!, cCore.NULL_VALUE, cCore.NULL_VALUE)

        End Sub

#End Region ' Events

#Region " Private method helpers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected shape function type in the type selection controls
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property SelectedShapeType() As eShapeFunctionType
            Get
                Dim item As Object = Me.m_lbShapeFunctionTypes.SelectedItem
                If (item Is Nothing) Then Return eShapeFunctionType.NotSet
                Return DirectCast(item, cShapeFunctionTypeItem).ShapeFunctionType
            End Get
            Set(ByVal value As eShapeFunctionType)
                For Each item As Object In Me.m_lbShapeFunctionTypes.Items
                    If DirectCast(item, cShapeFunctionTypeItem).ShapeFunctionType = value Then
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
        ''' <remarks>The formula here is extracted from EwE5 code</remarks>
        ''' -------------------------------------------------------------------
        Private Sub UpdatePreview()
            Me.m_bRecalc = True
            Me.m_plPreview.Invalidate()
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
                    Return (FuncType <> eShapeFunctionType.Betapdf And FuncType <> eShapeFunctionType.Normal)

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

            'Dim bBetaLabel As Boolean = False
            'Dim bNormalLabel As Boolean = False

            Dim bEnableSteep As Boolean = False
            Dim bEnableYBase As Boolean = False
            Dim bEnableYEnd As Boolean = False
            Dim bEnableYZero As Boolean = False

            'default labels
            Dim YZeroTxt As String = "Y Zero"
            Dim YEndTxt As String = "Y End"
            Dim YBaseTxt As String = "Y Base"

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

                Case eShapeFunctionType.Betapdf
                    bEnableYZero = True : bEnableYEnd = True
                    YZeroTxt = "a"
                    YEndTxt = "b"

                Case eShapeFunctionType.Normal
                    bEnableYBase = True : bEnableYEnd = True : bEnableYZero = True
                    YZeroTxt = "SD Left"
                    YEndTxt = "SD Right"
                    YBaseTxt = "SD Width"

                Case Else
                    Debug.Assert(False)
            End Select

            'labels
            Me.m_lbYZero.Text = YZeroTxt
            Me.m_lbYEnd.Text = YEndTxt
            Me.m_lbYBase.Text = YBaseTxt

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

                        If Me.SelectedShapeType = eShapeFunctionType.Hyperbolic Then sSteep = 1

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

                    Case eShapeFunctionType.Betapdf

                        'Beta probability distribution function
                        For i As Integer = 1 To nPoints
                            Dim x As Single = CSng(i / (nPoints + 1))
                            Me.m_asDataWork(i) = CSng(Me.betaPDF(sYZero, sYEnd, x))
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
                            Me.m_asDataWork(i) = CSng(Math.Exp(-0.5 * (x / sd) ^ 2))
                        Next

                    Case Else
                        Debug.Assert(False)
                        Return False

                End Select

            Catch ex As Exception
                Return False
            End Try

            Return True

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

#End Region ' Private method helpers

    End Class

End Namespace

