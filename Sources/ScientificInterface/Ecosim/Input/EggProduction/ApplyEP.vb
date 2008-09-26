'==============================================================================
'
' $Log: ApplyEP.vb,v $
' Revision 1.1  2008/09/26 07:31:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/06/06 16:01:36  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.21  2008/06/02 00:07:44  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.20  2008/04/07 02:31:05  jeroens
' Cleaning up resources
'
' Revision 1.19  2008/02/19 13:08:21  jeroens
' Local QuickEditHandler Set label and Button follow combo box enabled behaviour
'
' Revision 1.18  2008/01/27 03:07:48  jeroens
' Fixed bug 227
'
' Revision 1.17  2008/01/25 17:43:33  jeroens
' InitForm called when form needs to refresh
'
' Revision 1.16  2007/11/13 15:35:08  jeroens
' * Fixed bug 227
'
' Revision 1.15  2007/11/04 02:02:42  jeroens
' * Fixed bug 325
'
' Revision 1.14  2007/10/29 14:04:59  jeroens
' * Updated to reworked shape controls
'
' Revision 1.13  2007/10/20 02:50:38  jeroens
' * ApplyEP form did not update to shape updates
'
' Revision 1.12  2007/10/16 13:49:19  jeroens
' + Added header
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Drawing.Drawing2D
Imports EwEUtils.Core

#End Region

Namespace Ecosim
    Public Class ApplyEP

#Region "Constructor"

        Private m_Core As cCore

        Private m_EPManager As cEggProductionManager
        Private m_bInUpdate As Boolean = False

        Private Const ICON_WIDTH As Integer = 48
        Private Const ICON_HEIGHT As Integer = 48

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            m_Core = cCore.GetInstance()
            m_EPManager = m_Core.EggProdShapeManager

        End Sub

        Public Sub New(ByVal text As String)

            Me.New()
            'Set tab text
            Me.TabText = text
            'Set window text
            Me.Text = text

        End Sub

#End Region

