﻿Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.ServiceModel
Imports System.Workflow.ComponentModel.Compiler
Imports System.Workflow.ComponentModel.Serialization
Imports System.Workflow.ComponentModel
Imports System.Workflow.ComponentModel.Design
Imports System.Workflow.Runtime
Imports System.Workflow.Activities
Imports System.Workflow.Activities.Rules
Imports Microsoft.SharePoint.Workflow
Imports Microsoft.SharePoint.WorkflowActions

Public Class Workflow1
    Inherits SequentialWorkflowActivity

    Public workflowProperties As New SPWorkflowActivationProperties

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private Sub onWorkflowActivated1_Invoked(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ExternalDataEventArgs)
        'This simple workflow illustrates how to call an Azure WCF service from a 
        'workflow or workflow activity. Note that the workflow must run 
        'outside the sandbox to be able to call an external WCF service.

        'Deploy this solution to a SharePoint site that includes an Announcements
        'list. Package and deploy the AZURE_DayNamerService project before you run
        'This workflows. The workflow's getToday() method, calls the example WCF service to get 
        'today's name and adds it to the item.

        'Get the item that started the workflow
        Dim workflowItem As SPListItem = workflowProperties.Item
        'Modify the body field
        workflowItem("Body") += "This item was modified by a workflow on " + getToday()
        'Save the changes
        workflowItem.Update()
    End Sub

    Private Function getToday() As String
        'I used svcutil.exe to generate the proxy class for the service
        'in the generatedDayNamerProxy.cs file. I'm going to configure this
        'in code by using a channel factory.

        Dim today As String = String.Empty
        'Create the channel factory with a Uri, binding and endpoint
        Dim serviceUri As Uri = New Uri("http://daynamervb.cloudapp.net/dayinfoservice.svc")
        Dim serviceBinding As BasicHttpBinding = New BasicHttpBinding()
        Dim dayNamerEndPoint As EndpointAddress = New EndpointAddress(serviceUri)
        Dim channelFactory As ChannelFactory(Of IDayNamer) = New ChannelFactory(Of IDayNamer)(serviceBinding, dayNamerEndPoint)
        'Create a channel
        Dim dayNamer As IDayNamer = channelFactory.CreateChannel()
        'Now we can call the TodayIs method
        today = dayNamer.TodayIs()
        'close the factory with all its channels
        channelFactory.Close()
        Return today
    End Function

End Class