Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Mounting
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("mountingSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        BindBlindType()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Mounting"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                BindBlindType()
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Mounting"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Mountings WHERE Id = '" + lblId.Text + "'")
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()

                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    Dim blindArray() As String = myData.Tables(0).Rows(0).Item("BlindId").ToString().Split(",")
                    For Each i In blindArray
                        If Not (i.Equals(String.Empty)) Then
                            lbBlindType.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    Call MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Mountings' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "MOUNTING NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim blind As String = String.Empty
            For Each item As ListItem In lbBlindType.Items
                If item.Selected Then
                    blind += UCase(item.Value).ToString() & ","
                End If
            Next
            If blind = "" Then
                MessageError_Process(True, "BLIND TYPE IS REQUIRED !")
                lbBlindType.BackColor = Drawing.Color.Red
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                Dim blindType As String = blind.Remove(blind.Length - 1).ToString()

                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mountings OUTPUT INSERTED.Id VALUES (NEWID(), @BlindId, @Name, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@BlindId", UCase(blindType).ToString())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Mountings", thisId, Session("LoginId").ToString(), "Mounting Created"}
                    settingCfg.Log_Product(dataLog)

                    Session("mountingSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/mounting", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Mountings SET BlindId=@BlindId, Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@BlindId", UCase(blindType).ToString())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Mountings", lblId.Text, Session("LoginId").ToString(), "Mounting Updated"}
                    settingCfg.Log_Product(dataLog)

                    Session("mountingSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/mounting", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Mountings WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("mountingSearch") = txtSearch.Text
            Response.Redirect("~/setting/product/mounting", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("mountingSearch") = txtSearch.Text
        Response.Redirect("~/setting/product/mounting", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Name LIKE '%" + SearchText.Trim() + "%'"
            End If
            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Mountings {0} ORDER BY Name ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisString)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlindType()
        lbBlindType.Items.Clear()
        Try
            lbBlindType.DataSource = settingCfg.GetListData("SELECT UPPER(Blinds.Id) AS IdText,Designs.Name + ' | ' + Blinds.Name AS NameText FROM Blinds LEFT JOIN Designs ON Blinds.DesignId = Designs.Id WHERE Designs.Type = 'Panorama' OR Designs.Type = 'Evolve' ORDER BY Designs.Name ASC")
            lbBlindType.DataTextField = "NameText"
            lbBlindType.DataValueField = "IdText"
            lbBlindType.DataBind()

            If lbBlindType.Items.Count > 1 Then
                lbBlindType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindDetailBlindType(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty

            Dim myData As DataSet = settingCfg.GetListData("SELECT Blinds.Name AS BlindName FROM Mountings CROSS APPLY STRING_SPLIT(Mountings.BlindId, ',') AS blindArray LEFT JOIN Blinds ON blindArray.VALUE = Blinds.Id WHERE Mountings.Id = '" + Id + "'")
            If Not myData.Tables(0).Rows.Count = 0 Then
                For i As Integer = 0 To myData.Tables(0).Rows.Count - 1
                    Dim designName As String = myData.Tables(0).Rows(i).Item("BlindName").ToString()
                    hasil += designName & ", "
                Next
            End If
            If hasil.Length > 0 Then hasil = hasil.Remove(hasil.Length - 2)
            result = If(hasil.Length > 50, hasil.Substring(0, 40) & " .....", hasil)
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

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

    Protected Function VisibleAction() As Boolean
        If Session("LevelName") = "Leader" Then
            Return True
        End If
        Return False
    End Function
End Class
