'==============================================================================
'
' $Log: ucLayerEditorMigration.vb,v $
' Revision 1.2  2008/11/06 01:21:25  jeroens
' UpdateControls made 'safe'
'
' Revision 1.1  2008/11/04 04:40:34  jeroens
' Split into separate files, moved
'
' Revision 1.3  2008/10/15 23:57:33  jeroens
' Implemented
'
' Revision 1.2  2008/10/15 17:03:58  jeroens
' Reworking
'
' Revision 1.1  2008/10/14 20:21:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorMigration

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Private Sub OnMonthChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbMonth.SelectedIndexChanged
            Me.Editor.CellValue = Me.m_cmbMonth.SelectedIndex + 1
        End Sub

        Private Sub OnGroupChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbGroup.SelectedIndexChanged
            Me.Editor.Group = Me.m_cmbGroup.SelectedIndex + 1
        End Sub

        Public Overrides Sub EndEdit()
            If Me.m_chkAutoRotate.Checked Then
                Me.Editor.CellValue = CInt(CInt(Me.Editor.CellValue) Mod cCore.N_MONTHS) + 1
            End If
        End Sub

        Private Sub DoLoad(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

            ' Initialize group combo 
            Dim core As cCore = cCore.GetInstance()
            Dim group As cEcoPathGroupInput = Nothing

            Me.m_cmbGroup.Items.Clear()

            ' ToDo: this control will not respond to dynamic group name changes
            For iGroup As Integer = 1 To core.nLivingGroups
                group = core.EcoPathGroupInputs(iGroup)
                Me.m_cmbGroup.Items.Add(group.Name)
            Next iGroup

            Me.UpdateControls()
        End Sub

        Public Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            If (Me.m_cmbGroup Is Nothing) Then Return
            If (Me.m_cmbMonth Is Nothing) Then Return
            If (Me.m_chkAutoRotate Is Nothing) Then Return

            Try
                Me.m_cmbMonth.SelectedIndex = CInt(Me.Editor.CellValue) - 1
                Me.m_cmbGroup.SelectedIndex = CInt(Me.Editor.Group) - 1
            Catch ex As Exception

            End Try

        End Sub

        Protected Overloads Property Editor() As cLayerEditorMigration
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorMigration)
            End Get
            Set(ByVal editor As cLayerEditorMigration)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorMigration, "ucLayerEditorMigration connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 1
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

    End Class

End Namespace

