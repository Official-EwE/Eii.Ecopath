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

Public Class cVariable

    <XmlAttribute("name")>
    Public Property Name As String = ""
    <XmlAttribute("type")>
    Public Property VarType As String = ""

    Public Overrides Function ToString() As String
        Return "Variable " & Me.Name & " " & Me.VarType
    End Function

End Class
