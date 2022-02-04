Public Interface IReader

    Function TableNames(src As String) As String()
    Function Read(src As String, table As String) As DataTable

End Interface
