' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Eii.BlobStore
Imports Eii.ControlledVocabularies.Descriptors
Imports Eii.ControlledVocabularies.Inference.Field
Imports Eii.ControlledVocabularies.Vocabularies.LifeStage
Imports Eii.ControlledVocabularies.Vocabularies.Species
Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI
Imports EwEUtils.Logging
Imports Microsoft.Extensions.DependencyInjection
Imports ScientificInterfaceShared.Controls

Public Class EwESURIMICheckerPlugin
    Implements IMenuItemPlugin, IEcospaceInitializedPlugin, IUIContextPlugin

    Private m_uic As cUIContext = Nothing
    Private m_serviceProvider As IServiceProvider

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndSURIMIcheck"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "SURIMI checker"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Check if a model meets the SURIMI integration requirements"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE devteam"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        Try
            ' Ignore frmPlugin
            Dim dlg = New dlgSURIMIChecker(Me.m_uic, m_serviceProvider)
            dlg.ShowDialog(Me.m_uic.FormMain)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' Configure DI container

        Dim services As New ServiceCollection()
        services.AddLogging()

        ' Register the LoggerFactory from LoggingContext
        services.AddSingleton(LoggingContext.LoggerFactory)
        services.AddSingleton(Of IKeyFieldDescriptorRegistry, KeyFieldDescriptorRegistry)()
        services.AddSingleton(Of IKeyFieldDescriptorIndexer, KeyFieldDescriptorIndexer)()
        services.AddSingleton(Of IBlobStore)(Function(sp) New LocalBlobStore(inputRoot:="Includes", outputRoot:="Output"))
        services.AddSingleton(Of FieldInferenceOrchestrator)()

        ' Register ASFISSpeciesCodeVocabulary - it will automatically get ILogger<ASFISSpeciesCodeVocabulary> from the factory
        services.AddSingleton(Of ASFISSpeciesCodeVocabulary)()
        services.AddSingleton(Of SURIMILifestageVocabulary)()

        m_serviceProvider = services.BuildServiceProvider()

    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

End Class
