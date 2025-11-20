Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesign()

            ddlDesignId.SelectedValue = Session("productDesign")
            BindBlind(ddlDesignId.SelectedValue)
            ddlBlindId.SelectedValue = Session("productBlind")
            txtSearch.Text = Session("productSearch")
            ddlActive.SelectedValue = Session("productActive")

            BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDesignId_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesignId.SelectedValue)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlBlindId_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("productDesign") = ddlDesignId.SelectedValue
        Session("productBlind") = ddlBlindId.SelectedValue
        Session("productSearch") = txtSearch.Text
        Session("productActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/product/add", False)
    End Sub

    Protected Sub linkControlType_Click(sender As Object, e As EventArgs)
        Session("productDesign") = ddlDesignId.SelectedValue
        Session("productBlind") = ddlBlindId.SelectedValue
        Session("productSearch") = txtSearch.Text
        Session("productActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/product/control", False)
    End Sub

    Protected Sub linkTubeType_Click(sender As Object, e As EventArgs)
        Session("productDesign") = ddlDesignId.SelectedValue
        Session("productBlind") = ddlBlindId.SelectedValue
        Session("productSearch") = txtSearch.Text
        Session("productActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/product/tube", False)
    End Sub

    Protected Sub linkColourType_Click(sender As Object, e As EventArgs)
        Session("productDesign") = ddlDesignId.SelectedValue
        Session("productBlind") = ddlBlindId.SelectedValue
        Session("productSearch") = txtSearch.Text
        Session("productActive") = ddlActive.SelectedValue

        Response.Redirect("~/setting/product/colour", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Session("productDesign") = ddlDesignId.SelectedValue
                    Session("productBlind") = ddlBlindId.SelectedValue
                    Session("productSearch") = txtSearch.Text
                    Session("productActive") = ddlActive.SelectedValue
                    Session("productDetail") = dataId
                    Session("productAction") = "detail"

                    Response.Redirect("~/setting/product/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                End Try
            ElseIf e.CommandName = "Ubah" Then
                MessageError(False, String.Empty)
                Try
                    Session("productDesign") = ddlDesignId.SelectedValue
                    Session("productBlind") = ddlBlindId.SelectedValue
                    Session("productSearch") = txtSearch.Text
                    Session("productActive") = ddlActive.SelectedValue
                    Session("productDetail") = dataId
                    Session("productAction") = "Edit"

                    Response.Redirect("~/setting/product/edit", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Products' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim id As String = txtIdDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Products WHERE Id=@Id DELETE FROM HardwareKits WHERE ProductId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", id)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("productDesign") = ddlDesignId.SelectedValue
            Session("productBlind") = ddlBlindId.SelectedValue
            Session("productSearch") = txtSearch.Text
            Session("productActive") = ddlActive.SelectedValue

            Response.Redirect("~/setting/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim id As String = UCase(txtIdCopy.Text).ToString()

            Dim thisId As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Products OUTPUT INSERTED.Id SELECT NEWID(), DesignId, BlindId, Name + ' - Copy', TubeType, ControlType, ColourType, Description, 1 FROM Products WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", id)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    thisId = myCmd.ExecuteScalar().ToString()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"Products", thisId, Session("LoginId").ToString(), "Products Created | Duplicate from " & id}
            settingCfg.Log_Product(dataLog)

            Session("productDesign") = ddlDesignId.SelectedValue
            Session("productBlind") = ddlBlindId.SelectedValue
            Session("productSearch") = txtSearch.Text
            Session("productActive") = ddlActive.SelectedValue

            Response.Redirect("~/setting/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim newActive As Integer = 0
            Dim active As String = txtActive.Text
            If active = "0" Then : newActive = 1 : End If

            Dim id As String = txtIdActive.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Products SET Active=@Active WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Parameters.AddWithValue("@Active", newActive)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"Products", id, Session("LoginId").ToString(), "Products Updated | Active = " & newActive}
            settingCfg.Log_Product(dataLog)

            Session("productDesign") = ddlDesignId.SelectedValue
            Session("productBlind") = ddlBlindId.SelectedValue
            Session("productSearch") = txtSearch.Text
            Session("productActive") = ddlActive.SelectedValue

            Response.Redirect("~/setting/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(Design As String, Blind As String, Search As String, Active As String)
        MessageError(False, String.Empty)
        Try
            Dim designString As String = " WHERE Products.DesignId = '" + Design + "'"
            Dim blindString As String = " AND Products.BlindId = '" + Blind + "'"

            Dim searchString As String = " AND (Products.Id LIKE '%" + Search.Trim() + "%' OR Products.Name LIKE '%" + Search.Trim() + "%' OR Designs.Name LIKE '%" + Search.Trim() + "%' OR Blinds.Name LIKE '%" + Search.Trim() + "%' OR Products.ControlType LIKE '%" + Search.Trim() + "%' OR Products.TubeType LIKE '%" + Search.Trim() + "%')"
            Dim activeString As String = " AND Products.Active = '" + Active + "'"

            If Design = "" Then
                designString = " WHERE Products.DesignId IS NOT NULL"
            End If
            If Blind = "" Then
                blindString = " AND Products.BlindId IS NOT NULL"
            End If
            If Search = "" Then
                searchString = ""
            End If

            gvList.DataSource = settingCfg.GetListData(String.Format("SELECT Products.*, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM Products LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id {0} {1} {2} {3} ORDER BY Designs.Name, Blinds.Name, Products.Name ASC", designString, blindString, searchString, activeString))
            gvList.DataBind()

            btnAdd.Visible = False
            btnActions.Visible = False
            If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin") Then
                btnAdd.Visible = True
                btnActions.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        ddlDesignId.Items.Clear()
        Try
            ddlDesignId.Items.Clear()
            ddlDesignId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Company='LOOP' ORDER BY Name ASC")
            ddlDesignId.DataTextField = "NameText"
            ddlDesignId.DataValueField = "Id"
            ddlDesignId.DataBind()

            ddlDesignId.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlind(DesignId As String)
        ddlBlindId.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlindId.Items.Clear()
                ddlBlindId.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' ORDER BY Name ASC")
                ddlBlindId.DataTextField = "NameText"
                ddlBlindId.DataValueField = "Id"
                ddlBlindId.DataBind()

                If ddlBlindId.Items.Count > 1 Then
                    ddlBlindId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function TextActive(Active As Integer) As String
        Dim result As String = "Activate"
        If Active = 1 Then : Return "Deactivate" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
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
        If Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function
End Class
