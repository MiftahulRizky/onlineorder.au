<%@ WebHandler Language="VB" Class="UploadHandler" %>

Imports System
Imports System.Web
Imports System.IO
Public Class UploadHandler : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"
        Try
            Dim file = context.Request.Files("file")
            If file Is Nothing OrElse file.ContentLength = 0 Then
                context.Response.StatusCode = 400
                context.Response.Write("No file uploaded")
                Return
            End If

            Dim folderPath = context.Server.MapPath("~/Temp/")
            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            Dim fileName = Path.GetFileName(file.FileName)
            Dim savedPath = Path.Combine(folderPath, fileName)
            file.SaveAs(savedPath)

            ' Kembalikan nama file ke JS
            context.Response.Write(fileName)
        Catch ex As Exception
            context.Response.StatusCode = 500
            context.Response.Write("Error: " & ex.Message)
        End Try
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class