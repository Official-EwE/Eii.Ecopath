'==============================================================================
'
' $Log: cPluginManager.vb,v $
' Revision 1.22  2009/04/01 20:24:17  jeroens
' Removed assert when plug-in complained
'
' Revision 1.21  2009/04/01 17:35:55  jeroens
' Separated Enabled state and Incompatibility
' Relaxed compatibility tests
' Commented compatibility tests
'
' Revision 1.20  2009/03/31 17:01:08  jeroens
' Only initialize compatible plug-ins
'
' Revision 1.19  2009/03/31 16:09:36  jeroens
' Delegate distributes cPluginExceptions only
'
' Revision 1.18  2009/03/31 14:55:40  jeroens
' All plug-in calls pretected by try/catch
' Plug errors all reported as cPluginExceptions via events
'
' Revision 1.17  2009/03/31 02:17:55  jeroens
' All plug-in calls try/caught
'
' Revision 1.16  2009/03/26 02:06:07  sherman
' Added Plugin point EcosimModifyFGear
'
' Revision 1.15  2009/03/10 18:37:38  jeroens
' Added post-invoke plugin points
'
' Revision 1.14  2009/03/01 19:59:01  jeroens
' GetPlugins can be filtered by assembly
'
' Revision 1.13  2009/02/25 07:16:31  jeroens
' Implemented DatabasePlugin calls
'
' Revision 1.12  2009/01/31 00:57:44  joeb
' Added Plugin points to FPS
'
' Revision 1.11  2009/01/21 19:38:26  jeroens
' Added GetData
'
' Revision 1.10  2008/12/16 16:57:26  sherman
' Corrected EcospacePostFishingEffortModTimestep
'
' Revision 1.9  2008/12/08 16:44:08  jeroens
' Removed Seed/MPA opt plugins
' Added generic ISearchPlugin
'
' Revision 1.8  2008/12/03 02:33:38  jeroens
' Added levels of compatibility
'
' Revision 1.7  2008/12/01 02:52:08  jeroens
' Relaxed assert constraints
'
' Revision 1.6  2008/11/28 16:55:46  joeb
' Removed a ToDo
'
' Revision 1.5  2008/11/28 02:43:25  jeroens
' Added plugin compatibility checks to prevent the system from dying
'
' Revision 1.4  2008/10/31 16:48:14  jeroens
' Added MPA opt plugin invocation
'
' Revision 1.3  2008/10/28 02:46:08  jeroens
' Added space layer exchange plugin
'
' Revision 1.2  2008/10/07 21:20:56  jeroens
' Implemented data exchange plugin structure
'
' Revision 1.1  2008/09/26 07:31:04  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports System.Reflection
Imports System.IO
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEPlugin.Data

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in manager, handles loading and enabling of <see cref="IPlugin">EwE plug-ins</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginManager
    Implements IDataBroadcaster

#Region " Helper class "

    Friend Class cPluginContext

        Private g_plugin As IPlugin = Nothing
        Private g_assembly As cPluginAssembly = Nothing

        Public Sub New(ByVal plugin As IPlugin, ByVal assembly As cPluginAssembly)
            Me.g_plugin = plugin
            Me.g_assembly = assembly
        End Sub

        Public ReadOnly Property Plugin() As IPlugin
            Get
                Return Me.g_plugin
            End Get
        End Property

        Public ReadOnly Property Assembly() As cPluginAssembly
            Get
                Return Me.g_assembly
            End Get
        End Property
    End Class

#End Region ' Helper class

