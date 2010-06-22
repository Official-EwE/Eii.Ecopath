Imports EwECore
Imports System.Drawing

Public Class frmSelectParentOnly
    Inherits CreateCollectionForData

    Private Shared theInstance As frmSelectParentOnly
    Public Event FormExited()

    Public Sub New(ByVal i As cSelectionData, ByVal p As cCore)
        MyBase.New(i, p)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Width = 380
        chklstAttached.Hide()
        btnAttachAll.Hide()
        btnAttachNone.Hide()
        btnOk.Left = 280

    End Sub

    Public Overrides Sub PopulateAttachedList(ByVal i As String)

    End Sub

    Public Shared ReadOnly Property GetInstance(ByVal i As cSelectionData, ByVal p As cCore) As frmSelectParentOnly
        Get
            If theInstance Is Nothing Then
                theInstance = New frmSelectParentOnly(i, p)
            End If
            Return theInstance
        End Get
    End Property

    Private Sub frmSelectParentOnly_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
        theInstance = Nothing
    End Sub

End Class