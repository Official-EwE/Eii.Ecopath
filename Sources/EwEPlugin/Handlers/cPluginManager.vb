#Region " Imports "

Option Strict On
Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Threading
Imports System.Reflection
Imports System.ComponentModel
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEPlugin.Data
Imports System.Xml
Imports System.Text

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in manager, handles loading and enabling of <see cref="IPlugin">EwE plug-ins</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginManager
    Implements IDataBroadcaster

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, used to report the link between a plug-in and its assambly.
    ''' </summary>
    ''' <remarks>
    ''' Yes, you don't have to say it. You are totally right. This class is 
    ''' utterly obsolete if the reflection library is properly used, but hey.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Friend Class cPluginContext

        ''' <summary>Plug-in point.</summary>
        Private m_plugin As IPlugin = Nothing
        ''' <summary>Plug-in assembly this point was found in.</summary>
        Private m_assembly As cPluginAssembly = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Hatch me one, me harties!
        ''' </summary>
        ''' <param name="plugin"></param>
        ''' <param name="assembly"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal plugin As IPlugin, ByVal assembly As cPluginAssembly)
            Me.m_plugin = plugin
            Me.m_assembly = assembly
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the plug-in point.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Plugin() As IPlugin
            Get
                Return Me.m_plugin
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the plug-in assembly that contains the <see cref="Plugin">plug-in</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Assembly() As cPluginAssembly
            Get
                Return Me.m_assembly
            End Get
        End Property
    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Yet another helper class. This one serves to pass function parameter
    ''' info to InvokeMethod on a different thread.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cInvokeMethodInfo

        Private m_typePlugin As Type = Nothing
        Private m_strMethod As String = ""
        Private m_aArgs() As Object = Nothing
        Private m_invocation As eInvocationType = eInvocationType.All
        Private m_coll As ICollection(Of cPluginContext) = Nothing
        Private m_bResult As Boolean = False

        Public Sub New(ByVal typePlugin As Type, _
                       ByVal strMethod As String, _
                       ByVal aArgs() As Object, _
                       ByVal invocation As eInvocationType, _
                       ByVal coll As ICollection(Of cPluginContext))

            Me.m_typePlugin = typePlugin
            Me.m_strMethod = strMethod
            Me.m_aArgs = aArgs
            Me.m_invocation = invocation
            Me.m_coll = coll

        End Sub

        Public ReadOnly Property PluginType() As Type
            Get
                Return Me.m_typePlugin
            End Get
        End Property

        Public ReadOnly Property MethodName() As String
            Get
                Return Me.m_strMethod
            End Get
        End Property

        Public ReadOnly Property Arguments() As Object()
            Get
                Return Me.m_aArgs
            End Get
        End Property

        Public ReadOnly Property Invocation() As eInvocationType
            Get
                Return Me.m_invocation
            End Get
        End Property

        ReadOnly Property Plugins() As ICollection(Of cPluginContext)
            Get
                Return Me.m_coll
            End Get
        End Property

        Public Property Result() As Boolean
            Get
                Return Me.m_bResult
            End Get
            Set(ByVal value As Boolean)
                Me.m_bResult = value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to locate and create XML nodes for a configurable persist 
    ''' plug-in in the settings XML document.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cPluginConfiguration

        Public Shared Sub ProvideSettings(ByVal doc As XmlDocument, ByVal pi As IPlugin)

            If (doc Is Nothing) Then Return

            If Not (TypeOf pi Is IConfigurablePersistPlugin) Then Return

            Try
                Dim cpi As IConfigurablePersistPlugin = DirectCast(pi, IConfigurablePersistPlugin)
                cpi.SetConfigutationNode(doc, GetSettings(doc, cpi))
            Catch ex As Exception

            End Try

        End Sub

        Public Shared Function DefaultDocument() As XmlDocument
            Dim doc As New XmlDocument()
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "yes"))
            Return doc
        End Function

        ''' <summary>
        ''' Returns the settings node for a given IConfigurablePersistPlugin plug-in.
        ''' </summary>
        ''' <param name="pi"></param>
        ''' <returns></returns>
        Private Shared Function GetSettings(ByVal doc As XmlDocument, _
                                            ByVal pi As IConfigurablePersistPlugin) As XmlNode

            Dim node As XmlNode = Nothing
            Dim strName As String = ToValidNodeName(pi.Name)

            For Each node In doc.ChildNodes
                If (String.Compare(node.Name, strName, True) = 0) Then
                    Return node
                End If
            Next

            node = doc.CreateElement(strName)
            doc.AppendChild(node)
            Return node

        End Function

        Private Shared Function ToValidNodeName(ByVal strName As String) As String
            Dim sb As New StringBuilder()
            Dim c As Char
            For i As Integer = 0 To strName.Length - 1
                c = strName(i)
                If Char.IsLetterOrDigit(c) Then sb.Append(c)
            Next
            Return sb.ToString
        End Function

    End Class

#End Region ' Helper classes

#Region " Private variables "

    ''' <summary>The one core for this plugin manager.</summary>
    Private m_core As Object = Nothing
    ''' <summary>The one UI context for this plug-in manager.</summary>
    Private m_uic As Object = Nothing
    ''' <summary>Delegate that this class can use to check whether the current core
    ''' execution state allows a plug-in to run.</summary>
    Private m_dlgtCoreState As CanExecutePlugin = Nothing
    ''' <summary>Sync object to marshall plug-in calls across threads.</summary>
    Private m_sync As System.Threading.SynchronizationContext = Nothing
    ''' <summary>Id of the thread that create the plugin manager used to decide if the sync object should be used to marshall plug-in calls across threads.</summary>
    Private m_ThreadID As Integer = 0
    ''' <summary>Plug-in settings document</summary>
    Private m_settingsDoc As XmlDocument = cPluginConfiguration.DefaultDocument

#End Region ' Private variables

#Region " Initialization "

    Public Sub New()
        'Store the Thread ID of the thread that created the Plugin Manager 
        'It will be used to decide if a call to TryInvokeMethod needs to marshall the call
        Me.m_ThreadID = Threading.Thread.CurrentThread.ManagedThreadId
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Assign an EwECore to the plugin manager. This core will be used to 
    ''' initialize plugins when they are loaded.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Property Core() As Object
        Get
            Return m_core
        End Get
        Set(ByVal core As Object)
            ' Remember core
            m_core = core
            ' Initialize active plugins
            For Each pa As cPluginAssembly In Me.PluginAssemblies
                For Each ip As IPlugin In pa.Plugins
                    ip.Initialize(Me.m_core)
                Next
            Next
        End Set
    End Property

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Assign an UI Context to the plugin manager. This context will be passed to
    ''' any plug-in that requires this interface at startup.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Property UIContext() As Object
        Get
            Return Me.m_uic
        End Get
        Set(ByVal uic As Object)
            ' Remember core
            Me.m_uic = uic
            ' Initialize active plugins
            Me.TryInvokeMethod(GetType(IUIContextPlugin), _
                                      "UIContext", _
                                      New Object() {Me.m_uic})
        End Set
    End Property

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the delegate that the plug-in can invoke to test whether a plug-in
    ''' is allowed to execute.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Property CoreExecutionStateDelegate() As CanExecutePlugin
        Get
            Return Me.m_dlgtCoreState
        End Get
        Set(ByVal dlgtCoreState As CanExecutePlugin)
            ' Remember delegate
            Me.m_dlgtCoreState = dlgtCoreState
            ' Update all current plugins
            Me.UpdatePluginEnabledStates()
        End Set
    End Property

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the cross-threading synchronization context.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Property SyncObject() As System.Threading.SynchronizationContext
        Get
            Return Me.m_sync
        End Get
        Set(ByVal value As System.Threading.SynchronizationContext)
            Me.m_sync = value
        End Set
    End Property

#End Region ' Initialization 

