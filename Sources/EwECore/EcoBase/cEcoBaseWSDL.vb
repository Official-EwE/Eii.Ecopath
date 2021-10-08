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
Namespace WebServices.Ecobase


    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ServiceModel.ServiceContractAttribute([Namespace]:="http://sirs.agrocampus-ouest.fr/EcoTroph/php/webser/operation_1.wsdl", ConfigurationName:="getResultPortType")>
    Public Interface getResultPortType
        <System.ServiceModel.OperationContractAttribute(Action:="list_models", ReplyAction:="*")>
        <System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded)>
        Function list_models(ByVal request As list_modelsRequest) As list_modelsResponse
        <System.ServiceModel.OperationContractAttribute(Action:="list_models", ReplyAction:="*")>
        Function list_modelsAsync(ByVal request As list_modelsRequest) As System.Threading.Tasks.Task(Of list_modelsResponse)
        <System.ServiceModel.OperationContractAttribute(Action:="getModel", ReplyAction:="*")>
        <System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded)>
        Function getModel(ByVal request As getModelRequest) As getModelResponse
        <System.ServiceModel.OperationContractAttribute(Action:="getModel", ReplyAction:="*")>
        Function getModelAsync(ByVal request As getModelRequest) As System.Threading.Tasks.Task(Of getModelResponse)
        <System.ServiceModel.OperationContractAttribute(Action:="Upload_Model", ReplyAction:="*")>
        <System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded)>
        Function Upload_Model(ByVal request As Upload_ModelRequest) As getModelResponse
        <System.ServiceModel.OperationContractAttribute(Action:="Upload_Model", ReplyAction:="*")>
        Function Upload_ModelAsync(ByVal request As Upload_ModelRequest) As System.Threading.Tasks.Task(Of getModelResponse)
    End Interface

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <System.ServiceModel.MessageContractAttribute(WrapperName:="list_models", WrapperNamespace:="urn:sirs:getResult", IsWrapped:=True)>
    Partial Public Class list_modelsRequest
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=0)>
        Public operation As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal operation As String)
            Me.operation = operation
        End Sub
    End Class

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <System.ServiceModel.MessageContractAttribute(WrapperName:="list_modelsResponse", WrapperNamespace:="urn:sirs:getResult", IsWrapped:=True)>
    Partial Public Class list_modelsResponse
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=0)>
        Public result As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal result As String)
            Me.result = result
        End Sub
    End Class

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <System.ServiceModel.MessageContractAttribute(WrapperName:="getModel", WrapperNamespace:="urn:sirs:getResult", IsWrapped:=True)>
    Partial Public Class getModelRequest
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=0)>
        Public operation As String
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=1)>
        Public model_number As Integer
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=2)>
        Public admin As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal operation As String, ByVal model_number As Integer, ByVal admin As String)
            Me.operation = operation
            Me.model_number = model_number
            Me.admin = admin
        End Sub
    End Class

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <System.ServiceModel.MessageContractAttribute(WrapperName:="getModelResponse", WrapperNamespace:="urn:sirs:getResult", IsWrapped:=True)>
    Partial Public Class getModelResponse
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=0)>
        Public result As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal result As String)
            Me.result = result
        End Sub
    End Class

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <System.ServiceModel.MessageContractAttribute(WrapperName:="Upload_Model", WrapperNamespace:="urn:sirs:getResult", IsWrapped:=True)>
    Partial Public Class Upload_ModelRequest
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=0)>
        Public model_number As Integer
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="", Order:=1)>
        Public model_data As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal model_number As Integer, ByVal model_data As String)
            Me.model_number = model_number
            Me.model_data = model_data
        End Sub
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    Interface getResultPortTypeChannel
        Inherits getResultPortType, System.ServiceModel.IClientChannel
    End Interface

    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>
    Partial Public Class cEcobaseWDSL
        Inherits System.ServiceModel.ClientBase(Of getResultPortType)
        Implements getResultPortType

        Public Sub New()
        End Sub

        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub

        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub

        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub

        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function list_models(ByVal request As list_modelsRequest) As list_modelsResponse
            Return MyBase.Channel.list_models(request)
        End Function

        Public Function list_models(ByVal operation As String) As String
            Dim inValue As list_modelsRequest = New list_modelsRequest()
            inValue.operation = operation
            Dim retVal As list_modelsResponse = (CType((Me), getResultPortType)).list_models(inValue)
            Return retVal.result
        End Function

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function list_modelsAsync(ByVal request As list_modelsRequest) As System.Threading.Tasks.Task(Of list_modelsResponse)
            Return MyBase.Channel.list_modelsAsync(request)
        End Function

        Public Function list_modelsAsync(ByVal operation As String) As System.Threading.Tasks.Task(Of list_modelsResponse)
            Dim inValue As list_modelsRequest = New list_modelsRequest()
            inValue.operation = operation
            Return (CType((Me), getResultPortType)).list_modelsAsync(inValue)
        End Function

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function getModel(ByVal request As getModelRequest) As getModelResponse
            Return MyBase.Channel.getModel(request)
        End Function

        Public Function getModel(ByVal operation As String, ByVal model_number As Integer, ByVal admin As String) As String
            Dim inValue As getModelRequest = New getModelRequest()
            inValue.operation = operation
            inValue.model_number = model_number
            inValue.admin = admin
            Dim retVal As getModelResponse = (CType((Me), getResultPortType)).getModel(inValue)
            Return retVal.result
        End Function

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function getModelAsync(ByVal request As getModelRequest) As System.Threading.Tasks.Task(Of getModelResponse)
            Return MyBase.Channel.getModelAsync(request)
        End Function

        Public Function getModelAsync(ByVal operation As String, ByVal model_number As Integer, ByVal admin As String) As System.Threading.Tasks.Task(Of getModelResponse)
            Dim inValue As getModelRequest = New getModelRequest()
            inValue.operation = operation
            inValue.model_number = model_number
            inValue.admin = admin
            Return (CType((Me), getResultPortType)).getModelAsync(inValue)
        End Function

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function Upload_Model(ByVal request As Upload_ModelRequest) As getModelResponse
            Return MyBase.Channel.Upload_Model(request)
        End Function

        Public Function Upload_Model(ByVal model_number As Integer, ByVal model_data As String) As String
            Dim inValue As Upload_ModelRequest = New Upload_ModelRequest()
            inValue.model_number = model_number
            inValue.model_data = model_data
            Dim retVal As getModelResponse = (CType((Me), getResultPortType)).Upload_Model(inValue)
            Return retVal.result
        End Function

        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Private Function Upload_ModelAsync(ByVal request As Upload_ModelRequest) As System.Threading.Tasks.Task(Of getModelResponse)
            Return MyBase.Channel.Upload_ModelAsync(request)
        End Function

        Public Function Upload_ModelAsync(ByVal model_number As Integer, ByVal model_data As String) As System.Threading.Tasks.Task(Of getModelResponse)
            Dim inValue As Upload_ModelRequest = New Upload_ModelRequest()
            inValue.model_number = model_number
            inValue.model_data = model_data
            Return (CType((Me), getResultPortType)).Upload_ModelAsync(inValue)
        End Function

        Private Function getResultPortType_list_models(request As list_modelsRequest) As list_modelsResponse Implements getResultPortType.list_models
            Throw New NotImplementedException()
        End Function

        Private Function getResultPortType_list_modelsAsync(request As list_modelsRequest) As Task(Of list_modelsResponse) Implements getResultPortType.list_modelsAsync
            Throw New NotImplementedException()
        End Function

        Private Function getResultPortType_getModel(request As getModelRequest) As getModelResponse Implements getResultPortType.getModel
            Throw New NotImplementedException()
        End Function

        Private Function getResultPortType_getModelAsync(request As getModelRequest) As Task(Of getModelResponse) Implements getResultPortType.getModelAsync
            Throw New NotImplementedException()
        End Function

        Private Function getResultPortType_Upload_Model(request As Upload_ModelRequest) As getModelResponse Implements getResultPortType.Upload_Model
            Throw New NotImplementedException()
        End Function

        Private Function getResultPortType_Upload_ModelAsync(request As Upload_ModelRequest) As Task(Of getModelResponse) Implements getResultPortType.Upload_ModelAsync
            Throw New NotImplementedException()
        End Function
    End Class


End Namespace