#Region " Private variables "

    ''' <summary>The one core for this plugin manager.</summary>
    Private m_core As Object = Nothing
    ''' <summary>Delegate that this class can use to check whether the current core
    ''' execution state allows a plug-in to run.</summary>
    Private m_dlgtCoreState As CanExecutePlugin = Nothing

#End Region ' Private variables

#Region " Initialization "

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

#End Region ' Initialization 

#Region " Public assembly management "

    ''' <summary>Dictionary of <see cref="cPluginAssembly">Plugin assemblies</see>.</summary>
    Private m_dictAssemblies As New Dictionary(Of String, cPluginAssembly)

    Public Sub LoadPlugins(ByVal strPath As String)

        Dim di As DirectoryInfo = Nothing
        Dim afi() As FileInfo = Nothing

        Try
            di = New DirectoryInfo(strPath)
            'jb added "*.dll" to only get files that could contain a Plugin. Assemblies in an exe could contain a plugin but we won't go there
            afi = di.GetFiles("*.dll")

            For Each fi As FileInfo In afi
                Try
                    Me.LoadPluginAssembly(fi.FullName)
                Catch ex As Exception
                    ' Ignore this
                End Try
            Next

            ' Load plugins embedded in the main exe
            Me.LoadPluginAssembly(Application.ExecutablePath)

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

            ' Set compatible flag
            plugAssem.Compatibility = Me.GetCompatibility(clsAssembly)

            ' Look for appropriate types
            For Each clsType In clsAssembly.GetTypes
                ' Only look at types we can create
                If clsType.IsPublic = True Then
                    ' Ignore abstract classes
                    If Not ((clsType.Attributes And System.Reflection.TypeAttributes.Abstract) = _
                        System.Reflection.TypeAttributes.Abstract) Then
                        ' Check for the implementation of the specified interface
                        clsInterface = clsType.GetInterface("IPlugin", True)
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
                                    Catch ex As Exception
                                        ' Disable the plugin entirely
                                        plugAssem.Compatibility = cPluginAssembly.ePluginCompatibilityTypes.IncompatibleUndetermined
                                    End Try
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

            ' the ReflectionTypeLoadException is for diagnosing problems when the loader throwing an exception
            System.Console.WriteLine(Me.ToString & ".LoadPluginAssembly()")
            ' what the hell happend
            For Each ex As Exception In loaderEX.LoaderExceptions
                System.Console.WriteLine(ex.Message)
            Next
            Me.RaisePluginException(plugAssem, loaderEX)

            ' JS 29nov08: only assert when this is a confirmed plug-in.
            '             (which will not be the case since the manager could not access 
            '             the Types contained within the assembly)
            If bHasPlugins Then
                Debug.Assert(False, Me.ToString & ".LoadPluginAssembly() " & vbNewLine & strFileName & vbNewLine & loaderEX.Message)
            End If

        Catch ex As Exception

            'catch any generic exceptions
            Me.RaisePluginException(plugAssem, ex)
            Debug.Assert(False, Me.ToString & ".LoadPluginAssembly() " & vbNewLine & strFileName & vbNewLine & ex.Message)

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

        Dim pa As cPluginAssembly = Nothing

        ' Sanity check
        If (Not Me.m_dictAssemblies.ContainsKey(strFileName)) Then
            Return False
        End If

        ' Get plugin assembly
        pa = Me.m_dictAssemblies(strFileName)
        ' Inform the world
        RaiseEvent AssemblyRemoved(pa)
        ' Remove from internal admin
        Me.m_dictAssemblies.Remove(strFileName)

        Return True

    End Function

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

