Imports EwECore

Public Class frmSelectFleetOnly
    Inherits CreateCollectionForData

    Public Event FormExited()

    Public Sub New(ByVal i As cSelectionData, ByVal m_core As cCore)
        MyBase.New(i, m_core)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Width = 380
        chklstAttached.Hide()
        btnAttachAll.Hide()
        btnAttachNone.Hide()
        btnOk.Left = 280
        Me.Show()

    End Sub
    Public Overrides Sub PopulateAttachedList(ByVal i As String)

    End Sub

    Private Sub frmSelectFleetOnly_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class