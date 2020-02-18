' ===============================================================================
' This file is part of the Safenet toolkit.
'
' To use Safenet tools please contact Marta Coll or Jeroen Steenbeek at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports System.Reflection

Public Class DataTableEnumerator(Of T)
    Implements IEnumerator(Of T)

    Private m_enum As IEnumerator(Of DataRow)
    Private m_cols As PropertyInfo()

    Public Sub New(dt As DataTable)

        Me.m_enum = CType(dt.Rows.GetEnumerator(), IEnumerator(Of DataRow))

        Dim type As Type = GetType(T)
        Dim props As PropertyInfo() = type.GetProperties(BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.NonPublic)
        Dim cols As New List(Of PropertyInfo)
        Dim vt As Type = GetType(ValueType)

        For Each prop As PropertyInfo In props
            If dt.Columns.Contains(prop.Name) Then
                Dim col As DataColumn = dt.Columns(prop.Name)
                Dim ct As Type = col.DataType
                Dim pt As Type = prop.PropertyType

                If (pt.IsAssignableFrom(ct)) Or (ct Is GetType(String)) Or
                   (vt.IsAssignableFrom(pt) And vt.IsAssignableFrom(ct)) Then
                    cols.Add(prop)
                End If
            End If
        Next
        Me.m_cols = cols.ToArray()

    End Sub

    Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
        Get
            Dim type As Type = GetType(T)
            Dim inst As T = CType(Activator.CreateInstance(type), T)
            Dim dr As DataRow = CType(Me.IEnumerator_Current, DataRow)
            Dim vt As Type = GetType(ValueType)

            For Each prop As PropertyInfo In Me.m_cols
                Try
                    Dim val As Object = dr(prop.Name)
                    Dim pt As Type = prop.PropertyType
                    Dim st As Type = val.GetType()
                    Dim bNeedDefault As Boolean = False

                    ' Fix DBNull
                    If Convert.IsDBNull(val) Then
                        bNeedDefault = True
                    Else
                        If (Not pt.IsAssignableFrom(st)) Then
                            If pt.IsEnum Then
                                Try
                                    val = [Enum].Parse(pt, CStr(val))
                                Catch ex As Exception

                                End Try
                            ElseIf (vt.IsAssignableFrom(pt) And vt.IsAssignableFrom(st)) Then
                                val = Convert.ChangeType(val, prop.PropertyType)
                            ElseIf (pt.IsValueType) Then
                                val = Convert.ChangeType(val, pt)
                            ElseIf (st Is GetType(String)) Then
                                val = val(CStr(val))
                            End If
                        End If
                    End If

                    If (bNeedDefault) Then
                        If (pt Is GetType(String)) Then
                            val = ""
                        ElseIf (pt Is GetType(Boolean)) Then
                            val = False
                        Else
                            val = If(pt.IsValueType, Activator.CreateInstance(pt), Nothing)
                        End If
                    End If

                    prop.SetValue(inst, val, Nothing)
                Catch ex As Exception

                End Try
            Next

            Return inst
        End Get
    End Property

    Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
        Get
            Return Me.m_enum.Current
        End Get
    End Property

    Public Sub Reset() Implements IEnumerator.Reset
        Me.m_enum.Reset()
    End Sub

    Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
        Return Me.m_enum.MoveNext()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.m_enum.Dispose()
    End Sub

End Class
