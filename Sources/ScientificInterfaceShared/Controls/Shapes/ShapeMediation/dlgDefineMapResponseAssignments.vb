#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

#End Region


Public Class dlgDefineMapResponseAssignments

#Region "Private variables"

    Private m_shape As EwECore.cEnviroResponseFunction
    Private m_manager As cMapResponseInteractionManager
    Private m_zgh As cZedGraphHelper
    Private m_uic As cUIContext
    Private m_orgMin As Single
    Private m_orgMax As Single
    Private m_bHasInit As Boolean

#End Region

#Region "Construciton Initialization"

    Public Sub New(ByVal UIC As cUIContext, ByVal ResponseShape As EwECore.cEnviroResponseFunction, ByVal Manager As EwECore.cMapResponseInteractionManager)
        Me.InitializeComponent()

        Me.m_shape = ResponseShape
        Me.m_manager = Manager

        Me.m_uic = UIC

        Me.m_zgh = New cZedGraphHelper
        Me.m_zgh.Attach(Me.m_uic, Me.ZedGraph)
    End Sub

    Private Sub dlgDefineMapResponseAssignments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            'remember the original response function min and max
            Me.m_orgMin = Me.m_shape.XAxisMin
            Me.m_orgMax = Me.m_shape.XAxisMax

            Me.m_zgh.ConfigurePane("Map histogram & Response function", "Map values", "Capacity normalized", True)

            If Me.m_shape.XAxisMax = 0 Then
                Me.m_shape.XAxisMax = 1.0 'some kind of bugus default in nothing has been defined
            End If
            Me.updateControls()

            Me.PlotShape()
            Me.loadMaps()

        Catch ex As Exception

        End Try

        Me.m_bHasInit = True

    End Sub

#End Region

#Region "Control Event Handlers"

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel

        'set the response shape back to its original state
        Me.m_shape.XAxisMax = Me.m_orgMax
        Me.m_shape.XAxisMin = Me.m_orgMin

        Me.Close()
    End Sub

    Private Sub lvMaps_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvMaps.SelectedIndexChanged

        PlotMap()

    End Sub

    Private Sub txXMax_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txXMax.TextChanged, txXMin.TextChanged

        If Not m_bHasInit Then
            Exit Sub
        End If

        Try
            Dim maxX As Single = Single.Parse(Me.txXMax.Text)
            Dim minX As Single = Single.Parse(Me.txXMin.Text)
            Me.m_shape.XAxisMin = minX
            Me.m_shape.XAxisMax = maxX

            PlotShape()
            PlotMap()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btDefaultMinMax_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btDefaultMinMax.Click
        Me.setDefaultMinMax()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub updateControls()
        Try
            Me.txXMax.Text = Me.m_shape.XAxisMax.ToString
            Me.txXMin.Text = Me.m_shape.XAxisMin.ToString
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PlotShape()

        Try
            Dim Xmax As Single = Me.m_shape.XAxisMax
            Dim Xmin As Single = Me.m_shape.XAxisMin
            Dim Xrange As Single = Me.m_shape.XAxisMax - Me.m_shape.XAxisMin

            'Always clear out the old data????
            'Maybe not!!!
            Me.m_zgh.GetPane(1).CurveList.Clear()

            Dim dx As Single = Xrange / Me.m_shape.XMax
            Dim MaxY As Single = Me.m_shape.YMax
            Dim lstPts As New PointPairList
            For ipt As Integer = 1 To Me.m_shape.XMax
                lstPts.Add(Xmin + dx * (ipt - 1), Me.m_shape.ShapeData(ipt) / MaxY)
            Next

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Response", Definitions.eLineType.NotSet, Color.SandyBrown, lstPts)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            'if there is a selected map then use that to set the x axis
            Dim map As IEnviroInputMap = Me.getSelMap()
            If map IsNot Nothing Then
                Xmax = map.Max
            End If

            Me.m_zgh.XScaleMax = Xmax
            Me.m_zgh.YScaleMax = 1.2

        Catch ex As Exception

        End Try

    End Sub

    Private Sub loadMaps()
        Dim map As IEnviroInputMap
        For imap As Integer = 1 To Me.m_manager.nMaps
            map = Me.m_manager.Maps(imap)
            Me.lvMaps.Items.Add(map.Name).Tag = map
        Next

    End Sub


    Private Sub setDefaultMinMax()
        Dim map As IEnviroInputMap = Me.getSelMap
        If map Is Nothing Then
            'some kind of a warning
            Exit Sub
        End If

        Me.txXMax.Text = map.Max.ToString
        Me.txXMin.Text = map.Min.ToString

    End Sub

    Private Function getSelMap() As IEnviroInputMap
        Try

            Dim ob As Object
            ob = Me.lvMaps.SelectedItems(0).Tag
            If ob IsNot Nothing Then
                Return DirectCast(ob, IEnviroInputMap)
            End If

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Private Sub PlotMap()
        Try
            Dim map As IEnviroInputMap = Me.getSelMap
            If map Is Nothing Then
                Exit Sub
            End If

            If Me.m_zgh.GetPane(1).CurveList.Count > 1 Then
                Me.m_zgh.GetPane(1).CurveList.RemoveAt(1)
            End If

            Dim histPts() As Drawing.PointF = map.Histogram(Me.m_shape.XMax)

            Dim maxX As Single = map.Max
            Dim lstPts As New PointPairList
            For ipt As Integer = 0 To 100
                lstPts.Add(histPts(ipt).X, histPts(ipt).Y)
            Next

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Histogram", Definitions.eLineType.NotSet, Color.RoyalBlue, lstPts)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Me.m_zgh.XScaleMax = maxX
            Me.m_zgh.YScaleMax = 1.2

        Catch ex As Exception

        End Try

    End Sub

#End Region

End Class

