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