#Region " Event handlers "

        Private Sub ApplyFF_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' Apply resources text to Set button
            Me.m_tsbSet.Text = My.Resources.LABEL_SET
            ' Init the form to the current data
            InitForm()
            ' Hook up to baseclass refresh
            Me.MessageSources = New eMessageSource() {eMessageSource.EcoPath, eMessageSource.ShapesManager}
        End Sub

        Private Sub ApplyEP_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            ' Disconnect from baseclass refresh
            Me.MessageSources = Nothing
        End Sub

        Private Sub m_tscEggProdShapes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tscEggProdShapes.SelectedIndexChanged
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            Me.m_grid.SelectShapeName(Me.m_tscEggProdShapes.Text)
            Me.m_bInUpdate = False
        End Sub

        Private Sub tsbSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbSet.Click
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            Me.m_grid.SelectShapeName(Me.m_tscEggProdShapes.Text)
            Me.m_bInUpdate = False
        End Sub

        Private Sub m_lvShapes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_lvShapes.SelectedIndexChanged
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            If m_lvShapes.SelectedItems.Count = 1 Then
                Me.m_grid.SelectShapeName(Me.m_lvShapes.SelectedItems(0).Text)
            Else
                Me.m_grid.SelectShapeName("")
            End If
            Me.m_bInUpdate = False
        End Sub

        Private Sub m_grid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
            Me.UpdateSetControls()
        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub InitForm()

            ' JS 20sep07: Pragmatic fix, also test for available egg production shapes
            If m_Core.nStanzas > 0 And m_Core.EggProdShapeManager.Count > 0 Then
                Me.m_tlpContent.Visible = True
                Me.m_lblNoStanza.Visible = False
                Me.LoadShapes()
            Else
                Me.m_lblNoStanza.Visible = True
                Me.m_tlpContent.Visible = False
            End If

            Me.UpdateSetControls()

        End Sub

        Private Sub LoadShapes()

            'Set up the thumbnail image size
            Dim largeImageList As New ImageList
            Dim item As ListViewItem = Nothing
            Dim rcItem As Rectangle = New Rectangle(0, 0, ICON_WIDTH, ICON_HEIGHT)
            Dim bmp As Bitmap = Nothing
            Dim g As Graphics = Nothing
            Dim iItemIndex As Integer = 0
            Dim astrShapeNames As String()
            Dim strSelection As String = Me.m_tscEggProdShapes.Text

            If Me.m_bInUpdate Then Return

            Me.m_bInUpdate = True

            Try

                'Clear the thumbnail list
                m_lvShapes.Items.Clear()

                largeImageList.ImageSize = rcItem.Size

                If m_EPManager.Count > 0 Then

                    For Each shapeFunc As cForcingFunction In m_EPManager

                        ' Create image
                        bmp = New Bitmap(ICON_WIDTH, ICON_HEIGHT)
                        g = Graphics.FromImage(bmp)
                        ShapeImage.DrawShape(shapeFunc, rcItem, g, Color.Red, eSketchDrawModeTypes.Fill, Math.Max(2.0!, shapeFunc.YMax))
                        largeImageList.Images.Add(bmp)
                        g.Dispose()

                        ' Create label
                        item = New ListViewItem(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (shapeFunc.ID + 1), shapeFunc.Name))
                        item.ImageIndex = iItemIndex
                        m_lvShapes.Items.Add(item)

                        iItemIndex += 1

                    Next shapeFunc

                    m_lvShapes.View = View.LargeIcon
                    m_lvShapes.Items(0).Selected = True
                    m_lvShapes.LargeImageList = largeImageList

                End If

                Me.m_grid.ResetData()

                Me.m_tscEggProdShapes.Items.Clear()
                astrShapeNames = Me.m_grid.GetEPShapeNames()
                For Each str As String In astrShapeNames
                    Me.m_tscEggProdShapes.Items.Add(str)
                Next

                Me.m_tscEggProdShapes.SelectedIndex = Me.m_tscEggProdShapes.FindStringExact(strSelection)

            Catch ex As Exception

            End Try

            Me.m_bInUpdate = False

        End Sub

        ''' <summary>
        ''' Update the controls in the toolbar that allow the user to set a range of cells
        ''' to a specific shape. These controls implement a local version of
        ''' EwEGridForm.QuickEditHandler
        ''' </summary>
        Private Sub UpdateSetControls()

            ' Hackittyhack: enable when the shape interface is visible and the non-empty grid selection includes the shapes column
            Dim bEnabled As Boolean = (Me.m_tlpContent.Visible = True) And _
                                      (Me.m_grid.Selection.GetRange.IsEmpty = False) And _
                                      (Me.m_grid.Selection.GetRange.ContainsColumn(ApplyEPEwEGrid.eColumnTypes.Shape))
            Me.m_tlbSet.Enabled = bEnabled
            Me.m_tscEggProdShapes.Enabled = bEnabled
            Me.m_tsbSet.Enabled = bEnabled

        End Sub
#End Region ' Internals

#Region " Mandatory overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            Dim bRefreshGrid As Boolean = False
            Dim bRefreshForm As Boolean = False

            ' Check for relevant messages:
            ' * Refresh on any ShapesManager EggProd message
            If ((msg.Source = eMessageSource.ShapesManager) And (msg.DataType = eDataTypes.EggProd)) Then
                bRefreshGrid = (msg.Type = eMessageType.DataModified)
                bRefreshForm = (msg.Type = eMessageType.DataAddedOrRemoved)
            End If

            ' * Refresh on Ecopath stanza additions or removals
            If ((msg.Source = eMessageSource.EcoPath) And (msg.DataType = eDataTypes.Stanza)) Then
                bRefreshForm = (msg.Type = eMessageType.DataAddedOrRemoved)
            End If

            'If bRefreshGrid Then Me.m_grid.ResetData()
            If bRefreshForm Then Me.InitForm()

        End Sub

#End Region ' Mandatory overrides

    End Class

End Namespace
