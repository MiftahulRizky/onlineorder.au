
Partial Class Setting_PriceMatrix_DownloadExampleCSV
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("downloadExample") = "true" Then
            DownloadExampleFile()
        End If
    End Sub

    Private Sub DownloadExampleFile()
    ' Contoh isi file CSV yang sesuai format yang dibutuhkan
        Dim csvContent As String = String.Join(Environment.NewLine, New String() {
            ",600,750,900,1050,1200,1350,1500,1650,1800,1950,2100,2250,2400,2550,2700,2850,3000",
            "600,72,77,81,85,89,93,97,101,105,109,113,117,130,134,138,165,170",
            "800,74,79,83,87,92,96,101,105,109,114,118,123,135,140,144,172,177",
            "1000,75,80,85,90,95,100,105,110,115,120,125,130,145,150,155,185,190",
            "1200,77,82,87,92,97,102,107,112,117,122,127,133,150,155,160,195,200",
            "1500,78,83,88,93,98,103,108,113,118,123,128,134,155,160,165,205,210"
        })

        Dim fileName As String = "Contoh_PriceMatrix.csv"
        Dim fileBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(csvContent)

        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.OutputStream.Write(fileBytes, 0, fileBytes.Length)
        Response.Flush()
        Response.End()
    End Sub

End Class