#Region " Updates "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to implement plug-in overwrite confirmation handling.
    ''' </summary>
    ''' <param name="strPlugin">The short name of the plug-in to overwrite.</param>
    ''' <returns>True if the plug-in can be overwritten.</returns>
    ''' -----------------------------------------------------------------------
    Public Delegate Function OnConfirmOverwrite(ByVal strPlugin As String) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to update all plug-in assemblies.
    ''' </summary>
    ''' <param name="dlgOverwrite"><see cref="OnConfirmOverwrite">Delegate</see> 
    ''' for the calling process to implement an overwrite confirmation for possible 
    ''' conflicts.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function UpdatePlugins(Optional ByVal dlgOverwrite As OnConfirmOverwrite = Nothing) As Boolean

        Dim updater As cAutoUpdate = New cAutoUpdate(Me.m_core)
        Dim pluginAssembly As Assembly = Nothing
        Dim strPluginPath As String = ""

        Dim di As DirectoryInfo = Nothing
        Dim fi As FileInfo = Nothing
        Dim afi() As FileInfo = Nothing

        Dim strPluginName As String = ""
        Dim sProgress As Single = 0.0!
        Dim bDownload As Boolean = True

        Try

            ' Get plug-in assembly
            pluginAssembly = Assembly.GetAssembly(GetType(cPluginManager))
            ' Get location of plug-in assembly (which is where plug-ins are installed) ((win7 issues?!))
            strPluginPath = Path.GetDirectoryName(pluginAssembly.Location)

            ' Get inventory of all DLLs in the plug-in path
            ' JB: Added "*.dll" to only get files that could contain a Plugin.
            '     Other assembly types (exe, etc) may contain plugins but we won't go there.
            di = New DirectoryInfo(strPluginPath)
            afi = di.GetFiles("*.dll")

            ' For each DLL
            For iFile As Integer = 0 To afi.Length - 1

                ' Get file info
                fi = afi(iFile)
                ' Extract human legible plug-in name
                strPluginName = Path.GetFileNameWithoutExtension(fi.FullName)
                ' Calc progress
                sProgress = CSng(iFile / afi.Length)
                ' Think positive
                bDownload = True

                ' Can updater attach to this file?
                If updater.AttachAssembly(fi.FullName) Then

                    ' #Yes: process file
                    RaiseEvent AssemblyUpdating("", sProgress)

                    ' Check for update type
                    Select Case updater.CheckForUpdate()

                        Case cAutoUpdate.eUpdateStatusTypes.Info_CanMigrate
                            ' #Migration: not a simple update but a possibly risky migration.
                            ' Can ask user for confirmation?
                            If (dlgOverwrite IsNot Nothing) Then
                                ' #Yes: Ask overwrite confirmation 
                                bDownload = dlgOverwrite.Invoke(strPluginName)
                            Else
                                ' #No: Do not affect plug-in without confirmation
                                bDownload = False
                            End If

                        Case cAutoUpdate.eUpdateStatusTypes.Info_CanUpdate
                            ' #Update: ready to download
                            bDownload = True

                        Case cAutoUpdate.eUpdateStatusTypes.Error_Connection
                            ' #No connection: abort process to save time
                            Return False

                        Case Else
                            ' #Other status: either plug-in already up to date, or
                            ' the plug-in cannot (or should not) be treated. 
                            ' Move along folks, there's nothing to see here.
                            bDownload = False

                    End Select

                    ' Need to download?
                    If bDownload Then
                        ' #Yes: go at it
                        RaiseEvent AssemblyUpdating(strPluginName, sProgress)
                        ' ToDo: handle update result
                        updater.DownloadUpdate()
                    Else
                        ' #No: just reflect progress
                        RaiseEvent AssemblyUpdating("", sProgress)
                    End If

                End If
            Next

            ' Done
            RaiseEvent AssemblyUpdating("", 1.0!)

        Catch ex As Exception
            ' Kaboom
            Return False
        End Try

        Return True

    End Function

#End Region ' Updates

