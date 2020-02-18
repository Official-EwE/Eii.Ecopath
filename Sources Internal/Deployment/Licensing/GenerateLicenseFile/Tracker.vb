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

Imports System.Text

Public Class Tracker

    Private Class Job

        Private m_step As Integer = 0
        Private m_sw As New Stopwatch()

        Public Sub New(name As String)
            Me.Name = name
            Me.m_sw.Start()
        End Sub

        Public ReadOnly Property Name As String

        Public ReadOnly Property [Step] As Integer
            Get
                Me.m_step += 1
                Return Me.m_step
            End Get
        End Property

        Public ReadOnly Property ElapsedMilliseconds() As Long
            Get
                Return Me.m_sw.ElapsedMilliseconds
            End Get
        End Property

        Public ReadOnly Property Elapsed() As String
            Get
                Return Me.m_sw.Elapsed.ToString("c")
            End Get
        End Property

    End Class

    Private m_jobs As New Stack(Of Job)
    Private m_indent As String = ""

    Public Sub New()
        ' NOP
    End Sub

    Public Sub Start(jobname As String)
        Dim j As New Job(jobname)
        Me.m_jobs.Push(j)
        Me.UpdateIndent()
        Me.Write(String.Format("Started {0}", jobname))
    End Sub

    Public Sub Log(jobstep As String, Optional bIsStep As Boolean = True, Optional bTimed As Boolean = True)

        Dim j As Job = Me.m_jobs.Peek()
        If (bIsStep) Then
            jobstep = String.Format("  - {0}: {1}", j.Step, jobstep)
        End If
        If (bTimed) Then
            jobstep = String.Format("{0} at {1}s", jobstep, j.Elapsed)
        End If
        Me.Write(jobstep)

    End Sub

    Public Function [Stop]() As String
        Dim j As Job = Me.m_jobs.Peek()
        Dim jobname As String = j.Name
        Me.Log("Done " & jobname, False)
        Me.m_jobs.Pop()
        Me.UpdateIndent()
        If (m_jobs.Count = 0) Then Me.Write("")
        Return jobname
    End Function

    Private Sub UpdateIndent()
        Me.m_indent = New String("  ", Math.Max(0, Me.m_jobs.Count - 1) * 2)
    End Sub

    Private Sub Write(text As String)
        If (String.IsNullOrWhiteSpace(text)) Then
            Console.WriteLine()
        Else
            Console.WriteLine("{0}{1}", m_indent, text)
        End If
    End Sub

End Class