#Region " Generic invocation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke a generic method on all plugins of a specific type.
    ''' </summary>
    ''' <param name="typePlugin">The <see cref="Type">Type</see> of the plugin.</param>
    ''' <param name="strMethod">The name of the method to invoke.</param>
    ''' <param name="aArgs">The arguments to pass to the method to invoke.</param>
    ''' <returns>True if the method could be found for the given type.</returns>
    ''' <remarks>
    ''' <para>Note that this method tries to match argument types to the values
    ''' provided in <paramref name="aArgs">aArgs</paramref>. If this array of values 
    ''' happens to contain Null (or Nothing), call <see cref="InvokeMethod">InvokeMethod</see>
    ''' instead.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function TryInvokeMethod(ByVal typePlugin As Type, ByVal strMethod As String, ByVal aArgs() As Object) As Boolean

        Dim aArgTypes As Type() = Nothing

        ' Get the types of the method parameters
        If aArgs IsNot Nothing Then
            ReDim aArgTypes(aArgs.Length - 1)
            For i As Integer = 0 To aArgs.Length - 1
                aArgTypes(i) = aArgs(i).GetType()
            Next
        End If

        Return Me.InvokeMethod(typePlugin, strMethod, aArgTypes, aArgs)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke a generic method on all plugins of a specific type.
    ''' </summary>
    ''' <param name="typePlugin">The <see cref="Type">Type</see> of the plugin.</param>
    ''' <param name="strMethod">The name of the method to invoke.</param>
    ''' <param name="aArgTypes">The <see cref="Type">Type</see> of the individual
    ''' method parameters.</param>
    ''' <param name="aArgs">The arguments to pass to the method to invoke.</param>
    ''' <returns>True if the method could be found for the given type.</returns>
    ''' -----------------------------------------------------------------------
    Public Function InvokeMethod(ByVal typePlugin As Type, ByVal strMethod As String, _
            ByVal aArgTypes() As Type, ByVal aArgs() As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(typePlugin)
        Dim mi As MethodInfo = Nothing

        ' Try to get the method
        mi = typePlugin.GetMethod(strMethod, _
                BindingFlags.IgnoreCase Or BindingFlags.IgnoreReturn Or BindingFlags.Instance Or BindingFlags.Public, _
                Nothing, aArgTypes, Nothing)

        ' Any luck?
        If (mi Is Nothing) Then Return False

        ' Invoke method on each plugin
        For Each ipc As cPluginContext In collPlugins
            mi.Invoke(ipc.Plugin, aArgs)
        Next ipc

    End Function

#End Region ' Generic invocation

#Region " Database Plugin "

    Private Class IDatabaseUpdatePluginContextSort
        Implements IComparer(Of cPluginContext)

        Public Function Compare(ByVal x As cPluginContext, ByVal y As cPluginContext) As Integer _
                Implements System.Collections.Generic.IComparer(Of cPluginContext).Compare
            Return CInt(IIf(DirectCast(x.Plugin, IDatabaseUpdatePlugin).UpdateVersion < DirectCast(y.Plugin, IDatabaseUpdatePlugin).UpdateVersion, -1, 1))
        End Function

    End Class

    Public Sub UpdateDatabase(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single)

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDatabaseUpdatePlugin))
        Dim lPlugins As New List(Of cPluginContext)
        Dim ip As IDatabaseUpdatePlugin = Nothing
        Dim strDescription As String = ""

        ' Sanity checks
        If db Is Nothing Then Return
        If db.GetVersion() < sBaselineVersion Then Return

        ' Transform collection into list (there must be a better way?)
        For Each ipc As cPluginContext In collPlugins
            lPlugins.Add(ipc)
        Next

        lPlugins.Sort(New IDatabaseUpdatePluginContextSort())

        For Each ipc As cPluginContext In lPlugins
            ' Get plugin
            ip = DirectCast(ipc.Plugin, IDatabaseUpdatePlugin)
            ' Check
            If (ip.UpdateVersion > db.GetVersion() Or ip.UpdateVersion = -9999) Then
                Try
                    If ip.ApplyUpdate(db) Then

                        Dim sbDescription As New System.Text.StringBuilder()
                        Dim iBit As Integer = 0
                        For Each strBit As String In ip.UpdateDescription.Split(New String() {"." & vbNewLine, vbNewLine}, StringSplitOptions.RemoveEmptyEntries)
                            strBit = strBit.Trim
                            If Not String.IsNullOrEmpty(strBit) Then
                                If iBit > 0 Then sbDescription.Append("; ")
                                sbDescription.Append(strBit)
                                iBit += 1
                            End If
                        Next
                        db.SetVersion(ip.UpdateVersion, sbDescription.ToString())
                    Else
                        Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "IDatabaseUpdatePlugin.ApplyUpdate", New Exception("(generic failure)"))
                    End If

                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "IDatabaseUpdatePlugin.ApplyUpdate", ex)
                End Try

            End If
        Next
    End Sub

    Public Function HasDatabaseUpdates(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDatabaseUpdatePlugin))
        Dim lPlugins As New List(Of cPluginContext)
        Dim ip As IDatabaseUpdatePlugin = Nothing
        Dim sVerDB As Single = db.GetVersion()

        ' Sanity checks
        If db Is Nothing Then Return False
        If sVerDB < sBaselineVersion Then Return False

        ' Transform collection into list (there must be a better way?)
        For Each ipc As cPluginContext In collPlugins
            lPlugins.Add(ipc)
        Next

        lPlugins.Sort(New IDatabaseUpdatePluginContextSort())

        For Each ipc As cPluginContext In lPlugins
            ip = DirectCast(ipc.Plugin, IDatabaseUpdatePlugin)
            If (ip.UpdateVersion > sVerDB) Or (ip.UpdateVersion = -9999) Then
                Return True
            End If
        Next
        Return False

    End Function

