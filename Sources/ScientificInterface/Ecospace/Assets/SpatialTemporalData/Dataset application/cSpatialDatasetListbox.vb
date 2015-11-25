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
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Controls

    ' ToDo: respond to indexing changes
    ' ToDo: respond to dataset changes

    Public Class cSpatialDatasetListbox
        Inherits ListBox
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_filter As eVarNameFlags = Nothing
        Private m_manConn As cSpatialDataConnectionManager = Nothing
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
                    Me.m_manConn = Nothing
                    Me.m_manSets = Nothing
                    If (Me.m_mhEcospace IsNot Nothing) Then
                        Me.UIContext.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                        Me.m_mhEcospace.Dispose()
                        Me.m_mhEcospace = Nothing
                    End If
                End If
                Me.m_uic = uic
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_manConn = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_manConn.DatasetManager
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

            Me.SuspendLayout()
            Me.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_manSets
                If (Me.m_filter = eVarNameFlags.NotSet) Or (ds.VarName = eVarNameFlags.NotSet) Or ((ds.VarName = Me.m_filter)) Then
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
            Dim img As Image = cSystemUtils.IIF(Me.m_manConn.IsApplied(ds), SharedResources.Database, SharedResources.database_NA)
            Dim clrText As Color = e.ForeColor
            Dim fmt As New StringFormat(StringFormatFlags.NoWrap)
            fmt.LineAlignment = StringAlignment.Center
            fmt.Trimming = StringTrimming.EllipsisWord

            If Not Me.Enabled Then
                clrText = SystemColors.GrayText
            End If

            ' Render default background 
            e.DrawBackground()

            If (img IsNot Nothing) Then
                ' Render image
                e.Graphics.DrawImage(img, e.Bounds.X + 2, e.Bounds.Y + 2, 16, 16)
            End If
            ' Render default text, bumped to the right by 22 pixels
            Using br As New SolidBrush(clrText)
                Dim rcText As New Rectangle(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 22, e.Bounds.Height)
                e.Graphics.DrawString(ds.DisplayName, e.Font, br, rcText, fmt)
            End Using

            ' Render default focus rectangle
            e.DrawFocusRectangle()

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

                End If

            Catch ex As Exception

            End Try

        End Sub

    End Class

End Namespace
