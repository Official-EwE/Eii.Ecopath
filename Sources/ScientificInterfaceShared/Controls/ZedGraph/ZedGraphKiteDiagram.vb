#Region " Imports "

Option Strict On

Imports ZedGraph
Imports EwECore

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exploratory kite diagram in a ZedGraph. Class is not finished yet.
    ''' </summary>
    ''' <remarks>
    ''' <para>ToDo apr0404:</para>
    ''' <list type="bullet">
    ''' <item>Allow curves to be labeled, custom coloured, etc.</item>
    ''' <item>Use highlight index properly.</item>
    ''' <item>Use max scales to render control.</item>
    ''' </list>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class ZedGraphKiteDiagram

        Private m_iNumScaleCircles As Integer = 3
        Private m_sAutoScaleMax As Single = cCore.NULL_VALUE
        Private m_sCustomScaleMax As Single = 35
        Private m_iNumVariables As Integer = 4
        Private m_iHighlight As Integer = -1
        Private m_lsValues As New List(Of Single())
        Private m_bAutoscale As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a single curve.
        ''' </summary>
        ''' <param name="asValues"></param>
        ''' -------------------------------------------------------------------
        Public Sub AddCurve(ByVal asValues() As Single)

            ' Sanity checks
            Debug.Assert(asValues IsNot Nothing)
            Debug.Assert(asValues.Length > 0)

            ' Add curve
            Me.m_lsValues.Add(asValues)
            ' Invalidate cached max
            Me.InvalidateMaxValue()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clears all curves.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ClearCurves()

            ' Invalidate cached max
            Me.InvalidateMaxValue()
            ' Kill! Kill! Kill!
            Me.m_lsValues.Clear()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the graph.
        ''' </summary>
        ''' <param name="zgc"></param>
        ''' -------------------------------------------------------------------
        Public Sub Refresh(ByVal zgc As ZedGraphControl)
            Me.CreateGraph(zgc)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the curve to highlight.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property HiglightIndex() As Integer
            Get
                Return Me.m_iHighlight
            End Get
            Set(ByVal value As Integer)
                If value < Me.m_lsValues.Count - 1 Then
                    Me.m_iHighlight = value
                Else
                    Debug.Assert(False, "Highlight index out of bounds")
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the graphs auto-scales.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property AutoScale() As Boolean
            Get
                Return Me.m_bAutoscale
            End Get
            Set(ByVal bAutoscale As Boolean)
                Me.m_bAutoscale = bAutoscale
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the user-defined scale max value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property MaxScaleValue() As Single
            Get
                Return Me.m_sCustomScaleMax
            End Get
            Set(ByVal value As Single)
                Me.m_sCustomScaleMax = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the auto-scale max value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property AutoMaxScaleValue() As Single
            Get
                If (Me.m_sAutoScaleMax = cCore.NULL_VALUE) Then
                    For Each asData As Single() In Me.m_lsValues
                        If asData IsNot Nothing Then
                            For i As Integer = 0 To asData.Length - 1
                                Me.m_sAutoScaleMax = CSng(Math.Max(Me.m_sAutoScaleMax, asData(i)))
                            Next
                        End If
                    Next
                End If
                Return Me.m_sAutoScaleMax
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of scales for legend to render.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property NumScaleCircles() As Integer
            Get
                Return Me.m_iNumScaleCircles
            End Get
            Set(ByVal value As Integer)
                Me.m_iNumScaleCircles = value
            End Set
        End Property

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Invalidate the auto-scale max value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub InvalidateMaxValue()
            Me.m_sAutoScaleMax = cCore.NULL_VALUE
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Call this method from the Form_Load method, passing your ZedGraphControl
        ''' </summary>
        ''' <param name="zgc"></param>
        ''' -------------------------------------------------------------------
        Private Sub CreateGraph(ByVal zgc As ZedGraphControl)

            '  get a reference to the GraphPane
            Dim myPane As GraphPane = zgc.GraphPane
            Dim sScale As Single = cCore.NULL_VALUE 'CSng(MaxScaleValue) * CSng(0.8)

            If Me.m_bAutoscale Then
                sScale = Me.MaxScaleValue
            Else
                sScale = Me.AutoMaxScaleValue
            End If

            '  Set the Titles and axis proprieties
            myPane.Title.Text = "My Polar Graph"
            myPane.XAxis.Title.Text = ""
            myPane.YAxis.Title.Text = ""
            myPane.XAxis.Cross = 0
            myPane.YAxis.Cross = 0

            myPane.XAxis.MajorTic.IsAllTics = True
            myPane.XAxis.MinorTic.IsAllTics = True
            myPane.YAxis.MajorTic.IsAllTics = True
            myPane.YAxis.MinorTic.IsAllTics = True
            myPane.XAxis.Scale.IsVisible = True
            myPane.YAxis.Scale.IsVisible = True

            myPane.XAxis.Scale.Min = -MaxScaleValue
            myPane.XAxis.Scale.Max = MaxScaleValue
            myPane.YAxis.Scale.Max = MaxScaleValue
            myPane.YAxis.Scale.Min = -MaxScaleValue

            For Each asData As Single() In m_lsValues

                Dim dataList As RadarPointList = New RadarPointList()
                For i As Integer = 0 To asData.Length - 1

                    Dim x As Double = asData(i)
                    dataList.Add(x, 1)
                Next

                Dim Data As LineItem = myPane.AddCurve("Data", dataList, Color.Navy, SymbolType.Circle)
                'Data.Line.IsSmooth = True
                'Data.Line.SmoothTension = 0.6F
                Data.Line.Width = 2
            Next

            zgc.AxisChange()
        End Sub

#End Region ' Internals

    End Class
End Namespace