#End Region ' Database Plugin

#Region " Core Plugin "

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(ICorePlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, ICorePlugin).CoreInitialized(objEcoPath, objEcoSim, objEcoSpace)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "CoreInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDataValidatedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IDataValidatedPlugin).DataValidated(varname, datatype)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "DataValidated", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcopathPlugin))
        Dim bSucces As Boolean = True

        For Each ipc As cPluginContext In collPlugins
            Try
                bSucces = bSucces And DirectCast(ipc.Plugin, IEcopathPlugin).LoadModel(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "LoadModel", ex)
            End Try
        Next

        Return bSucces

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcopathPlugin))
        Dim bSucces As Boolean = True

        For Each ipc As cPluginContext In collPlugins
            Try
                bSucces = bSucces And DirectCast(ipc.Plugin, IEcopathPlugin).SaveModel(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SaveModel", ex)
            End Try
        Next

        Return bSucces

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, open a plug-in database link.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Function OpenDatabase(ByVal strName As String) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDatabasePlugin))
        Dim bSucces As Boolean = True

        For Each ipc As cPluginContext In collPlugins
            Try
                bSucces = bSucces And DirectCast(ipc.Plugin, IDatabasePlugin).Open(strName)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "Open", ex)
            End Try
        Next

        Return bSucces

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, polls all plug-ins for unsaved data modifications.
    ''' </summary>
    ''' <param name="pa">cPluginAssembly to check, if any.</param>
    ''' ---------------------------------------------------------------------------
    Public Function IsDatabaseModified(Optional ByVal pa As cPluginAssembly = Nothing) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDatabasePlugin), pa)
        Dim bIsChanged As Boolean = False

        For Each ipc As cPluginContext In collPlugins
            Try
                bIsChanged = bIsChanged Or DirectCast(ipc.Plugin, IDatabasePlugin).IsModified()
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "IsModified", ex)
            End Try
        Next

        Return bIsChanged

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Bridge, close a plug-in data link.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Sub CloseDatabase()

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDatabasePlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IDatabasePlugin).Close()
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "CloseDatabase", ex)
            End Try
        Next

    End Sub

#End Region ' Core Plugin

#Region " Ecopath Plugin"

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcopathMassBalancePlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                If DirectCast(ipc.Plugin, IEcopathMassBalancePlugin).EcopathMassBalance(EcoPathDataStructures, EstimateFor, iResult) = True Then
                    Return True
                End If
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcopathMassBalance", ex)
            End Try
        Next
        Return False

    End Function

    Public Function EcopathRunCompleted(ByVal EcoPathDataStructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcopathRunCompletedPlugin))
        Try
            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcopathRunCompletedPlugin).EcopathRunCompleted(EcoPathDataStructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcopathRunCompleted", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcopathRunCompletedPostPlugin))
        Try
            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcopathRunCompletedPostPlugin).EcopathRunCompletedPost(EcoPathDataStructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcopathRunCompletedPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

#End Region ' Ecopath Plugin

#Region " Ecosim Plugin "

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimPlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcosimPlugin).LoadEcosimScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "LoadEcosimScenario", ex)
            End Try
        Next

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimPlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcosimPlugin).SaveEcosimScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SaveEcosimScenario", ex)
            End Try
        Next

    End Sub

    Public Function EcosimInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimInitializedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimInitializedPlugin).EcosimInitialized(EcosimDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcosimModifyTimeseries(ByVal TimeSeriesDataStructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimModifyTimeseriesPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimModifyTimeseriesPlugin).EcosimModifyTimeseries(TimeSeriesDataStructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimModifyTimeseries", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcosimModifyFGear(ByVal FGear As Object, ByVal EcosimDataStructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimModifyFGearPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimModifyFGearPlugin).EcosimModifyFGear(FGear, EcosimDataStructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimModifyTimeseries", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, _
                                        ByVal EcosimDataStructures As Object, _
                                        ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimBeginTimestepPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimBeginTimestepPlugin).EcosimBeginTimeStep(BiomassAtTimestep, EcosimDataStructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimBeginTimeStep", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcosimBeginTimestepPostPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimBeginTimestepPostPlugin).EcosimBeginTimeStepPost(BiomassAtTimestep, EcosimDataStructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimBeginTimeStepPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimEndTimestepPlugin))
        Try
            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimEndTimestepPlugin).EcosimEndTimeStep(BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimEndTimeStep", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcosimEndTimestepPostPlugin))
        Try
            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimEndTimestepPostPlugin).EcosimEndTimeStepPost(BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimEndTimeStepPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcosimRunInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimRunInitializedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimRunInitializedPlugin).EcosimRunInitialized(EcosimDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimRunInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function


    Public Function EcosimRunCompleted(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcosimRunCompletedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimRunCompletedPlugin).EcosimRunCompleted(EcosimDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimRunCompleted", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcosimRunCompletedPostPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcosimRunCompletedPostPlugin).EcosimRunCompletedPost(EcosimDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcosimRunCompletedPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

#End Region ' Ecosim Plugins

#Region " Ecospace Plugins "

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospacePlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcospacePlugin).LoadEcospaceScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "LoadEcospaceScenario", ex)
            End Try
        Next

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Invokes right after LoadEcospaceScenario
    ''' </summary>
    ''' <param name="EcospaceDatastructures"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' ---------------------------------------------------------------------------
    Public Function EcospaceInitialized(ByVal EcospaceDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospaceInitializedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospaceInitializedPlugin).EcospaceInitialized(EcospaceDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospaceInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospacePlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcospacePlugin).SaveEcospaceScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SaveEcospaceScenario", ex)
            End Try
        Next

    End Sub

    Public Function EcospaceBeginTimeStep(ByVal EcospaceDataStructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospaceBeginTimestepPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospaceBeginTimestepPlugin).EcospaceBeginTimeStep(EcospaceDataStructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospaceBeginTimeStep", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcospaceBeginTimestepPostPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospaceBeginTimestepPostPlugin).EcospaceBeginTimeStepPost(EcospaceDataStructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospaceBeginTimeStepPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcospacePostFishingEffortModTimestep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospacePostFishingEffortModTimestepPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospacePostFishingEffortModTimestepPlugin).EcospacePostFishingEffortModTimestep(EcospaceDatastructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospacePostFishingEffortModTimestep", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcospaceEndTimestepPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospaceEndTimestepPlugin).EcospaceEndTimeStep(EcospaceDatastructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospaceEndTimeStep", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        collPlugins = Me.GetPlugins(GetType(IEcospaceEndTimestepPostPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcospaceEndTimestepPostPlugin).EcospaceEndTimeStepPost(EcospaceDatastructures, iTimeStep)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcospaceEndTimeStepPost", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

#End Region ' Ecospace Plugins

#Region " Ecotracer Plugins "

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcotracerPlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcotracerPlugin).LoadEcotracerScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "LoadEcotracerScenario", ex)
            End Try
        Next

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcotracerInitializedPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IEcotracerInitializedPlugin).EcotracerInitialized(EcotracerDatastructures)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "EcotracerInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IEcotracerPlugin))

        For Each ipc As cPluginContext In collPlugins
            Try
                DirectCast(ipc.Plugin, IEcotracerPlugin).SaveEcotracerScenario(dataSource)
            Catch ex As Exception
                Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SaveEcotracerScenario", ex)
            End Try
        Next

    End Sub

#End Region ' Ecotracer Plugins

#Region " Data Exchange Plugins "

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

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDataConsumerPlugin))
        Dim bHandled As Boolean = False

        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    bHandled = bHandled Or DirectCast(ipc.Plugin, IDataConsumerPlugin).ReceiveData(strDataName, data)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "ReceiveData", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return bHandled

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all <see cref="IPluginData">plug-in data</see> from loaded
    ''' <see cref="IDataProducerPlugin">IDataProducerPlugin</see>
    ''' instances that expose data under a given name.
    ''' </summary>
    ''' <param name="strDataName">The name of the data to match.</param>
    ''' <returns>An array of data, or an empty array if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetData(ByVal strDataName As String) As IPluginData()

        Dim coll As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDataProducerPlugin))
        Dim data As IPluginData = Nothing
        Dim lData As New List(Of IPluginData)

        Try
            For Each ipc As cPluginContext In coll
                Try
                    If DirectCast(ipc.Plugin, IDataProducerPlugin).GetDataByName(strDataName, data) Then
                        lData.Add(data)
                    End If
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "GetDataByName", ex)
                End Try
            Next

        Catch ex As Exception
        End Try

        Return lData.ToArray()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all <see cref="IPluginData">plug-in data</see> from loaded
    ''' <see cref="IDataProducerPlugin">IDataProducerPlugin</see>
    ''' instances that expose data of a given <see cref="Type">Type</see>.
    ''' </summary>
    ''' <param name="dataType">The <see cref="Type">Type</see> of the data to
    ''' obtain.</param>
    ''' <returns>An array of data, or an empty array if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetData(ByVal dataType As Type) As IPluginData()

        Dim coll As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IDataProducerPlugin))
        Dim data As IPluginData = Nothing
        Dim lData As New List(Of IPluginData)

        Try

            For Each ipc As cPluginContext In coll

                Try
                    If DirectCast(ipc.Plugin, IDataProducerPlugin).GetDataByType(dataType, data) Then
                        lData.Add(data)
                    End If
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "GetDataByType", ex)
                End Try

            Next

        Catch ex As Exception
        End Try

        Return lData.ToArray()

    End Function

