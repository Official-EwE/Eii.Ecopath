' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 2010- Centre for Environment, Fisheries & Aquaculture Science, Lowestoft, UK.
' ===============================================================================
'
Imports Microsoft.Win32
Imports Couplerlib
Public MustInherit Class PluginExtenderBase
    Inherits System.Object

    Public Overridable Sub Load(ByVal regkey1 As RegistryKey, ByVal iTestDataPath As String, ByVal ixmlfile As String, ByRef cci As CCouplerlib, ByVal itimebase As Double)

    End Sub
    Public Overridable Sub Edit(ByVal carryoutedit As Boolean, ByVal timerratio As Integer, ByVal spinupdays As Integer, ByRef spec As Xml.XmlDocument, ByRef cpp As CCouplerlib)
    End Sub
    Public Overridable Sub simulate(ByVal extfilename As String, ByVal EwEGOTMTimeRatio As Integer, ByVal spinupdays As Integer, ByRef spec As Xml.XmlDocument, ByVal iusespatial As Boolean)
    End Sub
    Public Overridable Sub display()
    End Sub
    Public Overridable Sub StoreInRegistry()

    End Sub
    Public Overridable Function UsesNetCDF() As Boolean
    End Function
End Class
