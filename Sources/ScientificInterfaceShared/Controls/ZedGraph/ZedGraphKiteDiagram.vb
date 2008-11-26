'==============================================================================
' $ Log: $
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports ZedGraph
#End Region ' Imports

Namespace Controls
    Public Class ZedGraphKiteDiagram

        Private m_noOfScaleCircles As Integer = 3

        Private m_maxScaleValue As Single = 35
        Private m_numVaraibles As Integer = 4
        Private m_hightLightIndex As Integer = -1
        Private m_Values As New List(Of Single())

        ''' <summary> Adds a single curve and removes the rest </summary>
        Public Sub AddOneCurve(ByVal arr() As Single)
            m_Values.Clear()
            findMaxValue(arr, True)
            m_Values.Add(arr)
        End Sub

        ''' <summary> Appends to the rest of the curves.  Use ClearCurves to remove all. </summary>
        Public Sub AddCurve(ByVal arrs As List(Of Single()))
            For Each ar As Single() In arrs
                findMaxValue(ar)
                m_Values.Add(ar)
            Next
        End Sub

        ''' <summary> Highlights index curve </summary>
        Public Sub ClearCurves()
            m_maxScaleValue = -1
            m_Values.Clear()
        End Sub

        Public Sub Repaint(ByVal zgc As ZedGraphControl)
            CreateGraph(zgc)
        End Sub

        Private Sub findMaxValue(ByVal arr As Single(), Optional ByVal reset As Boolean = False)
            If reset = True Then m_maxScaleValue = -1

            For i As Integer = 0 To arr.Length - 1
                If m_maxScaleValue < arr(i) * 1.2 Then m_maxScaleValue = CSng(arr(i) * 1.2)
            Next
        End Sub

        '  Call this method from the Form_Load method, passing your ZedGraphControl
        Private Sub CreateGraph(ByVal zgc As ZedGraphControl)

            '  get a reference to the GraphPane
            Dim myPane As GraphPane = zgc.GraphPane

            Dim scaleDiam As Single = CSng(MaxScaleValue) * CSng(0.8)

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

            '  Render the simulated polar decorations:
            Dim scaleCircleList As RadarPointList()
            Dim scaleCircle As LineItem()
            Dim delta As Double = CDbl(scaleDiam / NumScaleCircles)

            ReDim scaleCircle(NumScaleCircles)
            ReDim scaleCircleList(NumScaleCircles)

            '' Calculate the scales
            'For j As Integer = 0 To NumScaleCircles - 1

            '    scaleCircleList(j) = New RadarPointList()
            '    For i As Integer = 0 To CInt(MaxScaleValue * 0.8) - 1
            '        scaleCircleList(j).Add(scaleDiam, 1)
            '    Next

            '    scaleCircle(j) = myPane.AddCurve("", scaleCircleList(j), Color.Black, SymbolType.None)
            '    scaleCircle(j).Line.IsSmooth = True
            '    scaleCircle(j).Line.SmoothTension = 0.6F
            '    scaleCircle(j).Line.Style = Drawing2D.DashStyle.Custom
            '    scaleCircle(j).Line.DashOff = 2
            '    scaleCircle(j).Line.DashOn = 4

            '    scaleDiam = scaleDiam - delta

            'Next

            ''  Render the "rays" from the center
            'For j As Integer = 0 To 20 - 1
            '    Dim line As LineObj = New ArrowObj(Color.Black, 0, 0, 0, CSng(scaleCircleList(0)(j).X), CSng(scaleCircleList(0)(j).Y))
            '    line.Line.Style = Drawing2D.DashStyle.Custom
            '    line.Line.DashOn = 1
            '    line.Line.DashOff = 4
            '    myPane.GraphObjList.Add(line)
            'Next

            For Each val As Single() In m_Values

                Dim dataList As RadarPointList = New RadarPointList()
                For i As Integer = 0 To val.Length - 1

                    Dim x As Double = val(i)
                    dataList.Add(x, 1)
                Next

                Dim Data As LineItem = myPane.AddCurve("Data", dataList, Color.Navy, SymbolType.Circle)
                'Data.Line.IsSmooth = True
                'Data.Line.SmoothTension = 0.6F
                Data.Line.Width = 2
            Next

            zgc.AxisChange()
        End Sub


        ''' <summary> Sets the highlight index </summary>
        Public Property HiglightIndex() As Integer
            Get
                Return m_hightLightIndex
            End Get
            Set(ByVal value As Integer)
                If value < m_Values.Count - 1 Then
                    m_hightLightIndex = value
                Else
                    Debug.Assert(False, "Attempted to set a highlight value greater than size of list")
                End If
            End Set
        End Property

        ''' <summary> Sets the scale value </summary>
        Public Property MaxScaleValue() As Single
            Get
                Return m_maxScaleValue
            End Get
            Set(ByVal value As Single)
                m_maxScaleValue = value
            End Set
        End Property


        ''' <summary>
        ''' Draws the number of scales for ledgend
        ''' </summary>
        Public Property NumScaleCircles() As Integer
            Get
                Return m_noOfScaleCircles
            End Get
            Set(ByVal value As Integer)
                m_noOfScaleCircles = value
            End Set
        End Property
    End Class
End Namespace
