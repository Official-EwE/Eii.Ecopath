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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' <summary>
''' This class just serves as an UI anchor point to toggle the correct
''' setting
''' </summary>
Public Class cEcologicalIndEcospaceASCII
    Implements IAutoSavePlugin

    Public Const PluginName As String = "EwEEcoIndPluginAutosaveEcospaceASCII"

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.AutoSaveEcospaceMaps
        End Get
        Set(value As Boolean)
            My.Settings.AutoSaveEcospaceMaps = value
            My.Settings.Save()
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return PluginName
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return Me.Description
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, My.Resources.DISPLAYNAME, "Ecospace ASCII")
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Marta Coll Montón, Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "martacoll@yahoo.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecospace
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Return ""
    End Function

End Class
