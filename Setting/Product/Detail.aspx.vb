Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Detail
    Inherits Page

    Dim settingCfg As New SettingConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Session("productDetail") = "" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        lblId.Text = Session("productDetail")
        If Not IsPostBack Then
            BackColor()
            BindDataProduct(lblId.Text)
        End If
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                BackColor()
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblIdKit.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Hardware Kit"

                    divCustomName.Visible = False

                    Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM HardwareKits WHERE Id = '" + lblIdKit.Text + "'")

                    txtKitId.Text = thisData.Tables(0).Rows(0).Item("KitId").ToString()
                    txtVenId.Text = thisData.Tables(0).Rows(0).Item("VenId").ToString()
                    txtKitName.Text = thisData.Tables(0).Rows(0).Item("Name").ToString()
                    ddlBlindStatus.SelectedValue = thisData.Tables(0).Rows(0).Item("BlindStatus").ToString()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='HardwareKits' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/product/edit", False)
        Exit Sub
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Products WHERE Id=@Id DELETE FROM HardwareKits WHERE ProductId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnAddKit_Click(sender As Object, e As EventArgs)
        BackColor()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Hardware Kit"

            divCustomName.Visible = True
            txtKitName.Text = lblName.Text

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        BackColor()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim kitName As String = txtKitName.Text.Trim()
                    If Not String.IsNullOrEmpty(txtCutomKitName.Text) Then
                        kitName = txtKitName.Text.Trim() & " " & txtCutomKitName.Text.Trim()
                    End If

                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO HardwareKits OUTPUT INSERTED.Id VALUES (NEWID(), @ProductId, @KitId, @VenId, @Name, @BlindStatus, 1)")
                            myCmd.Parameters.AddWithValue("@ProductId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@KitId", txtKitId.Text)
                            myCmd.Parameters.AddWithValue("@VenId", txtVenId.Text)
                            myCmd.Parameters.AddWithValue("@Name", kitName)
                            myCmd.Parameters.AddWithValue("@BlindStatus", ddlBlindStatus.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"HardwareKits", thisId, Session("LoginId").ToString(), "HardwareKit Created"}
                    settingCfg.Log_Product(dataLog)

                    Response.Redirect("~/setting/product/detail", False)
                    Exit Sub
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE HardwareKits SET Name=@Name, KitId=@KitId, VenId=@VenId, BlindStatus=@BlindStatus WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdKit.Text)
                            myCmd.Parameters.AddWithValue("@ProductId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@KitId", txtKitId.Text)
                            myCmd.Parameters.AddWithValue("@VenId", txtVenId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtKitName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@BlindStatus", ddlBlindStatus.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"HardwareKits", lblIdKit.Text, Session("LoginId").ToString(), "HardwareKit Updated"}
                    settingCfg.Log_Product(dataLog)

                    Response.Redirect("~/setting/product/detail", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDeleteKit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            lblIdKit.Text = txtIdKitDelete.Text

            Dim id As String = txtIdKitDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM HardwareKits WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", id)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/product/detail", False)
            Exit Sub
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataProduct(Id As String)
        Session("productKit") = ""
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Products WHERE Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/product", False)
                Exit Sub
            End If

            lblDesignName.Text = settingCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + myData.Tables(0).Rows(0).Item("DesignId").ToString() + "'")
            lblBlindName.Text = settingCfg.GetItemData("SELECT Name FROM Blinds WHERE Id='" + myData.Tables(0).Rows(0).Item("BlindId").ToString() + "'")
            lblName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            lblControlName.Text = myData.Tables(0).Rows(0).Item("ControlType").ToString()
            lblTubeName.Text = myData.Tables(0).Rows(0).Item("TubeType").ToString()
            lblColourName.Text = myData.Tables(0).Rows(0).Item("ColourType").ToString()
            lblDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()

            divActiveYes.Visible = False : divActiveNo.Visible = False
            Dim active As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
            If active = 1 Then
                divActiveYes.Visible = True : divActiveNo.Visible = False
            End If
            If active = 0 Then
                divActiveYes.Visible = False : divActiveNo.Visible = True
            End If

            gvList.DataSource = settingCfg.GetListData("SELECT * FROM HardwareKits_Shutters WHERE ProductId = '" + Id + "' ORDER BY Name, BlindStatus ASC")
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)
        MessageError_Process(False, String.Empty)

        txtKitId.BackColor = Drawing.Color.Empty
        txtVenId.BackColor = Drawing.Color.Empty
        txtKitName.BackColor = Drawing.Color.Empty
        txtCutomKitName.BackColor = Drawing.Color.Empty
        ddlBlindStatus.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Process(Show As Boolean, Msg As String)
        divErrorProcess.Visible = Show : msgErrorProcess.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Products WHERE Id = '" + Id + "'")
            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function
End Class
