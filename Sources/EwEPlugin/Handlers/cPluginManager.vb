'==============================================================================
'
' $Log: cPluginManager.vb,v $
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

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in manager, handles loading and enabling of <see cref="IPlugin">EwE plug-ins</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginManager
    Implements IDataBroadcaster

#Region " Initialization "

    ''' <summary>The one core for this plugin manager.</summary>
    Private m_core As Object = Nothing
    ''' <summary>Delegate that this class can use to check whether the current core
    ''' execution state allows a plug-in to run.</summary>
    Private m_dlgtCoreState As CanExecutePlugin = Nothing

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
            For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
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
                            ' Stick it up
                            plugAssem.Plugin(ip.Name) = ip
                            ' Core assigned?
                            If (Me.m_core IsNot Nothing) Then
                                Try
                                    ' Initialize plugin
                                    ip.Initialize(Me.m_core)
                                Catch ex As Exception
                                    ' Disable the plugin entirely
                                    plugAssem.Compatibility = cPluginAssembly.ePluginCompatibilityTypes.IncompatibleUndetermined
                                End Try
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
            RaiseEvent PluginException(loaderEX)

            ' JS 29nov08: only assert when this is a confirmed plug-in.
            '             (which will not be the case since the manager could not access 
            '             the Types contained within the assembly)
            If bHasPlugins Then
                Debug.Assert(False, Me.ToString & ".LoadPluginAssembly() " & vbNewLine & strFileName & vbNewLine & loaderEX.Message)
            End If

        Catch ex As Exception

            'catch any generic exceptions
            RaiseEvent PluginException(ex)
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
    Public Delegate Sub PluginExceptionHandler(ByVal PluginException As Exception)

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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(typePlugin)
        Dim mi As MethodInfo = Nothing

        ' Try to get the method
        mi = typePlugin.GetMethod(strMethod, _
                BindingFlags.IgnoreCase Or BindingFlags.IgnoreReturn Or BindingFlags.Instance Or BindingFlags.Public, _
                Nothing, aArgTypes, Nothing)

        ' Any luck?
        If (mi Is Nothing) Then Return False

        ' Invoke method on each plugin
        For Each ip As IPlugin In collPlugins
            mi.Invoke(ip, aArgs)
        Next ip

    End Function

#End Region ' Generic invocation

#Region " Database Plugin "

    Private Class IDatabaseUpdatePluginSort
        Implements IComparer(Of IDatabaseUpdatePlugin)

        Public Function Compare(ByVal x As IDatabaseUpdatePlugin, ByVal y As IDatabaseUpdatePlugin) As Integer _
                Implements System.Collections.Generic.IComparer(Of IDatabaseUpdatePlugin).Compare
            Return CInt(IIf(x.UpdateVersion < y.UpdateVersion, -1, 1))
        End Function

    End Class

    Public Sub UpdateDatabase(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single)
        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IDatabaseUpdatePlugin))
        Dim lPlugins As New List(Of IDatabaseUpdatePlugin)
        Dim strDescription As String = ""

        ' Sanity checks
        If db Is Nothing Then Return
        If db.GetVersion() < sBaselineVersion Then Return

        ' Transform collection into list (there must be a better way?)
        For Each ip As IPlugin In collPlugins
            lPlugins.Add(DirectCast(ip, IDatabaseUpdatePlugin))
        Next

        lPlugins.Sort(New IDatabaseUpdatePluginSort())

        For Each ip As IDatabaseUpdatePlugin In lPlugins
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
                        Console.WriteLine("Failed to run plug-in {0}", ip.Description)
                    End If
                Catch ex As Exception
                    Debug.Assert(False, String.Format("Failed to run database plugin {0}, reason {1}", ip.Description, ex.Message))
                End Try
            End If
        Next
    End Sub

    Public Function HasDatabaseUpdates(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single) As Boolean
        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IDatabaseUpdatePlugin))
        Dim lPlugins As New List(Of IDatabaseUpdatePlugin)
        Dim sVerDB As Single = db.GetVersion()

        ' Sanity checks
        If db Is Nothing Then Return False
        If sVerDB < sBaselineVersion Then Return False

        ' Transform collection into list (there must be a better way?)
        For Each ip As IPlugin In collPlugins
            lPlugins.Add(DirectCast(ip, IDatabaseUpdatePlugin))
        Next

        lPlugins.Sort(New IDatabaseUpdatePluginSort())

        For Each ip As IDatabaseUpdatePlugin In lPlugins
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(ICorePlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, ICorePlugin).CoreInitialized(objEcoPath, objEcoSim, objEcoSpace)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " CoreInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IDataValidatedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IDataValidatedPlugin).DataValidated(varname, datatype)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " DataValidated() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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
    Public Sub LoadModel(ByVal dataSource As Object)

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcopathPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcopathPlugin).LoadModel(dataSource)
        Next

    End Sub

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
    Public Sub SaveModel(ByVal dataSource As Object)

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcopathPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcopathPlugin).SaveModel(dataSource)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcopathMassBalancePlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            If DirectCast(ip, IEcopathMassBalancePlugin).EcopathMassBalance(EcoPathDataStructures, EstimateFor, iResult) = True Then
                Return True
            End If
        Next
        Return False

    End Function

    Public Function EcopathRunCompleted(ByVal EcoPathDataStructures As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcopathRunCompletedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcopathRunCompletedPlugin).EcopathRunCompleted(EcoPathDataStructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcopathHasRun() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcosimPlugin).LoadEcosimScenario(dataSource)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcosimPlugin).SaveEcosimScenario(dataSource)
        Next

    End Sub


    Public Function EcosimInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimInitializedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimInitializedPlugin).EcosimInitialized(EcosimDatastructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcosimModifyTimeseries(ByVal TimeSeriesDataStructures As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimModifyTimeseriesPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimModifyTimeseriesPlugin).EcosimModifyTimeseries(TimeSeriesDataStructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimModifyTimeseries() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDataStructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimBeginTimestepPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimBeginTimestepPlugin).EcosimBeginTimeStep(BiomassAtTimestep, EcosimDataStructures, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimBeginTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    'Public Function EcosimEndTimeStepStats(ByVal EcosimIndicies As Object) As Boolean

    '    Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimEndTimestepStatsPlugin))
    '    Try

    '        ' give every plugin that supports this interface a chance at running
    '        For Each ip As IPlugin In collPlugins
    '            Try 'protect the core from a plugin exploding
    '                DirectCast(ip, IEcosimEndTimestepStatsPlugin).EcosimEndTimestepStatsPlugin(EcosimIndicies)
    '            Catch ex As Exception
    '                Debug.Assert(False, ip.Name & " EcosimEndTimeStatsStep() Error: " & ex.Message)
    '                'tell the world
    '                RaiseEvent PluginException(ex)
    '            End Try
    '        Next

    '    Catch ex As Exception
    '        Return False
    '    End Try

    'End Function

    Public Function EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTimeStep As Integer, ByVal Ecosimresults As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimEndTimestepPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimEndTimestepPlugin).EcosimEndTimeStep(BiomassAtTimestep, EcosimDatastructures, iTimeStep, Ecosimresults)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimEndTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcosimRunInitialized(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimRunInitializedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimRunInitializedPlugin).EcosimRunInitialized(EcosimDatastructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function


    Public Function EcosimRunCompleted(ByVal EcosimDatastructures As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcosimRunCompletedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcosimRunCompletedPlugin).EcosimRunCompleted(EcosimDatastructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcosimRunCompleted() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospacePlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcospacePlugin).LoadEcospaceScenario(dataSource)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceInitializedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceInitializedPlugin).EcospaceInitialized(EcospaceDatastructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospacePlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcospacePlugin).SaveEcospaceScenario(dataSource)
        Next

    End Sub

    Public Function EcospaceBeginTimeStep(ByVal EcospaceDataStructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceBeginTimestepPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceBeginTimestepPlugin).EcospaceBeginTimeStep(EcospaceDataStructures, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceBeginTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcospacePostFishingEffortModTimestep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceEndTimestepPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospacePostFishingEffortModTimestepPlugin).EcospacePostFishingEffortModTimestep(EcospaceDatastructures, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceEndTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceEndTimestepPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceEndTimestepPlugin).EcospaceEndTimeStep(EcospaceDatastructures, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceEndTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

#If 0 Then
    ' JS: disabled, exchanging layer data is NOT trivial and needs to be thought out very well

    Public Function EcospaceLayerExchangeStartRun(ByVal layer As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceLayerExchangePlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceLayerExchangePlugin).EcospaceStartRun(layer)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceLayerExchangeStartRun() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcospaceLayerExchangeBeginTimeStep(ByVal layer As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceLayerExchangePlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceLayerExchangePlugin).EcospaceBeginTimeStep(layer, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceBeginTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcospaceLayerExchangeEndTimeStep(ByVal layer As Object, ByVal iTimeStep As Integer) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceLayerExchangePlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceLayerExchangePlugin).EcospaceEndTimeStep(layer, iTimeStep)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceEndTimeStep() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function EcospaceLayerExchangeEndRun(ByVal layer As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcospaceLayerExchangePlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcospaceLayerExchangePlugin).EcospaceEndRun(layer)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcospaceEndRun() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

    End Function

#End If

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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcotracerPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcotracerPlugin).LoadEcotracerScenario(dataSource)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcotracerInitializedPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, IEcotracerInitializedPlugin).EcotracerInitialized(EcotracerDatastructures)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " EcotracerInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IEcotracerPlugin))

        ' Find first available plugin that implements a datasource save plugin point
        For Each ip As IPlugin In collPlugins
            DirectCast(ip, IEcotracerPlugin).SaveEcotracerScenario(dataSource)
        Next

    End Sub

#End Region ' Ecotracer Plugins

#Region " Data Exchange Plugins "

    ''' <summary>
    ''' Exchange data from a <see cref="IDataProducerPlugin">data producer plug-in</see>
    ''' to any interested <see cref="IDataConsumerPlugin">data consumer plug-in</see>.
    ''' </summary>
    ''' <param name="ds">The data to exchange.</param>
    ''' <returns></returns>
    Public Function BroadcastData(ByVal strDataName As String, ByVal ds As DataSet) As Boolean _
            Implements IDataBroadcaster.BroadcastData

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IDataConsumerPlugin))
        Dim bHandled As Boolean = False

        Try

            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    bHandled = bHandled Or DirectCast(ip, IDataConsumerPlugin).ReceiveData(strDataName, ds)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " BroadcastData() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
                End Try
            Next

        Catch ex As Exception
            Return False
        End Try

        Return bHandled

    End Function

#End Region ' Data Exchange Plugins 

#Region " Search plugins "

    Public Function SearchInitialized(ByVal SearchDS As Object) As Boolean

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(ISearchPlugin))
        Try

            ' give every plugin that supports this interface a chance at running
            For Each ip As IPlugin In collPlugins
                Try 'protect the core from a plugin exploding
                    DirectCast(ip, ISearchPlugin).SearchInitialized(SearchDS)
                Catch ex As Exception
                    Debug.Assert(False, ip.Name & " SearchInitialized() Error: " & ex.Message)
                    'tell the world
                    RaiseEvent PluginException(ex)
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
    ''' <returns>A collection of <see cref="IPlugin">plug-ins</see> of the given type.</returns>
    ''' ---------------------------------------------------------------------------
    Public Function GetPlugins(ByVal t As Type) As ICollection(Of IPlugin)
        Dim collPlugins As New List(Of IPlugin)
        For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
            collPlugins.AddRange(pa.Plugins(t))
        Next
        Return collPlugins
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a <see cref="IPlugin">plug-in</see> with a given name.
    ''' </summary>
    ''' <param name="strName">Name of the plugin to return. Names are
    ''' case insensitive.</param>
    ''' -----------------------------------------------------------------------
    Public Function GetPlugin(ByVal strName As String) As ICollection(Of IPlugin)
        Dim collPlugins As New List(Of IPlugin)
        For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
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
    ''' and (optionally) by <see cref="AssemblyName.Version">version</see>.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="ver"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property PluginAssembly(ByVal strName As String, Optional ByVal ver As Version = Nothing) As cPluginAssembly
        Get
            Dim an As AssemblyName = Nothing
            Dim bFound As Boolean = False
            For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
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
            Return m_dictAssemblies.Values
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
            For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
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
        For Each pa As cPluginAssembly In Me.m_dictAssemblies.Values
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

        Dim collPlugins As ICollection(Of IPlugin) = Me.GetPlugins(GetType(IGUIPlugin))
        Dim bEnable As Boolean = True

        If ip IsNot Nothing Then
            ' Check if plugin can execute
            bEnable = Me.m_dlgtCoreState.Invoke(DirectCast(ip, IGUIPlugin).EnabledState)
            ' Broadcast plugin enabled state event
            RaiseEvent PluginEnabled(DirectCast(ip, IGUIPlugin), bEnable)
        Else
            'For all GUI plugins
            For Each ip In collPlugins
                ' Check if plugin can execute
                bEnable = Me.m_dlgtCoreState.Invoke(DirectCast(ip, IGUIPlugin).EnabledState)
                ' Broadcast plugin enabled state event
                RaiseEvent PluginEnabled(DirectCast(ip, IGUIPlugin), bEnable)
            Next
        End If

    End Sub

#End Region ' Plugin core state response

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
                    ' #Yep: test if versions match
                    If anExpected.Version.Revision <> anameLoaded.Version.Revision Then
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution), cPluginAssembly.ePluginCompatibilityTypes)
                    End If
                    If anExpected.Version.Build <> anameLoaded.Version.Build Then
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution), cPluginAssembly.ePluginCompatibilityTypes)
                    End If
                    If anExpected.Version.Minor <> anameLoaded.Version.Minor Then
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionIncompatible), cPluginAssembly.ePluginCompatibilityTypes)
                    End If
                    If anExpected.Version.Major <> anameLoaded.Version.Major Then
                        compatibility = DirectCast(Math.Max(compatibility, cPluginAssembly.ePluginCompatibilityTypes.VersionIncompatible), cPluginAssembly.ePluginCompatibilityTypes)
                    End If
                End If
            Next
        Next

        Return compatibility

    End Function

#End Region ' Private helper methods

End Class
