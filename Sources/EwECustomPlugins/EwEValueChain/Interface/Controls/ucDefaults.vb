#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class ucDefaults

#Region " Helper class "

    Private Class cOOPStorableComboItem

        Private m_obj As cOOPStorable = Nothing
        Private m_strTitle As String = ""

        Public Sub New(ByVal obj As cOOPStorable, ByVal strTitle As String)
            Me.m_obj = obj
            Me.m_strTitle = strTitle
        End Sub

        Public Function ObjDefault() As cOOPStorable
            Return Me.m_obj
        End Function

        Public Overrides Function ToString() As String
            Return Me.m_strTitle
        End Function

    End Class

#End Region ' Helper class

#Region " Private vars "

    Private m_data As cData = Nothing
    Private m_dtDefaults As New Dictionary(Of cOOPStorable, ucDefault)
    Private m_bInUpdate As Boolean = False
    Private m_objSelected As cOOPStorable = Nothing

#End Region ' Private vars

    Public Sub New(ByVal data As cData)
        Me.InitializeComponent()

        Me.m_data = data

        ' Init defaults
        Me.AddControl(Me.m_lbProducer, Me.m_data.GetUnitDefault(cUnitFactory.eUnitType.Producer), "Producer")
        Me.AddControl(Me.m_lnkProd2Proc, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.ProducerToProcessing), "Producer to Processing")
        Me.AddControl(Me.m_lbProcessing, Me.m_data.GetUnitDefault(cUnitFactory.eUnitType.Processing), "Processing")
        Me.AddControl(Me.m_lnkProc2Dist, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.ProcessingToDistribution), "Processing to Distribution")
        Me.AddControl(Me.m_lbDistribution, Me.m_data.GetUnitDefault(cUnitFactory.eUnitType.Distribution), "Distribution")
        Me.AddControl(Me.m_lnkDist2Mkt, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.DistributionToMarket), "Distribution to Market")
        Me.AddControl(Me.m_lbMarket, Me.m_data.GetUnitDefault(cUnitFactory.eUnitType.Market), "Market")
        Me.AddControl(Me.m_lnkMkt2Cons, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.MarketToConsumer), "Market to Consumer")
        Me.AddControl(Me.m_lbConsumer, Me.m_data.GetUnitDefault(cUnitFactory.eUnitType.Consumer), "Consumer")
    End Sub

    Private Sub ucDefaults_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Me.RemoveControl(Me.m_lbProducer)
        Me.RemoveControl(Me.m_lnkProd2Proc)
        Me.RemoveControl(Me.m_lbProcessing)
        Me.RemoveControl(Me.m_lnkProc2Dist)
        Me.RemoveControl(Me.m_lbDistribution)
        Me.RemoveControl(Me.m_lnkDist2Mkt)
        Me.RemoveControl(Me.m_lbMarket)
        Me.RemoveControl(Me.m_lnkMkt2Cons)
        Me.RemoveControl(Me.m_lbConsumer)
        Me.m_data = Nothing
    End Sub

#Region " Events "

    Private Sub OnClickControl(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If TypeOf sender Is ucDefault Then
            Me.SelectedObject = DirectCast(sender, ucDefault).ObjDefault
        End If
    End Sub

    Private Sub OnSelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbDefault.SelectedIndexChanged
        Me.SelectedObject() = Me.SelectedComboItem()
    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub AddControl(ByVal c As ucDefault, ByVal obj As cOOPStorable, ByVal strTitle As String)
        Me.m_dtDefaults.Add(obj, c)
        c.ObjDefault = obj
        c.Text = strTitle
        AddHandler c.Click, AddressOf OnClickControl

        Me.m_cbDefault.Items.Add(New cOOPStorableComboItem(obj, strTitle))
    End Sub

    Private Sub RemoveControl(ByVal c As ucDefault)
        Me.m_dtDefaults.Remove(c.ObjDefault)
        RemoveHandler c.Click, AddressOf OnClickControl

        Me.m_cbDefault.Items.RemoveAt(Me.FindComboItem(c.ObjDefault))
    End Sub

    Private Function FindComboItem(ByVal obj As cOOPStorable) As Integer
        Dim item As cOOPStorableComboItem = Nothing
        For iItem As Integer = 0 To Me.m_cbDefault.Items.Count - 1
            If TypeOf Me.m_cbDefault.Items(iItem) Is cOOPStorableComboItem Then
                item = DirectCast(Me.m_cbDefault.Items(iItem), cOOPStorableComboItem)
                If Object.ReferenceEquals(item.ObjDefault, obj) Then
                    Return iItem
                End If
            End If
        Next
        Return -1
    End Function

    Private Function SelectedComboItem() As cOOPStorable
        Dim obj As Object = Me.m_cbDefault.SelectedItem
        If TypeOf obj Is cOOPStorableComboItem Then
            Return DirectCast(obj, cOOPStorableComboItem).ObjDefault
        End If
        Return Nothing
    End Function

    Private Property SelectedObject() As cOOPStorable
        Get
            Return Me.m_objSelected
        End Get
        Set(ByVal objSelNew As cOOPStorable)
            ' Optimization
            If Not Object.ReferenceEquals(objSelNew, Me.m_objSelected) Then

                ' Prevent loops
                If Me.m_bInUpdate = True Then Return

                ' Go at it, Jimmy
                Me.m_bInUpdate = True

                If Me.m_objSelected IsNot Nothing Then
                    Me.m_dtDefaults(Me.m_objSelected).Selected = False
                End If

                Me.m_objSelected = objSelNew

                ' Sync controls
                Me.m_cbDefault.SelectedIndex = Me.FindComboItem(Me.m_objSelected)
                Me.m_pgDefaults.SelectedObject = Me.m_objSelected

                If Me.m_objSelected IsNot Nothing Then
                    Me.m_dtDefaults(Me.m_objSelected).Selected = True
                End If

                Me.m_bInUpdate = False
            End If
        End Set
    End Property

#End Region ' Internals

End Class