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

Public Class cExperiment

    <XmlAttribute("name")>
    Public Property Name As String = ""

    ''' <summary>The earth system models (or global climate models)</summary>
    <XmlArray("periods")>
    <XmlArrayItem("period")>
    Public Forcings As New List(Of cForcing)

    Public Overrides Function ToString() As String
        Return "Experiment " & Me.Name & " (" & Me.Forcings.Count & " periods)"
    End Function

End Class
