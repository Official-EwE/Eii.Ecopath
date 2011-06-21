Imports System.Windows.Forms

Public Class ucSelector

    Private m_pg As PropertyGrid = Nothing
    Private m_selection As Object() = Nothing
    Private m_iSel As Integer = 0

    Public Property PropertyGrid() As PropertyGrid
        Get
            Return Me.m_pg
        End Get
        Set(ByVal value As PropertyGrid)
            Me.m_pg = value
        End Set
    End Property

    Public Property Selection() As Object
        Get
            Return Me.m_selection
        End Get
        Set(ByVal value As Object)
            Dim lObj As New List(Of Object)
            If TypeOf value Is Array Then
                For Each obj As Object In DirectCast(value, Array)
                    If TypeOf obj Is cLink Then
                        If DirectCast(obj, cLink).IsValid Then
                            lObj.Add(obj)
                        End If
                    Else
                        lObj.Add(obj)
                    End If
                Next
            Else
                lObj.Add(value)
            End If
            Me.m_selection = lObj.ToArray
            Me.m_iSel = 0
            Me.UpdateSelection()
        End Set
    End Property

    Private Sub OnPrev(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnPrev.Click
        Me.m_iSel = (Me.m_iSel + Me.m_selection.Length - 1) Mod Me.m_selection.Length
        Me.UpdateSelection()
    End Sub

    Private Sub OnNext(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRight.Click
        Me.m_iSel = (Me.m_iSel + Me.m_selection.Length + 1) Mod Me.m_selection.Length
        Me.UpdateSelection()
    End Sub

    Private Sub UpdateSelection()

        Dim obj As Object = Nothing
        Dim strText As String = ""

        Try
            If (Me.m_selection IsNot Nothing) Then
                If (Me.m_selection.Length > 0) Then
                    obj = Me.m_selection(Me.m_iSel)
                    strText = Me.m_selection(Me.m_iSel).ToString
                End If
            End If
        Catch ex As Exception

        End Try

        Me.UpdateControls()

        If Me.m_pg IsNot Nothing Then
            Me.m_pg.SelectedObject = obj
        End If

        If (Me.m_lblInfo IsNot Nothing) Then
            Me.m_lblInfo.Text = strText
        End If
    End Sub

    Private Sub UpdateControls()

        If (Me.m_selection Is Nothing) Then
            Me.Visible = False
        Else
            Me.m_btnPrev.Enabled = (Me.m_iSel > 0)
            Me.m_btnRight.Enabled = (Me.m_iSel < Me.m_selection.Length - 1)
            Me.Visible = (Me.m_selection.Length > 1)
        End If

    End Sub

End Class
