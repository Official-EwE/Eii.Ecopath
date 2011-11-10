Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

Public Class ucSelector2

    Private m_uic As cUIContext = Nothing
    Private m_data As cData = Nothing
    Private m_pg As PropertyGrid = Nothing
    Private m_selection As Object() = Nothing
    Private m_iSel As Integer = 0
    Private m_bCanAddRemoveItems As Boolean = False

    Public Sub Init(uic As cUIContext, data As cData, pg As PropertyGrid)
        Me.m_pg = pg
        Me.m_data = data
    End Sub

    Public Property Selection() As Object
        Get
            Return Me.m_selection
        End Get
        Set(ByVal value As Object)

            ' Gather selected objects
            Dim lObj As New List(Of Object)
            ' Assume the worst
            Me.m_bCanAddRemoveItems = False
            ' Explore incoming parameters
            If TypeOf value Is Array Then
                For Each obj As Object In DirectCast(value, Array)
                    If (TypeOf obj Is cLink) Then
                        If DirectCast(obj, cLink).IsVisible Then
                            lObj.Add(obj)
                        End If
                    Else
                        lObj.Add(obj)
                    End If
                    ' Ugh
                    Me.m_bCanAddRemoveItems = (TypeOf (obj) Is cLink And Not TypeOf (obj) Is cLinkLandings)
                Next
            Else
                lObj.Add(value)
            End If
            Me.m_selection = lObj.ToArray
            Me.m_iSel = 0
            Me.Fill()
        End Set
    End Property

    'Private Sub OnPrev(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Me.m_iSel = (Me.m_iSel + Me.m_selection.Length - 1) Mod Me.m_selection.Length
    '    Me.UpdateSelection()
    'End Sub

    'Private Sub OnNext(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Me.m_iSel = (Me.m_iSel + Me.m_selection.Length + 1) Mod Me.m_selection.Length
    '    Me.UpdateSelection()
    'End Sub

    Private Sub Fill()
        Me.m_lbxBits.Items.Clear()
        If (Me.m_selection IsNot Nothing) Then
            Try
                If (Me.m_selection.Length > 0) Then
                    For i As Integer = 0 To Me.m_selection.Length - 1
                        Me.m_lbxBits.Items.Add(Me.m_selection(i))
                    Next
                End If
                Me.m_lbxBits.SelectedIndex = Me.m_iSel
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub UpdateSelection()

        Dim obj As Object = Nothing

        Try
            If (Me.m_selection IsNot Nothing) Then
                If (Me.m_selection.Length > 0) Then
                    obj = Me.m_selection(Me.m_iSel)
                End If
            End If
        Catch ex As Exception

        End Try

        Me.UpdateControls()

        If Me.m_pg IsNot Nothing Then
            Me.m_pg.SelectedObject = obj
        End If

    End Sub

    Private Sub UpdateControls()

        Me.m_plControls.Visible = Me.m_bCanAddRemoveItems

        If (Me.m_lbxBits.Items.Count > 1) Or (Me.m_bCanAddRemoveItems) Then
            Me.m_btnAdd.Enabled = (Me.m_lbxBits.Items.Count > 1)
            Me.m_btnRemove.Enabled = (Me.m_lbxBits.Items.Count > 1)
        Else
            Me.Visible = False
        End If

    End Sub

    Private Sub OnAddItem(sender As System.Object, e As System.EventArgs) _
        Handles m_btnAdd.Click

    End Sub

    Private Sub OnRemoveItem(sender As System.Object, e As System.EventArgs) _
        Handles m_btnRemove.Click

    End Sub

    Private Sub OnSelectItem(sender As System.Object, e As System.EventArgs) _
        Handles m_lbxBits.SelectedIndexChanged
        Me.m_iSel = Me.m_lbxBits.SelectedIndex
        Me.UpdateSelection()
    End Sub

End Class
