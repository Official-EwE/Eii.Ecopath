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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls
Imports ValueChain

#End Region ' Imports

Public Class ucDefaults

#Region " Helper class "

    Private Class cValueChainEntityComboItem

        Private m_obj As cValueChainEntity = Nothing
        Private m_strTitle As String = ""

        Public Sub New(obj As cValueChainEntity, strTitle As String)
            Me.m_obj = obj
            Me.m_strTitle = strTitle
        End Sub

        Public Function ObjDefault() As cValueChainEntity
            Return Me.m_obj
        End Function

        Public Overrides Function ToString() As String
            Return Me.m_strTitle
        End Function

    End Class

#End Region ' Helper class

#Region " Private vars "

    Private m_data As cValueChainData = Nothing
    Private m_dtDefaults As New Dictionary(Of cValueChainEntity, ucDefault)
    Private m_bInUpdate As Boolean = False
    Private m_objSelected As cValueChainEntity = Nothing
    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

    Public Sub New(uic As cUIContext, data As cValueChainData)
        Me.InitializeComponent()

        Me.m_uic = uic
        Me.m_data = data

        Try
            ' ToDo: globalize this

            ' Init defaults
            Me.AddControl(Me.m_lbProducer, Me.m_data.GetUnitDefault(Me.m_lbProducer.UnitType), "Producer")
            Me.AddControl(Me.m_lnkProd2Proc, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.ProducerToProcessing), "Producer to Processing")
            Me.AddControl(Me.m_lbProcessing, Me.m_data.GetUnitDefault(Me.m_lbProcessing.UnitType), "Processing")
            Me.AddControl(Me.m_lnkProc2Dist, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.ProcessingToDistribution), "Processing to Distribution")
            Me.AddControl(Me.m_lbDistribution, Me.m_data.GetUnitDefault(Me.m_lbDistribution.UnitType), "Distribution")
            Me.AddControl(Me.m_lnkDist2Whole, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.DistributionToWholeseller), "Distribution to Wholeseller")
            Me.AddControl(Me.m_lbWholesaler, Me.m_data.GetUnitDefault(Me.m_lbWholesaler.UnitType), "Wholeseller")
            Me.AddControl(Me.m_lnkWhole2Ret, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.WholesellerToRetailer), "Wholeseller to Retailer")
            Me.AddControl(Me.m_lbRetailer, Me.m_data.GetUnitDefault(Me.m_lbRetailer.UnitType), "Retailer")
            Me.AddControl(Me.m_lnkRet2Cons, Me.m_data.GetLinkDefault(cLinkFactory.eLinkType.RetailerToConsumer), "Retailer to Consumer")
            Me.AddControl(Me.m_lbConsumer, Me.m_data.GetUnitDefault(Me.m_lbConsumer.UnitType), "Consumer")

        Catch ex As Exception

        End Try

    End Sub

    Private Sub ucDefaults_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        Me.RemoveControl(Me.m_lbProducer)
        Me.RemoveControl(Me.m_lnkProd2Proc)
        Me.RemoveControl(Me.m_lbProcessing)
        Me.RemoveControl(Me.m_lnkProc2Dist)
        Me.RemoveControl(Me.m_lbDistribution)
        Me.RemoveControl(Me.m_lnkDist2Whole)
        Me.RemoveControl(Me.m_lbWholesaler)
        Me.RemoveControl(Me.m_lnkWhole2Ret)
        Me.RemoveControl(Me.m_lbRetailer)
        Me.RemoveControl(Me.m_lnkRet2Cons)
        Me.RemoveControl(Me.m_lbConsumer)
        Me.m_data = Nothing
    End Sub

#Region " Events "

    Private Sub OnClickControl(sender As System.Object, e As System.EventArgs)
        If TypeOf sender Is ucDefault Then
            Me.SelectedObject = DirectCast(sender, ucDefault).ObjDefault
        End If
    End Sub

    Private Sub OnSelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cbDefault.SelectedIndexChanged
        Me.SelectedObject() = Me.SelectedComboItem()
    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub AddControl(c As ucDefault, obj As cValueChainEntity, strTitle As String)
        Me.m_dtDefaults.Add(obj, c)
        c.ObjDefault = obj
        c.Text = strTitle
        c.UIContext = Me.m_uic
        AddHandler c.Click, AddressOf Me.OnClickControl

        Me.m_cbDefault.Items.Add(New cValueChainEntityComboItem(obj, strTitle))
    End Sub

    Private Sub RemoveControl(c As ucDefault)
        c.UIContext = Nothing
        Me.m_dtDefaults.Remove(c.ObjDefault)
        RemoveHandler c.Click, AddressOf Me.OnClickControl

        Me.m_cbDefault.Items.RemoveAt(Me.FindComboItem(c.ObjDefault))
    End Sub

    Private Function FindComboItem(obj As cValueChainEntity) As Integer
        Dim item As cValueChainEntityComboItem = Nothing
        For iItem As Integer = 0 To Me.m_cbDefault.Items.Count - 1
            If TypeOf Me.m_cbDefault.Items(iItem) Is cValueChainEntityComboItem Then
                item = DirectCast(Me.m_cbDefault.Items(iItem), cValueChainEntityComboItem)
                If ReferenceEquals(item.ObjDefault, obj) Then
                    Return iItem
                End If
            End If
        Next
        Return -1
    End Function

    Private Function SelectedComboItem() As cValueChainEntity
        Dim obj As Object = Me.m_cbDefault.SelectedItem
        If TypeOf obj Is cValueChainEntityComboItem Then
            Return DirectCast(obj, cValueChainEntityComboItem).ObjDefault
        End If
        Return Nothing
    End Function

    Private Property SelectedObject() As cValueChainEntity
        Get
            Return Me.m_objSelected
        End Get
        Set(objSelNew As cValueChainEntity)
            ' Optimization
            If Not ReferenceEquals(objSelNew, Me.m_objSelected) Then

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