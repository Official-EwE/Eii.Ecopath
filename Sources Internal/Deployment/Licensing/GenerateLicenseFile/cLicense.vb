Public Class cLicense

    Public Property Name As String
    Public Property Email As String
    Public Property Entity As String
    Public Property Date_purchased As Date
    Public Property Months As Integer
    Public ReadOnly Property IsExpired()
        Get
            Return Date_purchased.AddMonths(Months) < Date.Now
        End Get
    End Property

End Class
