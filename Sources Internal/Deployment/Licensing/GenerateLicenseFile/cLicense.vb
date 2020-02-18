Public Class cLicense

    Public Property Name As String
    Public Property Email As String
    Public Property Entity As String
    Public Property Date_purchased As Date
    Public Property Months As Integer

    Public ReadOnly Property Date_expiry()
        Get
            Return Date_purchased.AddMonths(Months)
        End Get
    End Property

    Public ReadOnly Property IsExpired()
        Get
            Return Me.Date_expiry < Date.Now
        End Get
    End Property

End Class
