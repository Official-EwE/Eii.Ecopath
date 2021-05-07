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

Public Class cClimateScenario

    <XmlAttribute("name")>
    Public Property Name As String = ""
    <XmlAttribute("description")>
    Public Property Description As String = ""

    Public Overrides Function ToString() As String
        Return "Climate scenario " & Me.Name
    End Function

End Class
