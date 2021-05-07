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

Imports System.Xml.Serialization

Public Class cPeriod

    <XmlAttribute("name")>
    Public Property Name As String = ""
    <XmlAttribute("start-year")>
    Public Property StartYear As Integer = 0
    <XmlAttribute("end-year")>
    Public Property EndYear As Integer = 0

    Public Overrides Function ToString() As String
        Return "Period " & Me.Name & " " & Me.StartYear & "-" & Me.EndYear
    End Function

End Class
