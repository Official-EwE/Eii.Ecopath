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

#Region " Imports "

Imports EwEPlugin
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' This class just serves as an UI anchor point to toggle the correct
''' setting
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcologicalIndMCMCCSV
    Implements IAutoSavePlugin

    Public Const PluginName As String = "EwEEcoIndPluginAutosaveMCMC"

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.AutoSaveMCMC
        End Get
        Set(value As Boolean)
            My.Settings.AutoSaveMCMC = value
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
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, My.Resources.DISPLAYNAME, "MCMC CSV")
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
        Return eAutosaveTypes.Ecosim
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Return ""
    End Function

End Class
