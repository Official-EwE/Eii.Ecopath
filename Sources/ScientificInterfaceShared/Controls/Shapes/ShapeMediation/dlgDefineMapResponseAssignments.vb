#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

#End Region


Public Class dlgDefineMapResponseAssignments

    Private m_shape As EwECore.cEnviroResponseFunction
    Private m_manager As cMapResponseInteractionManager
    Private m_zgh As cZedGraphHelper
    Private m_uic As cUIContext

    Public Sub New(ByVal UIC As cUIContext, ByVal ResponseShape As EwECore.cEnviroResponseFunction, ByVal Manager As EwECore.cMapResponseInteractionManager)
        Me.InitializeComponent()

        Me.m_shape = ResponseShape
        Me.m_manager = Manager

        Me.m_uic = UIC

        Me.m_zgh = New cZedGraphHelper
        Me.m_zgh.Attach(Me.m_uic, Me.ZedGraph)
    End Sub


    Private Sub PlotShape()

        Try

            Dim maxX As Single = 10 'Me.m_shape.MapXAxis

            If Me.m_shape.XAxisMax > 0 Then
                maxX = Me.m_shape.XAxisMax
            End If

            If Me.m_zgh.GetPane(1).CurveList.Count > 0 Then
                Me.m_zgh.GetPane(1).CurveList.Clear()
            End If

            Dim dx As Single = maxX / Me.m_shape.XMax
            Dim MaxY As Single = Me.m_shape.YMax
            Dim lstPts As New PointPairList
            For ipt As Integer = 1 To Me.m_shape.XMax
                lstPts.Add(dx * (ipt - 1), Me.m_shape.ShapeData(ipt) / MaxY)
            Next

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Shape", Definitions.eLineType.NotSet, Color.SandyBrown, lstPts)

            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Dim map As IEnviroInputMap = Me.getSelMap()
            If map IsNot Nothing Then
                maxX = map.Max
            End If

            'Me.m_zgh.AutoscalePane = True
            Me.m_zgh.XScaleMax = maxX
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


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dlgDefineMapResponseAssignments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            Me.PlotShape()
            Me.loadMaps()

            'has to change 
            'shapes don't contain mapX at this time
            Me.txXMax.Text = "10.0"

        Catch ex As Exception

        End Try

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

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Map histogram", Definitions.eLineType.NotSet, Color.RoyalBlue, lstPts)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            'Me.m_zgh.AutoscalePane = True
            Me.m_zgh.XScaleMax = maxX
            Me.m_zgh.YScaleMax = 1.2

        Catch ex As Exception

        End Try

    End Sub


    Private Sub lvMaps_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvMaps.SelectedIndexChanged

        PlotMap()
    End Sub


    Private Sub txXMax_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txXMax.TextChanged
        Dim maxX As Single = Single.Parse(Me.txXMax.Text)
        Me.m_shape.XAxisMax = maxX

        PlotShape()
        PlotMap()

    End Sub
End Class

