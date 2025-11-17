Imports System.Data
Imports System.IO

Partial Class Export_XMLExact
    Inherits Page

    Dim exactCfg As New ExactConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim status As String = "In Production"
            Dim jobDate As String = Now.ToString("yyyy-MM-dd")
            If Not String.IsNullOrEmpty(Request.QueryString("status")) Then
                status = Request.QueryString("status").ToString()
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("jobdate")) Then
                jobDate = DateTime.Parse(Request.QueryString("jobdate")).ToString("yyyy-MM-dd")
            End If

            Process(status, jobDate)
        End If
    End Sub

    Private Sub Process(status As String, jobDate As String)
        Dim headerData As DataSet = exactCfg.GetListData("SELECT * FROM OrderHeaders WHERE Status = '" + status + "' AND (OrderType = 'Panorama' Or OrderType = 'Evolve') AND CONVERT(DATE, JobDate) = '" + jobDate + "'")

        If headerData.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To headerData.Tables(0).Rows.Count - 1
                Dim id As String = headerData.Tables(0).Rows(i).Item("Id").ToString()
                Dim orderId As String = headerData.Tables(0).Rows(i).Item("OrderId").ToString()

                Dim fileName As String = String.Format("Order-{0}.xml", orderId)
                Dim filePath As String = Server.MapPath("~/file/inv/")

                Dim finalPath As String = Path.Combine(filePath, fileName)

                exactCfg.CreateXML(id, fileName, filePath)
                exactCfg.Connect(finalPath)
            Next
        End If

        Response.Redirect("~/export/sunlight", False)
    End Sub
End Class
