Imports System.Data
Imports System.IO
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Partial Class Report
    Inherits Page

    Dim reportCfg As New ReportConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" And Not Session("RoleName") = "Data Entry" And Not Session("RoleName") = "Account" Then
            Response.Redirect("~/", False)
        End If
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindFileType(ddlReportType.SelectedValue)
            BindComponentForm(ddlReportType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlReportType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindFileType(ddlReportType.SelectedValue)
        BindComponentForm(ddlReportType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlReportType.SelectedValue = "" Then
                MessageError(True, "DATE TO IS REQUIRED !")
                ddlReportType.Focus()
                Exit Sub
            End If

            If ddlFileType.SelectedValue = "" Then
                MessageError(True, "FILE TYPE TO IS REQUIRED !")
                ddlFileType.Focus()
                Exit Sub
            End If

            If ddlReportType.SelectedValue = "ShuttersOrders" Then
                If txtStartDate.Text = "" Then
                    MessageError(True, "START DATE IS REQUIRED !")
                    txtStartDate.Focus()
                    Exit Sub
                End If

                If Not txtStartDate.Text = "" Then
                    If Not IsDate(txtStartDate.Text) Then
                        MessageError(True, "START DATE SHOULD BE DATE TYPE !")
                        txtStartDate.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If ddlReportType.SelectedValue = "BlindsDaily" Or ddlReportType.SelectedValue = "ShuttersDaily" Or ddlReportType.SelectedValue = "ShuttersOrder" Then
                If txtEndDate.Text = "" Then
                    MessageError(True, "DATE TO IS REQUIRED !")
                    txtEndDate.Focus()
                    Exit Sub
                End If

                If Not txtEndDate.Text = "" Then
                    If Not IsDate(txtEndDate.Text) Then
                        MessageError(True, "DATE TO SHOULD BE DATE TYPE !")
                        txtEndDate.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If msgError.InnerText = "" Then
                If ddlReportType.SelectedValue = "BlindsDaily" Then
                    BlindsDailyAction()
                End If

                If ddlReportType.SelectedValue = "ShuttersDaily" Then
                    ShuttersDailyAction()
                End If

                If ddlReportType.SelectedValue = "ShuttersOrder" Then
                    ShuttersOrdersAction()
                End If

                If ddlReportType.SelectedValue = "ShuttersMonth" Then
                    ShuttersMTMAction()
                End If

                If ddlReportType.SelectedValue = "Customer" Then
                    Dim pdfFilePath As String = Server.MapPath("~/file/report/Customer List.pdf")

                    reportCfg.Customers(pdfFilePath, ddlAdditional.SelectedValue)

                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=Customer List.pdf")
                    Response.TransmitFile(pdfFilePath)
                    Response.End()
                End If

                If ddlReportType.SelectedValue = "Fabric" Then
                    Dim fileName As String = "Lifestyle Emerge Fabric Collection.xlsx"
                    Dim xlsFilePath As String = Server.MapPath("~/file/report/" & fileName)
                    Dim logoPath As String = Server.MapPath("~/Content/static/LS.jpeg")

                    reportCfg.Fabrics(xlsFilePath, logoPath)

                    Response.Clear()
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
                    Response.WriteFile(xlsFilePath)
                    Response.End()
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub BindFileType(Report As String)
        ddlFileType.Items.Clear()
        Try
            If Not Report = "" Then
                ddlFileType.Items.Add(New ListItem("PDF", "pdf"))
                If Report = "Fabric" Then
                    ddlFileType.Items.Clear()
                    ddlFileType.Items.Add(New ListItem("EXCEL / XLSX", "xls"))
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BlindsDailyAction()
        Dim startDate As String = DateTime.Parse(txtEndDate.Text).ToString("yyyy-MM-01")
        Dim endDate As String = DateTime.Parse(txtEndDate.Text).ToString("yyyy-MM-dd")

        lblPeriod.Text = startDate & " - " & endDate

        Dim pdfFilePath As String = Server.MapPath("~/file/report/Blinds Daily Report.pdf")
        reportCfg.BlindsDaily(pdfFilePath, txtEndDate.Text, lblPeriod.Text)

        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "attachment; filename=Blinds Daily Report.pdf")
        Response.TransmitFile(pdfFilePath)
        Response.End()
    End Sub

    Private Sub ShuttersDailyAction()
        Dim startDate As String = DateTime.Parse(txtEndDate.Text).ToString("01 MMM yyyy")
        Dim endDate As String = DateTime.Parse(txtEndDate.Text).ToString("dd MMM yyyy")

        lblPeriod.Text = startDate & " - " & endDate

        Dim pdfFilePath As String = Server.MapPath("~/file/report/Shutters Daily Report.pdf")
        reportCfg.ShuttersDaily(pdfFilePath, txtEndDate.Text, lblPeriod.Text)

        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "attachment; filename=Shutters Daily Report.pdf")
        Response.TransmitFile(pdfFilePath)
        Response.End()
    End Sub

    Private Sub ShuttersOrdersAction()
        Dim pdfFilePath As String = Server.MapPath("~/file/report/Shutters Orders.pdf")

        Dim dateFrom As DateTime = DateTime.Parse(txtStartDate.Text)
        Dim dateTo As DateTime = DateTime.Parse(txtEndDate.Text)

        reportCfg.ShuttersOrders(pdfFilePath, dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"))

        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "attachment; filename=Shutters Orders.pdf")
        Response.TransmitFile(pdfFilePath)
        Response.End()
    End Sub

    Private Sub ShuttersMTMAction()
        Dim pdfFilePath As String = Server.MapPath("~/file/report/Shutters Month to Month.pdf")

        Dim dateFrom As String = DateTime.Parse(txtStartDate.Text).ToString("yyyy-MM-01")
        Dim dateTo As String = DateTime.Parse(txtEndDate.Text).ToString("yyyy-MM-01")

        reportCfg.ShuttersMTM(pdfFilePath, dateFrom, dateTo)

        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "attachment; filename=Shutters Month to Month.pdf")
        Response.TransmitFile(pdfFilePath)
        Response.End()
    End Sub

    Private Sub FabricAction()
        Dim ds As DataSet = reportCfg.GetListData("SELECT * FROM Fabrics WHERE DesignId LIKE '%E7959750-5CF8-48E5-9171-CD71B53CDC2F%' OR DesignId LIKE '%50CE8EDF-E106-414C-BDE3-D7AA8F8046D2%' OR DesignId LIKE '%B594C6C7-452D-4CB6-8A20-6914BD97B830%' OR DesignId LIKE '%B556E35C-CEAC-40F8-A6CF-156601BD57DA%' AND Active = 1")

        Using package As New ExcelPackage()
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Worksheet")

            worksheet.View.ShowGridLines = False

            worksheet.Cells("A1:N1").Merge = True
            worksheet.Cells("A2:N2").Merge = True
            worksheet.Cells("A3:N3").Merge = True
            worksheet.Row(1).Height = 40
            worksheet.Row(2).Height = 25
            worksheet.Row(4).Height = 60

            Dim logoPath As String = Server.MapPath("~/Content/static/LS.jpeg")
            If File.Exists(logoPath) Then
                Using logoStream As New FileStream(logoPath, FileMode.Open, FileAccess.Read)
                    Dim picture = worksheet.Drawings.AddPicture("Logo", logoStream)
                    picture.SetPosition(0, 0, 0, 0)
                    picture.SetSize(330, 80)
                End Using
            Else
                worksheet.Cells("A1").Value = "Logo Not Found"
            End If

            worksheet.Cells("A1").Value = "Lifestyle Emerge Fabric Collection"
            worksheet.Cells("A1").Style.Font.Bold = True
            worksheet.Cells("A1").Style.Font.Size = 28
            worksheet.Cells("A1").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
            worksheet.Cells("A1").Style.VerticalAlignment = ExcelVerticalAlignment.Center

            worksheet.Cells("A2").Value = "Effective: " & Now.ToString("dd MMM yyyy")
            worksheet.Cells("A2").Style.Font.Size = 14
            worksheet.Cells("A2").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
            worksheet.Cells("A2").Style.VerticalAlignment = ExcelVerticalAlignment.Center

            Dim headers As String() = {"Price Group", "Fabric Name", "Fabric Type", "Colours Available", "Composition", "Flame Retardant", "PVC Free = P Lead Free = L", "Greenguard Gold", "Fabric Weight (gsm)", "Vertical", "Roller", "Roman", "Panel Glide", "Usable Width (mm)"}
            Dim columnWidths As Double() = {7, 25, 11, 60, 25, 10, 12, 12, 10, 10, 10, 10, 10, 9}

            For colIndex As Integer = 0 To headers.Length - 1
                Dim cell = worksheet.Cells(4, colIndex + 1)
                cell.Value = headers(colIndex)
                cell.Style.WrapText = True
                cell.Style.Font.Bold = True
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                cell.Style.Font.Color.SetColor(System.Drawing.Color.White)
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black)
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center
                worksheet.Column(colIndex + 1).Width = columnWidths(colIndex)
            Next

            Dim rowIndex As Integer = 4

            For Each row As DataRow In ds.Tables(0).Rows
                rowIndex += 1

                Dim fabricId As String = row("Id").ToString()
                Dim designId As String = row("DesignId").ToString()
                Dim dataColour As DataSet = reportCfg.GetListData("SELECT Colour FROM FabricColours WHERE FabricId = '" & fabricId & "' AND Active = 1")
                Dim fabricColour As String = String.Join(", ", dataColour.Tables(0).AsEnumerable().Select(Function(r) r("Colour").ToString()))

                Dim flameReterdant As String = If(Convert.ToBoolean(row("FlameReterdant")), "Yes", "No")
                Dim greenguardGold As String = If(Convert.ToBoolean(row("GreenguardGold")), "Yes", "No")

                worksheet.Cells(rowIndex, 1).Value = row("Group").ToString().Replace("Group ", "")
                worksheet.Cells(rowIndex, 2).Value = row("Name")
                worksheet.Cells(rowIndex, 3).Value = row("Type")
                worksheet.Cells(rowIndex, 4).Value = fabricColour
                worksheet.Cells(rowIndex, 5).Value = row("Composition")
                worksheet.Cells(rowIndex, 6).Value = flameReterdant
                worksheet.Cells(rowIndex, 7).Value = row("PvcLead")
                worksheet.Cells(rowIndex, 8).Value = greenguardGold
                worksheet.Cells(rowIndex, 9).Value = row("Weight")
                Dim vertical As String = "No"
                If designId.Contains("B556E35C-CEAC-40F8-A6CF-156601BD57DA") Then
                    vertical = "Yes"
                End If
                Dim roller As String = "No"
                If designId.Contains("50CE8EDF-E106-414C-BDE3-D7AA8F8046D2") Then
                    roller = "Yes"
                End If
                Dim roman As String = "No"
                If designId.Contains("B594C6C7-452D-4CB6-8A20-6914BD97B830") Then
                    roman = "Yes"
                End If
                Dim pg As String = "No"
                If designId.Contains("E7959750-5CF8-48E5-9171-CD71B53CDC2F") Then
                    pg = "Yes"
                End If

                Dim usableWidth As Integer = reportCfg.GetItemData_Integer("SELECT TOP 1 Width FROM FabricColours WHERE FabricId = '" + fabricId + "'")
                worksheet.Cells(rowIndex, 10).Value = vertical
                worksheet.Cells(rowIndex, 11).Value = roller
                worksheet.Cells(rowIndex, 12).Value = roman
                worksheet.Cells(rowIndex, 13).Value = pg
                worksheet.Cells(rowIndex, 14).Value = usableWidth

                worksheet.Row(rowIndex).Height = 30
                worksheet.Row(rowIndex).Style.WrapText = True

                For colIndex As Integer = 1 To 14
                    worksheet.Cells(rowIndex, colIndex).Style.HorizontalAlignment = If(colIndex = 2 Or colIndex = 3 Or colIndex = 4 Or colIndex = 5, ExcelHorizontalAlignment.Left, ExcelHorizontalAlignment.Center)
                    worksheet.Cells(rowIndex, colIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center
                Next

                Dim cellRange = worksheet.Cells(rowIndex, 1, rowIndex, 14).Style.Border
                cellRange.Top.Style = ExcelBorderStyle.Thin
                cellRange.Bottom.Style = ExcelBorderStyle.Thin
                cellRange.Left.Style = ExcelBorderStyle.Thin
                cellRange.Right.Style = ExcelBorderStyle.Thin
            Next

            worksheet.PrinterSettings.PrintArea = worksheet.Cells(1, 1, rowIndex, 14)
            worksheet.PrinterSettings.FitToPage = True
            worksheet.PrinterSettings.FitToWidth = 1
            worksheet.PrinterSettings.FitToHeight = 1
            worksheet.PrinterSettings.Orientation = eOrientation.Landscape
            worksheet.PrinterSettings.PaperSize = ePaperSize.A4
            worksheet.PrinterSettings.TopMargin = 0
            worksheet.PrinterSettings.BottomMargin = 0
            worksheet.PrinterSettings.LeftMargin = 0
            worksheet.PrinterSettings.RightMargin = 0

            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("Content-Disposition", "attachment; filename=Lifestyle Emerge Fabric Collection.xlsx")
            Response.BinaryWrite(package.GetAsByteArray())
            Response.End()
        End Using
    End Sub

    Private Sub BindComponentForm(ReportType As String)
        Try
            divAdditional.Visible = False
            divStartDate.Visible = False
            divEndDate.Visible = False

            If ReportType = "BlindsDaily" Or ReportType = "ShuttersDaily" Then
                divEndDate.Visible = True
            End If

            If ReportType = "ShuttersOrder" Or ReportType = "ShuttersMonth" Then
                divStartDate.Visible = True
                divEndDate.Visible = True
            End If

            If ReportType = "Customer" Then
                divAdditional.Visible = True
                lblAdditional.Text = "CASH SALE"
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
