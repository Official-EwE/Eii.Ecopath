Public Class cCreatedObjects

    Private m_Parent As String
    Private m_Child As List(Of String)

    Public Sub New(ByVal Parent As String)
        m_Parent = Parent
        m_Child = New List(Of String)
    End Sub

    Public Sub Add(ByVal Child As String)
        For Each x In m_Child
            If x = Child Then Exit Sub
        Next
        m_Child.Add(Child)
    End Sub

    Public Sub Remove(ByVal Child As String)
        m_Child.Remove(Child)
    End Sub

    Public ReadOnly Property ParentName() As String
        Get
            Return m_Parent
        End Get
    End Property

    Public ReadOnly Property ChildNames() As List(Of String)
        Get
            Return m_Child
        End Get
    End Property

    Public ReadOnly Property CountChild() As Integer
        Get
            Return m_Child.Count
        End Get
    End Property

End Class