#Region " Public assembly management "

    ''' <summary>Dictionary of <see cref="cPluginAssembly">Plugin assemblies</see>.</summary>
    Private m_dictAssemblies As New Dictionary(Of String, cPluginAssembly)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load plug-ins
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub LoadPlugins()

        Dim di As DirectoryInfo = Nothing
        Dim afi() As FileInfo = Nothing

        'Get the location of the plugin manager assembly
        Dim pluginAssembly As Assembly = System.Reflection.Assembly.GetAssembly(GetType(cPluginManager))
        Dim strPluginPath As String = Path.GetDirectoryName(pluginAssembly.Location)

        Try

            di = New DirectoryInfo(strPluginPath)
            'jb added "*.dll" to only get files that could contain a Plugin. Assemblies in an exe could contain a plugin but we won't go there
            afi = di.GetFiles("*.dll")

            For Each fi As FileInfo In afi
                Try
                    Me.LoadPluginAssembly(fi.FullName)
                Catch ex As Exception
                    ' Ignore this
                End Try
            Next
        Catch ex As Exception
            ' Kaboom
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load EwE plugins from a file.
    ''' </summary>
    ''' <param name="strFileName">The file name to load plugins from.</param>
    ''' <returns>True if this assembly was loaded and contained plugins.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadPluginAssembly(ByVal strFileName As String) As Boolean

        Dim clsType As Type = Nothing
        Dim clsInterface As Type = Nothing
        Dim clsAssembly As Assembly = Nothing
        Dim nameAssembly As AssemblyName = Nothing
        Dim ip As IPlugin = Nothing
        Dim bHasPlugins As Boolean = False
        Dim plugAssem As cPluginAssembly = Nothing

        ' Sanity check
        If (Me.m_dictAssemblies.ContainsKey(strFileName)) Then
            Return False
        End If

        Try
            ' Get assembly info
            clsAssembly = Assembly.LoadFrom(strFileName)
            nameAssembly = clsAssembly.GetName

            ' Test if valid
            If clsAssembly Is Nothing Then Return False

            ' Create plugin assembly
            plugAssem = New cPluginAssembly(nameAssembly)

            ' Set compatible flag for EwE assemblies
            plugAssem.Compatibility = Me.GetCompatibility(clsAssembly)

            ' Look for appropriate types
            For Each clsType In clsAssembly.GetTypes
                ' Only look at types we can create
                If clsType.IsPublic = True Then
                    ' Ignore abstract classes
                    If Not ((clsType.Attributes And System.Reflection.TypeAttributes.Abstract) = _
                        System.Reflection.TypeAttributes.Abstract) Then
                        ' Check for the implementation of the specified interface
                        clsInterface = clsType.GetInterface("EwEPlugin.IPlugin", True)
                        If Not (clsInterface Is Nothing) Then
                            ' Get the plugin
                            ip = LoadPlugin(strFileName, clsType.FullName)
                            Try
                                ' Stick it up
                                plugAssem.Plugin(ip.Name) = ip
                            Catch ex As cPluginException
                                'Me.RaiseException()
                            End Try

                            ' Is assembly compatible to run?
                            If (plugAssem.Enabled) Then

                                ' Is core assigned?
                                If (Me.m_core IsNot Nothing) Then
                                    Try
                                        ' Initialize plugin
                                        ip.Initialize(Me.m_core)
                                        cPluginConfiguration.ProvideSettings(Me.m_settingsDoc, ip)
                                    Catch ex As Exception
                                        ' Disable the plugin entirely
                                        plugAssem.Compatibility = cPluginAssembly.ePluginCompatibilityTypes.IncompatibleUndetermined
                                    End Try
                                End If

                                ' Is UI Context assigned?
                                If (Me.m_uic IsNot Nothing) Then
                                    If (TypeOf (ip) Is IUIContextPlugin) Then
                                        Try
                                            DirectCast(ip, IUIContextPlugin).UIContext(Me.m_uic)
                                        Catch ex As Exception
                                            ' Disable the plugin entirely
                                            plugAssem.Compatibility = cPluginAssembly.ePluginCompatibilityTypes.IncompatibleUndetermined
                                        End Try
                                    End If
                                End If
                            End If

                            ' Yeah, got info allright
                            bHasPlugins = True
                        End If
                    End If
                End If
            Next

            If (bHasPlugins) Then

                plugAssem.Filename = strFileName

                Dim company As AssemblyCompanyAttribute = DirectCast(ExtractAssemblyAttribute(clsAssembly, GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
                If company IsNot Nothing Then plugAssem.Company = company.Company.ToString
                Dim copyright As AssemblyCopyrightAttribute = DirectCast(ExtractAssemblyAttribute(clsAssembly, GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
                If copyright IsNot Nothing Then plugAssem.Copyright = copyright.Copyright.ToString
                Dim description As AssemblyDescriptionAttribute = DirectCast(ExtractAssemblyAttribute(clsAssembly, GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
                If description IsNot Nothing Then plugAssem.Description = description.Description.ToString
                ' Okay, let's keep at least THIS one simple...
                plugAssem.Version = nameAssembly.Version.ToString()

                ' Store plugin assembly
                Me.m_dictAssemblies.Add(strFileName, plugAssem)

                ' Connect to manager where applicable
                For Each pi As IPlugin In plugAssem.Plugins(GetType(IDataProducerPlugin))
                    DirectCast(pi, IDataProducerPlugin).Broadcaster(Me)
                Next

                ' Inform the world
                RaiseEvent AssemblyAdded(plugAssem)

            End If

        Catch loaderEX As System.Reflection.ReflectionTypeLoadException

            ' A few things can have happened here, but for sure the DLL that the accessed module
            ' cannot be examined for types. This means that the module is incompatible with the
            ' current assembly file set. Since type detection has failed it cannot be determined
            ' whether the assembly is actually a plug-in or any other file.

            ' JS 29nov08: only assert when this is a confirmed plug-in assembly.
            '             (which will very likely not be the case since the manager could not access 
            '             the Types contained within the assembly)
            If bHasPlugins Then
                ' the ReflectionTypeLoadException is for diagnosing problems when the loader throwing an exception
                System.Console.WriteLine(Me.ToString & ".LoadPluginAssembly()")
                ' what the hell happend
                For Each ex As Exception In loaderEX.LoaderExceptions
                    System.Console.WriteLine(ex.Message)
                Next
                Me.RaisePluginException(plugAssem, loaderEX)

                Debug.Assert(False, Me.ToString & ".LoadPluginAssembly() " & strFileName & ": " & loaderEX.Message)
            End If

        Catch ex As Exception

            'catch any generic exceptions
            Me.RaisePluginException(plugAssem, ex)
            Debug.Assert(False, Me.ToString & ".LoadPluginAssembly() " & strFileName & ": " & ex.Message)

        End Try

        Return bHasPlugins
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Unload a plugin file.
    ''' </summary>
    ''' <param name="strFileName">The file name to unload.</param>
    ''' <returns>True if unloaded succesfully.</returns>
    ''' -----------------------------------------------------------------------
    Public Function UnloadPluginAssembly(ByVal strFileName As String) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Nothing
        Dim pa As cPluginAssembly = Nothing

        ' Sanity check
        If (Not Me.m_dictAssemblies.ContainsKey(strFileName)) Then
            Return False
        End If

        ' Get plugin assembly
        pa = Me.m_dictAssemblies(strFileName)
        ' Inform the world
        RaiseEvent AssemblyRemoved(pa)

        ' Invoke all IDisposedPlugin plug-ins
        Try
            collPlugins = Me.GetPluginDefs(GetType(IDisposedPlugin), pa)
            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IDisposedPlugin).Dispose()
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "Dispose", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        ' Remove from internal admin
        Me.m_dictAssemblies.Remove(strFileName)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly update in progress handler
    ''' </summary>
    ''' <param name="strName">The name of the plugin that is being updated.</param>
    ''' <param name="sProgress">Update progress.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub AssemblyUpdatingHandler(ByVal strName As String, ByVal sProgress As Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly update in progress handler
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event AssemblyUpdating As AssemblyUpdatingHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly added delegate.
    ''' </summary>
    ''' <param name="paAdded">The add plugin assembly.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub AssemblyAddedHandler(ByVal paAdded As cPluginAssembly)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly added event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event AssemblyAdded As AssemblyAddedHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly removed delegate.
    ''' </summary>
    ''' <param name="paRemoved">The add plugin assembly.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub AssemblyRemovedHandler(ByVal paRemoved As cPluginAssembly)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly removed event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event AssemblyRemoved As AssemblyRemovedHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A plugin has thrown an exception delegate.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub PluginExceptionHandler(ByVal PluginException As cPluginException)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A plugin has thrown an exception.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event PluginException As PluginExceptionHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A plugin enabled state change delegate.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Delegate Sub PluginEnabledHandler(ByVal ip As IGUIPlugin, ByVal bEnable As Boolean)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A plugin enabled state has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Event PluginEnabled As PluginEnabledHandler

#End Region ' Assembly management

#Region " Plugin invocation "

#Region " Core "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the Core Initialized plugin point on any available and responsive 
    ''' <see cref="ICorePlugin">ICorePlugin plug-in</see>.
    ''' </summary>
    ''' <param name="objEcoPath"></param>
    ''' <param name="objEcoSim"></param>
    ''' <param name="objEcoSpace"></param>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Public Function CoreInitialized(ByVal objEcoPath As Object, ByVal objEcoSim As Object, ByVal objEcoSpace As Object) As Boolean

        ' Invokes ICorePlugin.CoreInitialized(objEcoPath, objEcoSim, objEcoSpace)
        Return Me.TryInvokeMethod(GetType(ICorePlugin), _
                                  "CoreInitialized", _
                                  New Object() {objEcoPath, objEcoSim, objEcoSpace})

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the <see cref="IDataValidatedPlugin.DataValidated">DataValidated</see>
    ''' plugin point on any available and responsive <see cref="IDataValidatedPlugin">IDataValidatedPlugin</see>
    ''' plug-in.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' <param name="datatype"></param>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Public Function DataValidated(ByVal varname As eVarNameFlags, ByVal datatype As eDataTypes) As Boolean

        ' Invokes IDataValidatedPlugin.DataValidated(varname, datatype)
        Return Me.TryInvokeMethod(GetType(IDataValidatedPlugin), _
                                  "DataValidated", _
                                  New Object() {varname, datatype})

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the Load plug-in point on any available and responsive 
    ''' <see cref="IEcopathPlugin">Ecopath plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function LoadModel(ByVal dataSource As Object) As Boolean

        ' Invokes IEcopathPlugin.LoadModel(dataSource)
        Return Me.TryInvokeMethod(GetType(IEcopathPlugin), _
                                  "LoadModel", _
                                  New Object() {dataSource})

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the Save plug-in point on any available and responsive 
    ''' <see cref="IEcopathPlugin">Ecopath plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function SaveModel(ByVal dataSource As Object) As Boolean

        ' Invokes IEcopathPlugin.SaveModel(dataSource)
        Return Me.TryInvokeMethod(GetType(IEcopathPlugin), _
                                  "SaveModel", _
                                  New Object() {dataSource})

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the Closed plug-in point on any available and responsive 
    ''' <see cref="IEcopathClosedPlugin">Ecopath closed plug-in</see>.
    ''' </summary>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function CloseModel() As Boolean

        ' Invokes IEcopathClosedPlugin.CloseModel()
        Return Me.TryInvokeMethod(GetType(IEcopathClosedPlugin), "CloseModel")

    End Function


    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, polls all plug-ins for unsaved data modifications.
    ''' </summary>
    ''' <param name="pa">cPluginAssembly to check, if any.</param>
    ''' ---------------------------------------------------------------------------
    Public Function IsDatabaseModified(Optional ByVal pa As cPluginAssembly = Nothing) As Boolean

        ' Invokes IDatabasePlugin.IsModified()
        Return Me.TryInvokeMethod(GetType(IDatabasePlugin), "IsModified", Nothing, eInvocationType.Any)

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, close a plug-in data link.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Sub CloseDatabase()

        ' Invokes IDatabasePlugin.CloseDatabase()
        Me.TryInvokeMethod(GetType(IDatabasePlugin), "Close")

    End Sub

#End Region ' Core

#Region " Ecopath "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the MassBalance plug-in point on any available and responsive 
    ''' <see cref="IEcopathPlugin">Ecopath plug-in</see>.
    ''' </summary>
    ''' <param name="EcoPathDataStructures">Ecopath data structure, required for the 
    ''' mass balance calculation.</param>
    ''' <param name="EstimateFor">Purpose of invocation, required for the mass
    ''' balance calculation.</param>
    ''' <param name="iResult">Mass Balance calculation result.</param>
    ''' <returns>True if a MassBalance plugin was executed succesfully.</returns>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Core MassBalance documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function MassBalance(ByVal EcoPathDataStructures As Object, ByVal EstimateFor As Integer, ByRef iResult As Integer) As Boolean

        ' Invoke IEcopathMassBalancePlugin.EcopathMassBalance(EcoPathDataStructures, EstimateFor, iResult)
        Return Me.TryInvokeMethod(GetType(IEcopathMassBalancePlugin), _
                          "EcopathMassBalance", _
                          New Object() {EcoPathDataStructures, EstimateFor, iResult}, _
                          eInvocationType.Exclusive)

    End Function

    Public Function EcopathRunCompleted(ByVal EcoPathDataStructures As Object) As Boolean

        ' Invoke IEcopathRunCompletedPlugin.EcopathRunCompleted(EcoPathDataStructures)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcopathRunCompletedPlugin), _
                                                    "EcopathRunCompleted", _
                                                    New Object() {EcoPathDataStructures})


        ' Invoke IEcopathRunCompletedPostPlugin.EcopathRunCompletedPost(EcoPathDataStructures)
        bSucces = bSucces And Me.TryInvokeMethod(GetType(IEcopathRunCompletedPostPlugin), _
                                                 "EcopathRunCompletedPost", _
                                                 New Object() {EcoPathDataStructures})

        Return bSucces

    End Function

#End Region ' Ecopath

#Region " Ecosim "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the LoadEcosimScenario plug-in point on any available and responsive 
    ''' <see cref="IEcosimPlugin">Ecosim plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub LoadEcosimScenario(ByVal dataSource As Object)

        ' Invoke IEcosimPlugin.LoadEcosimScenario(datasource)
        Me.TryInvokeMethod(GetType(IEcosimPlugin), "LoadEcosimScenario", New Object() {dataSource})

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the SaveEcosimScenario plug-in point on any available and responsive 
    ''' <see cref="IEcosimPlugin">Ecosim plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub SaveEcosimScenario(ByVal dataSource As Object)

        ' Invoke IEcosimPlugin.SaveEcosimScenario(datasource)
        Me.TryInvokeMethod(GetType(IEcosimPlugin), "SaveEcosimScenario", New Object() {dataSource})

    End Sub

    Public Function EcosimPreDataInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Return Me.TryInvokeMethod(GetType(IEcosimDataInitializedPlugin), _
                                  "EcosimPreDataInitialized", _
                                  New Object() {EcosimDatastructures})

    End Function

    Public Function EcosimPreRunInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Return Me.TryInvokeMethod(GetType(IEcosimDataInitializedPlugin), _
                                  "EcosimPreRunInitialized", _
                                  New Object() {EcosimDatastructures})

    End Function


    Public Function EcosimInitialized(ByVal EcosimDatastructures As Object) As Boolean

        ' Invoke IEcosimInitializedPlugin.EcosimInitialized(datasource)
        Return Me.TryInvokeMethod(GetType(IEcosimInitializedPlugin), _
                                  "EcosimInitialized", _
                                  New Object() {EcosimDatastructures})

    End Function


    Public Function EcosimModifyTimeseries(ByVal TimeSeriesDataStructures As Object) As Boolean

        ' Invoke IEcosimModifyTimeseriesPlugin.EcosimModifyTimeseries(TimeSeriesDataStructures)
        Return Me.TryInvokeMethod(GetType(IEcosimModifyTimeseriesPlugin), _
                                  "EcosimModifyTimeseries", _
                                  New Object() {TimeSeriesDataStructures})

    End Function

    Public Function EcosimModifyFGear(ByVal FGear() As Single, ByVal BB() As Single, ByVal EcosimDataStructures As Object, ByVal CurrentTimeIndex As Integer) As Boolean

        ' Invoke IEcosimModifyFGearPlugin.EcosimModifyFGear(FGear, BB, EcosimDataStructures, CurrentTime)
        Return Me.TryInvokeMethod(GetType(IEcosimModifyFGearPlugin), _
                                  "EcosimModifyFGear", _
                                  New Object() {FGear, BB, EcosimDataStructures, CurrentTimeIndex})

    End Function

    Public Function EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, _
                                        ByVal EcosimDataStructures As Object, _
                                        ByVal iTimeStep As Integer) As Boolean

        ' Invoke IEcosimBeginTimestepPlugin.EcosimBeginTimeStep(iTimeStep)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcosimBeginTimestepPlugin), _
                                                    "EcosimBeginTimeStep", _
                                                    New Object() {BiomassAtTimestep, EcosimDataStructures, iTimeStep})

        ' Invoke IEcosimBeginTimestepPlugin.EcosimBeginTimeStepPost(iTimeStep)
        bSucces = bSucces And Me.TryInvokeMethod(GetType(IEcosimBeginTimestepPostPlugin), _
                                                 "EcosimBeginTimeStepPost", _
                                                 New Object() {BiomassAtTimestep, EcosimDataStructures, iTimeStep})

        Return bSucces

    End Function


    Public Function EcosimSubTimestepBegin(ByRef BiomassAtTimestep() As Single, _
                                           ByVal TimeInYears As Single, _
                                           ByVal DeltaT As Single, _
                                           ByVal SubTimestepIndex As Integer, _
                                           ByVal EcosimDatastructures As Object) As Boolean

        ' Invoke IEcosimBeginTimestepPlugin.EcosimBeginTimeStep(iTimeStep)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcosimSubTimestepsPlugin), _
                                                    "EcosimSubTimeStepBegin", _
                                                    New Object() {BiomassAtTimestep, TimeInYears, DeltaT, SubTimestepIndex, EcosimDatastructures})

        Return bSucces

    End Function

    Public Function EcosimSubTimestepEnd(ByRef BiomassAtTimestep() As Single, _
                                          ByVal TimeInYears As Single, _
                                          ByVal DeltaT As Single, _
                                          ByVal SubTimestepIndex As Integer, _
                                          ByVal EcosimDatastructures As Object) As Boolean

        ' Invoke IEcosimBeginTimestepPlugin.EcosimBeginTimeStep(iTimeStep)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcosimSubTimestepsPlugin), _
                                                    "EcosimSubTimeStepEnd", _
                                                    New Object() {BiomassAtTimestep, TimeInYears, DeltaT, SubTimestepIndex, EcosimDatastructures})

        Return bSucces

    End Function

    'Public Function EcosimEndTimeStepStats(ByVal EcosimIndicies As Object) As Boolean

    '    Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimEndTimestepStatsPlugin))
    '    Try

    '        ' give every plugin that supports this interface a chance at running
    '        For Each ipc As cPluginContext In collPlugins
    '            Try 
    '                DirectCast(ipc.Plugin, IEcosimEndTimestepStatsPlugin).EcosimEndTimestepStatsPlugin(EcosimIndicies)
    '            Catch ex As Exception
    '                Debug.Assert(False, ipc.Plugin.Name & " EcosimEndTimeStatsStep() Error: " & ex.Message)
    '                Me.RaiseException(ipc.Assembly, ipc.Plugin, ex)
    '            End Try
    '        Next

    '    Catch ex As Exception
    '        Return False
    '    End Try

    'End Function

    Public Function EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, _
                                      ByVal EcosimDatastructures As Object, _
                                      ByVal iTimeStep As Integer, _
                                      ByVal Ecosimresults As Object) As Boolean

        ' Invoke IEcosimEndTimestepPlugin.EcosimEndTimeStep(BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcosimEndTimestepPlugin), _
                                                    "EcosimEndTimeStep", _
                                                    New Object() {BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults})

        ' Invoke IEcosimEndTimestepPlugin.EcosimEndTimeStepPost(BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults)
        Return bSucces And Me.TryInvokeMethod(GetType(IEcosimEndTimestepPostPlugin), _
                                         "EcosimEndTimeStepPost", _
                                         New Object() {BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults})

    End Function

    Public Function EcosimRunInitialized(ByVal EcosimDatastructures As Object) As Boolean

        ' Invoke IEcosimRunInitializedPlugin.EcosimRunInitialized(EcosimDatastructures)
        Return Me.TryInvokeMethod(GetType(IEcosimRunInitializedPlugin), _
                                  "EcosimRunInitialized", _
                                  New Object() {EcosimDatastructures})

    End Function


    Public Function EcosimRunCompleted(ByVal EcosimDatastructures As Object) As Boolean

        ' Invoke IEcosimRunCompletedPlugin.EcosimRunCompleted(EcosimDatastructures)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcosimRunCompletedPlugin), _
                          "EcosimRunCompleted", _
                          New Object() {EcosimDatastructures})


        ' Invoke IEcosimRunInitializedPlugin.EcosimRunInitialized(EcosimDatastructures)
        Return bSucces And Me.TryInvokeMethod(GetType(IEcosimRunCompletedPostPlugin), _
                          "EcosimRunCompletedPost", _
                          New Object() {EcosimDatastructures})

    End Function

#End Region ' Ecosim

#Region " Ecospace "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the LoadEcospaceScenario plug-in point on any available and responsive 
    ''' <see cref="IEcospacePlugin">Ecospace plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub LoadEcospaceScenario(ByVal dataSource As Object)

        ' Invoke IEcospacePlugin.LoadEcospaceScenario(dataSource)
        Me.TryInvokeMethod(GetType(IEcospacePlugin), "LoadEcospaceScenario", New Object() {dataSource})

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Invokes right after LoadEcospaceScenario
    ''' </summary>
    ''' <param name="EcospaceDatastructures"></param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function EcospaceInitialized(ByVal EcospaceDatastructures As Object) As Boolean

        ' Invoke IEcospaceInitializedPlugin.EcospaceInitialized(EcospaceDatastructures)
        Me.TryInvokeMethod(GetType(IEcospaceInitializedPlugin), "EcospaceInitialized", New Object() {EcospaceDatastructures})

    End Function


    Public Function EcospaceRunCompleted(ByVal EcospaceDatastructures As Object) As Boolean

        ' Invoke IEcospaceInitializedPlugin.EcospaceInitialized(EcospaceDatastructures)
        Me.TryInvokeMethod(GetType(IEcospaceRunCompletedPlugin), "EcospaceRunCompleted", New Object() {EcospaceDatastructures})

    End Function


    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the SaveEcospaceScenario plug-in point on any available and responsive 
    ''' <see cref="IEcospacePlugin">Ecospace plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub SaveEcospaceScenario(ByVal dataSource As Object)

        ' Invoke IEcospacePlugin.SaveEcospaceScenario(dataSource)
        Me.TryInvokeMethod(GetType(IEcospacePlugin), "SaveEcospaceScenario", New Object() {dataSource})

    End Sub

    Public Function EcospaceBeginTimeStep(ByVal EcospaceDataStructures As Object, ByVal iTimeStep As Integer) As Boolean

        ' Invoke IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep(EcospaceDataStructures, iTimeStep)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcospaceBeginTimestepPlugin), _
                                                    "EcospaceBeginTimeStep", _
                                                    New Object() {EcospaceDataStructures, iTimeStep})

        ' Invoke IEcospaceBeginTimestepPostPlugin.EcospaceBeginTimeStepPost(dataSource)
        Return bSucces And Me.TryInvokeMethod(GetType(IEcospaceBeginTimestepPostPlugin), _
                                              "EcospaceBeginTimeStepPost", _
                                              New Object() {EcospaceDataStructures, iTimeStep})

    End Function

    Public Function EcospacePostFishingEffortModTimestep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        ' Invoke IEcospacePostFishingEffortModTimestepPlugin.EcospacePostFishingEffortModTimestep(EcospaceDataStructures, iTimeStep)
        Return Me.TryInvokeMethod(GetType(IEcospacePostFishingEffortModTimestepPlugin), _
                                  "EcospacePostFishingEffortModTimestep", _
                                  New Object() {EcospaceDatastructures, iTimeStep})

    End Function

    Public Function EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        ' Invoke IEcospaceEndTimestepPlugin.EcospaceEndTimeStep(EcospaceDataStructures, iTimeStep)
        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IEcospaceEndTimestepPlugin), _
                                                    "EcospaceEndTimeStep", _
                                                    New Object() {EcospaceDatastructures, iTimeStep})

        ' Invoke IEcospaceEndTimestepPostPlugin.EcospaceEndTimeStepPost(EcospaceDataStructures, iTimeStep)
        Return bSucces And Me.TryInvokeMethod(GetType(IEcospaceEndTimestepPostPlugin), _
                                                    "EcospaceEndTimeStepPost", _
                                                    New Object() {EcospaceDatastructures, iTimeStep})

    End Function

    Public Function EcospaceCalculateCostOfSailing(ByVal EcospaceDataStructures As Object, _
                                                   ByVal Depth(,) As Integer, _
                                                   ByVal Port(,,) As Boolean, _
                                                   ByVal Sail(,,) As Single) As Boolean

        ' Invoke IEcospacePostFishingEffortModTimestepPlugin.EcospacePostFishingEffortModTimestep(EcospaceDataStructures, iTimeStep)
        Return Me.TryInvokeMethod(GetType(IEcospaceCalcCostOfSailingPlugin), _
                                  "CalculateCostOfSailing", _
                                  New Object() {EcospaceDataStructures, Depth, Port, Sail}, _
                                  eInvocationType.Exclusive)

    End Function

#End Region ' Ecospace

#Region " Ecotracer "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the LoadEcotracerScenario plug-in point on any available and responsive 
    ''' <see cref="IEcotracerPlugin">Ecotracer plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub LoadEcotracerScenario(ByVal dataSource As Object)

        ' Invoke IEcotracerPlugin.LoadEcotracerScenario(dataSource)
        Me.TryInvokeMethod(GetType(IEcotracerPlugin), "LoadEcotracerScenario", New Object() {dataSource})

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Invokes right after LoadEcotracerScenario
    ''' </summary>
    ''' <param name="EcotracerDatastructures"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' ---------------------------------------------------------------------------
    Public Function EcotracerInitialized(ByVal EcotracerDatastructures As Object) As Boolean

        ' Invoke IEcotracerInitializedPlugin.EcotracerInitialized(EcotracerDatastructures)
        Return Me.TryInvokeMethod(GetType(IEcotracerInitializedPlugin), _
                                  "EcotracerInitialized", _
                                  New Object() {EcotracerDatastructures})

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, invokes the SaveEcotracerScenario plug-in point on any available and responsive 
    ''' <see cref="IEcotracerPlugin">Ecotracer plug-in</see>.
    ''' </summary>
    ''' <param name="dataSource">The datasource that invoked this plug-in point.</param>
    ''' <remarks>Due to avoid circular references, this project is unable to reference
    ''' the assembly EwECore. As such, links in this help text cannot be resolved.
    ''' Refer to the EwE Datasource documentation for calling conventions and 
    ''' proper parameter usage.</remarks>
    ''' ---------------------------------------------------------------------------
    Public Sub SaveEcotracerScenario(ByVal dataSource As Object)

        ' Invoke IEcotracerPlugin.SaveEcotracerScenario(dataSource)
        Me.TryInvokeMethod(GetType(IEcotracerPlugin), "SaveEcotracerScenario", New Object() {dataSource})

    End Sub

#End Region ' Ecotracer 

#Region " Data Exchange "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exchange data from a <see cref="IDataProducerPlugin">data producer plug-in</see>
    ''' to any interested <see cref="IDataConsumerPlugin">data consumer plug-in</see>.
    ''' </summary>
    ''' <param name="data">The <see cref="IPluginData">data</see> to exchange.</param>
    ''' <returns>True if broadcast succeeded.</returns>
    ''' -----------------------------------------------------------------------
    Public Function BroadcastData(ByVal strDataName As String, ByVal data As IPluginData) As Boolean _
            Implements IDataBroadcaster.BroadcastData

        ' Invoke IDataConsumerPlugin.ReceiveData(strDataName, data)
        Return Me.TryInvokeMethod(GetType(IDataConsumerPlugin), _
                                  "ReceiveData", _
                                  New Object() {strDataName, data}, _
                                  eInvocationType.Any)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Query whether any loaded <see cref="IDataProducerPlugin">IDataProducerPlugin</see>
    ''' exposes <see cref="IPluginData">plug-in data</see> under a given name.
    ''' </summary>
    ''' <param name="strDataName">The name of the data to match.</param>
    ''' <param name="runType">Run type that the data is requested for, or
    ''' Null if the run type is irrelevant.</param>
    ''' <returns>True if the requested data is available.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsDataAvailable(ByVal strDataName As String, Optional ByVal runType As IRunType = Nothing) As Boolean

        ' Invoke IDataProducerPlugin.IsDataAvailable(strDataName, runType)
        Return Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                  "IsDataAvailable", _
                                  New Object() {strDataName, runType}, _
                                  eInvocationType.Any)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Query whether any loaded <see cref="IDataProducerPlugin">IDataProducerPlugin</see>
    ''' exposes <see cref="IPluginData">plug-in data</see> of a given type.
    ''' </summary>
    ''' <param name="dataType">The type of the data to match.</param>
    ''' <param name="runType">Run type that the data is requested for, or
    ''' Null if the run type is irrelevant.</param>
    ''' <returns>True if the requested data is available.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsDataAvailable(ByVal dataType As Type, Optional ByVal runType As IRunType = Nothing) As Boolean

        ' Invoke IDataProducerPlugin.IsDataAvailable(dataType, runType)
        Return Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                  "IsDataAvailable", _
                                  New Object() {dataType, runType}, _
                                  eInvocationType.Any)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all <see cref="IPluginData">plug-in data</see> from loaded
    ''' <see cref="IDataProducerPlugin">IDataProducerPlugin</see>
    ''' instances that expose data of a given <see cref="Type">Type</see>.
    ''' </summary>
    ''' <param name="dataType">The type of the data to match.</param>
    ''' <returns>An array of data, or an empty array if an error occurred.</returns>
    ''' <remarks>This method is not thread-safe.</remarks>
    ''' -----------------------------------------------------------------------
    Public Function GetData(ByVal dataType As Type) As IPluginData()

        Dim coll As ICollection(Of cPluginContext) = Me.GetPluginDefs(GetType(IDataProducerPlugin))
        Dim data As IPluginData = Nothing
        Dim lData As New List(Of IPluginData)

        Try

            For Each ipc As cPluginContext In coll

                Try
                    If DirectCast(ipc.Plugin, IDataProducerPlugin).GetDataByType(dataType, data) Then
                        If (data IsNot Nothing) Then
                            lData.Add(data)
                        End If
                    End If
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "GetDataByType", ex)
                End Try

            Next

        Catch ex As Exception
        End Try

        Return lData.ToArray()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether remote data is enabled.
    ''' </summary>
    ''' <param name="dataType">The type of the data to match.</param>
    ''' <param name="runType">Run type that the data is requested for, or
    ''' Null if the run type is irrelevant.</param>
    ''' <returns>True if the requested data is available.</returns>
    ''' -----------------------------------------------------------------------
    Public Property EnableData(ByVal dataType As Type, ByVal runType As IRunType) As Boolean
        Get
            Return Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                      "IsEnabled", _
                                      New Object() {dataType, runType}, _
                                      eInvocationType.Any)
        End Get
        Set(ByVal value As Boolean)
            Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                      "SetEnabled", _
                                      New Object() {dataType, runType, value}, _
                                      eInvocationType.Any)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enable or disable a specific data producer.
    ''' </summary>
    ''' <param name="strProducer">The name of the producer to enable or disable.</param>
    ''' <param name="bEnable">Enable flag.</param>
    ''' <returns>True if the requested producer is enabled.</returns>
    ''' -----------------------------------------------------------------------
    Public Property EnableDataProducer(ByVal strProducer As String, ByVal bEnable As Boolean) As Boolean
        Get

            Return Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                      "IsEnabled", _
                                      New Object() {}, _
                                      eInvocationType.Any, _
                                      Me.GetPluginDefs(strProducer))
        End Get
        Set(ByVal value As Boolean)
            Me.TryInvokeMethod(GetType(IDataProducerPlugin), _
                                      "SetEnabled", _
                                      New Object() {strProducer, bEnable}, _
                                      eInvocationType.Any, _
                                      Me.GetPluginDefs(strProducer))
        End Set
    End Property

#End Region ' Data Exchange 

#Region " Search "

    Public Function SearchInitialized(ByVal SearchDS As Object) As Boolean

        ' Invoke ISearchPlugin.SearchInitialized(SearchDS)
        Return Me.TryInvokeMethod(GetType(ISearchPlugin), "SearchInitialized", New Object() {SearchDS})

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point, called whenever search objective results have been 
    ''' calculated.
    ''' </summary>
    ''' <param name="SearchDS">Search data structures holding the 
    ''' search results.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function PostRunSearchResults(ByVal SearchDS As Object) As Boolean

        ' Invoke ISearchPlugin.PostRunSearchResults(SearchDS)
        Return Me.TryInvokeMethod(GetType(ISearchPlugin), "PostRunSearchResults", New Object() {SearchDS})

    End Function

    Public Function SearchIterationsStarting() As Boolean

        ' Invoke ISearchPlugin.SearchIterationsStarting()
        Return Me.TryInvokeMethod(GetType(ISearchPlugin), "SearchIterationsStarting", New Object() {})

    End Function

#End Region ' Search 

#Region " MSE and MSY "

    Public Function MSERunStarted() As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSERunStarted", _
                                                    New Object() {})


    End Function

    Public Function MSERunCompleted() As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSERunCompleted", _
                                                    New Object() {})


    End Function

    Public Function MSEIterationStarted() As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSEIterationStarted", _
                                                    New Object() {})


    End Function

    Public Function MSEIterationCompleted() As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSEIterationCompleted", _
                                                    New Object() {})


    End Function

    Public Function MSEDoAssessment(ByVal Biomass() As Single) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSEDoAssessment", _
                                                    New Object() {Biomass})


    End Function

    Public Function MSEUpdateQuotas(ByVal Biomass() As Single) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSEUpdateQuotas", _
                                                    New Object() {Biomass})


    End Function

    Public Function MSERegulateEffort(ByVal Biomass() As Single, ByVal QMult() As Single, ByVal QYear() As Single, ByVal t As Integer) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSERunPlugin), "MSERegulateEffort", _
                                                    New Object() {Biomass, QMult, QYear, t})


    End Function

    Public Function MSEInitialized(ByVal MSEModel As Object, _
                                   ByVal MSEDataStructure As Object, _
                                   ByVal EcosimDatastructures As Object) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSEInitialized), "MSEInitialized", _
                                                    New Object() {MSEModel, MSEDataStructure, EcosimDatastructures})


    End Function

    Public Function MSYInitialized(ByVal MSEDataStructure As Object, _
                                   ByVal EcosimDatastructures As Object) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSYPlugin), "MSYInitialized", _
                                                    New Object() {MSEDataStructure, EcosimDatastructures})


    End Function


    Public Function MSYRunStarted(ByVal MSEDataStructure As Object, _
                                  ByVal EcosimDatastructures As Object) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSYPlugin), "MSYRunStarted", _
                                                    New Object() {MSEDataStructure, EcosimDatastructures})


    End Function


    Public Function MSYEffortCompleted(ByVal MSYEffortByFleet() As Single, ByVal MSYFbyGroup() As Single) As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSYPlugin), "MSYEffortCompleted", _
                                                    New Object() {MSYEffortByFleet, MSYFbyGroup})


    End Function

    Public Function MSYRunCompleted() As Boolean

        Dim bSucces As Boolean = Me.TryInvokeMethod(GetType(IMSYPlugin), "MSYRunCompleted", _
                                                    New Object() {})


    End Function

#End Region ' MSE and MSY

#Region " Settings "

    Public Property Settings() As XmlDocument
        Get
            Return Me.m_settingsDoc
        End Get
        Set(ByVal value As XmlDocument)
            Me.m_settingsDoc = value
        End Set
    End Property

#End Region ' Settings

#End Region ' Plugin invocation

#Region " Plugin access "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of <see cref="cPluginContext">plug-in definitions</see>
    ''' of a given <see cref="Type">Type</see>.
    ''' </summary>
    ''' <param name="t">The <see cref="Type">Type</see> of the plugins to retrieve.</param>
    ''' <param name="pa">The <see cref="cPluginAssembly">plug-in assembly</see> to search.
    ''' If not specified, all plug-in assemblies will be searched.</param>
    ''' <returns>A collection of <see cref="cPluginContext">plug-in contexts</see>
    ''' linking to plug-ins of the given type.</returns>
    ''' ---------------------------------------------------------------------------
    Friend Function GetPluginDefs(ByVal t As Type, _
                                   Optional ByVal pa As cPluginAssembly = Nothing) As ICollection(Of cPluginContext)

        Dim collPlugins As New List(Of cPluginContext)
        Dim lpa As New List(Of cPluginAssembly)

        If (pa IsNot Nothing) Then lpa.Add(pa) Else lpa.AddRange(Me.PluginAssemblies)
        For Each pa In lpa
            For Each pi As IPlugin In pa.Plugins(t)
                collPlugins.Add(New cPluginContext(pi, pa))
            Next pi
        Next pa
        Return collPlugins

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of <see cref="cPluginContext">plug-in definitions</see>
    ''' of a given <see cref="IPlugin.Name">name</see>.
    ''' </summary>
    ''' <param name="strName">The <see cref="Type">Type</see> of the plugins to retrieve.</param>
    ''' <param name="pa">The <see cref="cPluginAssembly">plug-in assembly</see> to search.
    ''' If not specified, all plug-in assemblies will be searched.</param>
    ''' <returns>A collection of <see cref="cPluginContext">plug-in contexts</see>
    ''' linking to plug-ins of the given type.</returns>
    ''' ---------------------------------------------------------------------------
    Friend Function GetPluginDefs(ByVal strName As String, _
                                   Optional ByVal pa As cPluginAssembly = Nothing) As ICollection(Of cPluginContext)

        Dim collPlugins As New List(Of cPluginContext)
        Dim lpa As New List(Of cPluginAssembly)

        If (pa IsNot Nothing) Then lpa.Add(pa) Else lpa.AddRange(Me.PluginAssemblies)
        For Each pa In lpa
            Dim pi As IPlugin = pa.Plugin(strName)
            If pi IsNot Nothing Then
                collPlugins.Add(New cPluginContext(pi, pa))
            End If
        Next pa
        Return collPlugins

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all <see cref="IPlugin">plug-ins</see> with a given name.
    ''' </summary>
    ''' <param name="strName">Name of the plugin to return. Names are
    ''' case insensitive.</param>
    ''' <returns>A collection of <see cref="IPlugin">plug-ins</see> with the 
    ''' given name.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetPlugins(ByVal strName As String, _
                               Optional ByVal pa As cPluginAssembly = Nothing) As ICollection(Of IPlugin)

        Dim collPlugins As New List(Of IPlugin)
        Dim lpa As New List(Of cPluginAssembly)

        If (pa IsNot Nothing) Then lpa.Add(pa) Else lpa.AddRange(Me.PluginAssemblies)
        For Each pa In lpa
            Dim pi As IPlugin = pa.Plugin(strName)
            If pi IsNot Nothing Then
                collPlugins.Add(pi)
            End If
        Next
        Return collPlugins

    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all <see cref="IPlugin">plug-ins</see> with a given type.
    ''' </summary>
    ''' <param name="t">Type of the plugin to return.</param>
    ''' <returns>A collection of <see cref="IPlugin">plug-ins</see> with the 
    ''' given name.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetPlugins(ByVal t As Type, _
                               Optional ByVal pa As cPluginAssembly = Nothing) As ICollection(Of IPlugin)

        Dim collPlugins As New List(Of IPlugin)
        Dim lpa As New List(Of cPluginAssembly)

        If (pa IsNot Nothing) Then lpa.Add(pa) Else lpa.AddRange(Me.PluginAssemblies)
        For Each pa In lpa
            For Each pi As IPlugin In pa.Plugins(t)
                collPlugins.Add(pi)
            Next pi
        Next
        Return collPlugins

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a list of available producers that produce data of a given
    ''' <paramref name="typeData">type</paramref>.
    ''' </summary>
    ''' <param name="typeData">The <see cref="Type">type</see> of data to test.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function DataProducers(ByVal typeData As Type) As ICollection(Of IDataProducerPlugin)

        Dim pa As cPluginAssembly = Nothing
        Dim pi As IPlugin = Nothing
        Dim dp As IDataProducerPlugin = Nothing
        Dim lpa As New List(Of IDataProducerPlugin)

        For Each pa In Me.PluginAssemblies
            For Each pi In pa.Plugins(GetType(IDataProducerPlugin))
                dp = DirectCast(pi, IDataProducerPlugin)
                If dp.IsDataAvailable(typeData) Then
                    lpa.Add(dp)
                End If
            Next
        Next
        Return lpa
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a plugin assembly by <see cref="AssemblyName.Name">name</see> 
    ''' and (optionally) by <see cref="AssemblyName.Version">version</see> number.
    ''' </summary>
    ''' <param name="strName">Name of the assembly</param>
    ''' <param name="ver"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property PluginAssembly(ByVal strName As String, _
                                            Optional ByVal ver As Version = Nothing) As cPluginAssembly
        Get
            Dim an As AssemblyName = Nothing
            Dim bFound As Boolean = False

            For Each pa As cPluginAssembly In Me.PluginAssemblies
                an = pa.AssemblyName
                If String.Compare(an.Name, strName, True) = 0 Then
                    If ver Is Nothing Then
                        bFound = True
                    Else
                        bFound = ver.Equals(an.Version)
                    End If
                End If
                If bFound Then Return pa
            Next
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of <see cref="cPluginAssembly">plug-in assemblies</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property PluginAssemblies() As ICollection(Of cPluginAssembly)
        Get
            Return Me.m_dictAssemblies.Values
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a list of <see cref="AssemblyName">AssemblyName</see> instances
    ''' for the loaded plugin assemblies.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property PluginAssemblyNames() As AssemblyName()
        Get
            Dim lan As New List(Of AssemblyName)
            For Each pa As cPluginAssembly In Me.PluginAssemblies
                lan.Add(pa.AssemblyName)
            Next
            Return lan.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a list of <see cref="AssemblyName">AssemblyName</see> instances
    ''' for incompatible plug-ins.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function GetIncompatiblePlugins() As ICollection(Of cPluginAssembly)
        Dim collPlugins As New List(Of cPluginAssembly)
        For Each pa As cPluginAssembly In Me.PluginAssemblies
            If pa.Compatibility <> cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible Then
                collPlugins.Add(pa)
            End If
        Next
        Return collPlugins
    End Function

#End Region ' Plugin access

#Region " Plugin core state response "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Callback delegate to be implemented by the class that can tell whether a
    ''' plugin is allowed to run given a specific <see cref="eCoreExecutionState">Core execution state</see>.
    ''' </summary>
    ''' <param name="coreExectionState">The state to verify.</param>
    ''' <returns>True if a plugin can execute for this state, false otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Delegate Function CanExecutePlugin(ByVal coreExectionState As eCoreExecutionState) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Method to call whenever the plugins need to respond to core state changes.
    ''' </summary>
    ''' <param name="ip">A <see cref="IGUIPlugin">GUI plugin</see> to update the
    ''' enabled state for (optional). If this parameter is omitted, the enabled
    ''' state of all currently loaded IGUIPlugin instances is checked.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UpdatePluginEnabledStates(Optional ByVal ip As IGUIPlugin = Nothing)

        If Me.m_dlgtCoreState = Nothing Then Return

        Dim collPlugins As ICollection(Of cPluginContext) = Nothing
        Dim bEnable As Boolean = True

        If ip IsNot Nothing Then
            ' Check if plugin can execute
            bEnable = Me.m_dlgtCoreState.Invoke(DirectCast(ip, IGUIPlugin).EnabledState)
            ' Broadcast plugin enabled state event
            RaiseEvent PluginEnabled(DirectCast(ip, IGUIPlugin), bEnable)
        Else
            'For all GUI plugins
            collPlugins = Me.GetPluginDefs(GetType(IGUIPlugin))
            For Each ipc As cPluginContext In collPlugins
                ' Check if plugin can execute
                bEnable = Me.m_dlgtCoreState.Invoke(DirectCast(ipc.Plugin, IGUIPlugin).EnabledState)
                ' Broadcast plugin enabled state event
                RaiseEvent PluginEnabled(DirectCast(ipc.Plugin, IGUIPlugin), bEnable)
            Next
        End If

    End Sub

#End Region ' Plugin core state response

#Region " Plugin exception "

    Friend Sub RaisePluginException(ByVal assembly As cPluginAssembly, ByVal ex As Exception)

        Dim strMessage As String = String.Format(My.Resources.PLUGIN_ERROR_GENERIC, _
                                                 assembly.AssemblyName.Name, _
                                                 ex.Message)

        Me.RaisePluginException(New cPluginException(assembly, strMessage, ex))

    End Sub

    Friend Sub RaisePluginException(ByVal assembly As cPluginAssembly, ByVal plugin As IPlugin, _
                                    ByVal strMethodName As String, ByVal ex As Exception)

        Dim strMessage As String = String.Format(My.Resources.PLUGIN_ERROR_POINT, _
                                                 assembly.AssemblyName.Name, _
                                                 plugin.Name, _
                                                 strMethodName, _
                                                 ex.Message)
        Me.RaisePluginException(New cPluginException(assembly, strMessage, ex))

    End Sub

    Friend Sub RaisePluginException(ByVal pex As cPluginException)

        'Debug.Assert(False, strMessage & vbNewLine & ex.Message)
        RaiseEvent PluginException(pex)

    End Sub

#End Region ' Plugin exception

#Region " Internal generic invocation "

    ''' <summary>
    ''' Enumerated type, stating how a plug-in calls are handled, and how the plug-in
    ''' manager gathers invocation results.
    ''' </summary>
    ''' <remarks>
    ''' Why is 'invoke' spelled with a 'k', and 'invocation' with a 'c'? Granted,
    ''' 'invoce' and 'invokation' look pretty silly, but... why? Shall we propose
    ''' to consistently use a 'q' instead? Or 'ck'? Wow, I think I need a life...
    ''' </remarks>
    Private Enum eInvocationType As Integer
        ''' <summary>
        ''' All plug-ins implementing a method will be invoked, and invocation
        ''' results will be combined via the logical AND operator. Effectively,
        ''' this means that all implementations will have to succeed for the 
        ''' plug-in point to succeed.
        ''' </summary>
        All
        ''' <summary>
        ''' All plug-ins implementing a method will be invoked, and invocation
        ''' results will be combined via the logical OR operator. Effectively,
        ''' this means that any implementation can succeed for the plug-in 
        ''' point to succeed.
        ''' </summary>
        Any
        ''' <summary>
        ''' Only the first encountered plug-in that implements a method will be
        ''' invoked, and the plug-in result will depend on the result of that
        ''' single invocation. Effectively, this means that this type of plug-in
        ''' point is invoked exclusively.
        ''' </summary>
        Exclusive
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke a generic method on all plugins of a specific type.
    ''' </summary>
    ''' <param name="typePlugin">The <see cref="Type">Type</see> of the plugin.</param>
    ''' <param name="strMethod">The name of the method to invoke.</param>
    ''' <param name="aArgs">The arguments to pass to the method to invoke.</param>
    ''' <param name="invocation">Flag stating whether the plug-in point is exclusive.
    ''' Exclusive plug-in points are meant to replace core functionality. The first
    ''' plug-in point encountered is invoked in which case True is returned. If no
    ''' suitable plug-in point is found, a return value of False is expected.
    ''' </param>
    ''' <returns>True if the method could be found for the given type.</returns>
    ''' <remarks>
    ''' <para>Note that this method tries to match argument types to the values
    ''' provided in <paramref name="aArgs">aArgs</paramref>. If this array of values 
    ''' happens to contain Null (or Nothing), call <see cref="InvokeMethod">InvokeMethod</see>
    ''' instead.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function TryInvokeMethod(ByVal typePlugin As Type, _
                                    ByVal strMethod As String, _
                                    Optional ByVal aArgs() As Object = Nothing, _
                                    Optional ByVal invocation As eInvocationType = eInvocationType.All, _
                                    Optional ByVal coll As ICollection(Of cPluginContext) = Nothing) As Boolean


        ' Fix arguments
        If (aArgs Is Nothing) Then aArgs = New Object() {}

        ' ---                                                               --- '
        ' Validate called prototype and number of parameters in DEBUG mode only '
        ' ---                                                               --- '
#If 0 Then

        Try

            Dim mi As MethodInfo = typePlugin.GetMethod(strMethod)
            If (mi Is Nothing) Then
                Debug.Assert(False, String.Format("Method {0}::{1} does not exist", typePlugin, strMethod))
                Return False
            End If

            Dim api() As ParameterInfo = mi.GetParameters
            If (api.Length <> aArgs.Length) Then
                Debug.Assert(False, String.Format("Method {0}::{1} called with wrong number of parameters", typePlugin, strMethod))
                Return False
            End If

        Catch ex As AmbiguousMatchException
            ' Ok, more than one method found with this name. No need to validate
            ' further, let invocaton do the rest
        Catch ex As Exception
            ' What?!
            Debug.Assert(False, ex.Message)
        End Try

#End If

        'Only marshall the call via the SyncObject if TryInvokeMethod() is not on the same thread as the PluginManager was created on
        If Not (System.Threading.Thread.CurrentThread.ManagedThreadId = Me.m_ThreadID) Then
            ' Has sync object?
            If (Me.m_sync IsNot Nothing) Then
                ' #Yes: build info to cross over
                Dim inf As New cInvokeMethodInfo(typePlugin, strMethod, aArgs, invocation, coll)
                ' Yo Maurice
                Me.m_sync.Send(New SendOrPostCallback(AddressOf Me.MarshallInvokeMethod), inf)
                ' Return result
                Return inf.Result
            End If
        End If

        Return Me.InvokeMethod(typePlugin, strMethod, aArgs, invocation, coll)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Marshall bridge for <see cref="InvokeMethod">InvokeMethod</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub MarshallInvokeMethod(ByVal state As Object)

        ' Sanity check
        Debug.Assert(TypeOf (state) Is cInvokeMethodInfo)

        If Not (TypeOf (state) Is cInvokeMethodInfo) Then Return

        Dim info As cInvokeMethodInfo = DirectCast(state, cInvokeMethodInfo)
        info.Result = Me.InvokeMethod(info.PluginType, info.MethodName, info.Arguments, info.Invocation, info.Plugins)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke a generic method on all plugins of a specific type.
    ''' </summary>
    ''' <param name="typePlugin">The <see cref="Type">Type</see> of the plugin.</param>
    ''' <param name="strMethod">The name of the method to invoke.</param>
    ''' <param name="aArgs">The arguments to pass to the method to invoke.</param>
    ''' <param name="invocation">Flag stating whether the plug-in point is exclusive.
    ''' Exclusive plug-in points are meant to replace core functionality. The first
    ''' plug-in point encountered is invoked in which case True is returned. If no
    ''' suitable plug-in point is found, a return value of False is expected.
    ''' </param>
    ''' <param name="collPlugins">Collection of plugins to test, if any.</param>
    ''' <returns>True if the method could be found for the given type.</returns>
    ''' -----------------------------------------------------------------------
    Private Function InvokeMethod(ByVal typePlugin As Type, _
                                  ByVal strMethod As String, _
                                  ByVal aArgs() As Object, _
                                  ByVal invocation As eInvocationType, _
                                  ByVal collPlugins As ICollection(Of cPluginContext)) As Boolean

        If (collPlugins Is Nothing) Then
            collPlugins = Me.GetPluginDefs(typePlugin)
        End If

        Dim bSucces As Boolean = True

        Select Case invocation
            Case eInvocationType.All
                bSucces = True
            Case eInvocationType.Any
                bSucces = False
            Case eInvocationType.Exclusive
                bSucces = False
            Case Else
                Debug.Assert(False)
        End Select

        ' Invoke method on each plugin
        For Each ipc As cPluginContext In collPlugins
            Try
                ' Try to invoke the member method
                Dim bHandled As Boolean = CBool(typePlugin.InvokeMember(strMethod, BindingFlags.InvokeMethod, _
                                                                    Type.DefaultBinder, ipc.Plugin, aArgs))

                Select Case invocation
                    Case eInvocationType.All
                        ' All implementing plug-ins need to succeed
                        bSucces = bSucces And bHandled
                    Case eInvocationType.Any
                        ' Any of the implementing plug-ins need to succeed
                        bSucces = bSucces Or bHandled
                    Case eInvocationType.Exclusive
                        ' Exclusive plug-in succeeded: run away!
                        If bSucces Then Return True
                End Select

            Catch ex As MissingMethodException

                ' Thrown whenever method[name + parameters] was not found.
                ' This could indicate a plug-in assembly incompatibility?
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, strMethod, ex)
                bSucces = False

            Catch ex As Exception

                ' Error thrown within plug-in
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, strMethod, ex)
                bSucces = False

            End Try
        Next ipc

        Return bSucces

    End Function

#End Region ' Internal generic invocation

#Region " Private helper methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Loads a plugin by class name from a given assembly.
    ''' </summary>
    ''' <param name="AssemblyPath"></param>
    ''' <param name="ClassName"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadPlugin(ByVal AssemblyPath As String, ByVal ClassName As String, Optional ByVal args() As Object = Nothing) As IPlugin

        Dim clsRet As Object = Nothing
        Dim clsAssembly As System.Reflection.Assembly = Nothing

        clsAssembly = System.Reflection.Assembly.LoadFrom(AssemblyPath)
        If args Is Nothing Then
            clsRet = clsAssembly.CreateInstance(ClassName)
        Else
            clsRet = clsAssembly.CreateInstance(ClassName, False, Nothing, Nothing, args, Nothing, Nothing)
        End If
        Return DirectCast(clsRet, IPlugin)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Retrieves an embedded custom attribute from a .NET Assembly, such as 
    ''' company information, version number or copyright notice.
    ''' </summary>
    ''' <param name="assem">The Assembly to access.</param>
    ''' <param name="t">The Type of the attribute to obtain.</param>
    ''' <returns>An object, or Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ExtractAssemblyAttribute(ByVal assem As Assembly, ByVal t As Type) As Object
        Dim oValues() As Object = assem.GetCustomAttributes(t, False)
        If oValues Is Nothing Then Return Nothing
        If oValues.Length = 0 Then Return Nothing
        Return oValues(0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tests whether a specific assembly is compatible with the assemblies 
    ''' currently loaded by the main application.
    ''' </summary>
    ''' <param name="assemPlugin">The assembly to test</param>
    ''' <returns>True if compatible.</returns>
    ''' -----------------------------------------------------------------------
    Private Function GetCompatibility(ByVal assemPlugin As Assembly) As cPluginAssembly.ePluginCompatibilityTypes

        ' List of assemblies that the specified assembly is EXPECTING. 
        ' This list includes assembly version numbers.
        Dim aanameExpected As AssemblyName() = assemPlugin.GetReferencedAssemblies()
        ' List of assemblies that this application has loaded, including their version numbers.
        Dim aassemLoaded As Assembly() = AppDomain.CurrentDomain.GetAssemblies()
        ' A loaded assembly name
        Dim anameLoaded As AssemblyName = Nothing
        ' Assume all is well
        Dim compatibility As cPluginAssembly.ePluginCompatibilityTypes = cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible
        ' Running total per assembly
        Dim dtAssemblyCompatibility As New Dictionary(Of String, cPluginAssembly.ePluginCompatibilityTypes)

        ' For every expected assembly search its loaded counterpart
        For Each anExpected As AssemblyName In aanameExpected

            For Each asLoaded As Assembly In aassemLoaded
                ' Get the assenmbly name (e.g. definition) for this loaded assembly
                anameLoaded = asLoaded.GetName()
                ' Found a match?
                If String.Compare(anExpected.Name, anameLoaded.Name, True) = 0 Then
                    ' #Yep: test if versions (a.b.c.d) as (major.minor.build.revision) match:

                    ' Revision difference?
                    If anExpected.Version.Revision <> anameLoaded.Version.Revision Then
                        ' #Yes: assume compatible
                        compatibility = cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible
                    End If

                    ' Build difference?
                    If anExpected.Version.Build <> anameLoaded.Version.Build Then
                        ' #Yes: take caution
                        compatibility = cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution
                    End If

                    ' Minor version number difference?
                    If anExpected.Version.Minor <> anameLoaded.Version.Minor Then
                        ' #Yes: take caution
                        compatibility = cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution
                    End If

                    ' Major version number difference?
                    If anExpected.Version.Major <> anameLoaded.Version.Major Then
                        ' #Yes: assume incompatible
                        compatibility = cPluginAssembly.ePluginCompatibilityTypes.VersionIncompatible
                    End If

                End If

                ' Some plug-in assemblies may be referenced more than once. Just remember the most compatible version
                If dtAssemblyCompatibility.ContainsKey(anExpected.Name) Then
                    compatibility = DirectCast(Math.Min(compatibility, dtAssemblyCompatibility(anExpected.Name)), cPluginAssembly.ePluginCompatibilityTypes)
                End If
                dtAssemblyCompatibility(anExpected.Name) = compatibility
            Next
        Next

        ' Reasses overall compatibility
        compatibility = cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible
        For Each strAssem As String In dtAssemblyCompatibility.Keys
            compatibility = DirectCast(Math.Max(compatibility, dtAssemblyCompatibility(strAssem)), cPluginAssembly.ePluginCompatibilityTypes)
        Next
        Return compatibility

    End Function

#End Region ' Private helper methods

End Class
