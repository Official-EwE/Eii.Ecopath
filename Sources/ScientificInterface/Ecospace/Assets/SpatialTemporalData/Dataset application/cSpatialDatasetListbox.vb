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

Option Strict On
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore

#End Region ' Imports

Namespace Ecospace.Controls

    ' ToDo: respond to indexing changes
    ' ToDo: respond to dataset changes

    Public Class cSpatialDatasetListbox
        Inherits ListBox
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_filter As eVarNameFlags = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing
        Private m_mhEcospace As cMessageHandler = Nothing

        Public Sub New()
            MyBase.New()
            Me.DrawMode = Windows.Forms.DrawMode.OwnerDrawFixed
            Me.ItemHeight = SharedResources.Database.Height + 4
        End Sub

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As ScientificInterfaceShared.Controls.cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_manSets = Nothing
                    If (Me.m_mhEcospace IsNot Nothing) Then
                        Me.UIContext.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                        Me.m_mhEcospace.Dispose()
                        Me.m_mhEcospace = Nothing
                    End If
                End If
                Me.m_uic = uic
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_manSets = Me.m_uic.Core.SpatialDataConnectionManager.DatasetManager
                    Me.m_mhEcospace = New cMessageHandler(AddressOf OnCoreMessage, EwEUtils.Core.eCoreComponentType.External, eMessageType.Progress, Me.UIContext.SyncObject)
                    Me.UIContext.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
#If DEBUG Then
                    Me.m_mhEcospace.Name = "cSpatialDatasetListbox"
#End If
                    Me.RefreshContent()
                End If
            End Set
        End Property

        Public Property Filter As eVarNameFlags
            Get
                Return Me.m_filter
            End Get
            Set(value As eVarNameFlags)
                If (value = Me.m_filter) Then Return
                Me.m_filter = value
                Me.RefreshContent()
            End Set
        End Property

        Public Sub RefreshContent()

            Dim dsSel As ISpatialDataSet = Nothing

            Me.SuspendLayout()
            Me.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_manSets
                If (Me.m_filter = eVarNameFlags.NotSet) Or ((ds.VarName = Me.m_filter) Or (ds.VarName = eVarNameFlags.NotSet)) Then
                    Me.Items.Add(ds)
                End If
            Next
            Me.ResumeLayout()

        End Sub

        Public ReadOnly Property SelectedDataset As ISpatialDataSet
            Get
                Dim item As Object = Me.SelectedItem
                If (item Is Nothing) Then Return Nothing
                Return DirectCast(item, ISpatialDataSet)
            End Get
        End Property

        Protected Overrides Sub OnDrawItem(e As System.Windows.Forms.DrawItemEventArgs)

            ' Sanity check
            If (e.Index >= Me.Items.Count Or e.Index < 0) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim item As Object = Me.Items(e.Index)
            Dim ds As ISpatialDataSet = DirectCast(item, ISpatialDataSet)

            If (ds Is Nothing) Then Return

            Dim comp As New cDatasetCompatilibity(Me.m_uic.Core, ds)
            Dim img As Image = cStyleGuide.GetImage(comp)
            Dim strStatus As String = ""
            Dim clrText As Color = e.ForeColor
            Dim fmt As New StringFormat(StringFormatFlags.NoWrap)
            fmt.LineAlignment = StringAlignment.Center
            fmt.Trimming = StringTrimming.EllipsisWord

            If Not Me.Enabled Then
                clrText = SystemColors.GrayText
            End If

            ' Render default background 
            e.DrawBackground()

            ' Render compatibility image
            If Me.m_manSets.IsIndexing(ds) Then
                ' ToDo: globalize this
                strStatus = String.Format("indexing " & comp.PercentIndexed & "%")
                img = SharedResources.Question
            Else
                Dim sdcf As New cSpatialDatasetCompatibilityFormatter()
                strStatus = sdcf.GetDescriptor(comp)
            End If

            If (img IsNot Nothing) Then
                ' Render image
                e.Graphics.DrawImage(img, e.Bounds.X + 2, e.Bounds.Y + 2, 16, 16)
            End If
            ' Render default text, bumped to the right by 22 pixels
            Using br As New SolidBrush(clrText)
                Dim rcText As New Rectangle(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 22, e.Bounds.Height)
                e.Graphics.DrawString(String.Format(SharedResources.GENERIC_LABEL_DETAILED, ds.DisplayName, strStatus), _
                                      e.Font, br, rcText, fmt)
            End Using

            ' Render default focus rectangle
            e.DrawFocusRectangle()

        End Sub

        Private m_strTooltipText As String = ""

        Protected Overrides Sub OnMouseMove(e As System.Windows.Forms.MouseEventArgs)
            Me.UpdateTooltip(False)
            MyBase.OnMouseMove(e)
        End Sub

        Protected Overrides Sub OnMouseHover(e As System.EventArgs)
            Me.UpdateTooltip(True)
            MyBase.OnMouseHover(e)
        End Sub

        Private Sub OnCoreMessage(ByRef msg As cMessage)

            Try
                ' May have been disposed already
                If (msg.DataType = EwEUtils.Core.eDataTypes.EcospaceSpatialDataConnection) Then
                    Select Case msg.Type

                        Case eMessageType.DataModified
                            Me.Invoke(New MethodInvoker(AddressOf Me.RefreshContent))

                        Case eMessageType.DataAddedOrRemoved
                            Me.Invalidate()

                    End Select
                    Me.UpdateTooltip(False)

                End If

            Catch ex As Exception

            End Try

        End Sub

        Private Sub UpdateTooltip(bShowTip As Boolean)

            If (Me.UIContext Is Nothing) Then Return

            Dim strTextOld As String = Me.m_strTooltipText

            If (Not bShowTip) Then
                Me.m_strTooltipText = ""
            Else
                If (String.IsNullOrWhiteSpace(Me.m_strTooltipText)) Then
                    Dim pt As Point = Me.PointToClient(MousePosition)
                    Dim i As Integer = Me.IndexFromPoint(pt)
                    If (i >= 0) Then
                        Dim ds As ISpatialDataSet = CType(Me.Items(i), ISpatialDataSet)
                        Dim comp As New cDatasetCompatilibity(Me.UIContext.Core, ds)
                        Me.m_strTooltipText = comp.ToString
                    End If
                End If
            End If

            If String.Compare(Me.m_strTooltipText, strTextOld) <> 0 Then
                cToolTipShared.GetInstance().SetToolTip(Me, Me.m_strTooltipText)
            End If

        End Sub

    End Class

End Namespace