#End Region ' Data Exchange Plugins 

#Region " Search plugins "

    Public Function SearchInitialized(ByVal SearchDS As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IFishingPolicySearchPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc.Plugin, IFishingPolicySearchPlugin).SearchInitialized(SearchDS)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SearchInitialized", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function SearchFunctionCall(ByVal SearchDS As Object) As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IFishingPolicySearchPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc, IFishingPolicySearchPlugin).SearchFunctionCall(SearchDS)
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SearchFunctionCall", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function SearchIterationsStarting() As Boolean

        Dim collPlugins As ICollection(Of cPluginContext) = Me.GetPlugins(GetType(IFishingPolicySearchPlugin))
        Try

            For Each ipc As cPluginContext In collPlugins
                Try
                    DirectCast(ipc, IFishingPolicySearchPlugin).SearchIterationsStarting()
                Catch ex As Exception
                    Me.RaisePluginException(ipc.Assembly, ipc.Plugin, "SearchIterationsStarting", ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

#End Region ' Search plugins

#End Region ' Plugin invocation

#Region " Plugin access "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of <see cref="IPlugin">plug-ins</see> of a given 
    ''' <see cref="Type">Type</see>.
    ''' </summary>
    ''' <param name="t">The <see cref="Type">Type</see> of the plugins to retrieve.</param>
    ''' <param name="pa">The <see cref="cPluginAssembly">plug-in assembly</see> to search.
    ''' If not specified, all plug-in assemblies will be searched.</param>
    ''' <returns>A collection of <see cref="cPluginContext">plug-in contexts</see>
    ''' linking to plug-ins of the given type.</returns>
    ''' ---------------------------------------------------------------------------
    Friend Function GetPlugins(ByVal t As Type, _
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
            collPlugins = Me.GetPlugins(GetType(IGUIPlugin))
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
                                                 Path.GetFileNameWithoutExtension(assembly.Filename), _
                                                 ex.Message)

        Me.RaisePluginException(New cPluginException(assembly, strMessage, ex))

    End Sub

    Friend Sub RaisePluginException(ByVal assembly As cPluginAssembly, ByVal plugin As IPlugin, _
                                    ByVal strMethodName As String, ByVal ex As Exception)

        Dim strMessage As String = String.Format(My.Resources.PLUGIN_ERROR_POINT, _
                                                 Path.GetFileNameWithoutExtension(assembly.Filename), _
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
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible), cPluginAssembly.ePluginCompatibilityTypes)
                    End If

                    ' Build difference?
                    If anExpected.Version.Build <> anameLoaded.Version.Build Then
                        ' #Yes: take caution
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution), cPluginAssembly.ePluginCompatibilityTypes)
                    End If

                    ' Minor version number difference?
                    If anExpected.Version.Minor <> anameLoaded.Version.Minor Then
                        ' #Yes: take caution
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution), cPluginAssembly.ePluginCompatibilityTypes)
                    End If

                    ' Major version number difference?
                    If anExpected.Version.Major <> anameLoaded.Version.Major Then
                        ' #Yes: assume incompatible
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionIncompatible), cPluginAssembly.ePluginCompatibilityTypes)
                    End If

                End If
            Next
        Next

        Return compatibility

    End Function

#End Region ' Private helper methods

End Class
