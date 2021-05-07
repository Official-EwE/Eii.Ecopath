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

Public Class cForcing

    <XmlAttribute("name")>
    Public Property Name As String = ""
    <XmlAttribute("climate-scenario")>
    Public Property Climate As String = ""
    <XmlAttribute("socio-econ-scenario")>
    Public Property SocioEcon As String = ""

    Public Overrides Function ToString() As String
        Return "Forcing " & Me.Name & ", clim: " & Me.Climate & ", soc: " & Me.SocioEcon
    End Function

End Class
