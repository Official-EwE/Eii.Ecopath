' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Public Class cCell

    Private m_values As New Dictionary(Of String, Single)

    Public Sub New(X As Single, Y As Single, area As Single)
        Me.X = X
        Me.Y = Y
        Me.TotalArea = area
    End Sub

    Public Property TotalArea As Single
    Public Property X As Single
    Public Property Y As Single

    Public Property SubArea(key As String) As Single
        Get
            If (Not Me.m_values.ContainsKey(key)) Then Return 0
            Return Me.m_values(key)
        End Get
        Set(value As Single)
            Me.m_values(key) = value
        End Set
    End Property

    Public Sub Clear()
        Me.m_values.Clear()
    End Sub

End Class
