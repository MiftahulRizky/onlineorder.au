Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports OfficeOpenXml

Partial Class Setting_Price_Matrix
    Inherits Page

    Dim settingCfg As New SettingConfig

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindPriceGroupSearch()

            Call BindDataMatrix(ddlGroupSearch.SelectedValue, txtWidthSearch.Text, txtDropSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Call BindPriceGroup()
        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            lblAction.Text = "Add"
            titleProccess.InnerText = "Add Matrix"

            ddlGroupSearch.SelectedValue = ""
            txtWidth.Text = "" : txtDrop.Text = ""
            txtCost.Text = ""

            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            Call MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnImport_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Dim thisScript As String = "window.onload = function() { showImport(); };"
        Try
            ClientScript.RegisterStartupScript(Me.GetType(), "showImport", thisScript, True)
            Exit Sub
        Catch ex As Exception
            Call MessageError_Import(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showImport", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindDataMatrix(ddlGroupSearch.SelectedValue, txtWidthSearch.Text, txtDropSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call BackColor()
        Try
            gvList.EditIndex = -1
            Call BindDataMatrix(ddlGroupSearch.SelectedValue, txtWidthSearch.Text, txtDropSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Call BindPriceGroup()
        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            lblAction.Text = "Edit"
            titleProccess.InnerText = "Edit Matrix"
            lblId.Text = UCase(row.Cells(1).Text).ToString()

            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM PriceMatrixs WHERE Id = '" + lblId.Text + "'")

            ddlPriceGroup.SelectedValue = myData.Tables(0).Rows(0).Item("PriceGroupId").ToString()
            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtDrop.Text = myData.Tables(0).Rows(0).Item("Drop").ToString()
            txtCost.Text = myData.Tables(0).Rows(0).Item("Cost")

            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            Call MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()

            Response.Redirect("~/setting/price/matrix", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitProccess_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                Call MessageError_Proccess(True, "PRICE GROUP IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If Not txtWidth.Text = "" Then
                If Not IsNumeric(txtWidth.Text) Then
                    Call MessageError_Proccess(True, "WIDTH SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtDrop.Text = "" Then
                If Not IsNumeric(txtDrop.Text) Then
                    Call MessageError_Proccess(True, "DROP / HEIGHT SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtCost.Text = "" Then
                Call MessageError_Proccess(True, "COST IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                lblPriceGroupId.Text = UCase(ddlPriceGroup.SelectedValue).ToString()
                If lblAction.Text = "Add" Then
                    sdsPage.Insert()
                End If
                If lblAction.Text = "Edit" Then
                    sdsPage.Update()
                End If
                Response.Redirect("~/setting/price/matrix", False)
            End If
        Catch ex As Exception
            Call MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitImport_Click(sender As Object, e As EventArgs)
        Call MessageError_Import(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showImport(); };"
        Try
            If Not fuFile.HasFiles Then
                Call MessageError_Import(True, "FILE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showImport", thisScript, True)
                Exit Sub
            End If

            Dim fileName As String = fuFile.FileName
            Dim savePath As String = Server.MapPath("~/file/matrix/") & fileName
            fuFile.SaveAs(savePath)

            Dim result As DataTable = ReadAllSheets(savePath)
            SaveToDatabase(result)

        Catch ex As Exception
            Call MessageError_Import(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showImport", thisScript, True)
        End Try
    End Sub

    Private Sub BindDataMatrix(G As String, W As String, D As String)
        Call BackColor()
        Try
            Dim byGroup As String = ""
            Dim byWidth As String = " WHERE PriceMatrixs.Width >= '" + W + "'"
            Dim byDrop As String = " AND PriceMatrixs.[Drop] >= '" + D + "'"

            If Not G = "" Then : byGroup = " AND PriceMatrixs.PriceGroupId = '" + UCase(G).ToString() + "'" : End If
            If W = "" Then : byWidth = " WHERE PriceMatrixs.Width >= '0'" : End If
            If D = "" Then : byDrop = " AND PriceMatrixs.[Drop] >= '0'" : End If

            Dim thisString As String = String.Format("SELECT PriceMatrixs.*, PriceProductGroups.Name AS GroupName FROM PriceMatrixs INNER JOIN PriceProductGroups ON PriceMatrixs.PriceGroupId = PriceProductGroups.Id {0} {1} {2} ORDER BY PriceProductGroups.Name, PriceMatrixs.Width, PriceMatrixs.[Drop], PriceMatrixs.Cost ASC", byWidth, byDrop, byGroup)

            gvList.DataSource = settingCfg.GetListData(thisString)
            gvList.DataBind()

            btnAdd.Visible = False : btnImport.Visible = False
            If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin") Then
                btnAdd.Visible = True : btnImport.Visible = True
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindPriceGroupSearch()
        ddlGroupSearch.Items.Clear()
        Try
            ddlGroupSearch.DataSource = settingCfg.GetListData("SELECT PriceProductGroups.Id AS Id, Designs.Name + ' | ' + PriceProductGroups.Name AS GroupName FROM PriceProductGroups INNER JOIN Designs ON PriceProductGroups.DesignId = Designs.Id WHERE PriceProductGroups.Active = 1 ORDER BY Designs.Name, PriceProductGroups.Name ASC")
            ddlGroupSearch.DataTextField = "GroupName"
            ddlGroupSearch.DataValueField = "Id"
            ddlGroupSearch.DataBind()

            If ddlGroupSearch.Items.Count > 1 Then
                ddlGroupSearch.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingCfg.GetListData("SELECT PriceProductGroups.Id AS Id, Designs.Name + ' | ' + PriceProductGroups.Name AS GroupName FROM PriceProductGroups INNER JOIN Designs ON PriceProductGroups.DesignId = Designs.Id WHERE PriceProductGroups.Active = 1 ORDER BY Designs.Name, PriceProductGroups.Name ASC")
            ddlPriceGroup.DataTextField = "GroupName"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 1 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        Call MessageError_Proccess(False, String.Empty)
        Call MessageError_Import(False, String.Empty)

        Session("matrixPriceGroup") = ""
        Session("matrixWidth") = ""
        Session("matrixDrop") = ""
    End Sub

    Protected Function BindCost(Cost As Decimal) As String
        Dim result As String = String.Empty
        If Cost > 0 Then
            result = Cost.ToString("N2", enUS)
        End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Proccess(Show As Boolean, Msg As String)
        divErrorProccess.Visible = Show : msgErrorProccess.InnerText = Msg
    End Sub

    Private Sub MessageError_Import(Show As Boolean, Msg As String)
        divErrorImport.Visible = Show : msgErrorImport.InnerText = Msg
    End Sub

    Private Function ReadAllSheets(ByVal filePath As String) As DataTable
        Dim result As New DataTable()
        result.Columns.Add("SheetName")
        result.Columns.Add("Col1")
        result.Columns.Add("Col2")
        result.Columns.Add("Value")

        Using package As New ExcelPackage(New FileInfo(filePath))
            For Each worksheet As ExcelWorksheet In package.Workbook.Worksheets
                Dim dt As New DataTable()

                For col As Integer = 1 To worksheet.Dimension.Columns
                    dt.Columns.Add(worksheet.Cells(1, col).Text)
                Next

                For row As Integer = 2 To worksheet.Dimension.Rows
                    Dim newRow As DataRow = dt.NewRow()
                    For col As Integer = 1 To worksheet.Dimension.Columns
                        newRow(col - 1) = worksheet.Cells(row, col).Text
                    Next
                    dt.Rows.Add(newRow)
                Next

                Dim convertedData As DataTable = ConvertData(dt, worksheet.Name)
                For Each dataRow As DataRow In convertedData.Rows
                    result.ImportRow(dataRow)
                Next
            Next
        End Using
        Return result
    End Function

    Private Function ConvertData(ByVal dt As DataTable, ByVal sheetName As String) As DataTable
        Dim result As New DataTable()
        result.Columns.Add("SheetName")
        result.Columns.Add("Col1")
        result.Columns.Add("Col2")
        result.Columns.Add("Value")

        For i As Integer = 1 To dt.Columns.Count - 1
            For j As Integer = 0 To dt.Rows.Count - 1
                If Not String.IsNullOrEmpty(dt.Rows(j)(0).ToString()) Then
                    Dim newRow As DataRow = result.NewRow()
                    newRow("SheetName") = sheetName
                    newRow("Col1") = dt.Columns(i).ColumnName
                    newRow("Col2") = dt.Rows(j)(0).ToString()
                    newRow("Value") = dt.Rows(j)(i).ToString()
                    result.Rows.Add(newRow)
                End If
            Next
        Next
        Return result
    End Function

    Private Sub SaveToDatabase(ByVal dt As DataTable)
        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
        Using conn As New SqlConnection(myConn)
            conn.Open()
            For Each row As DataRow In dt.Rows
                Dim query As String = "INSERT INTO ConvertedData VALUES (NEWID(), @SheetName, @Col1, @Col2, @Value)"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@SheetName", row("SheetName"))
                    cmd.Parameters.AddWithValue("@Col1", row("Col1"))
                    cmd.Parameters.AddWithValue("@Col2", row("Col2"))
                    cmd.Parameters.AddWithValue("@Value", row("Value"))
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub

    Protected Function VisibleEdit() As Boolean
        If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function

    Protected Function VisibleDelete() As Boolean
        If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function
End Class
