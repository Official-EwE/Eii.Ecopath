Imports Couplerlib
Imports System.Windows.Forms
Public Class FormHabitat
    Dim cplib As CCouplerlib
    Dim fsname As String
    Dim autorescale As Boolean
    Dim boundon As Boolean
    Dim boundaries(4) As Integer
    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick, DataGridView1.CellClick
        If (e.ColumnIndex() = 3) Or (e.ColumnIndex() = 7) Then
            DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Convert.ToString(DateTimePicker1.Value)
        End If
    End Sub

    Private Sub FormHabitat_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim a As Integer
    End Sub

    Private Sub DataGridView1_RowsAdded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles DataGridView1.RowsAdded
        Dim n As Integer
        For n = e.RowIndex To e.RowIndex + e.RowCount - 1
            DataGridView1.Rows(n).Cells(0).Value = Convert.ToString(n + 1)
        Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim b As Boolean = fValidate()
        If b Then
            autorescale = True
            fStore()
            fClose()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        fClose()
    End Sub
    Private Sub fClose()

    End Sub
    Private Sub fStore()
        Dim n As Integer

        Dim norows As Integer = DataGridView1.Rows.GetRowCount(DataGridViewElementStates.Visible)
        Dim dates(3, norows) As DateTime
        Dim minval(3, norows) As Double
        Dim maxval(3, norows) As Double
        Dim name(3, norows) As String
        For n = 0 To norows - 1
            dates(0, n) = Convert.ToDateTime(DataGridView1.Rows(n).Cells(3).Value)
            dates(1, n) = Convert.ToDateTime(DataGridView1.Rows(n).Cells(3).Value)
            dates(2, n) = Convert.ToDateTime(DataGridView1.Rows(n).Cells(7).Value)
            minval(0, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(1).Value)
            minval(1, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(4).Value)
            minval(2, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(8).Value)
            maxval(0, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(2).Value)
            maxval(1, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(5).Value)
            maxval(2, n) = Convert.ToDouble(DataGridView1.Rows(n).Cells(9).Value)
            name(0, n) = "Depth"
            name(1, n) = "Temperature"
            name(2, n) = DataGridView1.Rows(n).Cells(6).Value
        Next
        cplib.setgrid(norows, dates, minval, maxval, name, fsname, boundaries, boundon)
    End Sub
    Private Function fValidate() As Boolean
        Dim b As Boolean = True
        Return b
    End Function

    Public Function SetLinks(ByVal cp As CCouplerlib, ByVal fname As String) As Integer
        Dim n, nr As Integer
        cplib = cp
        fsname = fname
        Dim norows As Integer
        Dim dates(,) As DateTime
        Dim minval(,) As Double
        Dim maxval(,) As Double
        Dim name(,) As String
        If Not (cp Is Nothing) Then
            nr = cplib.getgrid(norows, dates, minval, maxval, name, fname, boundaries, boundon)
            For n = 0 To norows - 1
                DataGridView1.Rows.Add()
                DataGridView1.Rows(n).Cells(0).Value = Convert.ToString(n + 1)
                DataGridView1.Rows(n).Cells(3).Value = Convert.ToString(dates(0, n))
                DataGridView1.Rows(n).Cells(7).Value = Convert.ToString(dates(2, n))
                DataGridView1.Rows(n).Cells(1).Value = Convert.ToString(minval(0, n))
                DataGridView1.Rows(n).Cells(4).Value = Convert.ToString(minval(1, n))
                DataGridView1.Rows(n).Cells(8).Value = Convert.ToString(minval(2, n))
                DataGridView1.Rows(n).Cells(2).Value = Convert.ToString(maxval(0, n))
                DataGridView1.Rows(n).Cells(5).Value = Convert.ToString(maxval(1, n))
                DataGridView1.Rows(n).Cells(9).Value = Convert.ToString(maxval(2, n))
                DataGridView1.Rows(n).Cells(6).Value = name(2, n)
            Next

        Else
            norows = 0
        End If
        TextBox1.Text = Convert.ToString(boundaries(1))
        TextBox2.Text = Convert.ToString(boundaries(2))
        TextBox3.Text = Convert.ToString(boundaries(3))
        TextBox4.Text = Convert.ToString(boundaries(4))
        CheckBox1.Checked = boundon
        Return norows
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim dt As DateTime
        dt = DateTimePicker1.Value
    End Sub

    

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged

            Try
                boundaries(2) = Convert.ToInt32(TextBox2.Text)

            Catch ex As Exception

            boundaries(2) = 9999

            End Try
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
      
            Try
                boundaries(3) = Convert.ToInt32(TextBox3.Text)

            Catch ex As Exception
            boundaries(3) = 0
            End Try

    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged
       
            Try
                boundaries(4) = Convert.ToInt32(TextBox4.Text)

            Catch ex As Exception
            boundaries(4) = 9999

            End Try

    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        boundon = CheckBox1.Checked
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Try
            boundaries(1) = Convert.ToInt32(TextBox1.Text)

        Catch ex As Exception
            boundaries(1) = 0

        End Try
    End Sub
End Class