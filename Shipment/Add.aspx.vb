Imports System.Data.SqlClient
Imports System.IO
Imports OfficeOpenXml

Partial Class Shipment_Add
    Inherits Page

    Dim shipmentCfg As New ShipmentConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" Then
            Response.Redirect("~/", False)
        End If

        If Not IsPostBack Then
            BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If fuFile.HasFiles Then
                Dim fileExtension As String = Path.GetExtension(fuFile.FileName).ToLower()
                If fileExtension = ".xls" Or fileExtension = ".xlsx" Then
                    Dim fileName As String = fuFile.FileName

                    Dim savePath As String = Server.MapPath("~/file/shipment/") & fileName
                    fuFile.SaveAs(savePath)

                    ReadExcelData(savePath)
                Else
                    MessageError(True, "Please upload an Excel file (.xls or .xlsx).")
                End If
            Else
                MessageError(True, "No file selected. Please select a file to upload.")
                Exit Sub
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub ReadExcelData(filePath As String)
        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

            Dim thisId As String = shipmentCfg.CreateOrderShipmentId()
            Dim shipmentNumber As String = worksheet.Cells(2, 2).Text
            Dim etaData As Date = worksheet.Cells(4, 2).Text

            Dim etaPort As String = etaData.ToString("yyyy-MM-dd")
            Dim etaCustomer As String = etaData.AddDays(10).ToString("yyyy-MM-dd")
            Dim createdBy As String = UCase(Session("LoginId")).ToString()

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderShipments VALUES (@Id, @ShipmentNumber, @ETAPort, @ETACustomer, @CreatedBy, GETDATE(), 0, 1)")
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@ShipmentNumber", shipmentNumber)
                    myCmd.Parameters.AddWithValue("@ETAPort", etaPort)
                    myCmd.Parameters.AddWithValue("@ETACustomer", etaCustomer)
                    myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            ReadColumnFromRow(filePath, 9, thisId)
            Response.Redirect("~/shipment/", False)
        End Using
    End Sub

    Private Sub ReadColumnFromRow(filePath As String, startRow As Integer, shipmentId As String)
        If Not File.Exists(filePath) Then
            Console.WriteLine("File tidak ditemukan.")
            Return
        End If

        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
            Dim lastRow As Integer = worksheet.Dimension.End.Row
            If startRow > lastRow Then
                Console.WriteLine("Baris awal lebih besar dari jumlah baris data.")
                Exit Sub
            End If

            For row As Integer = startRow To lastRow
                Dim jobData As String = If(worksheet.Cells(row, 1).Text IsNot Nothing, worksheet.Cells(row, 1).Text, "")
                Dim orderId As String = jobData.Trim()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET ShipmentId=@ShipmentId WHERE OrderId=@OrderId")
                        myCmd.Parameters.AddWithValue("@ShipmentId", shipmentId)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            Next
        End Using
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/shipment/", False)
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
